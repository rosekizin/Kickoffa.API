using Kickoffa.API.AspNet.Infrastructure.Wrappers;

namespace Kickoffa.API.AspNet.Infrastructure.Configuration.Data
{
	public interface IPostgreDbConfiguration
	{
		public int CommandTimeout { get; }
		public string Server { get; }
		public string Database { get; }
		public string Username { get; }
		public string Password { get; }
		public string SslMode { get; }
		public int Port { get; }
		public string GetConnectionString();
	}

	public class PostgreDbConfiguration : IPostgreDbConfiguration
	{
		private const string POSTGRE_CONFIGURATION_PORT_PATH = "Postgre:Port";
		private const string POSTGRE_CONFIGURATION_SERVER_PATH = "Postgre:Server";
		private const string POSTGRE_CONFIGURATION_SSL_MODE_PATH = "Postgre:SslMode";
		private const string POSTGRE_CONFIGURATION_DATABASE_PATH = "Postgre:Database";
		private const string POSTGRE_CONFIGURATION_USERNAME_PATH = "Postgre:Username";
		private const string POSTGRE_CONFIGURATION_PASSWORD_PATH = "Postgre:Password";
		private const string POSTGRE_CONFIGURATION_TIMEOUT_PATH = "Postgre:CommandTimeout";

		public PostgreDbConfiguration(IConfigurationWrapper configurationWrapper)
		{
			Server = configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_SERVER_PATH)!;
			Database = configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_DATABASE_PATH)!;
			Username = configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_USERNAME_PATH)!;
			Password = configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_PASSWORD_PATH)!;
			SslMode = configurationWrapper.GetValue<string>(POSTGRE_CONFIGURATION_SSL_MODE_PATH)!;

			Port = configurationWrapper.GetValue<int>(POSTGRE_CONFIGURATION_PORT_PATH);
			CommandTimeout = configurationWrapper.GetValue<int>(POSTGRE_CONFIGURATION_TIMEOUT_PATH);
		}

		public int CommandTimeout { get; init; }

		public string Server { get; init; }
		public string Database { get; init; }

		public string Username { get; init; }

		public string Password { get; init; }

		public string SslMode { get; init; }

		public int Port { get; init; }

		public string GetConnectionString()
		{
			return $"Host={Server};Port={Port};Database={Database};Username={Username};Password={Password};SSL Mode={SslMode};Trust Server Certificate=true;Timeout={CommandTimeout};";
		}
	}
}
