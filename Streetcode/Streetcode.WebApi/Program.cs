using FluentValidation;
using Hangfire;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.BLL.Validators;
using Streetcode.WebApi.Extensions;
using Streetcode.BLL.Interfaces.WebParsingUtils;
using Streetcode.BLL.Services.WebParsingUtils;

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
builder.Services.AddCustomServices(builder.Configuration);
builder.Services.ConfigureBlob(builder);
builder.Services.ConfigurePayment(builder);
builder.Services.ConfigureInstagram(builder);
builder.Services.ConfigureSerilog(builder);
builder.Services.AddHttpClient();
builder.Services.AddValidatorsFromAssembly(typeof(BllAssemblyMarker).Assembly);
builder.Services.AddScoped<ICsvAddressParser, CsvAddressParserService>();
builder.Services.AddScoped<IGeocoding, GeocodingService>();
builder.Services.AddScoped<IToponymData, ToponymDataService>();
builder.Services.AddScoped<IWebParsingUtils, WebParsingUtilsService>();
builder.Services.Configure<UkrPoshtaParserSettings>(builder.Configuration.GetSection("UkrPoshtaParser"));
builder.Services.Configure<GeocodingSettings>(builder.Configuration.GetSection("Geocoding"));

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

var shouldApplyMigrations = builder.Configuration.GetValue<bool>("ApplyMigrations");

if (shouldApplyMigrations)
{
    await app.ApplyMigrations();
}

// await app.SeedDataAsync(); // uncomment for seeding data in local
app.UseCors();
app.UseCustomMiddlewares();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

var isHangfireEnabled = builder.Configuration.GetValue<bool>("Hangfire:Enabled");

if (isHangfireEnabled)
{
    app.UseHangfireDashboard("/dash");

    BackgroundJob.Schedule<WebParsingUtilsService>(
        wp => wp.ParseZipFileFromWebAsync(),
        TimeSpan.FromMinutes(1));

    RecurringJob.AddOrUpdate<WebParsingUtilsService>(
        recurringJobId: "parse-zip-from-web-monthly",
        wp => wp.ParseZipFileFromWebAsync(),
        Cron.Monthly);

    RecurringJob.AddOrUpdate<BlobService>(
        recurringJobId: "clean-blob-monthly",
        b => b.CleanBlobStorage(),
        Cron.Monthly);
}

app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();