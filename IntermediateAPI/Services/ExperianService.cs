using IntermediateAPI.Models.UserValidation;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace IntermediateAPI.Services
{
    public class ExperianService
    {
        private readonly HttpClient client;
        private readonly ExperianSettings settings;
        private readonly ILogger<ExperianService> logger;
        public ExperianService(IHttpClientFactory factory, IOptions<ExperianSettings> options, ILogger<ExperianService> logger)
        {
            client = factory.CreateClient();
            settings = options.Value;
            this.logger = logger;
            client.BaseAddress = new Uri(settings?.ApiBaseUrl ?? "");
        }
        public async Task<(bool successful, ExperianData? response, ExperianErrorResponse? error)> GetQuestions(UserInfo? info)
        {
            var result = await PostAsync<ExperianData, ExperianErrorResponse>("api/v2/sso/linkviaexperian", info);
            return result;
        }
        public async Task<(bool successful, ExperianVerificationResponse? response, ExperianErrorResponse? error)> SubmitAnswers(ExperianAnswers? answers)
        {
            var result = await PostAsync<ExperianVerificationResponse, ExperianErrorResponse>("api/v2/sso/submitanswerstoexperian", answers);
            return result;
        }

        #region API Calls
        private async Task<(bool successful, SuccessType? response, FailureType? error)> PostAsync<SuccessType, FailureType>(string endpoint, object? data)
        {
            try
            {
                //var url = $"{settings.ApiBaseUrl}/{endpoint}";
                //var jsonContent = JsonConvert.SerializeObject(data);
                logger.LogInformation("Sending Request to {url}. Content: {data}", endpoint, data);
                using (var request = new HttpRequestMessage(HttpMethod.Post, endpoint))
                {
                    using (var content = JsonContent.Create(data))
                    {
                        request.Content = content;
                        using (HttpResponseMessage response = await client.SendAsync(request))
                        {
                            try
                            {
                                response.EnsureSuccessStatusCode();
                                var responseBody = await response.Content.ReadFromJsonAsync<SuccessType>();
                                return (true, responseBody, default);

                            }
                            catch(Exception)
                            {
                                try
                                {
                                    var responseBody = await response.Content.ReadFromJsonAsync<FailureType>();
                                    logger.LogError("Request was unsuccessful. Response: {responseBody}", responseBody);
                                    return (false, default, responseBody);

                                }
                                catch (Exception)
                                {
                                    var statusCode = response.StatusCode;
                                    var responseString = await response.Content.ReadAsStringAsync();
                                    logger.LogError("Error: {statusCode}. The upstream server has returned an enexpected response: {responseString}", statusCode, responseString);
                                    throw;
                                }
                            }
                        }
                    }
                }

            }
            catch
            {
                throw;
            }
        }
        private async Task<(bool successful, SuccessType? response, object? error)> PostAsync<SuccessType>(string endpoint, object data)
        {
            return await PostAsync<SuccessType, object>(endpoint, data);
        }
        #endregion
    }
}
