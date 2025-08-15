using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;

namespace WeatherMonitor.Infrastructure.Services
{
    public class BoMWeatherObservationRepository : IWeatherObservationDataRepository
    {
        public BoMWeatherObservationRepository()
        {
            
        }

        public Task<WeatherObservation?> GetLatestObservationAsync(string wmoId)
        {
            throw new NotImplementedException();
        }

        public Task<List<WeatherObservation>> GetWeatherObservationsAsync(string wmoId)
        {
            throw new NotImplementedException();
        }
    }
}
