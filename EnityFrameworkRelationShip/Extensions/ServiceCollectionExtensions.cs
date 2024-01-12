using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Repositories;
using EnityFrameworkRelationShip.Services;

namespace EnityFrameworkRelationShip.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IPostsRepository, PostsRepository>();
            services.AddScoped<ITagsRepository, TagsRepository>();
            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IPostsService, PostsService>();
            return services;
        }
    }
}
