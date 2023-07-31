using B2CIntermediateAPI.Models.Graph;
using B2CIntermediateAPI.Models.MFAMethods;
using B2CIntermediateAPI.Services;
using B2CIntermediateAPI.Utilities;
using IntermediateAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace B2CIntermediateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TotpFactors : ControllerBase
    {
        private readonly GraphService _graphService;
        private readonly ILogger<TotpFactors> _logger;

        public TotpFactors(
            GraphService graphService,
            ILogger<TotpFactors> logger
        )
        {
            _graphService = graphService;
            _logger = logger;
        }


        /// <summary>
        ///     Gets the registered TOTP factors for the account
        /// </summary>
        /// <param name="userId">B2C User ID</param>
        /// <returns>Result object with TOTP factors</returns>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SoftwareOAuthMethodsIds))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckSoftwareAuthMethods([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId)) return BadRequest();

            // Get Software OAuth methods (TOTP registrations) for the user by calling graph API with user's objectId.  
            SoftwareOAuthMethods? methods = await _graphService.GetSoftwareOAuthMethods(userId);
            SoftwareOAuthMethodsIds response = new SoftwareOAuthMethodsIds
            {
                MethodsIds = methods?.GetIds().ToArray()
            };

            return Ok(response);
        }

        /// <summary>
        /// Removes the registered TOTP factors for the account
        /// </summary>
        /// <param name="input">User ID / Locale object</param>
        /// <returns>Ok or conflict response</returns>
        [HttpPost("Clear")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearSoftwareAuthMethods(ClearSoftwareAuthMethodsInputClaims input)
        {
            if (input?.MethodsIds?.IsNullOrEmpty() ?? true)
            {
                return Ok();
            }

            try
            {
                await _graphService.DeleteSoftwareOAuthMethods(input.UserId, input.MethodsIds);

                return Ok();
            }
            catch (Exception ex)
            {
                // Log errors and send telemetry on exception.
                _logger.LogError(ex, "Request Failed for user {UserId}", input.UserId);

                return Conflict(new B2CErrorResponseContent("MFA Reset Error",
                    $"Request Failed for user. Check API logs for more information. {ex.Message}"));
            }
        }

        /// <summary>
        /// Removes the registered TOTP factors for the account
        /// </summary>
        /// <param name="input">User ID / Locale object</param>
        /// <returns>Ok or conflict response</returns>
        [HttpPost("ClearAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ClearAllSoftwareAuthMethods(ClearSoftwareAuthMethodsInputClaims input)
        {
            try
            {
                // Get all registered totp registrations for the user by calling graph API with user's objectId.
                SoftwareOAuthMethods? authMethods = await _graphService.GetSoftwareOAuthMethods(input.UserId);

                // Delete all the registered totp registrations for the user by calling graph API. 
                await _graphService.DeleteSoftwareOAuthMethods(input.UserId, authMethods.GetIds());

                return Ok();
            }
            catch (Exception ex)
            {
                // Log errors and send telemetry on exception.
                _logger.LogError(ex, "Request Failed for user {UserId}", input.UserId);

                return Conflict(new B2CErrorResponseContent("MFA Reset Error",
                    $"Request Failed for user. Check API logs for more information. {ex.Message}"));
            }
        }
    }
}