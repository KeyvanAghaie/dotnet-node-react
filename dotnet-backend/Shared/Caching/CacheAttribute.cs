using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace webapi.Filters;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class CacheAttribute : Attribute, IAsyncActionFilter
{
    private readonly int _durationInSeconds;
    private readonly string _cacheKeyPrefix;
    private readonly bool _varyByQueryParams;

    public CacheAttribute(int durationInSeconds = 300, string cacheKeyPrefix = "", bool varyByQueryParams = true)
    {
        _durationInSeconds = durationInSeconds;
        _cacheKeyPrefix = cacheKeyPrefix;
        _varyByQueryParams = varyByQueryParams;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<CacheAttribute>>();

        // Build cache key
        var cacheKey = BuildCacheKey(context);

        // Try to get from cache
        if (cache.TryGetValue(cacheKey, out object? cachedValue))
        {
            logger.LogInformation("Cache HIT for key: {CacheKey}", cacheKey);

            var contentResult = new ContentResult
            {
                Content = cachedValue?.ToString(),
                ContentType = "application/json",
                StatusCode = 200
            };

            context.Result = contentResult;
            return;
        }

        logger.LogInformation("Cache MISS for key: {CacheKey}", cacheKey);

        // Execute the action
        var executedContext = await next();

        // Cache the result if successful
        if (executedContext.Result is ObjectResult objectResult && objectResult.StatusCode == 200)
        {
            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(_durationInSeconds))
                .SetSlidingExpiration(TimeSpan.FromSeconds(_durationInSeconds / 2));

            var serializedValue = JsonSerializer.Serialize(objectResult.Value);
            cache.Set(cacheKey, serializedValue, options);

            logger.LogInformation("Cached result for key: {CacheKey}", cacheKey);
        }
    }

    private string BuildCacheKey(ActionExecutingContext context)
    {
        var keyBuilder = new System.Text.StringBuilder();

        // Base key: prefix + controller + action
        keyBuilder.Append(string.IsNullOrEmpty(_cacheKeyPrefix)
            ? $"{context.RouteData.Values["controller"]}_{context.RouteData.Values["action"]}"
            : _cacheKeyPrefix);

        // Add route parameters
        foreach (var arg in context.ActionArguments)
        {
            if (arg.Value != null && !IsComplexType(arg.Value))
            {
                keyBuilder.Append($"_{arg.Key}:{arg.Value}");
            }
        }

        // Add query string if enabled
        if (_varyByQueryParams && context.HttpContext.Request.QueryString.HasValue)
        {
            keyBuilder.Append(context.HttpContext.Request.QueryString.Value);
        }

        return keyBuilder.ToString();
    }

    private static bool IsComplexType(object value)
    {
        var type = value.GetType();
        return type.IsClass && type != typeof(string) && !type.IsPrimitive;
    }
}