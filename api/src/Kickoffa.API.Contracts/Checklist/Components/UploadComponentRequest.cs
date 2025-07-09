using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist.Components
{
	/// <summary>
	/// Request para representação de componente de upload
	/// </summary>
	public sealed record UploadComponentRequest : ComponentRequest
	{
		public string? Placeholder { get; init; }
		public int? MaxSizeMB { get; init; }
		public required ICollection<long> AllowedFileTypeIds { get; init; } = [];

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
}