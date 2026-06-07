using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Streetcode.EmailService.Contracts;
using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Models;

namespace Streetcode.EmailService.Services;

// RabbitMqEmailConsumer is an infrastructure component that requires
// a running RabbitMQ broker and connection lifecycle management.
// Unit testing it would require heavy mocking of external dependencies
// and provide limited value, therefore it is excluded from code coverage.
[ExcludeFromCodeCoverage]
public class RabbitMqEmailConsumer : BackgroundService
{
    private const int RetryDelayMilliseconds = 5000;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly RabbitMqSettings _rabbitMqSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RabbitMqEmailConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public RabbitMqEmailConsumer(
        IOptions<RabbitMqSettings> rabbitMqSettings,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RabbitMqEmailConsumer> logger)
    {
        _rabbitMqSettings = rabbitMqSettings.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await WaitForRabbitMqAsync(stoppingToken);

        if (stoppingToken.IsCancellationRequested || _channel is null)
        {
            return;
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, eventArgs) =>
        {
            await ProcessMessageAsync(eventArgs);
        };

        _channel.BasicConsume(
            queue: _rabbitMqSettings.QueueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("RabbitMQ email consumer started.");
    }

    private async Task WaitForRabbitMqAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                InitializeRabbitMq();

                _logger.LogInformation("RabbitMQ connection established.");
                return;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning(
                    ex,
                    "RabbitMQ is not available yet. Retrying in {DelaySeconds} seconds.",
                    RetryDelayMilliseconds / 1000);

                await Task.Delay(RetryDelayMilliseconds, stoppingToken);
            }
        }
    }

    private void InitializeRabbitMq()
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqSettings.HostName,
            Port = _rabbitMqSettings.Port,
            UserName = _rabbitMqSettings.UserName,
            Password = _rabbitMqSettings.Password,
            DispatchConsumersAsync = true,
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(
            queue: _rabbitMqSettings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);
    }

    private async Task ProcessMessageAsync(BasicDeliverEventArgs eventArgs)
    {
        try
        {
            _logger.LogInformation("RabbitMQ email message received.");

            var json = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var contract = JsonSerializer.Deserialize<EmailMessageContract>(json, JsonOptions);

            if (contract is null)
            {
                _logger.LogWarning("RabbitMQ email message deserialization failed.");
                _channel?.BasicNack(eventArgs.DeliveryTag, false, false);
                return;
            }

            var message = new Message
            {
                To = contract.To,
                From = contract.From,
                Subject = contract.Subject,
                Content = contract.Content,
            };

            using var scope = _serviceScopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var isSent = await emailService.SendEmailAsync(message);

            if (isSent)
            {
                _logger.LogInformation("RabbitMQ email message processed successfully.");
                _channel?.BasicAck(eventArgs.DeliveryTag, false);
                return;
            }

            _logger.LogWarning("RabbitMQ email message processing failed.");
            _channel?.BasicNack(eventArgs.DeliveryTag, false, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process RabbitMQ email message.");
            _channel?.BasicNack(eventArgs.DeliveryTag, false, false);
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();

        GC.SuppressFinalize(this);
        base.Dispose();
    }
}