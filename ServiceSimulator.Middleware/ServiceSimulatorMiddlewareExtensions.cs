using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ServiceSimulator
{
    /// <summary>
    /// Extension methods for the ServiceSimulator middleware.
    /// </summary>
    public static class ServiceSimulatorMiddlewareExtensions
    {
        public static IServiceCollection AddServiceSimulatorOptions(
            this IServiceCollection services,
            string requestStart,
            string rootDirectory,
            IEnumerable<OperationInfo> operationInfo)
        {
            services.Configure<ServiceSimulatorOptions>(options =>
            {
                options.RequestStart = requestStart;
                options.RootDirectory = rootDirectory;
                foreach (var item in operationInfo)
                {
                    options.Add(item);
                }
            });

            return services;
        }

        /// <summary>
        /// Adds middleware for using ServiceSimulator.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        public static IApplicationBuilder UseServiceSimulator(this IApplicationBuilder app)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<ServiceSimulatorMiddleware>();
        }
    }
}
