using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.Core.Interfaces
{
    public interface IWeatherObservationDataRepository
    {
        Task<List<WeatherObservation>> GetWeatherObservationsAsync(string wmoId);

    }
}
