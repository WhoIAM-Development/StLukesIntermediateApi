using IntermediateAPI.Models;
using IntermediateAPI.Utilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace IntermediateAPI.Services
{
    public class DfpAuthProvider : IAuthProvider
    {
        private readonly FraudProtectionSettings fraudProtectionSettings;
        public DfpAuthProvider(IOptions<FraudProtectionSettings> tokenProviderServiceSettings)
        {
            this.fraudProtectionSettings = tokenProviderServiceSettings.Value;
        }
        public async Task<string> AcquireTokenAsync()
        {
            return string.IsNullOrEmpty(fraudProtectionSettings.CertificateThumbprint)
                ? await AcquireTokenWithSecretAsync()
                : await AcquireTokenWithCertificateAsync();
        }
        private async Task<string> AcquireTokenWithCertificateAsync()
        {
            var x509Cert = CertificateUtility.GetByThumbprint(fraudProtectionSettings.CertificateThumbprint);
            var clientAssertion = new ClientAssertionCertificate(fraudProtectionSettings.ClientId, x509Cert);
            var context = new AuthenticationContext(fraudProtectionSettings.Authority);
            var authenticationResult = await context.AcquireTokenAsync(fraudProtectionSettings.Resource, clientAssertion);
            return authenticationResult.AccessToken;
        }
        private async Task<string> AcquireTokenWithSecretAsync()
        {
            var clientAssertion = new ClientCredential(fraudProtectionSettings.ClientId, fraudProtectionSettings.ClientSecret);
            var context = new AuthenticationContext(fraudProtectionSettings.Authority);
            var authenticationResult = await context.AcquireTokenAsync(fraudProtectionSettings.Resource, clientAssertion);
            return authenticationResult.AccessToken;
        }
    }
}
