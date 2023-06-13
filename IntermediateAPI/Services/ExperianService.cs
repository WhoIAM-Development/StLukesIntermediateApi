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
        }
        public async Task<string> GetQuestions(UserInfo info)
        {
            var result = await PostAsync<ExperianData, ExperianErrorResponse>("/api/v2/sso/linkviaexperian", info);
        }
        public async Task<string> SubmitAnswers(ExperianAnswers answers)
        {
            var result = await PostAsync<ExperianVerificationResponse, ExperianErrorResponse>("/api/v2/sso/submitanswerstoexperian", answers);
        }

        #region API Calls
        private async Task<(SuccessType? result, FailureType? error, HttpStatusCode ResponseCode)> PostAsync<SuccessType, FailureType>(string endpoint, object data)
        {
            try
            {
                var url = $"{settings.ApiBaseUrl}/{endpoint}";
                //var jsonContent = JsonConvert.SerializeObject(data);
                logger.LogInformation("Sending Request to {url}. Content: {data}", url, data);
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
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
                                return (responseBody, default, response.StatusCode);

                            }
                            catch(Exception ex)
                            {
                                var responseBody = await response.Content.ReadFromJsonAsync<FailureType>();
                                return (default, responseBody, response.StatusCode);
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
        #endregion
    }
}
