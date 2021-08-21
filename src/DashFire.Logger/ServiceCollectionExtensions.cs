using DashFire.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace DashFire.Logger
{
    /// <summary>
    /// IServiceCollection extensions to handle DashFire.Logger initializations.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register DashFire.Logger as a service.
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <returns></returns>
        public static IServiceCollection AddDashLogger(this IServiceCollection services)
        {
            services.AddSingleton<IDashLogger, DashLogger>();

            return services;
        }
    }
}
