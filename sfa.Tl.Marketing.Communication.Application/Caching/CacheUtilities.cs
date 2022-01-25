using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.Application.Caching;

public static class CacheUtilities
{
    public const int DefaultCacheExpiryInSeconds = 60;

    public static MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions(
        int absoluteExpirationInSeconds = DefaultCacheExpiryInSeconds,
        ILogger logger = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(absoluteExpirationInSeconds)
        };

        if (logger is not null)
            options.PostEvictionCallbacks.Add(
                new PostEvictionCallbackRegistration
                {
                    EvictionCallback = EvictionLoggingCallback,
                    State = logger
                });

        return options;
    }

    public static MemoryCacheEntryOptions DefaultMemoryCacheEntryOptions(
        IDateTimeService dateTimeService,
        ILogger logger,
        int absoluteExpirationInSeconds = DefaultCacheExpiryInSeconds,
        int slidingExpirationInSeconds = DefaultCacheExpiryInSeconds,
        int size = 1) =>
        new()
        {
            AbsoluteExpiration = absoluteExpirationInSeconds > 0
                ? new DateTimeOffset(dateTimeService.Now.AddSeconds(absoluteExpirationInSeconds))
                : null,
            Priority = CacheItemPriority.Normal,
            SlidingExpiration = slidingExpirationInSeconds > 0
                ? TimeSpan.FromMinutes(slidingExpirationInSeconds)
                : null,
            Size = size,
            PostEvictionCallbacks =
            {
                new PostEvictionCallbackRegistration
                {
                    EvictionCallback = EvictionLoggingCallback,
                    State = logger
                }
            }
        };

    public static void EvictionLoggingCallback(object key, object value, EvictionReason reason, object state)
    {
        var logger = state as ILogger;
        logger?.LogInformation("Entry {key} was evicted from the cache. Reason: {reason}.", key, reason);
    }
}