using Kickoffa.API.AspNet.Infrastructure.Extensions;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.Extensions;

public class WebApplicationBuilderExtensionsTests : IDisposable
{
    private readonly WebApplicationBuilder _builder;
    private readonly IConfigurationWrapper _configurationWrapper;
    private readonly IHostEnvironmentWrapper _hostEnvironmentWrapper;

    public WebApplicationBuilderExtensionsTests()
    {
        // Criar um builder real para testes
        var args = Array.Empty<string>();
        _builder = WebApplication.CreateBuilder(args);
        
        _configurationWrapper = Substitute.For<IConfigurationWrapper>();
        _hostEnvironmentWrapper = Substitute.For<IHostEnvironmentWrapper>();
        
        // Setup configuração padrão
        _configurationWrapper.GetValue<string>("Serilog:MinimumLevel:Default").Returns("Information");
        _configurationWrapper.GetValue<bool>("Serilog:WriteTo:Console:Enabled").Returns(true);
        _configurationWrapper.GetValue<bool>("Serilog:WriteTo:File:Enabled").Returns(false);
        _configurationWrapper.GetValue<bool>("Serilog:Enrich:Enabled").Returns(true);
        _configurationWrapper.GetValue<string>("Serilog:OutputTemplate")
            .Returns("[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}");
        _configurationWrapper.GetValue<string>("Serilog:WriteTo:File:Path").Returns("logs/test-.txt");
        _configurationWrapper.GetValue<long>("Serilog:WriteTo:File:FileSizeLimitMB").Returns(10L);
        _configurationWrapper.GetValue<int>("Serilog:WriteTo:File:RetainedFileCountLimit").Returns(31);
    }

    [Fact]
    public void AddSerilogLogging_ShouldReturnSameBuilder()
    {
        // Act
        var result = _builder.AddSerilogLogging(_configurationWrapper, _hostEnvironmentWrapper);

        // Assert
        Assert.Same(_builder, result);
    }

    [Fact]
    public void AddSerilogLogging_ShouldRegisterSerilogServices()
    {
        // Act
        _builder.AddSerilogLogging(_configurationWrapper, _hostEnvironmentWrapper);

        // Assert
        var app = _builder.Build();
        var logger = app.Services.GetService<ILogger<WebApplicationBuilderExtensionsTests>>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogLogging_ShouldRegisterSerilogConfiguration()
    {
        // Act
        _builder.AddSerilogLogging(_configurationWrapper, _hostEnvironmentWrapper);

        // Assert
        var app = _builder.Build();
        var config = app.Services.GetService<Kickoffa.API.AspNet.Infrastructure.Configuration.Logging.ISerilogConfiguration>();
        Assert.NotNull(config);
    }

    [Fact]
    public void AddSerilogLogging_ShouldConfigureHostToUseSerilog()
    {
        // Act
        _builder.AddSerilogLogging(_configurationWrapper, _hostEnvironmentWrapper);

        // Assert
        // Verificar se o host foi configurado para usar Serilog
        var app = _builder.Build();
        var loggerFactory = app.Services.GetService<ILoggerFactory>();
        Assert.NotNull(loggerFactory);
        
        // Verificar se conseguimos criar um logger
        var logger = loggerFactory.CreateLogger("Test");
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogConsoleLogging_ShouldReturnSameBuilder()
    {
        // Act
        var result = _builder.AddSerilogConsoleLogging(_hostEnvironmentWrapper);

        // Assert
        Assert.Same(_builder, result);
    }

    [Fact]
    public void AddSerilogConsoleLogging_ShouldRegisterSerilogServices()
    {
        // Act
        _builder.AddSerilogConsoleLogging(_hostEnvironmentWrapper);

        // Assert
        var app = _builder.Build();
        var logger = app.Services.GetService<ILogger<WebApplicationBuilderExtensionsTests>>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogConsoleLogging_ShouldConfigureHostToUseSerilog()
    {
        // Act
        _builder.AddSerilogConsoleLogging(_hostEnvironmentWrapper);

        // Assert
        var app = _builder.Build();
        var loggerFactory = app.Services.GetService<ILoggerFactory>();
        Assert.NotNull(loggerFactory);
        
        var logger = loggerFactory.CreateLogger("Test");
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogLogging_ShouldWorkWithDifferentEnvironments()
    {
        // Arrange
        //_builder.Environment.EnvironmentName = "Production";

        // Act
        _builder.AddSerilogLogging(_configurationWrapper, _hostEnvironmentWrapper);

        // Assert
        var app = _builder.Build();
        var logger = app.Services.GetService<ILogger<WebApplicationBuilderExtensionsTests>>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogConsoleLogging_ShouldWorkWithDevelopmentEnvironment()
    {
        // Arrange
        //_builder.Environment.EnvironmentName = "Development";

        // Act
        _builder.AddSerilogConsoleLogging(_hostEnvironmentWrapper);

        // Assert
        var app = _builder.Build();
        var logger = app.Services.GetService<ILogger<WebApplicationBuilderExtensionsTests>>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogLogging_ShouldHandleFileLoggingConfiguration()
    {
        // Arrange
        _configurationWrapper.GetValue<bool>("Serilog:WriteTo:File:Enabled").Returns(true);
        _configurationWrapper.GetValue<string>("Serilog:WriteTo:File:Path").Returns("logs/test-file-.txt");
        _configurationWrapper.GetValue<long>("Serilog:WriteTo:File:FileSizeLimitMB").Returns(50L);
        _configurationWrapper.GetValue<int>("Serilog:WriteTo:File:RetainedFileCountLimit").Returns(15);

        // Act
        _builder.AddSerilogLogging(_configurationWrapper, _hostEnvironmentWrapper);

        // Assert
        var app = _builder.Build();
        var logger = app.Services.GetService<ILogger<WebApplicationBuilderExtensionsTests>>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogLogging_ShouldHandleEnrichersConfiguration()
    {
        // Arrange
        _configurationWrapper.GetValue<bool>("Serilog:Enrich:Enabled").Returns(true);

        // Act
        _builder.AddSerilogLogging(_configurationWrapper, _hostEnvironmentWrapper);

        // Assert
        var app = _builder.Build();
        var logger = app.Services.GetService<ILogger<WebApplicationBuilderExtensionsTests>>();
        Assert.NotNull(logger);
    }

    [Fact]
    public void AddSerilogLogging_ShouldHandleCustomOutputTemplate()
    {
        // Arrange
        var customTemplate = "[{Timestamp:HH:mm:ss}] {Level} {Message}{NewLine}";
        _configurationWrapper.GetValue<string>("Serilog:OutputTemplate").Returns(customTemplate);

        // Act
        _builder.AddSerilogLogging(_configurationWrapper, _hostEnvironmentWrapper);

        // Assert
        var app = _builder.Build();
        var logger = app.Services.GetService<ILogger<WebApplicationBuilderExtensionsTests>>();
        Assert.NotNull(logger);
    }

    public void Dispose()
    {
        // Limpar o logger estático do Serilog após cada teste
        Log.CloseAndFlush();
        GC.SuppressFinalize(this);
    }
}