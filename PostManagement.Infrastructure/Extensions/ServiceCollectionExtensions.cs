using Microsoft.Extensions.DependencyInjection;
using PostManagement.Core.Interfaces;
using PostManagement.Infrastructure.Repositories;

namespace PostManagement.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }
    }
}
