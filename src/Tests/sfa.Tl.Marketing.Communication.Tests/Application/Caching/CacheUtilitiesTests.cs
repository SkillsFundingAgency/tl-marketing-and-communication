using System;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using sfa.Tl.Marketing.Communication.Application.Caching;
using sfa.Tl.Marketing.Communication.Application.Interfaces;

namespace sfa.Tl.Marketing.Communication.UnitTests.Application.Caching;

public class CacheUtilitiesTests
{
    [Fact]
    public void Basic_DefaultMemoryCacheEntryOptions_Returns_Expected_Value()
    {
        const int absoluteExpirationInSeconds = 90;
        var logger = Substitute.For<ILogger<object>>();

        var options = CacheUtilities.DefaultMemoryCacheEntryOptions(
            absoluteExpirationInSeconds,
            logger);

        options.Should().NotBeNull();

        options.PostEvictionCallbacks.Count.Should().Be(1);
        options.AbsoluteExpirationRelativeToNow
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void Basic_DefaultMemoryCacheEntryOptions_Returns_Expected_Value2()
    {
        const int absoluteExpirationInSeconds = 90;

        var options = CacheUtilities.DefaultMemoryCacheEntryOptions(
            absoluteExpirationInSeconds);

        options.Should().NotBeNull();

        options.PostEvictionCallbacks.Count.Should().Be(0);
        options.AbsoluteExpirationRelativeToNow
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void DefaultMemoryCacheEntryOptions_Returns_Expected_Value()
    {
        var dateTimeService = Substitute.For<IDateTimeService>();
        var logger = Substitute.For<ILogger<object>>();

        var options = CacheUtilities.DefaultMemoryCacheEntryOptions(
            dateTimeService,
            logger);

        options.Should().NotBeNull();

        options.PostEvictionCallbacks.Count.Should().Be(1);
        options.AbsoluteExpiration
            .Should()
            .Be(new DateTimeOffset(dateTimeService.Now.AddSeconds(CacheUtilities.DefaultCacheExpiryInSeconds)));
    }

    [Fact]
    public void EvictionLoggingCallback_Ignores_Null_Logger()
    {
        const string key = "TEST_KEY";
        const EvictionReason reason = EvictionReason.Removed;
        var value = new { x = "test " };

        Action act = () => CacheUtilities.EvictionLoggingCallback(key, value, reason, null);
        act
            .Should().NotThrow<ArgumentNullException>();
    }

    [Fact]
    public void EvictionLoggingCallback_Calls_Logger()
    {
        const string key = "TEST_KEY";
        const EvictionReason reason = EvictionReason.Removed;
        var value = new { x = "test " };
        var logger = Substitute.For<ILogger<object>>();

        CacheUtilities.EvictionLoggingCallback(key, value, reason, logger);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Information);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args => args[2] != null && args[2].ToString() == $"Entry {key} was evicted from the cache. Reason: {reason}.");
    }
}