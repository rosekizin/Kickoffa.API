using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist;

/// <summary>
/// Request para busca paginada de checklists
/// </summary>
public record ChecklistSearchRequest : IValidatableObject
{
    [JsonProperty(PropertyName = "search", Required = Required.Default)]
    public string? Search { get; init; }

    [JsonProperty(PropertyName = "page", Required = Required.Default)]
    public int Page { get; init; } = 1;

    [JsonProperty(PropertyName = "pageSize", Required = Required.Default)]
    public int PageSize { get; init; } = 10;

    [JsonProperty(PropertyName = "filters", Required = Required.Default)]
    public ChecklistFilters Filters { get; init; } = new();

	/// <summary>
	/// Valida as propriedades do request
	/// </summary>
	/// <param name="validationContext">Contexto de validação</param>
	/// <returns>Resultados da validação</returns>
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Validação da Page
        if (Page < 1)
        {
            results.Add(new ValidationResult("A página deve ser maior que 0", [nameof(Page)]));
        }

        // Validação do PageSize
        if (PageSize < 1 || PageSize > 100)
        {
            results.Add(new ValidationResult("O tamanho da página deve estar entre 1 e 100", [nameof(PageSize)]));
        }

        return results;
    }
}

/// <summary>
/// Filtros para busca de checklists
/// </summary>
public record ChecklistFilters
{
    [JsonProperty(PropertyName = "statuses", Required = Required.Default)]
    public IEnumerable<ChecklistStatus> Statuses { get; init; } = [];
}