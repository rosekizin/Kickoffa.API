using Kickoffa.API.Contracts.Checklist.Components;
using Kickoffa.API.Contracts.Newtonsoft;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist
{
	/// <summary>
	/// Request para representar uma seção
	/// </summary>
	public sealed record SectionRequest : IValidatableObject
	{
		public required long Id { get; init; }
		public required string Title { get; init; }
		public required SectionTypeRequest Type { get; init; } // "briefing" ou "checklist"
		public required int Order { get; init; }

		// Para seções de briefing
		public string? ContentJson { get; init; }
		public string? ContentHtml { get; init; }

		// Para seções de checklist
		[JsonConverter(typeof(ComponentRequestCollectionConverter))]
		public ICollection<ComponentRequest>? Components { get; init; }

		/// <summary>
		/// Valida as propriedades do request
		/// </summary>
		/// <param name="validationContext">Contexto de validação</param>
		/// <returns>Resultados da validação</returns>
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = new List<ValidationResult>();

			// Validação do Title
			if (string.IsNullOrWhiteSpace(Title))
			{
				results.Add(new ValidationResult("O título da seção é obrigatório", [nameof(Title)]));
			}
			else if (Title.Length < 2 || Title.Length > 200)
			{
				results.Add(new ValidationResult("O título da seção deve ter entre 2 e 200 caracteres", [nameof(Title)]));
			}

			// Validação do Type
			if (Type != SectionTypeRequest.Briefing && Type != SectionTypeRequest.Checklist)
			{
				results.Add(new ValidationResult("O tipo da seção deve ser 'briefing' ou 'checklist'", [nameof(Type)]));
			}

			// Validação do Order
			if (Order <= 0)
			{
				results.Add(new ValidationResult("A ordem da seção deve ser maior que zero", [nameof(Order)]));
			}

			// Validações específicas por tipo
			if (Type == SectionTypeRequest.Briefing)
			{
				// Para seções de briefing, pelo menos um dos conteúdos deve estar presente
				if (string.IsNullOrWhiteSpace(ContentJson) && string.IsNullOrWhiteSpace(ContentHtml))
				{
					results.Add(new ValidationResult("Seções de briefing devem ter conteúdo JSON ou HTML", [nameof(ContentJson), nameof(ContentHtml)]));
				}
			}
			else if (Type == SectionTypeRequest.Checklist)
			{
				// Para seções de checklist, deve ter pelo menos um componente
				if (Components == null || !Components.Any())
				{
					results.Add(new ValidationResult("Seções de checklist devem ter pelo menos um componente", [nameof(Components)]));
				}
				else
				{
					// Validar ordem dos componentes
					var orders = Components.Select(c => c.Order).ToList();
					if (orders.Distinct().Count() != orders.Count)
					{
						results.Add(new ValidationResult("Os componentes devem ter ordens únicas", [nameof(Components)]));
					}
				}
			}

			return results;
		}
	}
}