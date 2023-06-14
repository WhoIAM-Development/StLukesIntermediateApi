using IntermediateAPI.Models;
using IntermediateAPI.Models.UserValidation;
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
        public async Task<IActionResult> Question([FromBody] UserInfo userInfo)
        {
            var response = await service.GetQuestions(userInfo);
            if(response.successful)
            {
                return Ok(response.response);
            }
            else
            {
                return Conflict(new B2CErrorResponseContent(response.error?.Message, response.error?.Title));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Answer([FromBody] ExperianAnswers answers)
        {
            var response = await service.SubmitAnswers(answers);
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
