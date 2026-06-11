namespace Streetcode.Auth.Services.Interfaces
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(string queue, T message);
    }
}
