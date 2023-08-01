using IntermediateAPI.Models;
using IntermediateAPI.Models.External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using IntermediateAPI.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace IntermediateAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AdapterController : ControllerBase
    {
        private readonly HttpClient activationClient;
        private readonly HttpClient userClient;
        private readonly ILogger<AdapterController> logger;

        // TODO: Add a service
        public AdapterController(
            IHttpClientFactory factory, IOptions<ExternalApisSettings> options, ILogger<AdapterController> logger) 
        {
            activationClient = factory.CreateClient("activationClient");
            userClient = factory.CreateClient("userClient");
            var settings = options.Value;
            this.logger = logger;
            activationClient.BaseAddress = new Uri(settings?.ActivationApiBaseUrl ?? "");
            userClient.BaseAddress = new Uri(settings?.UserApiBaseUrl ?? "");
        }

        [HttpPost]
        public async Task<IActionResult> GetUserWithActivationCode(FetchUserDetailsInput request)
        {
            var response = await activationClient.PostAsync<UserProfile, ErrorResponse>("/api/v3/activation/fetchuserepicprofiledetails", request);
            if (response.successful)
            {
                return Ok(response.response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }

        }

        [HttpPost]
        public async Task<IActionResult> LinkUserWithActivationCode(LinkAccountRequest request)
        {
            var response = await activationClient.PostAsync<LinkAccountResponse, ErrorResponse>("/api/v3/activation/linkmycharttoaccount", request);
            if (response.successful)
            {
                return Ok(response.response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetQuestions(ExperianUserProfile request) 
        {
            var response = await activationClient.PostAsync<ExperianQuestions, ErrorResponse>("/api/v3/activation/linkviaexperian", request);
            if (response.successful)
            {
                return Ok(response.response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }

        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswers(ValidateUserAnswersInput request) 
        {
            var payload = new VerifyAnswersInput
            {
                SessionId =  request.SessionId,
                AnswerIndex = request.AnswerIndex?.Split(',').Select(int.Parse).ToList()
            };

            var response = await activationClient.PostAsync<ExperianValidateAnswerResult, ErrorResponse>("/api/v3/activation/submitanswerstoexperian", payload);
            if (response.successful)
            {
                return Ok(response.response);
            }
            else
            {
                return Ok(new ExAnswerVerificationResponse()
                {
                    IsIdentityVerified = false,
                    MyChartUserId = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserProfile request)
        {
            var response = await userClient.PostAsync<UserCreatedResponse, ErrorResponse>("/api/v4/user", request);
            if (response.successful)
            {
                return Ok(response.response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetUser(UserObjectId userObjectId)
        {
            var response = await userClient.GetAsync<UserProfile, ErrorResponse>($"/api/v4/user/{userObjectId?.ObjectId}", null);
            if (response.successful)
            {
                return Ok(response.response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }

        }
    }
}
