using Kickoffa.API.AspNet.Infrastructure.Configuration.Logging;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using NSubstitute;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.Configuration.Logging;

public class SerilogConfigurationTests
{
    private readonly IConfigurationWrapper _configurationWrapper;

    public SerilogConfigurationTests()
    {
        _configurationWrapper = Substitute.For<IConfigurationWrapper>();
    }

    [Fact]
    public void Constructor_ShouldSetDefaultValues_WhenConfigurationIsEmpty()
    {
        // Arrange
        _configurationWrapper.GetValue<string>(Arg.Any<string>()).Returns((string?)null);
        _configurationWrapper.GetValue<bool>(Arg.Any<string>()).Returns(false);
        _configurationWrapper.GetValue<long>(Arg.Any<string>()).Returns(0L);
        _configurationWrapper.GetValue<int>(Arg.Any<string>()).Returns(0);

        // Act
        var config = new SerilogConfiguration(_configurationWrapper);

        // Assert
        Assert.Equal("Information", config.MinimumLevel);
        Assert.False(config.WriteToFile);
        Assert.Equal("logs/kickoffa-api-.txt", config.FilePath);
        Assert.Equal(10L, config.FileSizeLimitMB);
        Assert.Equal(31, config.RetainedFileCountLimit);
        Assert.False(config.WriteToConsole);
        Assert.False(config.EnableEnrichers);
        Assert.Equal("[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}", config.OutputTemplate);
    }

    [Fact]
    public void Constructor_ShouldUseConfigurationValues_WhenProvided()
    {
        // Arrange
        _configurationWrapper.GetValue<string>("Serilog:MinimumLevel:Default").Returns("Debug");
        _configurationWrapper.GetValue<bool>("Serilog:WriteTo:File:Enabled").Returns(true);
        _configurationWrapper.GetValue<string>("Serilog:WriteTo:File:Path").Returns("custom/path/logs-.txt");
        _configurationWrapper.GetValue<long>("Serilog:WriteTo:File:FileSizeLimitMB").Returns(50L);
        _configurationWrapper.GetValue<int>("Serilog:WriteTo:File:RetainedFileCountLimit").Returns(15);
        _configurationWrapper.GetValue<bool>("Serilog:WriteTo:Console:Enabled").Returns(true);
        _configurationWrapper.GetValue<bool>("Serilog:Enrich:Enabled").Returns(true);
        _configurationWrapper.GetValue<string>("Serilog:OutputTemplate").Returns("Custom template");

        // Act
        var config = new SerilogConfiguration(_configurationWrapper);

        // Assert
        Assert.Equal("Debug", config.MinimumLevel);
        Assert.True(config.WriteToFile);
        Assert.Equal("custom/path/logs-.txt", config.FilePath);
        Assert.Equal(50L, config.FileSizeLimitMB);
        Assert.Equal(15, config.RetainedFileCountLimit);
        Assert.True(config.WriteToConsole);
        Assert.True(config.EnableEnrichers);
        Assert.Equal("Custom template", config.OutputTemplate);
    }

    [Theory]
    [InlineData("Debug")]
    [InlineData("Information")]
    [InlineData("Warning")]
    [InlineData("Error")]
    [InlineData("Fatal")]
    public void MinimumLevel_ShouldAcceptValidLogLevels(string logLevel)
    {
        // Arrange
        _configurationWrapper.GetValue<string>("Serilog:MinimumLevel:Default").Returns(logLevel);

        // Act
        var config = new SerilogConfiguration(_configurationWrapper);

        // Assert
        Assert.Equal(logLevel, config.MinimumLevel);
    }

    [Fact]
    public void FileSizeLimitMB_ShouldUseDefaultValue_WhenZero()
    {
        // Arrange
        _configurationWrapper.GetValue<long>("Serilog:WriteTo:File:FileSizeLimitMB").Returns(0L);

        // Act
        var config = new SerilogConfiguration(_configurationWrapper);

        // Assert
        Assert.Equal(10L, config.FileSizeLimitMB);
    }

    [Fact]
    public void RetainedFileCountLimit_ShouldUseDefaultValue_WhenZero()
    {
        // Arrange
        _configurationWrapper.GetValue<int>("Serilog:WriteTo:File:RetainedFileCountLimit").Returns(0);

        // Act
        var config = new SerilogConfiguration(_configurationWrapper);

        // Assert
        Assert.Equal(31, config.RetainedFileCountLimit);
    }

    [Fact]
    public void Properties_ShouldBeInitOnly()
    {
        // Arrange & Act
        var config = new SerilogConfiguration(_configurationWrapper);

        // Assert - Verificar se as propriedades são init-only (não podem ser alteradas após construção)
        var properties = typeof(SerilogConfiguration).GetProperties();
        foreach (var property in properties)
        {
            Assert.True(property.CanRead, $"Property {property.Name} should be readable");
            
            // Verificar se tem setter init-only
            var setMethod = property.GetSetMethod();
            if (setMethod != null)
            {
                Assert.True(setMethod.ReturnParameter.GetRequiredCustomModifiers().Any(t => t.Name == "IsExternalInit"), 
                    $"Property {property.Name} should be init-only");
            }
        }
    }
}
