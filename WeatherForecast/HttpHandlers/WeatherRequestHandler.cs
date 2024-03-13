using Microsoft.Extensions.Configuration;
using System.Net;

namespace WeatherForecast.HttpHandlers
{

    public class WeatherRequestHandler : DelegatingHandler
    {
        private readonly IConfiguration _config;

        public WeatherRequestHandler(IConfiguration config)
        {
            _config = config;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                //Replace access key with the actual access key
                request.RequestUri = new Uri(request.RequestUri.AbsoluteUri.Replace("$#ACCESS_KEY#$", _config["WeatherService:APIAccessKey"]));

                var response = await base.SendAsync(request, cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    return response;
                }
                throw new Exception($"Error processing request {request.RequestUri} returned {response.StatusCode} - {response.ReasonPhrase}");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
