using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Triceratops.Dashboard.Services.NotificationService;
using Triceratops.Libraries.Http.Api;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Core;

namespace Triceratops.Dashboard
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTriceratopsApiClient(this IServiceCollection services)
        {
            services.AddScoped<ITriceratopsApiClient>(s =>
            {
                var logger = s.GetLoggerFor<ITriceratopsApiClient>();

                return new TriceratopsApiClient(new CoreHttpClient(logger));
            });
        }

        public static void AddNotificationService(this IServiceCollection services)
        {
            services.AddScoped<INotificationService>(s =>
            {
                var contextAccessor = s.GetRequiredService<IHttpContextAccessor>();
                var logger = s.GetRequiredService<ILogger<INotificationService>>();
                var context = contextAccessor.HttpContext;

                return new NotificationService(logger, context.Session);
            });
        }

        public static ILogger<T> GetLoggerFor<T>(this IServiceProvider provider)
        {
            return provider.GetRequiredService<ILogger<T>>();
        }
    }
}
