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

        public Task<WeatherObservationStation?> FindStationAsync(string searchTerm)
        {
            throw new NotImplementedException();
        }

        public Task<List<WeatherObservationDto>> GetAllWeatherDataAsync(string wmoId)
        {
            throw new NotImplementedException();
        }

        public Task<WeatherObservationSummaryDto> GetWeatherSummaryAsync(string wmoId)
        {
            throw new NotImplementedException();
        }

        public Task<double> CalculateAverageTemperatureAsync(string wmoId, int hours = 72)
        {
            throw new NotImplementedException();
        }

        public Task<List<WeatherObservationStation>> GetAvailableStationsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
