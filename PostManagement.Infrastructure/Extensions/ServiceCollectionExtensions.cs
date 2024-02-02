using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using PostManagement.Core.Interfaces;
using PostManagement.Infrastructure.Authorization;
using PostManagement.Infrastructure.Repositories;

namespace PostManagement.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }
    }
}
