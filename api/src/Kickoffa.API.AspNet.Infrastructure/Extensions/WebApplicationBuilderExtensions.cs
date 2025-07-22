using Kickoffa.API.AspNet.Infrastructure.Configuration.Logging;
using Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Kickoffa.API.AspNet.Infrastructure.Extensions
{
	/// <summary>
	/// Extensões para WebApplicationBuilder
	/// </summary>
	public static class WebApplicationBuilderExtensions
	{
		/// <summary>
		/// Configura o Serilog como provedor de logging principal da aplicação
		/// Este método deve ser chamado no início do Program.cs, antes de outras configurações
		/// </summary>
		/// <param name="builder">WebApplicationBuilder</param>
		/// <param name="configurationWrapper">Wrapper de configuração</param>
		/// <returns>WebApplicationBuilder para chaining</returns>
		public static WebApplicationBuilder AddSerilogLogging(
			this WebApplicationBuilder builder,
			IConfigurationWrapper configurationWrapper,
            IHostEnvironmentWrapper hostEnvironmentWrapper)
		{
			// Criar configuração do Serilog
			var serilogConfiguration = new SerilogConfiguration(configurationWrapper);

			// Configurar Serilog no container de DI
			builder.Services.AddSerilog(serilogConfiguration, hostEnvironmentWrapper);

			// Configurar Serilog como provedor de logging do ASP.NET Core
			builder.Host.UseSerilog();

			return builder;
		}

		/// <summary>
		/// Configura o Serilog apenas para console (configuração simplificada para desenvolvimento)
		/// </summary>
		/// <param name="builder">WebApplicationBuilder</param>
		/// <returns>WebApplicationBuilder para chaining</returns>
		public static WebApplicationBuilder AddSerilogConsoleLogging(this WebApplicationBuilder builder, IHostEnvironmentWrapper hostEnvironmentWrapper)
		{
			// Configurar Serilog apenas para console
			builder.Services.AddSerilogConsoleOnly(hostEnvironmentWrapper);
			//builder.Services.AddSerilogConsoleOnly(builder.Environment);

			// Configurar Serilog como provedor de logging do ASP.NET Core
			builder.Host.UseSerilog();

			return builder;
		}
	}
}
