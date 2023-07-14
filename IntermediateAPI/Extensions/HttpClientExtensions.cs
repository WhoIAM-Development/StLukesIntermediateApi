using Azure;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace IntermediateAPI.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<(bool successful, SuccessType? response, FailureType? error)> PostAsync<SuccessType, FailureType>(
            this HttpClient client,
            string endpoint,
            object? data)
        {
            return await SendAsync<SuccessType, FailureType>(client, HttpMethod.Post, endpoint, data);
        }

        public static async Task<(bool successful, SuccessType? response, FailureType? error)> GetAsync<SuccessType, FailureType>(
            this HttpClient client,
            string endpoint,
            object? data)
        {
            return await SendAsync<SuccessType, FailureType>(client, HttpMethod.Get, endpoint, data);
        }

        public static async Task<(bool successful, SuccessType? response, FailureType? error)> PutAsync<SuccessType, FailureType>(
            this HttpClient client,
            string endpoint,
            object? data)
        {
            return await SendAsync<SuccessType, FailureType>(client, HttpMethod.Put, endpoint, data);
        }

        private static async Task<(bool successful, SuccessType? response, FailureType? error)> SendAsync<SuccessType, FailureType>(
            HttpClient client,
            HttpMethod method,
            string endpoint,
            object? data)
        {
            try
            {
                Log.Logger.Information("Sending Request to {endpoint}. Content: {@data}", endpoint, data);
                using var request = new HttpRequestMessage(method, endpoint);
                using var content = JsonContent.Create(data);
                request.Content = content;
                using HttpResponseMessage response = await client.SendAsync(request);
                try
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Log.Logger.Information("Received Respose Content: {responseContent}", responseContent);

                        if (string.IsNullOrEmpty(responseContent))
                        {
                            return (true, default, default);
                        }

                        var responseBody = JsonConvert.DeserializeObject<SuccessType>(responseContent);
                        return (true, responseBody, default);
                    }
                    else
                    {
                        var responseBody = await response.Content.ReadFromJsonAsync<FailureType>();
                        Log.Logger.Warning("Request was unsuccessful. Response: {@responseBody}", responseBody);
                        return (false, default, responseBody);
                    }
                }
                catch (Exception)
                {
                    var statusCode = response.StatusCode;
                    var responseString = await response.Content.ReadAsStringAsync();
                    Log.Logger.Error("Error: {statusCode}. The upstream server has returned an unexpected response: {responseString}", statusCode, responseString);
                    throw;
                }
            }
            catch
            {
                throw;
            }
        }      
    }
}
