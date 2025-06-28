using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using Microsoft.Extensions.Configuration;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.Wrappers;

public class ConfigurationWrapperTests
{
	private ConfigurationWrapper? _configurationWrapper;

	[Fact]
	public void GetValue_WithValidKey_ShouldReturnValue()
	{
		// Arrange
		var key = "TestKey";
		var expectedValue = "TestValue";
		var inMemorySettings = new Dictionary<string, string?>
		{
			{key, expectedValue},
			{"SectionName:SomeKey", "SectionValue"}
		};

		IConfiguration configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(inMemorySettings)
			.Build();

		_configurationWrapper = new ConfigurationWrapper(configuration);

		// Act
		var result = _configurationWrapper.GetValue<string>(key);

		// Assert
		Assert.Equal(expectedValue, result);
	}

	[Fact]
	public void GetValue_WithNonExistentKey_ShouldReturnNull()
	{
		// Arrange
		var key = "NonExistentKey";
		var inMemorySettings = new Dictionary<string, string?>
		{
			{"SectionName:SomeKey", "SectionValue"}
		};

		IConfiguration configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(inMemorySettings)
			.Build();

		_configurationWrapper = new ConfigurationWrapper(configuration);

		// Act
		var result = _configurationWrapper.GetValue<string>(key);

		// Assert
		Assert.Null(result);
	}

	[Theory]
	[InlineData("Key1", "Value1")]
	[InlineData("Database:Host", "localhost")]
	[InlineData("Logging:LogLevel:Default", "Information")]
	public void GetValue_WithDifferentKeys_ShouldReturnCorrectValues(string key, string expectedValue)
	{
		// Arrange
		var inMemorySettings = new Dictionary<string, string?>
		{
			{key, expectedValue}
		};

		IConfiguration configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(inMemorySettings)
			.Build();

		_configurationWrapper = new ConfigurationWrapper(configuration);

		// Act
		var result = _configurationWrapper.GetValue<string>(key);

		// Assert
		Assert.Equal(expectedValue, result);
	}
}