using Microsoft.Extensions.Logging;
using WeatherForecast.Constants;
using WeatherForecast.Helpers;
using WeatherForecast.Interfaces;
using WeatherForecast.Models;

namespace WeatherForecast.Services
{
    class HostedService
    {
        private readonly ILogger<HostedService> _logger;
        private readonly IHttpClientHandler _httpClientHandler;


        public HostedService(ILogger<HostedService> logger, IHttpClientHandler httpClientHandler)
        {
            _logger = logger;
            _httpClientHandler = httpClientHandler;
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken = default)
        {
            try
            {
                // Prompt the user to enter their name
                Console.WriteLine("Please enter your name:");
                string userName = Console.ReadLine().Trim();
                userName = string.IsNullOrEmpty(userName) ? "User" : userName;

                Console.WriteLine($"Hello, {userName}! Welcome to the weather forecast application.");

                string zipCode = string.Empty;
                string locationName = string.Empty;
                WeatherResponse weatherData = new();
                while (true)
                {
                    if (!string.IsNullOrEmpty(zipCode))
                    {
                        Console.WriteLine(("\n").PadRight(48, '-'));
                        ConsoleHelper.WriteLineWithColor($"\nYou are currently searching for the location: {locationName} [{zipCode}]", ConsoleColor.Blue);
                    }
                    else
                    {
                        Console.WriteLine("\nPlease enter your area Zipcode:");
                        zipCode = Console.ReadLine().Trim();

                        //validate on Zipcode until user enters a valid one
                        while (String.IsNullOrEmpty(zipCode))
                        {
                            ConsoleHelper.WriteLineWithColor("You need Zipcode to proceed further! Please enter your area Zipcode:", ConsoleColor.Red);
                            zipCode = Console.ReadLine().Trim();
                        }
                        weatherData = await FetchWeatherData(zipCode);
                        locationName = weatherData.Location?.Name;
                        if (!string.IsNullOrEmpty(locationName))
                        {
                            ConsoleHelper.WriteLineWithColor($"\nYou current location is {locationName}", ConsoleColor.Blue);
                        }
                    }

                    if (weatherData is null || weatherData.Current is null)
                    {
                        ConsoleHelper.WriteLineWithColor("No weather data found for the given zipcode. Please try again.", ConsoleColor.Red);
                        ConsoleHelper.WriteLineWithColor("***Only UK/Canada/US Zipcodes are allowed.", ConsoleColor.Magenta);
                        zipCode = string.Empty;
                        continue;
                    }

                    //print all the allowed questions. Can come from a config file or database later
                    PrintWeatherQuestions();

                    Console.WriteLine("Please enter your question number:");
                    string? selection = Console.ReadLine().Trim();

                    //validate on Zipcode until user enters a valid one
                    while (String.IsNullOrEmpty(selection) || !Constats.AllowedQuestionSelections.Contains(selection))
                    {
                        ConsoleHelper.WriteLineWithColor("Invalid selection! Please select one among the above questions", ConsoleColor.Red);
                        selection = Console.ReadLine().Trim();
                    }

                    ProcessWeatherData(weatherData, selection);

                    ConsoleHelper.WriteLineWithColor("\nYou want to try another location? [y,n]", ConsoleColor.Blue);
                    ConsoleHelper.WriteLineWithColor("Or you want to try another question in same location? [q]", ConsoleColor.Blue);
                    string? tryAnotherLocation = Console.ReadLine().Trim().ToLower();
                    if (tryAnotherLocation == Constats.TryAnotherLocationNo)
                    {
                        zipCode = string.Empty;
                        break;
                    }
                    else if (tryAnotherLocation == Constats.TryAnotherLocationYes)
                    {
                        zipCode = string.Empty;
                        continue;
                    }
                    else if (tryAnotherLocation == Constats.TrySameLocation)
                    {
                        continue;
                    }
                    else
                    {
                        ConsoleHelper.WriteLineWithColor("Invalid selection! Please enter 'y', 'n or 'q'", ConsoleColor.Red);
                    }
                    Console.ReadKey();
                }
            }
            catch (Exception ee)
            {
                //todo: log the exception to other sources later
                _logger.LogError(ee, ee.Message);
            }
        }

        private static void PrintWeatherQuestions()
        {
            Console.WriteLine();

            Console.WriteLine($"Please select one among the following questions: \n");
            Console.WriteLine($"Should I go outside? (enter 1)");
            Console.WriteLine($"Should I wear sunscreen? (enter 2)");
            Console.WriteLine($"Can I fly my kite? (enter 3)");
            Console.WriteLine($"Basic weather info at your location? (enter 4)");
            Console.WriteLine($"Full weather info at your location? (enter 5)\n");
        }

        private async Task<WeatherResponse> FetchWeatherData(string? zipCode)
        {
            //make api call
            return await _httpClientHandler.GetAsync<WeatherResponse>("WeaterService", $"current?access_key=$#ACCESS_KEY#$&query={zipCode}");
        }

        private static void ProcessWeatherData(WeatherResponse weatherData, string selection)
        {
            Console.WriteLine(("\n").PadRight(48, '-'));
            if (weatherData is not null && weatherData.Current is not null)
            {
                if (selection == "1")
                {
                    PrintRainInfo(weatherData);
                }
                if (selection == "2")
                {
                    PrintUVIndex(weatherData);
                }
                if (selection == "3")
                {
                    PrintWindInfo(weatherData);
                }
                if (selection == "4")
                {
                    PrintRainInfo(weatherData);
                    PrintUVIndex(weatherData);
                    PrintWindInfo(weatherData);
                }
                if (selection == "5")
                {
                    Console.WriteLine($"\nThe weather in {weatherData.Location?.Name} is {weatherData.Current?.Temperature} degrees.");
                    if (weatherData.Current?.WeatherDescriptions.Count > 0)
                    {
                        Console.WriteLine($"The weather description is {string.Join(",", weatherData.Current?.WeatherDescriptions)}");
                    }
                    Console.WriteLine($"The wind speed is {weatherData.Current?.WindSpeed} km/h");
                    Console.WriteLine($"The humidity is {weatherData.Current?.Humidity}%");
                    Console.WriteLine($"The UV index is {weatherData.Current?.UvIndex}");
                    Console.WriteLine($"The visibility is {weatherData.Current?.Visibility} km");
                    Console.WriteLine($"The cloud cover is {weatherData.Current?.Cloudcover}%");
                    Console.WriteLine($"The pressure is {weatherData.Current?.Pressure} mb");
                    Console.WriteLine($"The feels like temperature is {weatherData.Current?.Feelslike} degrees");
                    if (!string.IsNullOrEmpty(weatherData.Current?.ObservationTime))
                    {
                        Console.WriteLine($"The observation time is {weatherData.Current?.ObservationTime}");
                    }
                }
            }
            else
            {
                ConsoleHelper.WriteLineWithColor("No weather data found for the given zipcode. Please try again.", ConsoleColor.Red);
                ConsoleHelper.WriteLineWithColor("***Only UK/Canada/US Zipcodes are allowed.", ConsoleColor.Magenta);
            }
        }

        private static void PrintWindInfo(WeatherResponse weatherResponse)
        {
            var israining = weatherResponse.Current?.WeatherDescriptions.Contains("rain");
            var flyKites = !(israining.HasValue && israining.Value) && weatherResponse.Current?.WindSpeed > 15;

            if (flyKites)
            {
                ConsoleHelper.WriteLineWithColor("It is a good day to fly kites.", ConsoleColor.Green);
            }
            else
            {
                ConsoleHelper.WriteLineWithColor($"It is not a good day to fly kites. The wind speed is only {weatherResponse.Current?.WindSpeed} km/h", ConsoleColor.Red);
            }
        }

        private static void PrintUVIndex(WeatherResponse weatherResponse)
        {
            var wearSunscreen = weatherResponse.Current?.UvIndex > 3;

            if (wearSunscreen)
            {
                ConsoleHelper.WriteLineWithColor("High UV index. You should wear sunscreen today.", ConsoleColor.Red);
            }
            else
            {
                ConsoleHelper.WriteLineWithColor("Sun looks good today. You can skip sunscreen.", ConsoleColor.Green);
            }
        }

        private static void PrintRainInfo(WeatherResponse weatherResponse)
        {
            var israining = weatherResponse.Current?.WeatherDescriptions.Any(x => x.ToLower() == "rain");
            if (israining.HasValue && israining.Value)
            {
                ConsoleHelper.WriteLineWithColor("It is raining.You are advised not to go outside", ConsoleColor.Green);
            }
            else
            {
                var weatherInfoString = string.Join(",", weatherResponse.Current?.WeatherDescriptions);
                var rainInfo = "It is not raining" + (string.IsNullOrEmpty(weatherInfoString) ? "" : $" but {weatherInfoString}");
                ConsoleHelper.WriteLineWithColor(rainInfo, ConsoleColor.Green);
            }
        }
    }
}
