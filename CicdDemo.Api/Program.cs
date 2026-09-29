
using CicdDemo.Api;
using CicdDemo.Api.Services;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

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
builder.Services.AddSingleton<WeatherService>();

// Bind AppSettings section to strongly-typed class
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
}

app.UseHttpsRedirection();

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

// Health check endpoint (needed for Module 6)
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.Run();