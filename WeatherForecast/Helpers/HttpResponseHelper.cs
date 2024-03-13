using Newtonsoft.Json;

namespace WeatherForecast.Helpers
{
    public static class HttpResponseHelper
    {
        public static async Task<T> DeserializeResponse<T>(HttpResponseMessage httpResponseMessage)
        {
            T response = default;

            string responseJson = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                if (!string.IsNullOrEmpty(responseJson))
                {
                    response = JsonConvert.DeserializeObject<T>(responseJson);
                }
            }
            else
            {
                throw new Exception($"{httpResponseMessage.StatusCode} - {responseJson}");
            }
            return response;
        }
    }
}
