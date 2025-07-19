using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kickoffa.API.Contracts.Checklist;

/// <summary>
/// Status possíveis para um checklist
/// </summary>
[JsonConverter(typeof(StringEnumConverter), true)] // true para camelCase
public enum ChecklistStatus
{
	/// <summary>
	/// Checklist em desenvolvimento, ainda não publicado
	/// </summary>
	Draft = 0,

	/// <summary>
	/// Checklist publicado e ativo para uso
	/// </summary>
	Active = 1,

	/// <summary>
	/// Checklist concluído pelo cliente
	/// </summary>
	Completed = 2,

	/// <summary>
	/// Checklist arquivado (não mais em uso)
	/// </summary>
	Archived = 3
}
