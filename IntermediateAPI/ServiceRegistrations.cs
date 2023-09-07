using B2CIntermediateAPI.Models.Services.Graph;
using B2CIntermediateAPI.Services;
using IntermediateAPI.Models;
using IntermediateAPI.Services;
using IntermediateAPI.Utilities;

namespace IntermediateAPI
{
    public static class ServiceRegistrations
    {
        public static void AddDataServices(this IServiceCollection services)
        {
            services.AddSingleton<DfpService>();

            services.AddOptions<FraudProtectionSettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("FraudProtectionSettings").Bind(settings);
            });
            
            services.AddOptions<ExternalApisSettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("ExternalApisSettings").Bind(settings);
            });
            services.AddOptions<AzureAdB2COptions>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("AzureAdB2C").Bind(settings);
            });

            services.AddSingleton<AdapterService>();
            services.AddSingleton<GraphService>();


            services.AddSingleton(new MessagingUtility(Environment.GetEnvironmentVariable("ENVIRONMENT") == "Development"));

            services.AddHttpClient();
        }
    }
}
