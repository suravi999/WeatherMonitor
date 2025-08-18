using System.Text;
using System.Text.Json;
using WeatherMonitor.Application.DTOs;
using WeatherMonitor.Core.Entities;

namespace WeatherMonitor.ConsoleApp
{
    class Program
    {
        private const string DefaultWmoId = "94672"; // Adelaide Airport by default
        private const string API_BASE_WEATHER = "https://localhost:44303"; // backend WebAPI base URL, if profile https use "https://localhost:7131"

        private static readonly HttpClient _httpClient = new();
        private static string _apiWeatherUrl = API_BASE_WEATHER + "/api/WeatherObservation";

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Weather Monitor Console Application ===\n");

            if(args.Length == 1 && (args[0].ToLower() == "--help" || args[0].ToLower() == "-h"))
            {
                await Help();//print help details
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            try
            {
                string wmoId = await GetWmoIdFromArgs(args);
                await DisplayWeatherSummary(wmoId, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static async Task<string> GetWmoIdFromArgs(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No WMO ID provided, using default: Adelaide Airport (94672)\n");
                return DefaultWmoId;
            }

            string input = args[0];

            //Try to find station by searching via API
            try
            {
                var response = await _httpClient.GetAsync($"{_apiWeatherUrl}/stations/search/{input}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var station = JsonSerializer.Deserialize<WeatherObservationStation>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (station != null)
                    {
                        Console.WriteLine($"Found station: {station.Name} (WMO: {station.WmoId})\n");
                        return station.WmoId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API search failed: {ex.Message}");
            }

            //If not found, assume it is a WMO ID
            Console.WriteLine($"Using WMO ID: {input}\n");
            return input;
        }

        private static async Task DisplayWeatherSummary(string wmoId, string[] args)
        {
            Console.WriteLine("Retrieving weather data from API...\n");

            try
            {
                //Determine what data to request based on command line arguments
                var requestData = BuildWeatherDataRequest(args);

                //Make POST request to get weather data
                var response = await PostWeatherDataRequest(wmoId, requestData);

                if (response != null)
                {
                    DisplayWeatherResponse(response, requestData.TimeRangeHours);
                }
                else
                {
                    Console.WriteLine("Failed to retrieve weather data");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Network error calling API: {ex.Message}");
                Console.WriteLine($"Make sure the WebAPI is running on {API_BASE_WEATHER}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static WeatherObservationSummaryDataRequest BuildWeatherDataRequest(string[] args)
        {
            var request = new WeatherObservationSummaryDataRequest
            {
                TimeRangeHours = 72, //Default 72 hours
                AdditionalFields = new List<string>()
            };

            //read command line arguments for additional options
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--detailed":
                        //Request all available fields
                        request.AdditionalFields = new List<string>
                        {
                            "temperature", "humidity", "pressure", "wind", "condition",
                            "apparent", "dewpoint"
                        };
                        break;

                    case "--temperature":
                        request.AdditionalFields.Add("temperature");
                        break;

                    case "--humidity":
                        request.AdditionalFields.Add("humidity");
                        break;

                    case "--pressure":
                        request.AdditionalFields.Add("pressure");
                        break;

                    case "--wind":
                        request.AdditionalFields.Add("wind");
                        request.AdditionalFields.Add("winddirection");
                        break;

                    case "--condition":
                        request.AdditionalFields.Add("condition");
                        break;

                    case "--hours":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int hours))
                        {
                            request.TimeRangeHours = hours;
                            i++; // need to skip next argument since it's the value
                        }
                        break;
                }
            }

            return request;
        }

        private static async Task<WeatherObservationSummaryDataResponse?> PostWeatherDataRequest(string wmoId, WeatherObservationSummaryDataRequest requestData)
        {
            try
            {
                var json = JsonSerializer.Serialize(requestData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_apiWeatherUrl}/{wmoId}/summary", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API Error ({response.StatusCode}): {errorContent}");
                    return null;
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<WeatherObservationSummaryDataResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error making POST request: {ex.Message}");
                return null;
            }
        }

        private static void DisplayWeatherResponse(WeatherObservationSummaryDataResponse response, int timeRangeHours)
        {
            Console.WriteLine("=== Weather Data Response ===");

            //Display required default fields
            Console.WriteLine($"Station Name: {response.StationName}");
            Console.WriteLine($"WMO ID: {response.WmoId}");
            Console.WriteLine($"Average Temperature ({timeRangeHours}h): {response.AverageTemperature:F1}°C");
            Console.WriteLine($"Total Observations: {response.ObservationCount}");

            //Display additional requested data
            if (response.AdditionalRequestedFields.Any())
            {
                Console.WriteLine("\n=== Additional Weather Data ===");
                foreach (var field in response.AdditionalRequestedFields)
                {
                    var value = FormatFieldValue(field.Key, field.Value);
                    Console.WriteLine($"{FormatFieldName(field.Key)}: {value}");
                }
            }

            //Show any unavailable fields that were requested
            if (response.UnavailableRequestedFields.Any())
            {
                Console.WriteLine($"\nUnavailable fields: {string.Join(", ", response.UnavailableRequestedFields)}");
            }

            //Display available stations by calling API
            Task.Run(async () => await DisplayAvailableStations());
        }

        private static string FormatFieldName(string fieldName)
        {
            return fieldName.ToLower() switch
            {
                "temperature" or "temp" => "Current Temperature",
                "humidity" or "humid" => "Humidity",
                "pressure" or "press" => "Pressure",
                "wind" => "Wind Speed",
                "winddirection" or "winddir" => "Wind Direction",
                "condition" or "weather" => "Weather Condition",
                "apparent" or "apptemp" => "Apparent Temperature",
                "dewpoint" or "dew" => "Dew Point",
                _ => fieldName.ToString()
            };
        }

        private static string FormatFieldValue(string fieldName, object? value)
        {
            if (value == null) return "N/A";

            return fieldName.ToLower() switch
            {
                "temperature" or "temp" or "apparent" or "dewpoint" => $"{value:F1}°C",
                "humidity" => $"{value}%",
                "pressure" => $"{value} hPa",
                "wind" => $"{value} km/h",
                _ => value.ToString() ?? "N/A"
            };
        }

        private static async Task Help() 
        {
            Console.WriteLine("\n=== Usage Instructions ===");
            Console.WriteLine("WeatherMonitor.ConsoleApp.exe [STATION] [OPTIONS]");
            Console.WriteLine("\nSTATION:");
            Console.WriteLine("  94672                    - Use WMO ID");
            Console.WriteLine("  \"Adelaide Airport\"       - Use station name");
            Console.WriteLine("\nOPTIONS:");
            Console.WriteLine("  --detailed           - Show all available data");
            Console.WriteLine("  --temperature        - Show temperature data");
            Console.WriteLine("  --humidity           - Show humidity data");
            Console.WriteLine("  --pressure           - Show pressure data");
            Console.WriteLine("  --wind,              - Show wind data");
            Console.WriteLine("  --condition          - Show weather condition");
            Console.WriteLine("  --hours 72              - Set time range in hours");
            Console.WriteLine("\nEXAMPLES:");
            Console.WriteLine("  WeatherMonitor.ConsoleApp.exe");
            Console.WriteLine("  WeatherMonitor.ConsoleApp.exe 94672 --detailed");
            Console.WriteLine("  WeatherMonitor.ConsoleApp.exe \"Adelaide Airport\" --wind --humidity --pressure");
            Console.WriteLine("  WeatherMonitor.ConsoleApp.exe 94672 --hours 24");

            //Display available stations by calling API
            await DisplayAvailableStations();
        }

        private static async Task DisplayAvailableStations()
        {
            try
            {
                Console.WriteLine("\n=== Available Weather Stations ===");

                var stationsResponse = await _httpClient.GetAsync($"{_apiWeatherUrl}/stations");
                if (stationsResponse.IsSuccessStatusCode)
                {
                    var stationsJson = await stationsResponse.Content.ReadAsStringAsync();
                    var stations = JsonSerializer.Deserialize<List<WeatherObservationStation>>(stationsJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (stations != null)
                    {
                        foreach (var station in stations)
                        {
                            Console.WriteLine($"• {station.Name} (WMO: {station.WmoId})");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not retrieve stations: {ex.Message}");
            }
        }
    }
}
