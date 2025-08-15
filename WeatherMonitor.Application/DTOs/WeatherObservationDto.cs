using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Application.DTOs
{
    public class WeatherObservationDto
    {
        public string WmoId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public DateTime ObservationTime { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public double Pressure { get; set; }
        public string? Condition { get; set; }
        public string WindDirection { get; set; }
        public double? DewPoint { get; set; }
        public double? ApparentTemperature { get; set; }
    }
}
