using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog.Extensions;

public static class AddLimiterRules
{
    public static void AddRateLimiterRules(this IServiceCollection services)
    {
        services.AddRateLimiter(limiterOptions => 
            limiterOptions.AddFixedWindowLimiter(policyName: "fixed", options =>
            {
                options.PermitLimit = 4;
                options.Window = TimeSpan.FromSeconds(20);
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                options.QueueLimit = 0;
            }).OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = 429;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    await context.HttpContext.Response.WriteAsync(
                        $"Too many requests. Please, try again after {retryAfter.TotalMinutes} minute(s). ",
                        cancellationToken: token);
                }
                else
                {
                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please, try again. ", cancellationToken: token);
                }
            });
    }
}
