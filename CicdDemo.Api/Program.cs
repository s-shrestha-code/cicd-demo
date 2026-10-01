using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using CicdDemo.Api.Data;
using CicdDemo.Api.Helpers;
using CicdDemo.Api.Helpers.HealthCheck;
using CicdDemo.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add Key Vault configuration for non-development environments
if (!builder.Environment.IsDevelopment())
{
    var keyVaultName = builder.Configuration["AZURE_KEYVAULT_NAME"]
        ?? Environment.GetEnvironmentVariable("AZURE_KEYVAULT_NAME");

    if (!string.IsNullOrEmpty(keyVaultName))
    {
        var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

        builder.Configuration.AddAzureKeyVault(
            keyVaultUri,
            new DefaultAzureCredential(),
            new AzureKeyVaultConfigurationOptions
            {
                // Key Vault uses -- for hierarchy, .NET maps it to :
                // e.g. ConnectionStrings--Default → ConnectionStrings:Default
                Manager = new KeyVaultSecretManager()
            });
    }
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

// Bind AppSettings section to strongly-typed class
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("InMemoryDb")
    //Tells Entity Framework Core to stop throwing exceptions or warnings when your code tries
    //to use database transactions on an in-memory database provider, which does not natively support them
    .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
});

builder.Services.AddScoped<CustomerService>();

builder.Services.AddSingleton<WeatherService>();

// Register health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API is running"))
    .AddCheck("weather-service", () =>
    {
        // Verify WeatherService can generate forecasts
        try
        {
            var svc = new WeatherService();
            var result = svc.GetForecast(1);
            return result.Any()
                ? HealthCheckResult.Healthy("WeatherService is working")
                : HealthCheckResult.Unhealthy("WeatherService returned no results");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"WeatherService failed: {ex.Message}");
        }
    })
    //Normal connectoin check
    .AddDbContextCheck<AppDbContext>("db_connection_check")
    .AddCheck<DatabaseHealthCheck>("db_write_check");

var app = builder.Build();

//SeedDatabase.Seed(app, app.Environment.IsDevelopment());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

// Map health check endpoint
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.MapGet("/weatherforecast", (
    WeatherService svc,
    IOptions<AppSettings> settings) =>
{
    // In real code you'd use settings.Value.ApiKey to authenticate
    // For now just confirm it's being read
    Console.WriteLine($"ApiKey in use: {settings.Value.ApiKey}");
    return svc.GetForecast(days: 5);
})
.WithName("GetWeatherForecast");

app.MapGet("/customers", async (CustomerService customerService) =>
{
    await customerService.GetAllCustomersAsync();
});

app.Run();