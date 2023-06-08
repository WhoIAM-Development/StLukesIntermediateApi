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

            services.AddOptions<TokenProviderServiceSettings>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("FraudProtectionSettings:TokenProviderConfig").Bind(settings);
            });            
            services.AddSingleton(new MessagingUtility(Environment.GetEnvironmentVariable("ENVIRONMENT") == "Development"));

            services.AddHttpClient();
        }
    }
}
