using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherLibrary
{
    public static class WeatherClass
    {
        //getCityCoordsAsync retrieves the coordinates of a city given a city name as a parameter.
        //getCityCoordsAsync uses the name to fetch the coordinates from a website called openweathermap.
        //After retrieving the coords successfully, getCityCoordsAsync then calls the getCityWeatherInfoAsync to complete the user request.
        public async static Task getCityCoordsAsync(string name,CancellationToken c_token)
        {
            HttpClient client = new HttpClient();
            try
            {
                string response = await client.GetStringAsync($"http://api.openweathermap.org/geo/1.0/direct?q={name}&limit=1&appid=f969e333cfeeb731537640f27a2e9201",c_token);
                //appid is Nooraldein Asia's authentication token as openweathermap does not provide an api to retrieve a users token.
                //The weather info is retrieved in a string form.
                JArray cities = JArray.Parse(response);
                //JArray parses the retrieved data as a JSON array or array of JSON objectss.
                if (cities.Count() > 0)
                {
                    string latitude = cities[0]["lat"]?.ToString() ?? "Not";
                    string longitude = cities[0]["lon"]?.ToString() ?? "Not";
                    //If the latitude and longitude of the provided city name actually exists, initialize latitude and longitude with them.
                    //Else initialize latitude and longitude with "Not".

                    if (latitude is "Not" || longitude is "Not")
                    {
                        Console.WriteLine($"Coords could not be found for {name}");
                    }
                    else
                    {
                        await getCityWeatherInfoAsync(latitude, longitude, c_token);
                    }
                }

            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Coords retrieval has been cancelled.");
            }
            //In case a timeout occured after 15 seconds.
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //If the request is invalid
            catch (HttpRequestException)
            {
                Console.WriteLine($"Sorry but the coordinates for {name} does not exist in openweathermaps database!");
            }
            //If a city does not exist
        }

        //getCityWeatherInfoAsync uses the latitude and longitude of a location to retrieve its weather data.
        //The end result of getCityWeatherInfoAsync is printing the retrieved weather info.
        public async static Task getCityWeatherInfoAsync(string lat, string lon,CancellationToken token)
        {
            HttpClient client = new HttpClient();
            try
            {
                string response = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid=f969e333cfeeb731537640f27a2e9201",token);
                //The weather info is retrieved in a string form.
                JObject weatherInfo = JObject.Parse(response);
                //JObject parses the retrieved data as a JSON object.
                if(weatherInfo.Count > 0)
                 //If the data retrieved actually exists
                {
                    Console.WriteLine(
                        $"City Name: {weatherInfo["name"]}.\n" +
                        $"Current Temperature: {weatherInfo["main"]?["temp"]}.\n" +
                        $"Weather Condition: {weatherInfo["weather"]?[0]?["main"]}k\n" +
                        $"Weather Description: {weatherInfo["weather"]?[0]?["description"]}\n" +
                        $"Coords: Latitude: {weatherInfo["coord"]?["lat"]}, Longitude: {weatherInfo["coord"]?["lon"]}.\n\n"
                        );
                }


            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Weather Info retrieval has been cancelled.");
            }
            //In case a timeout occured after 15 seconds.
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
            }
            //If the request is invalid
            catch (HttpRequestException)
            {
                Console.WriteLine($"Sorry but a location for latitude: {lat}, longitude: {lon} does not exist in openweathermaps database!");
            }
            //If a location for coordinates do no exist in the database
            
        }
    }
}
