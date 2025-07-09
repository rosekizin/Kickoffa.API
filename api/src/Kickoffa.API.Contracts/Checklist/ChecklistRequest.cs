using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.Checklist;

/// <summary>
/// Request para criação de um novo checklist
/// </summary>
public sealed record ChecklistRequest : IValidatableObject
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public DateTime? Deadline { get; init; }
    public required ICollection<SectionRequest> Sections { get; init; } = [];

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
            results.Add(new ValidationResult("O título é obrigatório", [nameof(Title)]));
        }
        else if (Title.Length < 3 || Title.Length > 200)
        {
            results.Add(new ValidationResult("O título deve ter entre 3 e 200 caracteres", [nameof(Title)]));
        }

        // Validação da Description (opcional)
        if (!string.IsNullOrWhiteSpace(Description) && Description.Length > 1000)
        {
            results.Add(new ValidationResult("A descrição deve ter no máximo 1000 caracteres", [nameof(Description)]));
        }

        // Validação da Deadline (opcional)
        if (Deadline.HasValue && Deadline.Value <= DateTime.UtcNow)
        {
            results.Add(new ValidationResult("A data limite deve ser no futuro", [nameof(Deadline)]));
        }

        // Validação das Sections
        if (Sections == null || Sections.Count == 0)
        {
            results.Add(new ValidationResult("O checklist deve ter pelo menos uma seção", [nameof(Sections)]));
        }
        else
        {
            // Validar ordem das seções
            var orders = Sections.Select(s => s.Order).ToList();
            if (orders.Distinct().Count() != orders.Count)
            {
                results.Add(new ValidationResult("As seções devem ter ordens únicas", [nameof(Sections)]));
            }

            // Validar se há pelo menos uma seção de checklist
            if (!Sections.Any(s => s.Type == SectionTypeRequest.Checklist))
            {
                results.Add(new ValidationResult("O checklist deve ter pelo menos uma seção de checklist", [nameof(Sections)]));
            }
        }

        return results;
    }
}