using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist.Components.Request
{
	/// <summary>
	/// Request para configuração de tamanho máximo por tipo de arquivo
	/// </summary>
	public sealed record FileTypeSizeConfigRequest : IValidatableObject
	{
		/// <summary>
		/// ID do tipo de arquivo
		/// </summary>
		public required long FileTypeId { get; init; }

		/// <summary>
		/// Tamanho máximo personalizado em MB para este tipo de arquivo
		/// </summary>
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