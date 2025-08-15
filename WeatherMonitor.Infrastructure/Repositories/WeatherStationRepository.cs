using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;

namespace WeatherMonitor.Infrastructure.Repositories
{
    public class WeatherStationRepository : IWeatherStationRepository
    {
        public WeatherStationRepository()
        {
         
        }

        public Task<List<WeatherObservationStation>> GetAllStationAsync()
        {
            throw new NotImplementedException();
        }

        public Task<WeatherObservationStation?> GetStationByNameAsync(string country)
        {
            throw new NotImplementedException();
        }

        public Task<WeatherObservationStation?> GetStationByWmoIdAsync(string wmoId)
        {
            throw new NotImplementedException();
        }

        public Task<List<WeatherObservationStation>> GetStationsByStateAsync(string state)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsValidWmoAsync(string wmoId)
        {
            throw new NotImplementedException();
        }
    }
}
