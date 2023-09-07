using IntermediateAPI;

namespace IntermediateAPI.Models
{
    public class FraudProtectionSettings
    {
        private string _apiBaseUrl;
        private string _authority;
        private string _resource;

        public bool IsProductionEnvironment { get; set; } = false;

        private string BaseUri
        {
            get
            {
                return this.IsProductionEnvironment ? Constants.DEFAULT_DFP_PROD_URI : Constants.DEFAULT_DFP_SANDBOX_URI;
            }
        }

        public string EnvironmentId { get; set; } = string.Empty;

        public string AADTenantId { get; set; } = string.Empty;

        public string ClientId { get; set; } = string.Empty;

        public string ClientSecret { get; set; } = string.Empty;

        public string CertificateThumbprint { get; set; } = string.Empty;

        public FraudProtectionOverrides Overrides { get; set; } = new FraudProtectionOverrides();

        public string ApiBaseUrl
        {
            get
            {
                _apiBaseUrl ??= string.IsNullOrWhiteSpace(Overrides.ApiBaseUrl)
                    ? $"https://{EnvironmentId}.{BaseUri}"
                    : Overrides.ApiBaseUrl;
                return _apiBaseUrl;
            }
        }

        public string Authority
        {
            get
            {
                _authority ??= string.IsNullOrWhiteSpace(Overrides.TokenAuthority)
                    ? $"https://login.microsoftonline.com/{AADTenantId}"
                    : Overrides.TokenAuthority;

                return _authority;
            }
        }

        public string Resource
        {
            get
            {
                _resource ??= string.IsNullOrWhiteSpace(Overrides.ApiResourceUri)
                    ? "https://" + BaseUri
                    : Overrides.ApiResourceUri;

                return _resource;
            }
        }

        public string DeviceFingerprintingDomain => this.Overrides.DeviceFingerprintingDomain ?? this.EnvironmentId;
    }

    public class FraudProtectionOverrides
    {
        public string DeviceFingerprintingDomain { get; set; }

        public string ApiBaseUrl { get; set; }

        public string ApiResourceUri { get; set; }

        public string TokenAuthority { get; set; }
    }
}
