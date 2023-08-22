using IntermediateAPI.Models.Hashing;
using IntermediateAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace IntermediateAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class HashingController : ControllerBase
    {
        public HashingController()
        {

        }

        [HttpPost]
        public async Task<IActionResult> HashPassword([FromBody] HashingInput input)
        {
            return Ok(new
            {
                PwHash = IdentityPasswordHasher.GenerateIdentityHash(input.Password, input.Salt)
            });
        }
    }


}
