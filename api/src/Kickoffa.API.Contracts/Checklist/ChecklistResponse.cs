using Kickoffa.API.Contracts.Checklist.Sections;
using Kickoffa.API.Contracts.Customer;
using Kickoffa.API.Contracts.Newtonsoft;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Checklist;

/// <summary>
/// Response da criação/consulta de um checklist
/// </summary>
public sealed record ChecklistResponse
{
	[JsonProperty(PropertyName = "id", Required = Required.Always)]
	public required long Id { get; init; }

	[JsonProperty(PropertyName = "ownerId", Required = Required.Always)]
	public required long OwnerId { get; init; }

	[JsonProperty(PropertyName = "customerId", Required = Required.Always)]
	public required long CustomerId { get; init; }

	[JsonProperty(PropertyName = "customer", Required = Required.Default)]
	public CustomerResponse? Customer { get; init; }

	[JsonProperty(PropertyName = "title", Required = Required.Always)]
	public required string Title { get; init; }

	[JsonProperty(PropertyName = "slug", Required = Required.Always)]
	public required string Slug { get; init; }

	[JsonProperty(PropertyName = "description", Required = Required.Default)]
	public string? Description { get; init; }

	[JsonProperty(PropertyName = "deadline", Required = Required.Default)]
	public DateTime? Deadline { get; init; }

	[JsonProperty(PropertyName = "accessToken", Required = Required.Default)]
	public string? AccessToken { get; init; }

	[JsonProperty(PropertyName = "status", Required = Required.Always)]
	public required ChecklistStatus Status { get; init; }

	[JsonProperty(PropertyName = "sections", Required = Required.Always)]
	[JsonConverter(typeof(SectionResponseCollectionConverter))]
	public required ICollection<SectionResponse> Sections { get; init; } = [];

	[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Always)]
	public required DateTime CreatedDateUtc { get; init; }

	[JsonProperty(PropertyName = "lastUpdatedDateUtc", Required = Required.Always)]
	public required DateTime LastUpdatedDateUtc { get; init; }
}

/// <summary>
/// Request para busca paginada de checklists
/// </summary>
public sealed record ChecklistSearchRequest
{
	/// <summary>
	/// Termo de busca (título do checklist ou nome/documento do cliente)
	/// </summary>
	[JsonProperty(PropertyName = "search", Required = Required.Default)]
	public string? Search { get; init; }

	/// <summary>
	/// Número da página (baseado em 1)
	/// </summary>
	[JsonProperty(PropertyName = "page", Required = Required.Default)]
	public int Page { get; init; } = 1;

	/// <summary>
	/// Tamanho da página (máximo 100)
	/// </summary>
	[JsonProperty(PropertyName = "pageSize", Required = Required.Default)]
	public int PageSize { get; init; } = 10;

	/// <summary>
	/// Campo para ordenação
	/// </summary>
	[JsonProperty(PropertyName = "sortBy", Required = Required.Default)]
	public string? SortBy { get; init; } = "createdDateUtc";

	/// <summary>
	/// Direção da ordenação (asc/desc)
	/// </summary>
	[JsonProperty(PropertyName = "sortDirection", Required = Required.Default)]
	public string? SortDirection { get; init; } = "desc";
}

/// <summary>
/// Response paginada para checklists
/// </summary>
public sealed record ChecklistPagedResponse
{
	/// <summary>
	/// Lista de checklists da página atual
	/// </summary>
	[JsonProperty(PropertyName = "data", Required = Required.Always)]
	public required IEnumerable<ChecklistResponse> Data { get; init; }

	/// <summary>
	/// Número total de registros
	/// </summary>
	[JsonProperty(PropertyName = "totalCount", Required = Required.Always)]
	public required int TotalCount { get; init; }

	/// <summary>
	/// Número da página atual
	/// </summary>
	[JsonProperty(PropertyName = "page", Required = Required.Always)]
	public required int Page { get; init; }

	/// <summary>
	/// Tamanho da página
	/// </summary>
	[JsonProperty(PropertyName = "pageSize", Required = Required.Always)]
	public required int PageSize { get; init; }

	/// <summary>
	/// Número total de páginas
	/// </summary>
	[JsonProperty(PropertyName = "totalPages", Required = Required.Always)]
	public required int TotalPages { get; init; }

	/// <summary>
	/// Indica se há página anterior
	/// </summary>
	[JsonProperty(PropertyName = "hasPreviousPage", Required = Required.Always)]
	public required bool HasPreviousPage { get; init; }

	/// <summary>
	/// Indica se há próxima página
	/// </summary>
	[JsonProperty(PropertyName = "hasNextPage", Required = Required.Always)]
	public required bool HasNextPage { get; init; }
}