using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherMonitor.Infrastructure.Models
{
    public class BoMWeatherResponse
    {
        public BoMObservations? Observations { get; set; }
    }

    public class BoMObservations
    {
        public List<BoMWeatherData>? Data { get; set; }
        public List<BoMNotice>? Notice { get; set; }
    }

    public class BoMWeatherData
    {
        public int? Sort_Order { get; set; }
        public int? Wmo { get; set; }
        public string Name { get; set; }
        public string History_Product { get; set; }
        public string Local_Date_Time { get; set; }
        public string Local_Date_Time_Full { get; set; }
        public string Aifstime_Utc { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public double? Apparent_T { get; set; }
        public string Cloud { get; set; }
        public int? Cloud_Base_M { get; set; }
        public int? Cloud_Oktas { get; set; }
        public int? Cloud_Type_Id { get; set; }
        public string Cloud_Type { get; set; }
        public double? Delta_T { get; set; }
        public int? Gust_Kmh { get; set; }
        public int? Gust_Kt { get; set; }
        public double? Air_Temp { get; set; }
        public double? Dewpt { get; set; }
        public double? Press { get; set; }
        public double? Press_Qnh { get; set; }
        public double? Press_Msl { get; set; }
        public string Press_Tend { get; set; }
        public string Rain_Trace { get; set; }
        public int? Rel_Hum { get; set; }
        public string Sea_State { get; set; }
        public string Swell_Dir_Worded { get; set; }
        public object Swell_Height { get; set; }
        public object Swell_Period { get; set; }
        public string Vis_Km { get; set; }
        public string Weather { get; set; }
        public string Wind_Dir { get; set; }
        public int? Wind_Spd_Kmh { get; set; }
        public int? Wind_Spd_Kt { get; set; }


    }

    public class BoMNotice
    {
        public string? Copyright { get; set; }
        public string? Copyright_Url { get; set; }
        public string? Disclaimer_Url { get; set; }
        public string? Feedback_Url { get; set; }
    }
}
