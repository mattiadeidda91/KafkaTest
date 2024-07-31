using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions
{
    public static class AuthorizationBuilderExtension
    {
        public static IServiceCollection AuthorizationBuild(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                   .RequireAuthenticatedUser()
                   .Build();
            });

            return services;
        }
    }
}
