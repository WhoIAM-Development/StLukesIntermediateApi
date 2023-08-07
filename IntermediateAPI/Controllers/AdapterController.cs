using IntermediateAPI.Models;
using IntermediateAPI.Models.External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using IntermediateAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using IntermediateAPI.Services;

namespace IntermediateAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AdapterController : ControllerBase
    {
        private readonly AdapterService service;

        public AdapterController(AdapterService service) 
        {
            this.service = service;

        }

        [HttpPost]
        public async Task<IActionResult> GetUserWithActivationCode(FetchUserDetailsInput request)
        {
            var response = await service.GetUserDetailsWithActivationCode(request);


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
            var response = await service.LinkUserWithActivationCode(request);
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
            var response = await service.GetQuestions(request);
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
            var response = await service.SubmitAnswers(request);
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
        public async Task<IActionResult> CreateUser(UserProfile request)
        {
            var response = await service.CreateUser(request);
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
            var response = await service.GetUser(userObjectId);
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
