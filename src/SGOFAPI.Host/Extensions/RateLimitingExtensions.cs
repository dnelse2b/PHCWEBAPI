using Microsoft.AspNetCore.RateLimiting;
using PHCAPI.Host.Configuration;
using Serilog;
using System.Threading.RateLimiting;

namespace PHCAPI.Host.Extensions;

/// <summary>
/// Extension methods for configuring rate limiting from appsettings.json
/// 🛡️ SECURITY: Centralized rate limiting configuration
/// </summary>
public static class RateLimitingExtensions
{
    /// <summary>
    /// Adds rate limiting policies from configuration
    /// </summary>
    public static IServiceCollection AddConfigurableRateLimiting(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var rateLimitingOptions = configuration
            .GetSection(RateLimitingOptions.SectionName)
            .Get<RateLimitingOptions>() ?? new RateLimitingOptions();

        services.AddRateLimiter(rateLimiterOptions =>
        {
            // 🔴 CRITICAL: Login endpoint (Anti-Brute Force)
            if (rateLimitingOptions.LoginEndpoint.Enabled)
            {
                rateLimiterOptions.AddPolicy("login-endpoint", context =>
                {
                    var config = rateLimitingOptions.LoginEndpoint;
                    var ip = context.Connection.RemoteIpAddress?.ToString() ??
                             context.Connection.LocalIpAddress?.ToString() ?? "unknown";
                    var username = context.Request.Headers["X-Username"].ToString();
                    var partitionKey = string.IsNullOrEmpty(username) ? ip : $"{ip}_{username}";

                    return CreateRateLimiter(partitionKey, config);
                });
            }

            // 🟠 HIGH: Parameter Create endpoint
            if (rateLimitingOptions.ParametersCreate.Enabled)
            {
                rateLimiterOptions.AddPolicy("parameters-create", context =>
                {
                    var config = rateLimitingOptions.ParametersCreate;
                    var partitionKey = GetPartitionKey(context);
                    return CreateRateLimiter(partitionKey, config);
                });
            }

            // 🔴 CRITICAL: Parameter Delete endpoint
            if (rateLimitingOptions.ParametersDelete.Enabled)
            {
                rateLimiterOptions.AddPolicy("parameters-delete", context =>
                {
                    var config = rateLimitingOptions.ParametersDelete;
                    var partitionKey = GetPartitionKey(context);
                    return CreateRateLimiter(partitionKey, config);
                });
            }

            // 🟡 MEDIUM: Parameter Update endpoint
            if (rateLimitingOptions.ParametersUpdate.Enabled)
            {
                rateLimiterOptions.AddPolicy("parameters-update", context =>
                {
                    var config = rateLimitingOptions.ParametersUpdate;
                    var partitionKey = GetPartitionKey(context);
                    return CreateRateLimiter(partitionKey, config);
                });
            }

            // 🟢 LOW: Parameter Query endpoints
            if (rateLimitingOptions.ParametersQuery.Enabled)
            {
                rateLimiterOptions.AddPolicy("parameters-query", context =>
                {
                    var config = rateLimitingOptions.ParametersQuery;
                    var partitionKey = GetPartitionKey(context);
                    return CreateRateLimiter(partitionKey, config);
                });
            }

            // ⚠️ Response when rate limit is exceeded
            rateLimiterOptions.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var retryAfter = 60;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfterMetadata))
                {
                    retryAfter = (int)retryAfterMetadata.TotalSeconds;
                }

                context.HttpContext.Response.Headers.RetryAfter = retryAfter.ToString();

                var endpoint = context.HttpContext.GetEndpoint()?.DisplayName ?? "Unknown";
                var method = context.HttpContext.Request.Method;
                var path = context.HttpContext.Request.Path;
                var ipAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString() ??
                               context.HttpContext.Connection.LocalIpAddress?.ToString() ?? "Unknown";
                var user = context.HttpContext.User?.Identity?.Name ?? "Anonymous";

                Log.Warning(
                    "🚨 RATE LIMIT EXCEEDED: User={User}, IP={IP}, Method={Method}, Path={Path}, Endpoint={Endpoint}, RetryAfter={RetryAfter}s",
                    user, ipAddress, method, path, endpoint, retryAfter);

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "Rate limit exceeded",
                    message = $"Too many requests. Please try again in {retryAfter} seconds.",
                    retryAfter,
                    statusCode = 429,
                    endpoint = path.ToString()
                }, cancellationToken: cancellationToken);
            };

            // 🌍 Global fallback rate limiter
            if (rateLimitingOptions.GlobalLimit.Enabled)
            {
                rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var config = rateLimitingOptions.GlobalLimit;
                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ??
                                   context.Connection.LocalIpAddress?.ToString() ?? "unknown";

                    return CreateRateLimiter(ipAddress, config);
                });
            }

            Log.Information(
                "✅ Rate Limiting configured: Login({LoginLimit}/{LoginWindow}s), " +
                "Create({CreateLimit}/{CreateWindow}s), Delete({DeleteLimit}/{DeleteWindow}s), " +
                "Update({UpdateLimit}/{UpdateWindow}s), Query({QueryLimit}/{QueryWindow}s), " +
                "Global({GlobalLimit}/{GlobalWindow}s)",
                rateLimitingOptions.LoginEndpoint.PermitLimit, rateLimitingOptions.LoginEndpoint.WindowInSeconds,
                rateLimitingOptions.ParametersCreate.PermitLimit, rateLimitingOptions.ParametersCreate.WindowInSeconds,
                rateLimitingOptions.ParametersDelete.PermitLimit, rateLimitingOptions.ParametersDelete.WindowInSeconds,
                rateLimitingOptions.ParametersUpdate.PermitLimit, rateLimitingOptions.ParametersUpdate.WindowInSeconds,
                rateLimitingOptions.ParametersQuery.PermitLimit, rateLimitingOptions.ParametersQuery.WindowInSeconds,
                rateLimitingOptions.GlobalLimit.PermitLimit, rateLimitingOptions.GlobalLimit.WindowInSeconds);
        });

        return services;
    }

    /// <summary>
    /// Gets partition key based on user authentication status
    /// Authenticated users: partition by username
    /// Anonymous users: partition by IP address
    /// </summary>
    private static string GetPartitionKey(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ??
                 context.Connection.LocalIpAddress?.ToString() ?? "unknown";
        var user = context.User?.Identity?.Name ?? "anonymous";
        return context.User?.Identity?.IsAuthenticated == true ? user : ip;
    }

    /// <summary>
    /// Creates rate limiter based on configuration
    /// </summary>
    private static RateLimitPartition<string> CreateRateLimiter(
        string partitionKey,
        EndpointLimitOptions config)
    {
        return config.Algorithm == RateLimitAlgorithm.FixedWindow
            ? RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: partitionKey,
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = config.PermitLimit,
                    Window = TimeSpan.FromSeconds(config.WindowInSeconds),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                })
            : RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: partitionKey,
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = config.PermitLimit,
                    Window = TimeSpan.FromSeconds(config.WindowInSeconds),
                    SegmentsPerWindow = config.SegmentsPerWindow,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 0
                });
    }
}
