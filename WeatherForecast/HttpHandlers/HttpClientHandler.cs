using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using WeatherForecast.Helpers;
using WeatherForecast.Interfaces;

namespace WeatherForecast.HttpHandlers
{
    public class HttpClientHandler : IHttpClientHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HttpClientHandler> _logger;


        public HttpClientHandler(IHttpClientFactory httpClientFactory, ILogger<HttpClientHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        #region Get

        public async Task<TResponse> GetAsync<TResponse>(string clientName, string relativeUri)
        {
            try
            {
                _logger.LogInformation($"Get data request received for {clientName}");

                var httpClient = _httpClientFactory.CreateClient(clientName);

                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(relativeUri, UriKind.Relative)
                };

                var httpResponseMessage = await httpClient.SendAsync(request);
                return await HttpResponseHelper.DeserializeResponse<TResponse>(httpResponseMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        #endregion

        #region Post

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string clientName, TRequest requestData, string apiEndpoint)
        {
            try
            {
                _logger.LogInformation($"Insert data request received for {clientName}");

                var httpClient = _httpClientFactory.CreateClient(clientName);
                var content = JsonConvert.SerializeObject(requestData, Formatting.None);
                var json = new StringContent(content, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(apiEndpoint, UriKind.Relative),
                    Content = json
                };

                var httpResponseMessage = await httpClient.SendAsync(request);
                return await HttpResponseHelper.DeserializeResponse<TResponse>(httpResponseMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        #endregion

        #region Put

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string clientName, TRequest requestData, string apiEndpoint)
        {
            try
            {
                _logger.LogInformation($"Insert data request received for {clientName}");

                var httpClient = _httpClientFactory.CreateClient(clientName);
                var content = JsonConvert.SerializeObject(requestData, Formatting.None);
                var json = new StringContent(content, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(apiEndpoint, UriKind.Relative),
                    Content = json
                };

                var httpResponseMessage = await httpClient.SendAsync(request);
                return await HttpResponseHelper.DeserializeResponse<TResponse>(httpResponseMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        #endregion

        #region Delete

        public async Task<TResponse> DeleteAsync<TResponse>(string clientName, string relativeUri)
        {
            try
            {
                _logger.LogInformation($"Insert data request received for {clientName}");

                var httpClient = _httpClientFactory.CreateClient(clientName);

                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(relativeUri, UriKind.Relative),
                };

                var httpResponseMessage = await httpClient.SendAsync(request);
                return await HttpResponseHelper.DeserializeResponse<TResponse>(httpResponseMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        #endregion

    }
}
