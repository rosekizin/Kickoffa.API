using Serilog;
using Serilog.Core;

namespace Kickoffa.API.TestUtils.Serilog
{
    /// <summary>
    /// Test fixture that resets and isolates the global Serilog logger for each test run.
    /// </summary>
    /// <remarks>
    /// This fixture ensures that <c>Log.Logger</c> is cleared and reinitialized before and after 
    /// each test execution within the "Logging" collection. It prevents interference between tests 
    /// that modify Serilog's global logger.
    /// </remarks>
    public class LoggingFixture : IDisposable
    {
        /// <summary>
        /// Initializes the fixture and resets the global Serilog logger before the test.
        /// </summary>
        public LoggingFixture()
        {
            // Garante que nenhum logger anterior está ativo
            Log.CloseAndFlush();
            Log.Logger = Logger.None;
        }

        /// <summary>
        /// Disposes the fixture, flushing and resetting the global logger after the test.
        /// </summary>
        public void Dispose()
        {
            // Garante que qualquer logger usado durante o teste seja liberado
            Log.CloseAndFlush();
            Log.Logger = Logger.None;

            GC.SuppressFinalize(this);
        }
    }
}