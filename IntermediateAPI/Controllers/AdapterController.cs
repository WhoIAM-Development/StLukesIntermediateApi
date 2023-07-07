using IntermediateAPI.Models;
using IntermediateAPI.Models.External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using IntermediateAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using IntermediateAPI.Models.External.Experian;

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
            var response = await activationClient.PostAsync<UserDemographicsResponse, ErrorResponse>("/fetchuserepicprofiledetails", request);
            if (response.successful)
            {
                return Ok(response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }

        }

        [HttpPost]
        public async Task<IActionResult> LinkUserWithActivationCode(LinkAccountRequest request)
        {
            var response = await activationClient.PostAsync<LinkAccountResponse, ErrorResponse>("/linkmycharttoaccount", request);
            if (response.successful)
            {
                return Ok(response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetQuestions(UserProfileRequest request) 
        {
            var response = await activationClient.PostAsync<ExperianQuestions, ErrorResponse>("/linkviaexperian", request);
            if (response.successful)
            {
                return Ok(response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }

        }

        [HttpPost]
        public async Task<IActionResult> SubmitAnswers(VerifyAnswersInput request) 
        {
            var response = await activationClient.PostAsync<ExperianQuestions, ErrorResponse>("/submitanswerstoexperian", request);
            if (response.successful)
            {
                return Ok(response);
            }
            else
            {
                return Ok(new ExAnswerVerificationResponse()
                {
                    IsIdentityVerified = false,
                    MyChartId = null
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserProfileRequest request)
        {
            var response = await userClient.PostAsync<UserProfileResponse, ErrorResponse>("/user", request);
            if (response.successful)
            {
                return Ok(response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(UserProfileRequest request) 
        {
            var response = await userClient.PutAsync<UserProfileResponse, ErrorResponse>("/user", request);
            if (response.successful)
            {
                return Ok(response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }

        }

        public async Task<IActionResult> GetUser(UserObjectId userObjectId)
        {
            var response = await userClient.GetAsync<UserProfileResponse, ErrorResponse>($"/user/{userObjectId?.ObjectId}", null);
            if (response.successful)
            {
                return Ok(response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }

        }


    }
}
