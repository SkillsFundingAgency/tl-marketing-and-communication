using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace sfa.Tl.Marketing.Communication.Functions.UnitTests.Extensions;

public static class LoggerExtensions
{
    public static void HasLoggedMessage(this ILogger logger, 
        string message,
        LogLevel logLevel = LogLevel.Information)
    {
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == logLevel &&
                args[2] != null && args[2].ToString() == message);
    }

    public static void HasLoggedMessageLike(this ILogger logger,
        string message,
        LogLevel logLevel = LogLevel.Information)
    {
        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == logLevel &&
                args[2] != null && args[2].ToString().Contains(message));
    }
}