using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherMonitor.Application.DTOs;
using WeatherMonitor.Application.Interfaces;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;

namespace WeatherMonitor.Application.Services
{
    public class WeatherObservationService : IWeatherObservationService
    {
        private readonly IWeatherObservationDataRepository _weatherObservationDataRepository; 
        private readonly IWeatherStationRepository _weatherStationRepository;
        public WeatherObservationService(IWeatherObservationDataRepository weatherObservationDataRepository, IWeatherStationRepository weatherStationRepository)
        {
            _weatherObservationDataRepository = weatherObservationDataRepository;
            _weatherStationRepository = weatherStationRepository;
        }

        public async Task<List<WeatherObservationDto>> GetAllWeatherDataAsync(string wmoId)
        {
            var station = await _weatherStationRepository.GetStationByWmoIdAsync(wmoId);
            if (station == null)
            {
                throw new ArgumentException($"Weather station with WMO ID '{wmoId}' not found", nameof(wmoId));
            }

            var observations = await _weatherObservationDataRepository.GetWeatherObservationsAsync(wmoId);

            return observations.Select(o => new WeatherObservationDto
            {
                StationName = o.StationName,
                WmoId = o.WmoId,
                ObservationTime = o.ObservationTime,
                Temperature = o.Temperature,
                ApparentTemperature = o.ApparentTemperature,
                DewPoint = o.DewPoint,
                Humidity = o.Humidity,
                WindDirection = o.WindDirection,
                WindSpeed = o.WindSpeed,
                Pressure = o.Pressure,
                WeatherCondition = o.WeatherCondition
            }).ToList();
        }

        public Task<WeatherObservationStation?> FindStationAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public async Task<WeatherObservationSummaryDto> GetWeatherSummaryAsync(string wmoId)
        {
            var observations = await _weatherObservationDataRepository.GetWeatherObservationsAsync(wmoId);
            var station = await _weatherStationRepository.GetStationByWmoIdAsync(wmoId);

            var averageTemp = await CalculateAverageTemperatureAsync(wmoId);

            return new WeatherObservationSummaryDto
            {
                StationName = station?.Name ?? "Unknown Station",
                WmoId = wmoId,
                AverageTemperature = averageTemp,
                ObservationCount = observations.Count
            };
        }

        public async Task<double> CalculateAverageTemperatureAsync(string wmoId, int hours = 72)
        {
            var observations = await _weatherObservationDataRepository.GetWeatherObservationsAsync(wmoId);
            var cutoffTime = DateTime.Now.AddHours(-hours);

            var recentObservations = observations
                .Where(o => o.ObservationTime >= cutoffTime && o.Temperature.HasValue)
                .ToList();

            if (!recentObservations.Any())
                return 0;

            return recentObservations.Average(o => o.Temperature!.Value);

        }

        public Task<List<WeatherObservationStation>> GetAvailableStationsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
