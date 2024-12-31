using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ServiceSimulator
{
    public class ServiceSimulatorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ServiceSimulatorOptions _options;
        private readonly ILogger<ServiceSimulatorMiddleware> _logger;

        public ServiceSimulatorMiddleware(RequestDelegate next,
            IOptions<ServiceSimulatorOptions> options,
            ILogger<ServiceSimulatorMiddleware> logger)
        {
            _next = next;
            _options = options.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var handled = await Handle(context);

            if (!handled)
                await _next(context);
        }

        private async Task<bool> Handle(HttpContext context)
        {
            var fetchRequest = string.Empty;
            try
            {
                if (_options?.RequestStart is null)
                    return false;

                var pathAndQuery = context.Request.GetEncodedPathAndQuery();

                if (pathAndQuery is null)
                    return false;

                if (!pathAndQuery.StartsWith(_options.RequestStart))
                {
                    return false;
                }

                fetchRequest = pathAndQuery.Substring(_options.RequestStart.Length).Replace(@"\u0026", "&");
                var info = _options.GetInfo(fetchRequest);
                if (info is null)
                {
                    _logger?.LogWarning("Not found request: {FetchRequest}", fetchRequest);
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync($"{fetchRequest} not found!");
                    return true;
                }

                var fullPath = Path.GetFullPath(Path.Combine(_options.RootDirectory, info.ResponseRelativeFilename));
                if (!File.Exists(fullPath))
                {
                    _logger?.LogWarning("File Not found for request: {FetchRequest}, filename: {Filename}", fetchRequest, info.ResponseRelativeFilename);
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync($"{fetchRequest} \r\n {info.ResponseRelativeFilename} file not found!");
                    return true;
                }

                await context.Response.SendFileAsync(fullPath);
                _logger?.LogInformation("Handled: {FetchRequest} -> {FullPath}", fetchRequest, fullPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "FetchRequest: {FetchRequest}", fetchRequest);
                throw;
            }
        }
    }
}
