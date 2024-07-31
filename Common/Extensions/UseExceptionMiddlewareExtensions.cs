using Common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Common.Extensions
{
    public static class UseExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
