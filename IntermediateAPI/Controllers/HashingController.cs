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
            var pwHashResult = IdentityPasswordHasher.GenerateIdentityHash(input.Password);
            return Ok(new { Salt = pwHashResult.Salt, Hash = pwHashResult.PasswordHash });
        }

        [HttpPost]
        public async Task<IActionResult> HashPasswordWithSalt([FromBody] HashingInput input)
        {
            var pwHashResult = IdentityPasswordHasher.GenerateIdentityHash(input.Password, input.Salt);
            return Ok(new { Salt = pwHashResult.Salt, Hash = pwHashResult.PasswordHash });
        }

        [HttpPost]
        public async Task<IActionResult> ExtractSalt([Bind] string hashedPassword)
        {
            var result = new { Salt = IdentityPasswordHasher.ExtractSalt(hashedPassword) };
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPassword([Bind] string hashedPassword, [Bind] string password)
        {
            var pwHashResult = IdentityPasswordHasher.VerifyPassword(hashedPassword, password);
            return Ok(new { Salt = pwHashResult.Salt, Hash = pwHashResult.PasswordHash });
        }

    }


}
