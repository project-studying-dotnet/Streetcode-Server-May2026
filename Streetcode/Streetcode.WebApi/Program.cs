using Hangfire;
using Streetcode.BLL.Services.BlobStorageService;
using Streetcode.DAL.Persistence;
using Streetcode.WebApi.Extensions;
using Streetcode.WebApi.Utils;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
builder.Configuration.ConfigureCustom(environment);

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddSwaggerServices();
builder.Services.AddCustomServices();
builder.Services.ConfigureBlob(builder);
builder.Services.ConfigurePayment(builder);
builder.Services.ConfigureInstagram(builder);
builder.Services.ConfigureSerilog(builder);
var app = builder.Build();

if (app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPIv5 v1"));
}
else
{
    app.UseHsts();
}

await app.ApplyMigrations();

// await app.SeedDataAsync(); // uncomment for seeding data in local
app.UseCors();

app.UseCustomMiddlewares();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHangfireDashboard("/dash");

if (app.Environment.EnvironmentName != "Local")
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
public partial class Program
{
}