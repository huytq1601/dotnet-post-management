using Microsoft.Extensions.DependencyInjection;
using PostManagement.Application.Interfaces;
using PostManagement.Application.Services;

namespace PostManagement.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IPostsService, PostsService>();
            services.AddScoped<ITagsService, TagsService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddSingleton<ICacheService, CacheService>();
            return services;
        }
    }
}
