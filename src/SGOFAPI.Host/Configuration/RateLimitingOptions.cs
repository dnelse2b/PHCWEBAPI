namespace PHCAPI.Host.Configuration;

/// <summary>
/// Rate Limiting configuration options
/// 🛡️ SECURITY: Configurable limits per endpoint to prevent brute force, DoS, and API abuse
/// </summary>
public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    /// <summary>
    /// Login endpoint rate limiting configuration
    /// 🔴 CRITICAL: Anti-brute force protection
    /// </summary>
    public EndpointLimitOptions LoginEndpoint { get; set; } = new();

    /// <summary>
    /// Parameter creation endpoint rate limiting
    /// 🟠 HIGH: Anti-spam protection
    /// </summary>
    public EndpointLimitOptions ParametersCreate { get; set; } = new();

    /// <summary>
    /// Parameter deletion endpoint rate limiting
    /// 🔴 CRITICAL: Prevent accidental/malicious mass deletion
    /// </summary>
    public EndpointLimitOptions ParametersDelete { get; set; } = new();

    /// <summary>
    /// Parameter update endpoint rate limiting
    /// 🟡 MEDIUM: Control frequent modifications
    /// </summary>
    public EndpointLimitOptions ParametersUpdate { get; set; } = new();

    /// <summary>
    /// Parameter query endpoint rate limiting
    /// 🟢 LOW: Prevent scraping and excessive queries
    /// </summary>
    public EndpointLimitOptions ParametersQuery { get; set; } = new();

    /// <summary>
    /// Global fallback rate limiter
    /// 🌍 Applies to endpoints without specific policy
    /// </summary>
    public EndpointLimitOptions GlobalLimit { get; set; } = new();
}

/// <summary>
/// Configuration for individual endpoint rate limits
/// </summary>
public sealed class EndpointLimitOptions
{
    /// <summary>
    /// Maximum number of requests allowed in the time window
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Time window in seconds
    /// </summary>
    public int WindowInSeconds { get; set; } = 60;

    /// <summary>
    /// Algorithm type: FixedWindow or SlidingWindow
    /// - FixedWindow: Hard reset at window boundary (better for critical operations)
    /// - SlidingWindow: Gradual reset (better for user experience)
    /// </summary>
    public RateLimitAlgorithm Algorithm { get; set; } = RateLimitAlgorithm.SlidingWindow;

    /// <summary>
    /// Number of segments for Sliding Window algorithm
    /// Higher = smoother reset, but more memory
    /// Ignored for FixedWindow
    /// </summary>
    public int SegmentsPerWindow { get; set; } = 6;

    /// <summary>
    /// Whether to enable this rate limit policy
    /// </summary>
    public bool Enabled { get; set; } = true;
}

/// <summary>
/// Rate limiting algorithm types
/// </summary>
public enum RateLimitAlgorithm
{
    /// <summary>
    /// Fixed window: All permits reset at the same time
    /// Example: 00:00-00:59 = 5 requests, 01:00-01:59 = reset
    /// </summary>
    FixedWindow,

    /// <summary>
    /// Sliding window: Gradual reset over time segments
    /// Example: Window slides every 10 seconds (6 segments of 10s each)
    /// </summary>
    SlidingWindow
}
