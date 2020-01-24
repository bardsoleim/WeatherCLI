using System;
using System.Text.RegularExpressions;
using ServiceStack;

namespace WeatherForecastCLI
{
    class Program
    {
       static string url = "https://api.openweathermap.org/data/2.5/weather?";
       static string key = "your-api-key-here";
       static string extra = "&units=metric";
        static void Main(string[] args)
        {
            if(args == null || args.Length == 0)
                 return;
             if(args[0].ToLower().Equals("h") || args[0].ToLower().Equals("help")){
                 Console.WriteLine("commands: [cityname], [cityname country], [zipcode], [cityId], [lon lat]");
                 return;
                }
         String query = ConstructQueryString(args);
            
         String finalUrl = url
                               + query
                               + "&appid="
                               + key + extra; 
            
         var WeatherDto =  finalUrl.GetJsonFromUrl().FromJson<WeatherDto>();
        
          WriteToConsole(WeatherDto);
        }


        private static void WriteToConsole(WeatherDto wdto){
            var NameOfCity = wdto.name;
            var Temp = wdto.main.temp;
            var Wind = wdto.wind.speed;
            var desciption =  wdto.weather[0].main;
            Console.WriteLine("Today in {0} there will be wind of {1} m/s with a temperature of {2} C and {3}", NameOfCity, Wind, Temp, desciption);
        }

        private static string ConstructQueryString(string[] args){
            
            if(args.Length == 0)
            return "";
            else if(args.Length == 1){
                return GetQueryAndPrefix(args[0]);
            }
            else if(args.Length == 2){
                return GetQueryAndPrefix(args[0], args[1]);
            }
            else {
                string query = "bbox=" + args[0];
                for(int i = 1; i < args.Length; i++){
                 query+=","+ args[i];
                }
                return query;

            }
        }
        private static string GetQueryAndPrefix(string arg) => Regex.IsMatch(arg, @"^\d+$") ? "id=" : "q=" + arg;
        private static string GetQueryAndPrefix(string arg1, string arg2){
            
             if(Regex.IsMatch(arg1, @"^\d+$") && Regex.IsMatch(arg2, @"^\d+$")){
                return "lat="+arg1+"&lon="+arg2;
                  // long, lat ?
             }
             else  if(Regex.IsMatch(arg1, @"^\d+$") && !Regex.IsMatch(arg2, @"^\d+$")){
                    //zip
                    return "zip="+ arg1+","+ arg2;

             }
             else  if(!Regex.IsMatch(arg2, @"^\d+$")&& !Regex.IsMatch(arg2, @"^\d+$")){
                     // city name, countryISo
                     return "q=" + arg1 + "," + arg2;
              }
              return "";
        }
    }

}
