namespace WeatherForecast.Interfaces
{
    public interface IHttpClientHandler
    {
        Task<TResponse> GetAsync<TResponse>(string clientName, string relativeUri);
        Task<TResponse> PostAsync<TRequest, TResponse>(string clientName, TRequest request, string relativeUri);
        Task<TResponse> PutAsync<TRequest, TResponse>(string clientName, TRequest requestData, string apiEndpoint);
        Task<TResponse> DeleteAsync<TResponse>(string clientName, string relativeUri);

    }
}
