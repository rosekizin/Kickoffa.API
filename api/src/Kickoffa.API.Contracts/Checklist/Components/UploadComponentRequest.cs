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
		public string? Placeholder { get; init; }

		/// <summary>
		/// Tamanho máximo por arquivo em MB
		/// </summary>
		public int? MaxSizeMB { get; init; }

		/// <summary>
		/// IDs dos tipos de arquivo permitidos para este componente
		/// </summary>
		public required IEnumerable<long> AllowedFileTypeIds { get; init; } = [];

		/// <summary>
		/// Arquivos enviados pelo cliente para este componente
		/// Correlato à propriedade ComponentFiles do domínio
		/// </summary>
		public IEnumerable<UploadComponentFileRequest>? ComponentFiles { get; init; }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = base.Validate(validationContext).ToList();

			if (MaxSizeMB.HasValue && MaxSizeMB.Value <= 0)
			{
				results.Add(new ValidationResult("O tamanho máximo deve ser maior que zero", [nameof(MaxSizeMB)]));
			}

			if (!AllowedFileTypeIds.Any())
			{
				results.Add(new ValidationResult("Componentes de upload devem ter pelo menos um tipo de arquivo permitido", [nameof(AllowedFileTypeIds)]));
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
		public required string FileName { get; init; }

		/// <summary>
		/// Caminho de armazenamento do arquivo
		/// </summary>
		public required string StoragePath { get; init; }

		/// <summary>
		/// Tamanho do arquivo em bytes
		/// </summary>
		public required long FileSize { get; init; }

		/// <summary>
		/// Tipo de conteúdo (MIME type)
		/// </summary>
		public required string ContentType { get; init; }

		/// <summary>
		/// Hash SHA256 do arquivo para verificação de integridade
		/// </summary>
		public required string Sha256Hash { get; init; }
	}
}