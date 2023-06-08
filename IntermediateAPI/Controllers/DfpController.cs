using IntermediateAPI.Models;
using IntermediateAPI.Services;
using IntermediateAPI.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace IntermediateAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DfpController : ControllerBase
    {
        private readonly DfpService dfpService;
        private readonly ILogger<DfpController> logger;
        private readonly MessagingUtility messaging;

        public DfpController(DfpService dfpService, ILogger<DfpController> logger, MessagingUtility messaging)
        {
            this.dfpService = dfpService;
            this.logger = logger;
            this.messaging = messaging;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAccount(DfpCreateAccountInputClaims input)
        {
            if (input == null || !input.Validate())
            {
                this.logger.LogError($"Failed to deserialize input claims of type {input.GetType().Name}");
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    "en",
                    MessagingUtility.Messages.GeneralSignInError,
                    "Cannot DfpCreateAccountInputClaims deserialize input claims")));
            }

            var correlationId = dfpService.NewCorrelationId;
            var signUpId = dfpService.NewCorrelationId;

            var response = await dfpService.CreateAccount(input, correlationId, signUpId);

            if (!response.Status)
            {
                this.logger.LogError($"DFP retured non-successful error {JsonConvert.SerializeObject(response)}");
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    input.Locale,
                    MessagingUtility.Messages.GeneralSignInError,
                    $"Correlation Id : {correlationId}")));
            }

            var result = response.Data?.ResultDetails?.FirstOrDefault();
            var deviceId = response.Data?.Enrichments?.DeviceAttributes?.DeviceId;

            if (result == null)
            {
                this.logger.LogError($"DFP response result was null {JsonConvert.SerializeObject(response)}");
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    input.Locale,
                    MessagingUtility.Messages.GeneralSignInError,
                    $"Result is null - Correlation Id : {correlationId}")));
            }

            var botScore = result.Scores.FirstOrDefault(x => x.ScoreType == "Bot")?.ScoreValue ?? 0;
            var riskScore = result.Scores.FirstOrDefault(x => x.ScoreType == "Risk")?.ScoreValue ?? 0;

            var output = new DfpCreateAccountOutputClaims()
            {
                CorrelationId = correlationId,
                SignUpId = signUpId,
                Decision = result.Decision,
                BotScore = (int)botScore,
                RiskScore = (int)riskScore,
                TransactionReferenceId = response.Data?.TransactionReferenceId,
                DeviceId = deviceId
            };

            output.AugmentWithAdditionalProperties(
                response.Data?.Enrichments?.DeviceAttributes,
                response.Data?.Enrichments?.CalculatedFeatures);

            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccountStatus(DfpCreateAccountStatusInputClaims input)
        {
            if (input == null || !input.Validate())
            {
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    "en",
                    MessagingUtility.Messages.GeneralSignInError,
                    "Cannot deserialize DfpCreateAccountStatusInputClaims input claims")));
            }

            var correlationId = dfpService.NewCorrelationId;

            var response = await dfpService.CreateAccountStatus(input, correlationId);

            if (!response.Status)
            {
                this.logger.LogError($"DFP retured non-successful error {JsonConvert.SerializeObject(response)}");
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    input.Locale,
                    MessagingUtility.Messages.GeneralSignInError,
                    $"Response: {response.Message} Correlation Id : {correlationId}")));
            }

            return Ok(new DfpCreateAccountStatusOutputClaims() { CorrelationId = correlationId });
        }

        [HttpPost]
        public async Task<IActionResult> LoginAccount(DfpLoginAccountInputClaims input)
        {
            if (input == null || !input.Validate())
            {
                logger.LogError($"Recieved null or invalid claims: {JsonConvert.SerializeObject(input)}");
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                   "en",
                    MessagingUtility.Messages.GeneralSignInError,
                    "Cannot deserialize DfpLoginAccountInputClaims input claims")));
            }

            var correlationId = dfpService.NewCorrelationId;
            var loginId = dfpService.NewCorrelationId;

            var response = await dfpService.LoginAccount(input, correlationId, loginId);

            if (!response.Status)
            {
                var msg = $"DFP retured non-successful error {JsonConvert.SerializeObject(response)} | Correlation Id : {correlationId}";
                this.logger.LogError(msg);
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    input.Locale,
                    MessagingUtility.Messages.GeneralSignInError,
                    msg)));
            }

            var result = response.Data?.ResultDetails?.FirstOrDefault();
            var deviceId = response.Data?.Enrichments?.DeviceAttributes?.DeviceId;

            if (result == null)
            {
                var msg = $"DFP response result was null {JsonConvert.SerializeObject(response)}";
                this.logger.LogError(msg);
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    input.Locale,
                    MessagingUtility.Messages.GeneralSignInError,
                    msg)));
            }

            //TODO: Fix Decision DFP meeting
            var botScore = result.Scores.FirstOrDefault(x => x.ScoreType == "Bot")?.ScoreValue ?? 0;
            var riskScore = result.Scores.FirstOrDefault(x => x.ScoreType == "Risk")?.ScoreValue ?? 0;
            var output = new DfpLoginAccountOutputClaims()
            {
                CorrelationId = correlationId,
                LoginId = loginId,
                Decision = result.Decision,
                BotScore = (int)botScore,
                RiskScore = (int)riskScore,
                TransactionReferenceId = response.Data?.TransactionReferenceId,
                DeviceId = deviceId
            };
            output.AugmentWithAdditionalProperties(
                response.Data?.Enrichments?.DeviceAttributes,
                response.Data?.Enrichments?.CalculatedFeatures
            );

            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> LoginAccountStatus(DfpLoginAccountStatusInputClaims input)
        {
            if (input == null || !input.Validate())
            {
                var msg = "Cannot deserialize DfpLoginAccountStatusInputClaims input claims";
                logger.LogError(msg);
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    "en",
                    MessagingUtility.Messages.GeneralSignInError,
                    msg)));
            }

            var correlationId = dfpService.NewCorrelationId;

            var response = await dfpService.LoginAccountStatus(input, correlationId);

            if (!response.Status)
            {
                var msg = $"DFP retured non-successful error {JsonConvert.SerializeObject(response)}";
                this.logger.LogError(msg);
                return Conflict(new B2CErrorResponseContent(messaging.GetLocalizedString(
                    input.Locale,
                    MessagingUtility.Messages.GeneralSignInError,
                    msg)));
            }

            return Ok(new DfpCreateAccountStatusOutputClaims() { CorrelationId = correlationId });
        }
    }
}
