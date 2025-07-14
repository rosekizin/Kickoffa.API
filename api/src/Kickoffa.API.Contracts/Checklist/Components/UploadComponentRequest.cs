using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist.Components
{
	/// <summary>
	/// Request para representação de componente de upload
	/// Alinhado com o domínio UploadComponent
	/// </summary>
	public sealed record UploadComponentRequest : ComponentRequest
	{
		/// <summary>
		/// Texto de placeholder para a área de upload
		/// </summary>
		[JsonProperty(PropertyName = "placeholder", Required = Required.Default)]
		public string? Placeholder { get; init; }



		/// <summary>
		/// IDs dos tipos de arquivo permitidos para este componente
		/// </summary>
		[JsonProperty(PropertyName = "allowedFileTypeIds", Required = Required.Always)]
		public required IEnumerable<long> AllowedFileTypeIds { get; init; } = [];

		/// <summary>
		/// Configurações de tamanho máximo por tipo de arquivo
		/// </summary>
		[JsonProperty(PropertyName = "fileTypeSizeConfigs", Required = Required.Default)]
		public IEnumerable<FileTypeSizeConfigRequest>? FileTypeSizeConfigs { get; init; }

		/// <summary>
		/// Arquivos enviados pelo cliente para este componente
		/// Correlato à propriedade ComponentFiles do domínio
		/// </summary>
		[JsonProperty(PropertyName = "componentFiles", Required = Required.Default)]
		public IEnumerable<UploadComponentFileRequest>? ComponentFiles { get; init; }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = base.Validate(validationContext).ToList();



			if (!AllowedFileTypeIds.Any())
			{
				results.Add(new ValidationResult("Componentes de upload devem ter pelo menos um tipo de arquivo permitido", [nameof(AllowedFileTypeIds)]));
			}

			// Validar configurações de tamanho se fornecidas
			if (FileTypeSizeConfigs != null)
			{
				var configsList = FileTypeSizeConfigs.ToList();
				var allowedFileTypeIdsList = AllowedFileTypeIds.ToList();

				// Verificar se todas as configurações são para tipos de arquivo permitidos
				var invalidConfigs = configsList.Where(config => !allowedFileTypeIdsList.Contains(config.FileTypeId));
				foreach (var invalidConfig in invalidConfigs)
				{
					results.Add(new ValidationResult($"Configuração de tamanho para tipo de arquivo {invalidConfig.FileTypeId} não está na lista de tipos permitidos", [nameof(FileTypeSizeConfigs)]));
				}

				// Verificar duplicatas
				var duplicateFileTypeIds = configsList.GroupBy(config => config.FileTypeId)
					.Where(group => group.Count() > 1)
					.Select(group => group.Key);

				foreach (var duplicateId in duplicateFileTypeIds)
				{
					results.Add(new ValidationResult($"Configuração duplicada encontrada para tipo de arquivo {duplicateId}", [nameof(FileTypeSizeConfigs)]));
				}

				// Validar cada configuração individualmente
				for (int i = 0; i < configsList.Count; i++)
				{
					var config = configsList[i];
					var configValidationContext = new ValidationContext(config, validationContext, validationContext.Items);
					var configResults = config.Validate(configValidationContext);

					foreach (var configResult in configResults)
					{
						results.Add(new ValidationResult($"Configuração de tamanho {i + 1}: {configResult.ErrorMessage}", [nameof(FileTypeSizeConfigs)]));
					}
				}
			}

			return results;
		}
	}

	/// <summary>
	/// Request para arquivo enviado em componente de upload
	/// </summary>
	public sealed record UploadComponentFileRequest
	{
		/// <summary>
		/// Nome do arquivo
		/// </summary>
		[JsonProperty(PropertyName = "fileName", Required = Required.Always)]
		public required string FileName { get; init; }

		/// <summary>
		/// Caminho de armazenamento do arquivo
		/// </summary>
		[JsonProperty(PropertyName = "storagePath", Required = Required.Always)]
		public required string StoragePath { get; init; }

		/// <summary>
		/// Tamanho do arquivo em bytes
		/// </summary>
		[JsonProperty(PropertyName = "fileSize", Required = Required.Always)]
		public required long FileSize { get; init; }

		/// <summary>
		/// Tipo de conteúdo (MIME type)
		/// </summary>
		[JsonProperty(PropertyName = "contentType", Required = Required.Always)]
		public required string ContentType { get; init; }

		/// <summary>
		/// Hash SHA256 do arquivo para verificação de integridade
		/// </summary>
		[JsonProperty(PropertyName = "sha256Hash", Required = Required.Default)]
		public required string Sha256Hash { get; init; }
	}
}