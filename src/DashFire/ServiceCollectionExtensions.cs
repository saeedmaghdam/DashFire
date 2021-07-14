using Microsoft.Extensions.DependencyInjection;

namespace DashFire
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJob<T>(this IServiceCollection services) where T : IJob
        {
            JobContext.Instance.RegisterJob<T>();

            return services;
        }
    }
}
