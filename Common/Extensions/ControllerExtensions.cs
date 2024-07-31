using Common.Configurations.Swagger;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Common.Configurations.JsonSerializer;

namespace Common.Extensions
{
    public static class ControllerExtensions
    {
        public static IServiceCollection BuildControllerConfigurations(this IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                config.Conventions.Add(new ProducesResponseTypeConvention());
            })
            .AddJsonOptions(options => 
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter());
            });

            return services;
        }
    }
}
