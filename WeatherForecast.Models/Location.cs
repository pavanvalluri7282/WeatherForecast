namespace WeatherForecast.Models
{
    public class Location
    {
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? Lat { get; set; }
        public string? Lon { get; set; }
        public string? TimezoneId { get; set; }
        public string? Localtime { get; set; }
        public long LocaltimeEpoch { get; set; }
        public string? UtcOffset { get; set; }
    }
}
