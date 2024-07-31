using Common.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Common.Extensions
{
    public static class AuthenticationBuilderExtension
    {
        public static IServiceCollection AuthenticationBuild(this IServiceCollection services, bool isProduction, JwtTokenOptions? jwtOptions)
        {
            ArgumentNullException.ThrowIfNull(jwtOptions);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = isProduction;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions?.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions?.Audience,
                    ValidateLifetime = true, //Validate token expiration
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions?.Signature!)),
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.FromMinutes(5) //The Default is 5 minutes, Token is valid for 5 mins after expiration
                };
            });

            return services;
        }
    }
}
