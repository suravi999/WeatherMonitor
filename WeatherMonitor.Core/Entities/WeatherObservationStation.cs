using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Core.Entities
{
    public class WeatherObservationStation
    {
        public string WmoId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
