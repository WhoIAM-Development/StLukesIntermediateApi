using IntermediateAPI.Models.Hashing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace IntermediateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HashingController : ControllerBase
    {
        public HashingController()
        {

        }
        [HttpPost]
        public async Task<IActionResult> HashPasswword([FromBody] HashingInput input)
        {
            throw new NotImplementedException();
            //return Ok(new HashingOutput { HashedPassword = Hash(input.Password)});
        }
        
    }
    

}
