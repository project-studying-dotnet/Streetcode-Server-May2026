using FluentValidation;
using MediatR;
using Serilog;
using Streetcode.Auth.Extensions;
using Streetcode.Auth.MediatR.Behaviors;
using Streetcode.Auth.Services;
using Streetcode.Auth.Services.Interfaces;
using Streetcode.Common.Models;
using Streetcode.Shared.Web.Middleware;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddSwaggerDocumentation();

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .MinimumLevel.Debug() 
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration));

var app = builder.Build();



app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}


var retryCount = 0;
while (retryCount < 10)
{
    try
    {
        Log.Information("Attempting to start DB migrations and siding...");
        await app.SeedDataAsync();
        Log.Information("Database successfully initialized!");
        break;
    }
    catch (Exception ex)
    {
        retryCount++;
        Log.Warning("Failed to connect to database. Attempt {count}/10. Error: {msg}", retryCount, ex.Message);
        await Task.Delay(10000); 
    }
}

app.UseCors(policy =>
    policy.WithOrigins("http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


