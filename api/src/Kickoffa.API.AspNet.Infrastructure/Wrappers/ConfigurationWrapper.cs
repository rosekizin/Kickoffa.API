using Microsoft.Extensions.Configuration;

namespace Kickoffa.API.AspNet.Infrastructure.Wrappers
{
	public interface IConfigurationWrapper
	{
		T? GetValue<T>(string key);
	}

	public class ConfigurationWrapper : IConfigurationWrapper
	{
		private readonly IConfiguration _configuration;

		public ConfigurationWrapper(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public T? GetValue<T>(string key)
		{
			return _configuration.GetValue<T>(key);
		}
	}
}