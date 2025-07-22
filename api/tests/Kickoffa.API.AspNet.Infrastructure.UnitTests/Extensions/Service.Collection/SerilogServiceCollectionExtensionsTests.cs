using Kickoffa.API.AspNet.Infrastructure.Configuration.Logging;
using Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using Kickoffa.API.TestUtils.InternalMember;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Serilog;
using Serilog.Events;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.Extensions.Service.Collection;

public class SerilogServiceCollectionExtensionsTests : IDisposable
{
    private readonly IServiceCollection _services;
    private readonly ISerilogConfiguration _serilogConfiguration;
    private readonly IHostEnvironmentWrapper _hostEnvironmentWrapper;

    public SerilogServiceCollectionExtensionsTests()
    {
        _services = new ServiceCollection();
        _serilogConfiguration = Substitute.For<ISerilogConfiguration>();
        _hostEnvironmentWrapper = Substitute.For<IHostEnvironmentWrapper>();
        
        // Setup default configuration values
        _serilogConfiguration.MinimumLevel.Returns("Information");
        _serilogConfiguration.WriteToConsole.Returns(true);
        _serilogConfiguration.WriteToFile.Returns(false);
        _serilogConfiguration.EnableEnrichers.Returns(true);
        _serilogConfiguration.OutputTemplate.Returns("[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
        _serilogConfiguration.FilePath.Returns("logs/test-.txt");
        _serilogConfiguration.FileSizeLimitMB.Returns(10L);
        _serilogConfiguration.RetainedFileCountLimit.Returns(31);

        _hostEnvironmentWrapper.IsDevelopment().Returns(false);
    }

    [Fact]
    public void AddSerilog_ShouldRegisterSerilogServices()
    {
        // Act
        _services.AddSerilog(_serilogConfiguration, _hostEnvironmentWrapper);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        
        // Verificar se ILogger foi registrado
        var logger = serviceProvider.GetService<Serilog.ILogger>();
        Assert.NotNull(logger);
        
        // Verificar se a configuração foi registrada
        var config = serviceProvider.GetService<ISerilogConfiguration>();
        Assert.NotNull(config);
        Assert.Same(_serilogConfiguration, config);
    }

    [Fact]
    public void AddSerilog_ShouldConfigureMinimumLevel_Information()
    {
        // Arrange
        _serilogConfiguration.MinimumLevel.Returns("Information");

        // Act
        _services.AddSerilog(_serilogConfiguration, _hostEnvironmentWrapper);

        // Assert
        // Verificar se o logger foi criado sem exceções
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);

        var loggerMinimumLevel = logger.GetPrivateFieldValue<ILogger, LogEventLevel>("_minimumLevel");
        Assert.Equal(LogEventLevel.Information, loggerMinimumLevel);
    }

    [Fact]
    public void AddSerilog_ShouldConfigureMinimumLevel_Debug()
    {
        // Arrange
        _serilogConfiguration.MinimumLevel.Returns("Debug");

        // Act
        _services.AddSerilog(_serilogConfiguration, _hostEnvironmentWrapper);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);

        var loggerMinimumLevel = logger.GetPrivateFieldValue<ILogger, LogEventLevel>("_minimumLevel");
        Assert.Equal(LogEventLevel.Debug, loggerMinimumLevel);
    }

    [Fact]
    public void AddSerilog_ShouldHandleInvalidMinimumLevel()
    {
        // Arrange
        _serilogConfiguration.MinimumLevel.Returns("InvalidLevel");

        // Act & Assert - Não deve lançar exceção
        _services.AddSerilog(_serilogConfiguration, _hostEnvironmentWrapper);
        
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);

        var loggerMinimumLevel = logger.GetPrivateFieldValue<ILogger, LogEventLevel>("_minimumLevel");
        Assert.Equal(LogEventLevel.Information, loggerMinimumLevel);
    }

    [Fact]
    public void AddSerilog_ShouldConfigureConsoleOutput_WhenEnabled()
    {
        // Arrange
        _serilogConfiguration.WriteToConsole.Returns(true);

        // Act
        _services.AddSerilog(_serilogConfiguration, _hostEnvironmentWrapper);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilog_ShouldConfigureFileOutput_WhenEnabled()
    {
        // Arrange
        _serilogConfiguration.WriteToFile.Returns(true);
        _serilogConfiguration.FilePath.Returns("logs/test-.txt");
        _serilogConfiguration.FileSizeLimitMB.Returns(50L);
        _serilogConfiguration.RetainedFileCountLimit.Returns(15);

        // Act
        _services.AddSerilog(_serilogConfiguration, _hostEnvironmentWrapper);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilog_ShouldConfigureEnrichers_WhenEnabled()
    {
        // Arrange
        _serilogConfiguration.EnableEnrichers.Returns(true);

        // Act
        _services.AddSerilog(_serilogConfiguration, _hostEnvironmentWrapper);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilog_ShouldNotConfigureEnrichers_WhenDisabled()
    {
        // Arrange
        _serilogConfiguration.EnableEnrichers.Returns(false);

        // Act
        _services.AddSerilog(_serilogConfiguration, _hostEnvironmentWrapper);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogConsoleOnly_ShouldRegisterBasicSerilogServices()
    {
        // Act
        _services.AddSerilogConsoleOnly(_hostEnvironmentWrapper);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<Serilog.ILogger>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogConsoleOnly_ShouldWorkInDevelopmentEnvironment()
    {
        // Arrange
        _hostEnvironmentWrapper.IsDevelopment().Returns(true);

        // Act
        _services.AddSerilogConsoleOnly(_hostEnvironmentWrapper);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogConsoleOnly_ShouldWorkInProductionEnvironment()
    {
        // Arrange
        _hostEnvironmentWrapper.IsDevelopment().Returns(false);

        // Act
        _services.AddSerilogConsoleOnly(_hostEnvironmentWrapper);

        // Assert
        var serviceProvider = _services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILogger>();
        Assert.NotNull(logger);
    }

    public void Dispose()
    {
        // Limpar o logger estático do Serilog após cada teste
        Log.CloseAndFlush();
        GC.SuppressFinalize(this);
    }
}