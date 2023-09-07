using IntermediateAPI.Extensions;
using IntermediateAPI.Models;
using IntermediateAPI.Models.External.Dfp;
using IntermediateAPI.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace IntermediateAPI.Services
{
    public class DfpService
    {
        private readonly FraudProtectionSettings fraudProtectionSettings;
        private readonly HttpClient client;
        private readonly IAuthProvider dfpAuthService;
        private readonly ILogger<DfpService> logger;
        public string NewCorrelationId => Guid.NewGuid().ToString();

        public DfpService(
            IOptions<FraudProtectionSettings> fraudProtectionSettings,
            IAuthProvider dfpAuthService,
            ILogger<DfpService> logger,
            System.Net.Http.IHttpClientFactory factory)
        {
            this.fraudProtectionSettings = fraudProtectionSettings.Value;
            this.dfpAuthService = dfpAuthService;
            this.logger = logger;
            client = factory.CreateClient();
        }

        public async Task<ApiResponse<DfpAccountActionResponse>> CreateAccount(DfpCreateAccountInputClaims input, string correlationId, string signUpId)
        {
            var endpoint = $"/v1.0/action/account/create/{signUpId}";

            var createAccountInput = new
            {
                Metadata = new
                {
                    TrackingId = Guid.NewGuid().ToString(),
                    SignUpId = signUpId,
                    CustomerLocalDate = DateTime.Now,
                    MerchantTimeStamp = DateTime.Now,
                    AssessmentType = "Protect"
                },
                User = new
                {
                    Username = input.Email,
                    input.FirstName,
                    input.LastName,
                    input.Language,
                    UserType = "Consumer",
                },
                Email = new[] {
                    new
                    {
                        EmailValue= input.Email,
                        input.IsEmailValidated,
                        input.IsEmailUsername,
                        EmailType= "Primary"
                    }
                },
                Device = new
                {
                    input.IpAddress,
                    input.DeviceContextId,
                    Provider = "DFPFingerprinting"
                },

                Name = "AP.AccountCreation",
                Version = "0.5"
            };

            if (!string.IsNullOrWhiteSpace(input?.DisplayName))
            {
                createAccountInput.AddProperty("SsoAuthenticationProvider", new
                {
                    input.DisplayName,
                    AuthenticationProvider = input.DisplayName
                });
            }

            return await PostAsync<DfpAccountActionResponse>(endpoint, createAccountInput, correlationId);
        }

        public async Task<ApiResponse<DfpAccountStatusResponse>> CreateAccountStatus(DfpCreateAccountStatusInputClaims input, string correlationId)
        {
            var endpoint = $"/v1.0/observe/account/create/status/{input.SignUpId}";

            var createAccountInput = new
            {
                Metadata = new
                {
                    TrackingId = Guid.NewGuid().ToString(),
                    input.SignUpId,
                    MerchantTimeStamp = DateTime.Now,
                    UserId = input.UserId ?? "UnKnown"
                },
                StatusDetails = new
                {
                    input.StatusType,
                    input.ReasonType,
                    input.ChallengeType,
                    StatusDate = DateTime.Now
                },
                Name = "AP.AccountCreation.Status",
                Version = "0.5"
            };

            return await PostAsync<DfpAccountStatusResponse>(endpoint, createAccountInput, correlationId);
        }

        public async Task<ApiResponse<DfpAccountActionResponse>> LoginAccount(DfpLoginAccountInputClaims input, string correlationId, string loginId)
        {
            var endpoint = $"/v1.0/action/account/login/{input.UserId}";

            var createAccountInput = new
            {
                Metadata = new
                {
                    TrackingId = Guid.NewGuid().ToString(),
                    LoginId = loginId,
                    CustomerLocalDate = DateTime.Now,
                    MerchantTimeStamp = DateTime.Now,
                    AssessmentType = "Protect"
                },
                User = new
                {
                    Username = input.Email,
                    input.UserId,
                    UserType = "Consumer",
                },
                Device = new
                {
                    input.IpAddress,
                    input.DeviceContextId,
                    Provider = "DFPFingerprinting"
                },

                Name = "AP.AccountLogin",
                Version = "0.5"
            };

            return await PostAsync<DfpAccountActionResponse>(endpoint, createAccountInput, correlationId);
        }

        public async Task<ApiResponse<DfpAccountStatusResponse>> LoginAccountStatus(DfpLoginAccountStatusInputClaims input, string correlationId)
        {
            var endpoint = $"/v1.0/observe/account/login/status/{input.UserId}";

            var createAccountInput = new
            {
                Metadata = new
                {
                    TrackingId = Guid.NewGuid().ToString(),
                    input.LoginId,
                    MerchantTimeStamp = DateTime.Now,
                    input.UserId
                },
                StatusDetails = new
                {
                    input.StatusType,
                    input.ReasonType,
                    input.ChallengeType,
                    StatusDate = DateTime.Now
                },
                Name = "AP.AccountLogin.Status",
                Version = "0.5"
            };

            return await PostAsync<DfpAccountStatusResponse>(endpoint, createAccountInput, correlationId);
        }

        #region PRIVATE METHODS
        private async Task<ApiResponse<T>> PostAsync<T>(string endpoint, object content, string correlationId)
        {
            try
            {
                var authToken = await dfpAuthService.AcquireTokenAsync();

                var url = $"{fraudProtectionSettings.ApiBaseUrl}{endpoint}";
                var serializationSettings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var json = JsonConvert.SerializeObject(content, serializationSettings);
                logger.LogInformation($"Sending object to DFP URL: {url} | Payload: {json}");

                using (var request = BuildDfpRequest(correlationId, authToken, url, json))
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<T>(responseBody);

                    //return (true, "Success", data);
                    return new ApiResponse<T>
                    {
                        Status = true,
                        Message = "Success",
                        Data = data,
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DFP POST call failed");
                //return (false, ex.Message, default(T));
                return new ApiResponse<T>
                {
                    Status = false,
                    Message = ex.Message,
                    Data = default
                };
            }
        }

        private HttpRequestMessage BuildDfpRequest(string correlationId, string authToken, string url, string json)
        {
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Headers =
                    {
                        { HttpRequestHeader.Authorization.ToString(), $"Bearer {authToken}" },
                        { "x-ms-correlation-id", correlationId },
                        //{ HttpRequestHeader.Accept.ToString(), "application/json" },
                        { "x-ms-dfpenvid", fraudProtectionSettings.EnvironmentId }
                    },
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        #endregion
    }
}
