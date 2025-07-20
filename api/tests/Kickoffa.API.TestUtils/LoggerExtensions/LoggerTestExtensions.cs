using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.Core;
using Xunit;
using Xunit.Sdk;

namespace Kickoffa.API.TestUtils.LoggerExtensions
{
    public static class LoggerTestExtensions
    {
        public static void ShouldHaveLoggedContain<T>(this ILogger<T> logger, LogLevel level, string expectedLogSubstring, int times)
        {
            var logs = logger.ReceivedCalls().Where(call => call.GetMethodInfo().Name == "Log");

            var filteredLogCalls = logs
            .Where(call => 
                        (LogLevel)call.GetArguments()[0]! == level &&
                        (call.GetArguments()[2]?.ToString()?.Contains(expectedLogSubstring) ?? false)
            ).ToList();

            if (filteredLogCalls.Count == 0)
            {
                static string buildCapturedLogCall(ICall call)
                {
                    return $"Level: {(LogLevel)call.GetArguments()[0]!} | Text: \"{call.GetArguments()[2]?.ToString() ?? "<null>"}\"";
                }

                throw new XunitException(
                    $"Esperava log de nível {level} contendo: \"{expectedLogSubstring}\".\n" +
                    $"Lista de todas as mensagens capturadas:\n{string.Join("\n", logs.Select(buildCapturedLogCall))}");
            }
            else
            {
                Assert.Equal(times, filteredLogCalls.Count);
            }
        }

        public static void ShouldHaveLoggedException<T, TException>(
            this ILogger<T> logger,
            LogLevel level,
            string expectedLogSubstring,
            string? expectedExceptionMessageSubstring = null)
            where TException : Exception
        {
            var logCalls = logger.ReceivedCalls()
            .Where(
                call => call.GetMethodInfo().Name == "Log" &&
                        (LogLevel)call.GetArguments()[0]! == level &&
                        (call.GetArguments()[2]?.ToString()?.Contains(expectedLogSubstring) ?? false)
            ).ToList();

            if (logCalls.Count == 0)
            {
                throw new XunitException(
                    $"Esperava log de nível {level} contendo: \"{expectedLogSubstring}\".\n" +
                    $"Mensagens capturadas:\n{string.Join("\n", logCalls.Select(c => c.GetArguments()[2]?.ToString() ?? "<null>"))}");
            }
            else
            {
                foreach (var call in logCalls)
                {
                    var exception = call.GetArguments()[3];  // exception argument

                    if (exception is TException typedEx)
                    {
                        var matchesExceptionMessage = expectedExceptionMessageSubstring == null ||
                                                      (typedEx.Message?.Contains(expectedExceptionMessageSubstring) ?? false);

                        if (!matchesExceptionMessage)
                        {
                            throw new XunitException(
                                $"Esperava log de nível {level} com exceção do tipo {typeof(TException).Name},\n" +
                                $"contendo mensagem: \"{expectedLogSubstring ?? "<ignorado>"}\"\n" +
                                $"e exceção contendo: \"{expectedExceptionMessageSubstring ?? "<ignorado>"}\".\n\n" +
                                $"Chamadas encontradas:\n{string.Join("\n---\n", logCalls.Select(c =>
                                {
                                    var s = c.GetArguments()[2]?.ToString() ?? "<null>";
                                    var e = c.GetArguments()[3] as Exception;
                                    return $"Mensagem: {s}\nExceção: {e?.GetType().Name ?? "<null>"} - {e?.Message ?? "<null>"}";
                                }))}");
                        }
                    }
                }
            }
        }
    }
}