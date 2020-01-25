using System;
using System.Text.RegularExpressions;
using ServiceStack;

namespace WeatherForecastCLI
{
    class Program
    {
        static string url = "https://api.openweathermap.org/data/2.5/weather?";
        static string key = "YOUR-KEY-HERE";
        static string extra = "&units=metric";
        static void Main(string[] args)
        {

            CheckCommandEmptyOrHelp(args);

            String query = ConstructQueryString(args);

            String finalUrl = url
                                  + query
                                  + "&appid="
                                  + key + extra;

            try
            {
                var WeatherDto = finalUrl.GetJsonFromUrl().FromJson<WeatherDto>();
                WriteToConsole(WeatherDto);
            }
            catch (Exception)
            {
                Console.WriteLine("An error occured while trying to pull your request");
                Console.WriteLine("Check lists of commands by typing h or check your connection");
            }
            Environment.Exit(0);

        }


        private static void WriteToConsole(WeatherDto wdto)
        {
            var nameOfCity = wdto.name;
            var temp = wdto.main.temp;
            var wind = wdto.wind.speed;
            var description = wdto.weather[0].main;
            Console.WriteLine($"Today in {nameOfCity} there will be wind of {wind} m/s with a temperature of {temp} C and {description}");
        }
        private static void CheckCommandEmptyOrHelp(string[] args)
        {
            if (args == null || args.Length == 0)
                Environment.Exit(0);
            if (args[0].ToLower().Equals("h") || args[0].ToLower().Equals("help"))
            {
                Console.WriteLine("commands: [cityname], [cityname country], [zipcode], [cityId], [lon lat]");
                Environment.Exit(0);
            }
        }
        private static string ConstructQueryString(string[] args)
        {

            if (args.Length == 0)
                return "";
            else if (args.Length == 1)
            {
                return GetQueryAndPrefix(args[0]);
            }
            else if (args.Length == 2)
            {
                return GetQueryAndPrefix(args[0], args[1]);
            }
            else
            {
                string query = $"#bbox={args[0]}";
                for (int i = 1; i < args.Length; i++)
                {
                    query += $",{args[i]}";
                }
                return query;

            }
        }
        private static string GetQueryAndPrefix(string arg) => Regex.IsMatch(arg, @"^\d+$") ? $"id={arg}" : $"q={arg}";

        private static string GetQueryAndPrefix(string arg1, string arg2)
        {

            if (Regex.IsMatch(arg1, @"^\d+$") && Regex.IsMatch(arg2, @"^\d+$"))
            {
                // long, lat 
                return $"lat={arg1}&lon={arg2}";

            }
            else if (Regex.IsMatch(arg1, @"^\d+$") && !Regex.IsMatch(arg2, @"^\d+$"))
            {
                //zip
                return $"zip={arg1},{arg2}";

            }
            else if (!Regex.IsMatch(arg2, @"^\d+$") && !Regex.IsMatch(arg2, @"^\d+$"))
            {
                // city name, countryISo
                return $"q={arg1},{arg2}";
            }
            return "";
        }
    }

}
