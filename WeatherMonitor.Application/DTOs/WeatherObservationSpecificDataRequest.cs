using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Application.DTOs
{
    public class WeatherObservationSpecificDataRequest
    {
        public List<string> RequestedDataTypes { get; set; } = new();
    }
}
