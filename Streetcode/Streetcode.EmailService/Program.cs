using Streetcode.EmailService.Interfaces;
using Streetcode.EmailService.Models;
using Streetcode.EmailService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<EmailConfiguration>(
    builder.Configuration.GetSection("EmailConfiguration"));

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => Results.Ok("EmailService is running"))
    .WithName("HealthCheck");

app.MapControllers();

await app.RunAsync();