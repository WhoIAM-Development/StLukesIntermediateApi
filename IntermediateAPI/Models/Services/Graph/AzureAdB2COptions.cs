namespace B2CIntermediateAPI.Models.Services.Graph
{
    public class AzureAdB2COptions
    {
        public string? Instance { get; set; }

        public string? Domain { get; set; }

        public string? TenantId { get; set; }

        public string? ClientId { get; set; }

        public string? ClientSecret { get; set; }

        public string Authority => $"{Instance}{TenantId}";
    }
}