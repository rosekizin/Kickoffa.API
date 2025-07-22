using Kickoffa.API.AspNet.Infrastructure.Configuration.Logging;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using Microsoft.Extensions.Configuration;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.Configuration.Logging;

/// <summary>
/// Testes de integração para verificar se a configuração do Serilog funciona com arquivos reais de configuração
/// </summary>
public class SerilogConfigurationIntegrationTests
{
    [Fact]
    public void SerilogConfiguration_ShouldLoadFromAppsettingsJson()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json", optional: false)
            .Build();

        var configurationWrapper = new ConfigurationWrapper(configuration);

        // Act
        var serilogConfig = new SerilogConfiguration(configurationWrapper);

        // Assert
        Assert.NotNull(serilogConfig);
        Assert.NotEmpty(serilogConfig.MinimumLevel);
        Assert.NotEmpty(serilogConfig.OutputTemplate);
    }

    [Fact]
    public void SerilogConfiguration_ShouldHandleProductionSettings()
    {
        // Arrange
        var configurationData = new Dictionary<string, string?>
        {
            ["Serilog:MinimumLevel:Default"] = "Information",
            ["Serilog:WriteTo:Console:Enabled"] = "true",
            ["Serilog:WriteTo:File:Enabled"] = "false",
            ["Serilog:Enrich:Enabled"] = "true",
            ["Serilog:OutputTemplate"] = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var configurationWrapper = new ConfigurationWrapper(configuration);

        // Act
        var serilogConfig = new SerilogConfiguration(configurationWrapper);

        // Assert
        Assert.Equal("Information", serilogConfig.MinimumLevel);
        Assert.True(serilogConfig.WriteToConsole);
        Assert.False(serilogConfig.WriteToFile);
        Assert.True(serilogConfig.EnableEnrichers);
        Assert.Equal("[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}", 
            serilogConfig.OutputTemplate);
    }

    [Fact]
    public void SerilogConfiguration_ShouldHandleDevelopmentSettings()
    {
        // Arrange
        var configurationData = new Dictionary<string, string?>
        {
            ["Serilog:MinimumLevel:Default"] = "Debug",
            ["Serilog:WriteTo:Console:Enabled"] = "true",
            ["Serilog:WriteTo:File:Enabled"] = "true",
            ["Serilog:WriteTo:File:Path"] = "logs/kickoffa-api-dev-.txt",
            ["Serilog:WriteTo:File:FileSizeLimitMB"] = "50",
            ["Serilog:WriteTo:File:RetainedFileCountLimit"] = "7",
            ["Serilog:Enrich:Enabled"] = "true"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var configurationWrapper = new ConfigurationWrapper(configuration);

        // Act
        var serilogConfig = new SerilogConfiguration(configurationWrapper);

        // Assert
        Assert.Equal("Debug", serilogConfig.MinimumLevel);
        Assert.True(serilogConfig.WriteToConsole);
        Assert.True(serilogConfig.WriteToFile);
        Assert.Equal("logs/kickoffa-api-dev-.txt", serilogConfig.FilePath);
        Assert.Equal(50L, serilogConfig.FileSizeLimitMB);
        Assert.Equal(7, serilogConfig.RetainedFileCountLimit);
        Assert.True(serilogConfig.EnableEnrichers);
    }

    [Fact]
    public void SerilogConfiguration_ShouldHandleMinimalConfiguration()
    {
        // Arrange - Configuração mínima, apenas o essencial
        var configurationData = new Dictionary<string, string?>
        {
            ["Serilog:MinimumLevel:Default"] = "Warning",
            ["Serilog:WriteTo:Console:Enabled"] = "true"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var configurationWrapper = new ConfigurationWrapper(configuration);

        // Act
        var serilogConfig = new SerilogConfiguration(configurationWrapper);

        // Assert
        Assert.Equal("Warning", serilogConfig.MinimumLevel);
        Assert.True(serilogConfig.WriteToConsole);
        Assert.False(serilogConfig.WriteToFile); // Default
        Assert.False(serilogConfig.EnableEnrichers); // Default
        Assert.Equal("logs/kickoffa-api-.txt", serilogConfig.FilePath); // Default
    }

    [Fact]
    public void SerilogConfiguration_ShouldHandleFileOnlyConfiguration()
    {
        // Arrange
        var configurationData = new Dictionary<string, string?>
        {
            ["Serilog:MinimumLevel:Default"] = "Error",
            ["Serilog:WriteTo:Console:Enabled"] = "false",
            ["Serilog:WriteTo:File:Enabled"] = "true",
            ["Serilog:WriteTo:File:Path"] = "logs/errors-only-.txt",
            ["Serilog:WriteTo:File:FileSizeLimitMB"] = "100",
            ["Serilog:WriteTo:File:RetainedFileCountLimit"] = "90"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var configurationWrapper = new ConfigurationWrapper(configuration);

        // Act
        var serilogConfig = new SerilogConfiguration(configurationWrapper);

        // Assert
        Assert.Equal("Error", serilogConfig.MinimumLevel);
        Assert.False(serilogConfig.WriteToConsole);
        Assert.True(serilogConfig.WriteToFile);
        Assert.Equal("logs/errors-only-.txt", serilogConfig.FilePath);
        Assert.Equal(100L, serilogConfig.FileSizeLimitMB);
        Assert.Equal(90, serilogConfig.RetainedFileCountLimit);
    }

    [Fact]
    public void SerilogConfiguration_ShouldHandleCustomOutputTemplate()
    {
        // Arrange
        var customTemplate = "[{Timestamp:HH:mm:ss}] {Level} {Message}{NewLine}";
        var configurationData = new Dictionary<string, string?>
        {
            ["Serilog:OutputTemplate"] = customTemplate
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var configurationWrapper = new ConfigurationWrapper(configuration);

        // Act
        var serilogConfig = new SerilogConfiguration(configurationWrapper);

        // Assert
        Assert.Equal(customTemplate, serilogConfig.OutputTemplate);
    }

    [Theory]
    [InlineData("Debug")]
    [InlineData("Information")]
    [InlineData("Warning")]
    [InlineData("Error")]
    [InlineData("Fatal")]
    public void SerilogConfiguration_ShouldAcceptAllValidLogLevels(string logLevel)
    {
        // Arrange
        var configurationData = new Dictionary<string, string?>
        {
            ["Serilog:MinimumLevel:Default"] = logLevel
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var configurationWrapper = new ConfigurationWrapper(configuration);

        // Act
        var serilogConfig = new SerilogConfiguration(configurationWrapper);

        // Assert
        Assert.Equal(logLevel, serilogConfig.MinimumLevel);
    }

    [Fact]
    public void SerilogConfiguration_ShouldHandleBooleanValues()
    {
        // Arrange - Testar diferentes formas de representar booleanos
        var configurationData = new Dictionary<string, string?>
        {
            ["Serilog:WriteTo:Console:Enabled"] = "True",
            ["Serilog:WriteTo:File:Enabled"] = "false",
            ["Serilog:Enrich:Enabled"] = "true"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var configurationWrapper = new ConfigurationWrapper(configuration);

        // Act
        var serilogConfig = new SerilogConfiguration(configurationWrapper);

        // Assert
        Assert.True(serilogConfig.WriteToConsole);
        Assert.False(serilogConfig.WriteToFile);
        Assert.True(serilogConfig.EnableEnrichers);
    }
}