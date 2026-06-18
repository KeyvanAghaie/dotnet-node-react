using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace webapi.Filters;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class InvalidateCacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly string[] _cacheKeys;

    public InvalidateCacheAttribute(params string[] cacheKeys)
    {
        _cacheKeys = cacheKeys;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        // Only invalidate if the action was successful
        if (executedContext.Exception == null &&
            (executedContext.Result is Microsoft.AspNetCore.Mvc.ObjectResult obj &&
             (obj.StatusCode == null || obj.StatusCode < 400)))
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<InvalidateCacheAttribute>>();

            foreach (var key in _cacheKeys)
            {
                // Support wildcards by iterating all cache entries (optional enhancement)
                cache.Remove(key);
                logger.LogInformation("Cache invalidated: {CacheKey}", key);
            }
        }
    }
}