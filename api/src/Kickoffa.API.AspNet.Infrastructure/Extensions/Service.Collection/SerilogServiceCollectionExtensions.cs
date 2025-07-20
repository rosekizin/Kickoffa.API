using Kickoffa.API.AspNet.Infrastructure.Configuration.Logging;
using Kickoffa.API.AspNet.Infrastructure.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection
{
	/// <summary>
	/// Extensões para configuração do Serilog no container de DI
	/// </summary>
	public static class SerilogServiceCollectionExtensions
	{
		/// <summary>
		/// Adiciona e configura o Serilog como provedor de logging
		/// </summary>
		/// <param name="services">Collection de serviços</param>
		/// <param name="serilogConfiguration">Configuração do Serilog</param>
		/// <param name="environment">Ambiente da aplicação</param>
		/// <returns>IServiceCollection para chaining</returns>
		public static IServiceCollection AddSerilog(
			this IServiceCollection services, 
			ISerilogConfiguration serilogConfiguration,
            IHostEnvironmentWrapper hostEnvironmentWrapper)
		{
			// Configurar o Serilog
			var loggerConfiguration = new LoggerConfiguration();

			// Configurar nível mínimo de log
			if (Enum.TryParse<LogEventLevel>(serilogConfiguration.MinimumLevel, true, out var minimumLevel))
			{
				loggerConfiguration.MinimumLevel.Is(minimumLevel);
			}
			else
			{
				loggerConfiguration.MinimumLevel.Information();
			}

			// Configurar níveis específicos para reduzir ruído
			loggerConfiguration
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

			// Adicionar enrichers se habilitado
			if (serilogConfiguration.EnableEnrichers)
			{
				loggerConfiguration
					.Enrich.FromLogContext()
					.Enrich.WithEnvironmentName()
					.Enrich.WithMachineName()
					.Enrich.WithProcessId()
					.Enrich.WithThreadId();
			}

			// Configurar sink do Console se habilitado
			if (serilogConfiguration.WriteToConsole)
			{
				loggerConfiguration.WriteTo.Console(
					outputTemplate: serilogConfiguration.OutputTemplate,
					restrictedToMinimumLevel: hostEnvironmentWrapper.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information
				);
			}

			// Configurar sink de Arquivo se habilitado
			if (serilogConfiguration.WriteToFile)
			{
				loggerConfiguration.WriteTo.File(
					path: serilogConfiguration.FilePath,
					outputTemplate: serilogConfiguration.OutputTemplate,
					rollingInterval: RollingInterval.Day,
					fileSizeLimitBytes: serilogConfiguration.FileSizeLimitMB * 1024 * 1024, // Converter MB para bytes
					retainedFileCountLimit: serilogConfiguration.RetainedFileCountLimit,
					shared: true,
					restrictedToMinimumLevel: LogEventLevel.Information
				);
			}

			// Configurar sink do Elasticsearch se habilitado (comentado por enquanto)
			/*
			if (serilogConfiguration.WriteToElasticsearch)
			{
				loggerConfiguration.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(serilogConfiguration.ElasticsearchUri))
				{
					IndexFormat = $"{serilogConfiguration.ElasticsearchIndex}-{{0:yyyy.MM.dd}}",
					AutoRegisterTemplate = true,
					AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
					TemplateName = "kickoffa-api-logs",
					NumberOfShards = 2,
					NumberOfReplicas = 1,
					MinimumLogEventLevel = LogEventLevel.Information,
					EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
									   EmitEventFailureHandling.WriteToFailureSink |
									   EmitEventFailureHandling.RaiseCallback,
					FailureSink = new FileSink("logs/elasticsearch-failures-.txt", new JsonFormatter(), null)
				});
			}
			*/

			// Criar o logger
			Log.Logger = loggerConfiguration.CreateLogger();

			// Adicionar Serilog ao container de DI
			services.AddSerilog(Log.Logger);

			// Registrar a configuração no container para uso posterior se necessário
			services.AddSingleton(serilogConfiguration);

			return services;
		}

		/// <summary>
		/// Adiciona configuração básica do Serilog apenas para console (para desenvolvimento rápido)
		/// </summary>
		/// <param name="services">Collection de serviços</param>
		/// <param name="environment">Ambiente da aplicação</param>
		/// <returns>IServiceCollection para chaining</returns>
		public static IServiceCollection AddSerilogConsoleOnly(
			this IServiceCollection services,
            IHostEnvironmentWrapper hostEnvironmentWrapper)
		{
			var loggerConfiguration = new LoggerConfiguration()
				.MinimumLevel.Information()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.Console(
					outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
					restrictedToMinimumLevel: hostEnvironmentWrapper.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information
				);

			Log.Logger = loggerConfiguration.CreateLogger();
			services.AddSerilog(Log.Logger);

			return services;
		}
	}
}