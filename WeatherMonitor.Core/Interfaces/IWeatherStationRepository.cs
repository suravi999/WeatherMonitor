using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.Core.Interfaces
{
    public interface IWeatherStationRepository
    {
        Task<List<WeatherObservationStation>> GetAllStationAsync();
        Task<WeatherObservationStation?> GetStationByWmoIdAsync(string wmoId);
        Task<List<WeatherObservationStation>> GetStationsByStateAsync(string state);
        Task<WeatherObservationStation?> GetStationByNameAsync(string country);
        Task<bool> IsValidWmoAsync(string wmoId);
    }
}
