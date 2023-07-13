using IntermediateAPI.Models;
using IntermediateAPI.Models.External.Experian;
using IntermediateAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntermediateAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class UserValidationController : ControllerBase
    {
        private readonly ExperianService service;

        public UserValidationController(ExperianService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Question([FromBody] GetVerificationQuestionsInput getQuestionsInput)
        {
            var response = await service.GetQuestions(getQuestionsInput);
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
        public async Task<IActionResult> Answer([FromBody] ValidateUserAnswersInput answers)
        {
            var response = await service.SubmitAnswers(answers);
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
        public async Task<IActionResult> GetUserDetails([FromBody] GetUserDetailsInput input)
        {
            var response = await service.GetUserDetails(input);

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
        public async Task<IActionResult> ValidateUserDetails([FromBody] ValidateUserDetailsInput input)
        {
            var response = await service.ValidateUserDetails(input);

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
        public async Task<IActionResult> CreateUser([FromBody] CreateUserInput input)
        {
            var response = await service.CreateUserDetails(input);

            if (response.successful)
            {
                return Ok();
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }
        }
    }
}
