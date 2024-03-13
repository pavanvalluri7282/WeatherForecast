namespace WeatherForecast.Models
{
    public class WeatherResponse
    {
        public WeatherRequest? Request { get; set; }
        public Location? Location { get; set; }
        public CurrentWeather? Current { get; set; }
    }
}
