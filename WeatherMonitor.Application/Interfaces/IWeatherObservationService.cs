using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherMonitor.Application.DTOs;
using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.Application.Interfaces
{
    public interface IWeatherObservationService
    {
        Task<WeatherObservationSummaryDto> GetWeatherSummaryAsync(string wmoId);
        Task<List<WeatherObservationDto>> GetAllWeatherDataAsync(string wmoId);
        Task<double> CalculateAverageTemperatureAsync(string wmoId, int hours = 72);
        Task<WeatherObservationStation?> FindStationAsync(string searchTerm);
    }
}
