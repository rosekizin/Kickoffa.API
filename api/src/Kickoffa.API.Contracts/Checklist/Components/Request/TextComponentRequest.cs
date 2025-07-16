using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist.Components.Request
{
	/// <summary>
	/// Request para representação de componente de texto
	/// </summary>
	public sealed record TextComponentRequest : ComponentRequest
	{
		[JsonProperty(PropertyName = "placeholder", Required = Required.Default)]
		public string? Placeholder { get; init; }

		[JsonProperty(PropertyName = "maxLength", Required = Required.Default)]
		public int? MaxLength { get; init; }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = base.Validate(validationContext).ToList();

			if (MaxLength.HasValue && MaxLength.Value <= 0)
			{
				results.Add(new ValidationResult("O comprimento máximo deve ser maior que zero", [nameof(MaxLength)]));
			}

			return results;
		}
	}
}