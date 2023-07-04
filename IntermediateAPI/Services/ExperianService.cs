using IntermediateAPI.Models;
using IntermediateAPI.Models.External.Experian;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

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
        public async Task<(bool successful, ExVerificationQuestionsResponse? response, ExGeneralErrorResponse? error)> GetQuestions(GetVerificationQuestionsInput getQuestionsInput)
        {
            var payload = new
            {
                getQuestionsInput.FirstName,
                getQuestionsInput.MiddleName,
                getQuestionsInput.LastName,
                getQuestionsInput.StateProvinceCode,
                getQuestionsInput.DateOfBirth,
                getQuestionsInput.PhoneNumber,
                getQuestionsInput.Ssn,
                getQuestionsInput.Street,
                getQuestionsInput.Street2,
                getQuestionsInput.City,
                getQuestionsInput.ZipCode,
                getQuestionsInput.Email,
                getQuestionsInput.State,
                getQuestionsInput.Gender,
                getQuestionsInput.ObjectId
            };

            var result = await PostAsync<ExVerificationQuestionsResponse, ExGeneralErrorResponse>("api/v2/sso/linkviaexperian", payload);
            return result;
        }
        public async Task<(bool successful, ExAnswerVerificationResponse? response, ExGeneralErrorResponse? error)> SubmitAnswers(ValidateUserAnswersInput validateAnswersInput)
        {
            var payload = new
            {
                validateAnswersInput.SessionId,
                AnswerIndex = validateAnswersInput.AnswerIndex?.Split(',').Select(int.Parse).ToList(),
                validateAnswersInput.ProfileId,
                validateAnswersInput.ObjectId
            };

            var result = await PostAsync<ExAnswerVerificationResponse, ExGeneralErrorResponse>("api/v2/sso/submitanswerstoexperian", payload);
            return result;
        }

        public async Task<(bool successful, ExUserInfo? response, ExGeneralErrorResponse? error)> GetUserDetails(GetUserDetailsInput getUserDetailsInput)
        {
            var payload = new
            {
                DateOfBirth = getUserDetailsInput.DateofBirth,
                MyChartActivationCode = getUserDetailsInput.ActivationCode
            };

            var result = await PostAsync<ExUserInfo, ExGeneralErrorResponse>("api/v2/sso/fetchuserepicprofiledetails", payload);
            return result;
        }

        public async Task<(bool successful, ExValidateUserDetailsResult? response, ExGeneralErrorResponse? error)> ValidateUserDetails(ValidateUserDetailsInput validateUserDetailsInput)
        {
            var payload = new
            {
                validateUserDetailsInput.FirstName,
                validateUserDetailsInput.MiddleName,
                validateUserDetailsInput.LastName,
                validateUserDetailsInput.StateProvinceCode,
                validateUserDetailsInput.DateOfBirth,
                validateUserDetailsInput.PhoneNumber,
                validateUserDetailsInput.Ssn,
                validateUserDetailsInput.Street,
                validateUserDetailsInput.Street2,
                validateUserDetailsInput.City,
                validateUserDetailsInput.ZipCode,
                validateUserDetailsInput.Email,
                validateUserDetailsInput.State,
                validateUserDetailsInput.Gender
            };

            var result = await PostAsync<ExValidateUserDetailsResult, ExGeneralErrorResponse>("api/v3/user/validate", payload);
            return result;
        }

        public async Task<(bool successful, ExGeneralErrorResponse? error)> CreateUserDetails(CreateUserInput validateUserDetailsInput)
        {
            var payload = new
            {
                validateUserDetailsInput.FirstName,
                validateUserDetailsInput.MiddleName,
                validateUserDetailsInput.LastName,
                validateUserDetailsInput.StateProvinceCode,
                validateUserDetailsInput.DateOfBirth,
                validateUserDetailsInput.PhoneNumber,
                validateUserDetailsInput.Ssn,
                validateUserDetailsInput.Street,
                validateUserDetailsInput.Street2,
                validateUserDetailsInput.City,
                validateUserDetailsInput.ZipCode,
                validateUserDetailsInput.Email,
                validateUserDetailsInput.State,
                validateUserDetailsInput.Gender
            };

            var result = await PostAsync<dynamic, ExGeneralErrorResponse>("api/v3/user/create", payload);
            return (result.successful, result.error);
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
