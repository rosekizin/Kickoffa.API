using Kickoffa.API.AspNet.Infrastructure.Configuration.Data;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using NSubstitute;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.Configuration.Data;

public class PostgreDbConfigurationTests
{
	private const string POSTGRE_CONFIGURATION_PORT_PATH = "Postgre:Port";
	private const string POSTGRE_CONFIGURATION_SERVER_PATH = "Postgre:Server";
	private const string POSTGRE_CONFIGURATION_SSL_MODE_PATH = "Postgre:SslMode";
	private const string POSTGRE_CONFIGURATION_DATABASE_PATH = "Postgre:Database";
	private const string POSTGRE_CONFIGURATION_USERNAME_PATH = "Postgre:Username";
	private const string POSTGRE_CONFIGURATION_PASSWORD_PATH = "Postgre:Password";
	private const string POSTGRE_CONFIGURATION_TIMEOUT_PATH = "Postgre:CommandTimeout";

	private readonly IConfigurationWrapper _configurationWrapper;

	public PostgreDbConfigurationTests()
	{
		_configurationWrapper = Substitute.For<IConfigurationWrapper>();
	}

	[Fact]
	public void Constructor_WithValidConfiguration_ShouldSetConnectionString()
	{
		// Arrange
		var port = 10542;
		var server = "localhost";
		var database = "database";
		var username = "usernama";
		var password = "Password";
		var sslMode = "SslMode";
		var commandTimeout = 123;
		var expectedConnectionString = $"Host={server};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};Trust Server Certificate=true;Timeout={commandTimeout};";

		_configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_SERVER_PATH).Returns(server);
		_configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_DATABASE_PATH).Returns(database);
		_configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_USERNAME_PATH).Returns(username);
		_configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_PASSWORD_PATH).Returns(password);
		_configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_SSL_MODE_PATH).Returns(sslMode);
		_configurationWrapper.GetValue<int>(POSTGRE_CONFIGURATION_PORT_PATH).Returns(port);
		_configurationWrapper.GetValue<int>(POSTGRE_CONFIGURATION_TIMEOUT_PATH).Returns(commandTimeout);

		// Act
		var config = new PostgreDbConfiguration(_configurationWrapper);

		// Assert
		Assert.Equal(expectedConnectionString, config.GetConnectionString());
	}
}