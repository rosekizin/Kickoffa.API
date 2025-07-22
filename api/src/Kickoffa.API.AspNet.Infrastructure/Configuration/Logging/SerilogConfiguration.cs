using Kickoffa.API.AspNet.Infrastructure.Wrappers;

namespace Kickoffa.API.AspNet.Infrastructure.Configuration.Logging
{
	/// <summary>
	/// Interface para configuração do Serilog
	/// </summary>
	public interface ISerilogConfiguration
	{
		/// <summary>
		/// Nível mínimo de log
		/// </summary>
		string MinimumLevel { get; }

		/// <summary>
		/// Se deve escrever logs em arquivo
		/// </summary>
		bool WriteToFile { get; }

		/// <summary>
		/// Caminho do arquivo de log
		/// </summary>
		string FilePath { get; }

		/// <summary>
		/// Tamanho máximo do arquivo de log em MB
		/// </summary>
		long FileSizeLimitMB { get; }

		/// <summary>
		/// Número máximo de arquivos de log para manter
		/// </summary>
		int RetainedFileCountLimit { get; }

		/// <summary>
		/// Se deve escrever logs no console
		/// </summary>
		bool WriteToConsole { get; }

		/// <summary>
		/// Se deve incluir enrichers (Environment, Process, Thread)
		/// </summary>
		bool EnableEnrichers { get; }

		/// <summary>
		/// Template de output para logs
		/// </summary>
		string OutputTemplate { get; }

		// Configurações para Elasticsearch (comentadas por enquanto)
		// bool WriteToElasticsearch { get; }
		// string ElasticsearchUri { get; }
		// string ElasticsearchIndex { get; }
	}

	/// <summary>
	/// Implementação da configuração do Serilog
	/// </summary>
	public class SerilogConfiguration : ISerilogConfiguration
	{
		private const string SERILOG_MINIMUM_LEVEL_PATH = "Serilog:MinimumLevel:Default";
		private const string SERILOG_WRITE_TO_FILE_PATH = "Serilog:WriteTo:File:Enabled";
		private const string SERILOG_FILE_PATH = "Serilog:WriteTo:File:Path";
		private const string SERILOG_FILE_SIZE_LIMIT_PATH = "Serilog:WriteTo:File:FileSizeLimitMB";
		private const string SERILOG_RETAINED_FILE_COUNT_PATH = "Serilog:WriteTo:File:RetainedFileCountLimit";
		private const string SERILOG_WRITE_TO_CONSOLE_PATH = "Serilog:WriteTo:Console:Enabled";
		private const string SERILOG_ENABLE_ENRICHERS_PATH = "Serilog:Enrich:Enabled";
		private const string SERILOG_OUTPUT_TEMPLATE_PATH = "Serilog:OutputTemplate";

		// Configurações para Elasticsearch (comentadas)
		// private const string SERILOG_WRITE_TO_ELASTICSEARCH_PATH = "Serilog:WriteTo:Elasticsearch:Enabled";
		// private const string SERILOG_ELASTICSEARCH_URI_PATH = "Serilog:WriteTo:Elasticsearch:Uri";
		// private const string SERILOG_ELASTICSEARCH_INDEX_PATH = "Serilog:WriteTo:Elasticsearch:Index";

		public SerilogConfiguration(IConfigurationWrapper configurationWrapper)
		{
			MinimumLevel = configurationWrapper.GetValue<string>(SERILOG_MINIMUM_LEVEL_PATH) ?? "Information";
			WriteToFile = configurationWrapper.GetValue<bool>(SERILOG_WRITE_TO_FILE_PATH);
			FilePath = configurationWrapper.GetValue<string>(SERILOG_FILE_PATH) ?? "logs/kickoffa-api-.txt";
			FileSizeLimitMB = configurationWrapper.GetValue<long>(SERILOG_FILE_SIZE_LIMIT_PATH) != 0 
				? configurationWrapper.GetValue<long>(SERILOG_FILE_SIZE_LIMIT_PATH) 
				: 10;
			RetainedFileCountLimit = configurationWrapper.GetValue<int>(SERILOG_RETAINED_FILE_COUNT_PATH) != 0 
				? configurationWrapper.GetValue<int>(SERILOG_RETAINED_FILE_COUNT_PATH) 
				: 31;
			WriteToConsole = configurationWrapper.GetValue<bool>(SERILOG_WRITE_TO_CONSOLE_PATH);
			EnableEnrichers = configurationWrapper.GetValue<bool>(SERILOG_ENABLE_ENRICHERS_PATH);
			OutputTemplate = configurationWrapper.GetValue<string>(SERILOG_OUTPUT_TEMPLATE_PATH) 
				?? "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

			// Configurações para Elasticsearch (comentadas)
			// WriteToElasticsearch = configurationWrapper.GetValue<bool>(SERILOG_WRITE_TO_ELASTICSEARCH_PATH);
			// ElasticsearchUri = configurationWrapper.GetValue<string>(SERILOG_ELASTICSEARCH_URI_PATH) ?? "http://localhost:9200";
			// ElasticsearchIndex = configurationWrapper.GetValue<string>(SERILOG_ELASTICSEARCH_INDEX_PATH) ?? "kickoffa-api-logs";
		}

		public string MinimumLevel { get; init; }
		public bool WriteToFile { get; init; }
		public string FilePath { get; init; }
		public long FileSizeLimitMB { get; init; }
		public int RetainedFileCountLimit { get; init; }
		public bool WriteToConsole { get; init; }
		public bool EnableEnrichers { get; init; }
		public string OutputTemplate { get; init; }

		// Propriedades para Elasticsearch (comentadas)
		// public bool WriteToElasticsearch { get; init; }
		// public string ElasticsearchUri { get; init; }
		// public string ElasticsearchIndex { get; init; }
	}
}
