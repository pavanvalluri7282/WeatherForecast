namespace WeatherForecast.Models
{
    public class WeatherRequest
    {
        public string? Type { get; set; }
        public string? Query { get; set; }
        public string? Language { get; set; }
        public string? Unit { get; set; }
    }
}
