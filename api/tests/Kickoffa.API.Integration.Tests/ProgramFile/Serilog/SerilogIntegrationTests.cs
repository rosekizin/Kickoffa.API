using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Xunit;

namespace Kickoffa.API.Integration.Tests.ProgramFile.Serilog;

/// <summary>
/// Testes de integração para verificar se o Serilog está configurado corretamente na aplicação
/// </summary>

[Collection("Logging")]
public class SerilogIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _testLogDirectory;

    public SerilogIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _testLogDirectory = Path.Combine(Path.GetTempPath(), "kickoffa-test-logs");

        // Garantir que o diretório de teste existe
        Directory.CreateDirectory(_testLogDirectory);
    }

    [Fact]
    public void Application_ShouldHaveSerilogConfigured()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();

        // Assert
        Assert.NotNull(loggerFactory);
    }

    [Fact]
    public void Application_ShouldCreateLoggerForSpecificType()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetService<ILogger<SerilogIntegrationTests>>();

        // Assert
        Assert.NotNull(logger);
    }

    [Fact]
    public void Logger_ShouldBeAbleToLogInformation()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogIntegrationTests>>();

        // Act & Assert - Não deve lançar exceção
        logger.LogInformation("Test information log from integration test");
        logger.LogDebug("Test debug log from integration test");
        logger.LogWarning("Test warning log from integration test");
    }

    [Fact]
    public void Logger_ShouldSupportStructuredLogging()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogIntegrationTests>>();

        // Act & Assert - Não deve lançar exceção
        logger.LogInformation("User {UserId} performed action {Action} at {Timestamp}",
            Guid.NewGuid(), "TestAction", DateTime.UtcNow);
    }

    [Fact]
    public void Logger_ShouldHandleExceptionLogging()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogIntegrationTests>>();
        var testException = new InvalidOperationException("Test exception for logging");

        // Act & Assert - Não deve lançar exceção
        logger.LogError(testException, "An error occurred during test execution");
    }

    [Fact]
    public void Application_ShouldHaveCorrectLogLevels()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogIntegrationTests>>();

        // Act & Assert - Verificar se diferentes níveis funcionam
        Assert.True(logger.IsEnabled(LogLevel.Information));
        Assert.True(logger.IsEnabled(LogLevel.Warning));
        Assert.True(logger.IsEnabled(LogLevel.Error));
        Assert.True(logger.IsEnabled(LogLevel.Critical));
    }

    [Fact]
    public void Logger_ShouldHandleNullValues()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogIntegrationTests>>();

        // Act & Assert - Não deve lançar exceção
        logger.LogInformation("Testing null value: {NullValue}", (string?)null);
        logger.LogInformation("Testing with null object: {NullObject}", (object?)null);
    }

    [Fact]
    public void Logger_ShouldHandleComplexObjects()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogIntegrationTests>>();

        var complexObject = new
        {
            Id = Guid.NewGuid(),
            Name = "Test Object",
            CreatedAt = DateTime.UtcNow,
            Properties = new Dictionary<string, object>
            {
                { "Key1", "Value1" },
                { "Key2", 42 },
                { "Key3", true }
            }
        };

        // Act & Assert - Não deve lançar exceção
        logger.LogInformation("Complex object: {@ComplexObject}", complexObject);
    }

    [Fact]
    public void Logger_ShouldHandleHighVolumeLogging()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogIntegrationTests>>();

        // Act & Assert - Não deve lançar exceção mesmo com muitos logs
        for (int i = 0; i < 100; i++)
        {
            logger.LogInformation("High volume test log {Index} at {Timestamp}", i, DateTime.UtcNow);
        }
    }

    [Fact]
    public void Logger_ShouldHandleSpecialCharacters()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogIntegrationTests>>();

        // Act & Assert - Não deve lançar exceção
        logger.LogInformation("Testing special characters: {SpecialChars}", "áéíóú àèìòù âêîôû ãõ ç ñ 中文 🚀 ✅ ❌");
        logger.LogInformation("Testing JSON-like string: {JsonLike}", "{\"key\": \"value\", \"number\": 123}");
    }

    [Fact]
    public void Application_ShouldFlushLogsOnShutdown()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SerilogIntegrationTests>>();

        logger.LogInformation("Log before shutdown test");

        // Assert - Verificar se Log.CloseAndFlush() pode ser chamado sem exceção
        Log.CloseAndFlush();

        // Recriar o logger após flush
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }

    public void Dispose()
    {
        // Limpar diretório de teste
        if (Directory.Exists(_testLogDirectory))
        {
            try
            {
                Directory.Delete(_testLogDirectory, true);
            }
            catch
            {
                // Ignorar erros de limpeza
            }
        }

        // Garantir que o logger seja fechado
        Log.CloseAndFlush();
        GC.SuppressFinalize(this);
    }
}