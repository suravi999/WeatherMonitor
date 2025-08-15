using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Core.Entities
{
    public class WeatherObservation
    {
        public string WmoId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public DateTime ObservationTime { get; set; }
        public double? Temperature { get; set; }
        public double? ApprentTemperature { get; set; }
        public double? Humidity { get; set; }
        public double? WindSpeed { get; set; }
        public double? Pressure { get; set; }
        public string WindDirection { get; set; }
        public string? WeatherCondition { get; set; }
        public double? DewPoint { get; set; }
    }
}
