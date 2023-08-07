using IntermediateAPI.Extensions;
using IntermediateAPI.Models;
using IntermediateAPI.Models.External;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace IntermediateAPI.Services
{
    public class AdapterService
    {
        private readonly HttpClient client;
        private readonly ExternalApisSettings settings;
        private readonly ILogger<AdapterService> logger;
        public AdapterService(IHttpClientFactory factory, IOptions<ExternalApisSettings> options, ILogger<AdapterService> logger)
        {
            client = factory.CreateClient();
            settings = options.Value;
            this.logger = logger;
            client.BaseAddress = new Uri(settings?.ApiBaseUrl ?? "");
        }

        public async Task<(bool successful, ExperianQuestions? response, ErrorResponse? error)> GetQuestions(ExperianUserProfile getQuestionsInput)
        {
            var result = await client.PostAsync<ExperianQuestions, ErrorResponse>("api/v1/B2C/getExperianQuestions", getQuestionsInput);
            
            return result;
        }

        public async Task<(bool successful, ExAnswerVerificationResponse? response, ErrorResponse? error)> SubmitAnswers(ValidateUserAnswersInput request)
        {
            var payload = new VerifyAnswersInput
            {
                SessionId = request.SessionId,
                AnswerIndex = request.AnswerIndex?.Split(',').Select(int.Parse).ToList(),
                B2CObjectId = request.B2CObjectId
            };

            var result = await client.PostAsync<ExAnswerVerificationResponse, ErrorResponse>("/api/v1/B2C/submitAnswersToExperianToLinkAccount", payload);
            return result;
        }

        public async Task<(bool successful, LinkAccountResponse? response, ErrorResponse? error)> LinkUserWithActivationCode(LinkAccountRequest request)
        {
            var result = await client.PostAsync<LinkAccountResponse, ErrorResponse>("/api/v1/B2C/linkAccountWithActivationCode", request);
            return result;
        }


        public async Task<(bool successful, UserProfile? response, ErrorResponse? error)> GetUserDetailsWithActivationCode(FetchUserDetailsInput getUserDetailsInput)
        {

            var result = await client.PostAsync<UserProfile, ErrorResponse>("/api/v1/B2C/getEpicDemographicsWithActivationCode", getUserDetailsInput);
            return result;
        }

        public async Task<(bool successful, UserCreatedResponse? response, ErrorResponse? error)> CreateUser(UserProfile validateUserDetailsInput)
        {
            var result = await client.PostAsync<UserCreatedResponse, ErrorResponse>("/api/v1/B2C/user/create", validateUserDetailsInput);

            return result;
        }

        public async Task<(bool successful, UserProfile? response, ErrorResponse? error)> GetUser(UserObjectId userObjectId)
        {
            var result = await client.PostAsync<UserProfile, ErrorResponse>("/api/v1/B2C/user/profile", userObjectId);

            return result;
        }

        #region API Calls

        private async Task<(bool successful, SuccessType? response, FailureType? error)> PostAsync<SuccessType, FailureType>(string endpoint, object? data)
        {
            try
            {
                //var url = $"{settings.ApiBaseUrl}/{endpoint}";
                //var jsonContent = JsonConvert.SerializeObject(data);
                logger.LogInformation("Sending Request to {endpoint}. Content: {@data}", endpoint, data);
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

                                var responseContent = await response.Content.ReadAsStringAsync();

                                if (string.IsNullOrEmpty(responseContent))
                                {
                                    return (true, default, default);
                                }

                                var responseBody = JsonConvert.DeserializeObject<SuccessType>(responseContent);
                                return (true, responseBody, default);

                            }
                            catch (Exception)
                            {
                                try
                                {
                                    var responseBody = await response.Content.ReadFromJsonAsync<FailureType>();
                                    logger.LogError($"Request was unsuccessful. Response: {responseBody}", responseBody);
                                    return (false, default, responseBody);

                                }
                                catch (Exception)
                                {
                                    var statusCode = response.StatusCode;
                                    var responseString = await response.Content.ReadAsStringAsync();
                                    logger.LogError($"Error: {statusCode}. The upstream server has returned an unexpected response: {responseString}", statusCode, responseString);
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
