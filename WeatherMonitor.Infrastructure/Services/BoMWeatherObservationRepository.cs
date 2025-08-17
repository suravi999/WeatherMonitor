using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;
using WeatherMonitor.Infrastructure.Models;

namespace WeatherMonitor.Infrastructure.Services
{
    public class BoMWeatherObservationRepository : IWeatherObservationDataRepository
    {
        private const string BOM_BASE_URL = "https://www.bom.gov.au/fwo/IDS60901/IDS60901";
        private readonly HttpClient _httpClient;
        private readonly IWeatherStationRepository _stationRepository;
        private readonly JsonSerializerOptions _jsonOptions;
        public BoMWeatherObservationRepository(HttpClient httpClient, IWeatherStationRepository stationRepository)
        {
            _stationRepository = stationRepository;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("WeatherMonitor/1.0");
            _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public Task<WeatherObservation?> GetLatestObservationAsync(string wmoId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<WeatherObservation>> GetWeatherObservationsAsync(string wmoId)
        {
            try
            {
                var url = $"{BOM_BASE_URL}.{wmoId}.json";
                var response = await _httpClient.GetStringAsync(url);

                var bomData = JsonSerializer.Deserialize<BoMWeatherResponse>(response, _jsonOptions);

                if (bomData?.Observations?.Data == null)
                    return new List<WeatherObservation>();

                var station = await _stationRepository.GetStationByWmoIdAsync(wmoId);

                return bomData.Observations.Data.Select(d => new WeatherObservation
                {
                    StationName = station?.Name ?? d.Name ?? "Unknown",
                    WmoId = wmoId,
                    ObservationTime = ParseBoMDateTime(d.Aifstime_Utc),
                    Temperature = d.Air_Temp,
                    ApparentTemperature = d.Apparent_T,
                    DewPoint = d.Dewpt,
                    Humidity = d.Rel_Hum,
                    WindDirection = d.Wind_Dir,
                    WindSpeed = d.Wind_Spd_Kmh,
                    Pressure = d.Press_Msl,
                    WeatherCondition = d.Weather
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather data: {ex.Message}");
                return new List<WeatherObservation>();
            }
        }

        private DateTime ParseBoMDateTime(string? dateTimeString)
        {
            if (string.IsNullOrEmpty(dateTimeString))
                return DateTime.MinValue;

            if (dateTimeString.Length == 14 &&
                DateTime.TryParseExact(dateTimeString, "yyyyMMddHHmmss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            return DateTime.MinValue;
        }
    }
}
