using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WeatherMonitor.Core.Entities;
using WeatherMonitor.Core.Interfaces;

namespace WeatherMonitor.Infrastructure.Repositories
{
    public class WeatherStationRepository : IWeatherStationRepository
    {
        private readonly List<WeatherObservationStation> _stations;
        public WeatherStationRepository()
        {
            _stations = LoadObservationStations();
        }

        public async Task<WeatherObservationStation?> GetStationByNameAsync(string name)
        {
            await Task.CompletedTask;//For async consistency
            return _stations.FirstOrDefault(s =>
                s.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<WeatherObservationStation?> GetStationByWmoIdAsync(string wmoId)
        {
            await Task.CompletedTask;
            return _stations.FirstOrDefault(s => s.WmoId == wmoId);
        }


        public async Task<bool> IsValidWmoAsync(string wmoId)
        {
            await Task.CompletedTask;
            return _stations.Any(s => s.WmoId == wmoId);
        }

        public async Task<List<WeatherObservationStation>> GetAllStationsAsync()
        {
            await Task.CompletedTask; 
            return _stations.ToList();
        }

        private List<WeatherObservationStation> LoadObservationStations()
        {
            return new List<WeatherObservationStation>
            {
                new()
                {
                    Name = "Adelaide Airport",
                    WmoId = "94672"
                },
                new()
                {
                    Name = "Edinburgh",
                    WmoId = "95676"
                },
                new()
                {
                    Name = "Hindmarsh Island",
                    WmoId = "94677"
                },
                new()
                {
                    Name = "Kuitpo",
                    WmoId = "94683"
                }
            };
        }
    }
}
