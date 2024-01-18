using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Interfaces.Repository;
using EnityFrameworkRelationShip.Interfaces.Service;
using EnityFrameworkRelationShip.Repositories;
using EnityFrameworkRelationShip.Services;

namespace EnityFrameworkRelationShip.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IPostsService, PostsService>();
            services.AddScoped<ITagsService, TagsService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddMemoryCache();
            return services;
        }
    }
}
