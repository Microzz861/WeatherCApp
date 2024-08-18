using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeatherLibrary;

namespace WeatherConsoleApp
{
    class Program
    {
        static readonly CancellationTokenSource c_token = new CancellationTokenSource();
        static async Task Main(string[] args)
        {
            int choice;
            Console.WriteLine("WELCOME TO THE WEATHER APP!");
            Console.WriteLine("What would you like to do?\n");
            Console.WriteLine("1) Find weather info using coordinates?\n");
            Console.WriteLine("2) Find weather info using city name?\n");
            Console.WriteLine("3) Exit the app.");
            string input1 = Console.ReadLine() ?? "";
            if (int.TryParse(input1, out choice))
            {
                switch (choice)
                {
                    case 1:
                        int choiceCoords;
                        Console.WriteLine("\n1) Find weather info for just one location.\n");
                        Console.WriteLine("2) Find weather info for multiple locations.");

                        //You are given the choice to retrieve info using one latitude and longitude or multiple.
                        string inputCoords = Console.ReadLine() ?? "";
                        //Null coalescing case for console.readline
                        if (int.TryParse(inputCoords, out choiceCoords))
                        {
                            if (choiceCoords == 1)
                            {
                                Console.WriteLine("Please input the latitude: ");
                                string lat = Console.ReadLine() ?? "";
                                Console.WriteLine("\nPlease input the longitude: ");
                                string lon = Console.ReadLine() ?? "";
                                Console.WriteLine();

                                if (lat != "" && lon != "")
                                {
                                    await WeatherClass.getCityWeatherInfoAsync(lat, lon, c_token.Token);
                                    //c_token, again, is a cancellation token.
                                }
                            }
                            else if (choiceCoords == 2)
                            {
                                List<string> Latitudes = new List<string>();
                                List<string> Longitudes = new List<string>();
                                while (true) //Loop that allows the user to keep inputting latitudes and longitudes until no is entered
                                {
                                    Console.WriteLine("\nPlease input the latitude: ");
                                    string lat = Console.ReadLine() ?? "";
                                    if (lat != "") Latitudes.Add(lat);
                                    Console.WriteLine("\nPlease input the longitude: ");
                                    string lon = Console.ReadLine() ?? "";
                                    if (lon != "") Longitudes.Add(lon);
                                    Console.WriteLine("\nWould you like to continue inputting coords? any input/no?");
                                    if (Console.ReadLine() == "no")
                                    {
                                        Console.WriteLine();
                                        break;
                                        //Breaks the loop
                                    }
                                }
                                if (Latitudes.Count() > 0 && Longitudes.Count() > 0)
                                {
                                    //Ensures that both arrays actually contain indexes
                                    Task cancelTask = Task.Run(() =>
                                    {
                                        c_token.CancelAfter(15000);

                                    });
                                    //This operation will cancel the data retrieval.
                                    //If the weather data isn't retrieved within 15 seconds.
                                    List<Task> finishedTasks = new List<Task>();
                                    foreach ((string lat, string lon) in Latitudes.Zip(Longitudes))
                                    {
                                        Task task = WeatherClass.getCityWeatherInfoAsync(lat, lon, c_token.Token);
                                        finishedTasks.Add(task);
                                    }
                                    //The zip methods binds each index of lat array to the indexes of lon array.
                                    while (finishedTasks.Any())
                                    {
                                        Task t = await Task.WhenAny(finishedTasks);
                                        await t;
                                        finishedTasks.Remove(t);
                                    }
                                    //Whenever any Task object has been completed for any of the locations (data retrieved), the data will automatically be displayed
                                }

                            }
                            else
                            {
                                Console.WriteLine("Invalid coords choice Selected! Exiting app");
                                //In case the user inputs a incorrect choice
                            }
                        }
                        break;

                    case 2:
                        int choiceName;
                        Console.WriteLine("\n1) Find weather info for just one location.\n");
                        Console.WriteLine("2) Find weather info for multiple locations.");
                        //You are given the choice to retrieve info for one city using city name or multiple.
                        string inputName = Console.ReadLine() ?? "";
                        if (int.TryParse(inputName, out choiceName))
                        {
                            if (choiceName == 1)
                            {
                                Console.WriteLine("\nPlease input the city name: ");
                                string name = Console.ReadLine() ?? "";
                                Console.WriteLine();

                                if (name != "" || name != null)
                                {
                                    await WeatherClass.getCityCoordsAsync(name, c_token.Token);
                                }
                            }
                            else if (choiceName == 2)
                            {
                                List<string> Names = new List<string>();

                                while (true)
                                {
                                    Console.WriteLine("\nPlease input the city name: ");
                                    string name = Console.ReadLine() ?? "";
                                    if (name != "") Names.Add(name);
                                    Console.WriteLine("\nWould you like to continue inputting city names? any input/no?");
                                    if (Console.ReadLine() == "no")
                                    {
                                        Console.WriteLine();
                                        break;
                                    }
                                }
                                if (Names.Count() > 0)
                                {
                                    Task cancelTask = Task.Run(() =>
                                    {
                                        c_token.CancelAfter(15000);

                                    });
                                    List<Task> finishedTasks = new List<Task>();
                                    foreach (string name in Names)
                                    {
                                        Task task = WeatherClass.getCityCoordsAsync(name, c_token.Token);
                                        finishedTasks.Add(task);
                                    }
                                    while (finishedTasks.Any())
                                    {
                                        Task t = await Task.WhenAny(finishedTasks);
                                        await t;
                                        finishedTasks.Remove(t);
                                    }
                                }

                            }
                            else
                            {
                                Console.WriteLine("Invalid Choice Selected! Exiting app");
                            }
                        }
                        break;
                    case 3:
                        Environment.Exit(0);
                        break;
                    //Exits the app
                    default:
                        Console.WriteLine("Invalid Choice Selected! Exiting app");
                        break;

                }


            }
        }

    }
}
