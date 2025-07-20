using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Kickoffa.API.AspNet.Infrastructure.Configuration.Logging;
using Serilog;
using Xunit;

namespace Kickoffa.API.Integration.Tests.Serilog;

/// <summary>
/// Testes para verificar se o Program.cs está configurando o Serilog corretamente
/// </summary>
public class ProgramSerilogTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramSerilogTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Program_ShouldRegisterSerilogConfiguration()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var serilogConfig = scope.ServiceProvider.GetService<ISerilogConfiguration>();

        // Assert
        Assert.NotNull(serilogConfig);
    }

    [Fact]
    public void Program_ShouldRegisterILoggerFactory()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetService<ILoggerFactory>();

        // Assert
        Assert.NotNull(loggerFactory);
    }

    [Fact]
    public void Program_ShouldCreateTypedLoggers()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var logger1 = scope.ServiceProvider.GetService<ILogger<ProgramSerilogTests>>();
        var logger2 = scope.ServiceProvider.GetService<ILogger<Program>>();

        // Assert
        Assert.NotNull(logger1);
        Assert.NotNull(logger2);
        Assert.NotSame(logger1, logger2); // Devem ser instâncias diferentes
    }

    [Fact]
    public void Program_ShouldConfigureSerilogWithCorrectSettings()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var serilogConfig = scope.ServiceProvider.GetRequiredService<ISerilogConfiguration>();

        // Assert
        Assert.NotNull(serilogConfig.MinimumLevel);
        Assert.NotEmpty(serilogConfig.MinimumLevel);
        Assert.NotNull(serilogConfig.OutputTemplate);
        Assert.NotEmpty(serilogConfig.OutputTemplate);
    }

    [Fact]
    public void Program_ShouldAllowLoggingAtDifferentLevels()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProgramSerilogTests>>();

        // Act & Assert - Não deve lançar exceções
        logger.LogTrace("Trace level log");
        logger.LogDebug("Debug level log");
        logger.LogInformation("Information level log");
        logger.LogWarning("Warning level log");
        logger.LogError("Error level log");
        logger.LogCritical("Critical level log");
    }

    [Fact]
    public void Program_ShouldSupportStructuredLogging()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProgramSerilogTests>>();

        // Act & Assert - Não deve lançar exceções
        logger.LogInformation("User {UserId} logged in at {LoginTime}", 
            Guid.NewGuid(), DateTime.UtcNow);
        
        logger.LogWarning("Failed login attempt for {Email} from {IpAddress}", 
            "test@example.com", "192.168.1.1");
        
        logger.LogError("Database connection failed for {ConnectionString} after {RetryCount} attempts", 
            "Server=localhost;Database=test", 3);
    }

    [Fact]
    public void Program_ShouldHandleExceptionLogging()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProgramSerilogTests>>();
        var testException = new InvalidOperationException("Test exception for Program tests");

        // Act & Assert - Não deve lançar exceções
        logger.LogError(testException, "An error occurred in Program test");
        logger.LogCritical(testException, "A critical error occurred in Program test");
    }

    [Fact]
    public void Program_ShouldHandleLogScopes()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProgramSerilogTests>>();

        // Act & Assert - Não deve lançar exceções
        using (logger.BeginScope("TestScope"))
        {
            logger.LogInformation("Log inside scope");
            
            using (logger.BeginScope("NestedScope"))
            {
                logger.LogInformation("Log inside nested scope");
            }
        }
        
        logger.LogInformation("Log outside scope");
    }

    [Fact]
    public void Program_ShouldHandleLogScopesWithProperties()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProgramSerilogTests>>();

        // Act & Assert - Não deve lançar exceções
        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["UserId"] = Guid.NewGuid(),
            ["RequestId"] = "REQ-123",
            ["CorrelationId"] = "CORR-456"
        }))
        {
            logger.LogInformation("Processing request");
            logger.LogWarning("Request took longer than expected");
        }
    }

    [Fact]
    public async Task Program_ShouldHandleHighVolumeLogging()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProgramSerilogTests>>();

        // Act & Assert - Não deve lançar exceções mesmo com muitos logs
        var tasks = new List<Task>();
        
        for (int i = 0; i < 10; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 10; j++)
                {
                    logger.LogInformation("High volume log from task {TaskId}, iteration {Iteration}", 
                        taskId, j);
                }
            }, TestContext.Current.CancellationToken));
        }

        await Task.WhenAll(tasks);
    }

    [Fact]
    public void Program_ShouldHandleLoggerDisposal()
    {
        // Arrange & Act
        ILogger<ProgramSerilogTests>? logger;
        
        using (var scope = _factory.Services.CreateScope())
        {
            logger = scope.ServiceProvider.GetRequiredService<ILogger<ProgramSerilogTests>>();
            logger.LogInformation("Log before scope disposal");
        }

        // Assert - Não deve lançar exceção após disposal do scope
        // Note: O logger ainda pode funcionar pois o Serilog é singleton
        logger.LogInformation("Log after scope disposal");
    }

    public void Dispose()
    {
        // Garantir que o logger seja fechado
        Log.CloseAndFlush();
        GC.SuppressFinalize(this);
    }
}