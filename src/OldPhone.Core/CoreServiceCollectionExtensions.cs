using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OldPhone.Core.Services;
using OldPhone.Core.Shared.Entities;
using OldPhone.Core.Validators;

namespace OldPhone.Core
{
    /// <summary>
    /// Extension methods for configuring Core services
    /// </summary>
    public static class CoreServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Core business services
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<IUserMessageService, UserMessageService>();

            // Register FluentValidation validators
            services.AddScoped<IValidator<UserMessage>, UserMessageValidator>();

            return services;
        }
    }
} 