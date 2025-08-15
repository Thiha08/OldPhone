using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OldPhone.UI.Shared.Services.SignalR
{
    public static class SignalRServiceCollectionExtensions
    {
        /// <summary>
        /// Registers SignalR-related services and options.
        /// </summary>
        public static IServiceCollection AddSignalRClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SignalROptions>(configuration.GetSection(nameof(SignalROptions)));

            services.AddSingleton<ISignalRPhoneService, SignalRPhoneService>();

            return services;
        }
    }
}
