using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist.Components
{
	/// <summary>
	/// Request base para criação de componentes
	/// </summary>
	public abstract record ComponentRequest : IValidatableObject
	{
		[JsonProperty(PropertyName = "id", Required = Required.Default)]
		public required long Id { get; init; }

		[JsonProperty(PropertyName = "title", Required = Required.Always)]
		public required string Title { get; init; }

		[JsonProperty(PropertyName = "description", Required = Required.Default)]
		public string? Description { get; init; }

		[JsonProperty(PropertyName = "isRequired", Required = Required.Always)]
		public required bool IsRequired { get; init; }

		[JsonProperty(PropertyName = "order", Required = Required.Always)]
		public required int Order { get; init; }

		[JsonProperty(PropertyName = "type", Required = Required.Always)]
		public ComponentTypeRequest Type { get; init; }

		/// <summary>
		/// Valida as propriedades base do request
		/// </summary>
		/// <param name="validationContext">Contexto de validação</param>
		/// <returns>Resultados da validação</returns>
		public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = new List<ValidationResult>();

			// Validação do Title
			if (string.IsNullOrWhiteSpace(Title))
			{
				results.Add(new ValidationResult("O título do componente é obrigatório", [nameof(Title)]));
			}
			else if (Title.Length < 2 || Title.Length > 200)
			{
				results.Add(new ValidationResult("O título do componente deve ter entre 2 e 200 caracteres", [nameof(Title)]));
			}

			// Validação da Description (opcional)
			if (!string.IsNullOrWhiteSpace(Description) && Description.Length > 500)
			{
				results.Add(new ValidationResult("A descrição do componente deve ter no máximo 500 caracteres", [nameof(Description)]));
			}

			// Validação do Order
			if (Order <= 0)
			{
				results.Add(new ValidationResult("A ordem do componente deve ser maior que zero", [nameof(Order)]));
			}

			if(!Enum.IsDefined(Type))
			{
				results.Add(new ValidationResult("Tipo de Componente atualmente não suportado.", [nameof(Type)]));
			}

			return results;
		}
	}
}