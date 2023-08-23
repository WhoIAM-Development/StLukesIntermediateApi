using IntermediateAPI.Models;
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
    public class HashingController : ControllerBase
    {
        private readonly ILogger<HashingController> logger;
        private readonly MessagingUtility messaging;
        public HashingController(ILogger<HashingController> logger, MessagingUtility messaging)
        {
            this.logger = logger;
            this.messaging = messaging;
        }

        [HttpPost]
        public async Task<IActionResult> HashPassword([FromBody] HashingInput input)
        {
            try
            {
                return Ok(new
                {
                    PwHash = IdentityPasswordHasher.GenerateIdentityHash(input.Password, input.Salt)
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to hash the given password");
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    input.Locale,
                    MessagingUtility.Messages.GeneralSignInError)));

            }
        }
    }


}
