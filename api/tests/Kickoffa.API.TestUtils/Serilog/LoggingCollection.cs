using Xunit;

namespace Kickoffa.API.TestUtils.Serilog
{
    /// <summary>
    /// Declares the "Logging" test collection for tests that manipulate Serilog's global logger.
    /// </summary>
    /// <remarks>
    /// All tests marked with <c>[Collection("Logging")]</c> will run sequentially and share
    /// the same instance of <see cref="LoggingFixture"/>. This ensures logger isolation and
    /// avoids side effects when configuring <c>Log.Logger</c> globally.
    /// </remarks>
    [CollectionDefinition("Logging")]
    public class LoggingCollection : ICollectionFixture<LoggingFixture>
    {
        // This class has no code, and is never created. Its purpose is to associate
        // the LoggingFixture with the "Logging" test collection.
    }
}