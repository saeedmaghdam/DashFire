using Microsoft.Extensions.DependencyInjection;

namespace DashFire
{
    /// <summary>
    /// IServiceCollection extensions to handle DashFire initializations.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a job to the microsoft hosting to be registered and used when the service starts.
        /// </summary>
        /// <typeparam name="T">IJob's concrete class.</typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <returns>Configures the IServiceCollection and return it.</returns>
        public static IServiceCollection AddJob<T>(this IServiceCollection services) where T : IJob
        {
            JobContext.Instance.RegisterJob<T>();

            return services;
        }
    }
}
