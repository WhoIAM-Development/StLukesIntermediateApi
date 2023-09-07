namespace IntermediateAPI
{
    public static class Constants
    {
        public const string Default = "default";
        public const string Local = "local";
        public const string AAD = "aad";
        public const string HomeRealmTable = "DomainHomeRealm";
        public const string LoginTypes = "LoginType";

        public static readonly string DEFAULT_DFP_PROD_URI = "api.dfp.dynamics.com";
        public static readonly string DEFAULT_DFP_SANDBOX_URI = "api.dfp.dynamics-int.com";
        public static readonly string Validation_Dfp_MissingSecretOrCert = "Please configure either the client secret or certificate thumbprint";
        public static readonly string Validation_Dfp_CertAndSecretConfigured = "Please only configure certificate or secret authentication (not both) in the appsettings file.";
    }
}
