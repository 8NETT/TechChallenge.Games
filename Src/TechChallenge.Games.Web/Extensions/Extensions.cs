using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.Extensions.Hosting
{
    public static class Extensions
    {
        public static TBuilder AddOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            builder.Logging.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
            });

            builder.ConfigureOpenTelemetry();

            return builder;
        }

        public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            builder.Services.AddOpenTelemetry()
                .WithMetrics(metrics =>
                {
                    metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddSqlClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    });

                    tracing.AddSource(builder.Environment.ApplicationName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            AddTagsFromRequest(activity, httpRequest);
                        };
                    })
                    .AddHttpClientInstrumentation();
                });

            return builder;
        }

        /// <summary>
        /// Adds tags from the HTTP request to the activity.
        /// </summary>
        /// <param name="activity">The activity to add tags to.</param>
        /// <param name="httpRequest">The HTTP request to extract tags from.</param>
        private static void AddTagsFromRequest(Activity activity, HttpRequest httpRequest)
        {
            var authHeader = httpRequest.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader["Bearer ".Length..].Trim();
                var handler = new JwtSecurityTokenHandler();

                var jwtToken = handler.ReadJwtToken(token);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                if (userId != null)
                    activity?.SetTag("userId", userId);

                activity?.SetTag("requestId", httpRequest.HttpContext.TraceIdentifier);

                if (activity?.Id != null)
                    activity?.SetTag("traceId", activity?.Id);
            }
        }

        public static TBuilder AddOpenTelemetryExporters<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
        {
            if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
            {
                builder.Services.AddOpenTelemetry()
                    .UseAzureMonitor();
            }

            return builder;
        }
    }
}
