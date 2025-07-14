using Kickoffa.API.Contracts.Checklist.Components;
using Kickoffa.API.Contracts.Newtonsoft;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist.Sections;

/// <summary>
/// Classe base para requests de seções
/// </summary>
public abstract record SectionRequest : IValidatableObject
{
	[JsonProperty(PropertyName = "id", Required = Required.Default)]
	public required long Id { get; init; }

	[JsonProperty(PropertyName = "title", Required = Required.Always)]
	public required string Title { get; init; }

	[JsonProperty(PropertyName = "type", Required = Required.Always)]
	public required SectionTypeRequest Type { get; init; }

	[JsonProperty(PropertyName = "order", Required = Required.Always)]
	public required int Order { get; init; }

    /// <summary>
    /// Valida as propriedades básicas do request
    /// </summary>
    /// <param name="validationContext">Contexto de validação</param>
    /// <returns>Resultados da validação</returns>
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
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

        // Validação do Order
        if (Order <= 0)
        {
            results.Add(new ValidationResult("A ordem da seção deve ser maior que zero", [nameof(Order)]));
        }

        return results;
    }
}

public enum SectionTypeRequest
{
	Briefing,
	Checklist
}

/// <summary>
/// Request para seção de briefing
/// </summary>
public sealed record BriefingSectionRequest : SectionRequest
{
	[JsonProperty(PropertyName = "contentJson", Required = Required.Default)]
	public string? ContentJson { get; init; }

	[JsonProperty(PropertyName = "contentHtml", Required = Required.Default)]
	public string? ContentHtml { get; init; }

    public BriefingSectionRequest()
    {
        Type = SectionTypeRequest.Briefing;
        //Type = "briefing";
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = base.Validate(validationContext).ToList();

        // Para seções de briefing, pelo menos um dos conteúdos deve estar presente
        if (string.IsNullOrWhiteSpace(ContentJson) && string.IsNullOrWhiteSpace(ContentHtml))
        {
            results.Add(new ValidationResult("Seções de briefing devem ter conteúdo JSON ou HTML", [nameof(ContentJson), nameof(ContentHtml)]));
        }

        return results;
    }
}

/// <summary>
/// Request para seção de checklist
/// </summary>
public sealed record ChecklistSectionRequest : SectionRequest
{
    [JsonConverter(typeof(ComponentRequestCollectionConverter))]
    [JsonProperty(PropertyName = "components", Required = Required.Always)]
    public ICollection<ComponentRequest> Components { get; init; } = [];

    public ChecklistSectionRequest()
	{
		Type = SectionTypeRequest.Checklist;
		//Type = "checklist";
	}

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = base.Validate(validationContext).ToList();

        // Para seções de checklist, deve ter pelo menos um componente
        if (Components == null || Components.Count == 0)
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

        return results;
    }
}