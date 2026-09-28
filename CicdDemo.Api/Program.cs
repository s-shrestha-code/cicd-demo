
using CicdDemo.Api;
using CicdDemo.Api.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

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