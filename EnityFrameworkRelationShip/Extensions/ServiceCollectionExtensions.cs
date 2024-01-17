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
            services.AddScoped<IPostsRepository, PostsRepository>();
            services.AddScoped<ITagsRepository, TagsRepository>();
            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IPostsService, PostsService>();
            services.AddScoped<ITagsService, TagsService>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
