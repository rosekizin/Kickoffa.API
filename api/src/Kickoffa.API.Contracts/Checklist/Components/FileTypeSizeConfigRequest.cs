using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist.Components
{
	/// <summary>
	/// Request para configuração de tamanho máximo por tipo de arquivo
	/// </summary>
	public sealed record FileTypeSizeConfigRequest : IValidatableObject
	{
		/// <summary>
		/// ID do tipo de arquivo
		/// </summary>
		[Required(ErrorMessage = "O ID do tipo de arquivo é obrigatório")]
		public required long FileTypeId { get; init; }

		/// <summary>
		/// Tamanho máximo personalizado em MB para este tipo de arquivo
		/// </summary>
		[Required(ErrorMessage = "O tamanho máximo é obrigatório")]
		[Range(1, 1000, ErrorMessage = "O tamanho máximo deve estar entre 1 e 1000 MB")]
		public required int MaxSizeMB { get; init; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = new List<ValidationResult>();

			if (FileTypeId <= 0)
			{
				results.Add(new ValidationResult("O ID do tipo de arquivo deve ser maior que zero", [nameof(FileTypeId)]));
			}

			if (MaxSizeMB <= 0)
			{
				results.Add(new ValidationResult("O tamanho máximo deve ser maior que zero", [nameof(MaxSizeMB)]));
			}

			if (MaxSizeMB > 1000)
			{
				results.Add(new ValidationResult("O tamanho máximo não pode exceder 1000 MB", [nameof(MaxSizeMB)]));
			}

			return results;
		}
	}
}