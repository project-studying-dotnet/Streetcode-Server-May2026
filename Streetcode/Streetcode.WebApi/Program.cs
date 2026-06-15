using FluentValidation;
using Hangfire;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.BLL.Validators;
using Streetcode.WebApi.Extensions;
using Streetcode.WebApi.Utils;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

builder.Configuration.ConfigureCustom(environment);

if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Local")
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddSwaggerServices();
builder.Services.AddCustomServices();
builder.Services.ConfigureBlob(builder);
builder.Services.ConfigurePayment(builder);
builder.Services.ConfigureInstagram(builder);
builder.Services.ConfigureSerilog(builder);
builder.Services.AddValidatorsFromAssembly(typeof(BllAssemblyMarker).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
}
else
{
    app.UseHsts();
}

await app.ApplyMigrations();

await app.SeedDataAsync(); // uncomment for seeding data in local
app.UseCors();

app.UseCustomMiddlewares();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/dash");

var shouldRegisterBackgroundJobs =
    !app.Environment.IsDevelopment() &&
    app.Environment.EnvironmentName != "Local";

if (shouldRegisterBackgroundJobs)
{
    BackgroundJob.Schedule<WebParsingUtils>(
        wp => wp.ParseZipFileFromWebAsync(),
        TimeSpan.FromMinutes(1));

    RecurringJob.AddOrUpdate<WebParsingUtils>(
        recurringJobId: "parse-zip-from-web-monthly",
        wp => wp.ParseZipFileFromWebAsync(),
        Cron.Monthly);

    RecurringJob.AddOrUpdate<BlobService>(
        recurringJobId: "clean-blob-monthly",
        b => b.CleanBlobStorage(),
        Cron.Monthly);
}

app.MapControllers();

app.Run();