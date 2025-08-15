using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WeatherMonitor.Application.Interfaces;
using WeatherMonitor.Application.Services;
using WeatherMonitor.Core.Interfaces;
using WeatherMonitor.Infrastructure.Repositories;
using WeatherMonitor.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//register http client
builder.Services.AddHttpClient<IWeatherObservationDataRepository, BoMWeatherObservationRepository>();

//register repos
builder.Services.AddScoped<IWeatherStationRepository, WeatherStationRepository>();
builder.Services.AddScoped<IWeatherObservationDataRepository, BoMWeatherObservationRepository>();

//register app service
builder.Services.AddScoped<IWeatherObservationService, WeatherObservationService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();

app.UseSwaggerUI();

app.Run();
