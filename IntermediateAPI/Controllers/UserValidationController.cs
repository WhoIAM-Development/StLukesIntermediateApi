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
        public async Task<IActionResult> Question()
        {

        }
        [HttpPost]
        public async Task<IActionResult> Answer()
        {

        }
    }
}
