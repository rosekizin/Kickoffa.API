using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist.Components
{
	/// <summary>
	/// Request para representação de componente de confirmação
	/// </summary>
	public sealed record ConfirmationComponentRequest : ComponentRequest
	{
		[JsonProperty(PropertyName = "confirmationText", Required = Required.Always)]
		public required string ConfirmationText { get; init; }

		public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = base.Validate(validationContext).ToList();

			if (string.IsNullOrWhiteSpace(ConfirmationText))
			{
				results.Add(new ValidationResult("Componentes de confirmação devem ter texto de confirmação", [nameof(ConfirmationText)]));
			}

			return results;
		}
	}
}