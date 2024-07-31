using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Common.Configurations.Swagger
{
    public static class SwaggerBuilderExtension
    {
        public static IServiceCollection SwaggerBuild(this IServiceCollection services, List<string> apiVersions)
        {
            services.AddSwaggerGen(options =>
            {
                foreach (var version in apiVersions)
                {
                    options.SwaggerDoc(version, new OpenApiInfo { Title = Assembly.GetEntryAssembly()?.GetName().Name + " " + version.ToUpper(), Version = version });
                }
                
                options.EnableAnnotations();
                options.OperationFilter<OperationFilter>();
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = HeaderNames.Authorization,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    In = ParameterLocation.Header,
                    Description = string.Join(Environment.NewLine
                               , $"<p>Please enter '{JwtBearerDefaults.AuthenticationScheme}' [space] and your token in the text input below.</p>"
                               , $"<p>Example: '{JwtBearerDefaults.AuthenticationScheme} 12345abcdef' </p>")
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            return services;
        }
    }
}