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

	[JsonProperty(PropertyName = "isPublished", Required = Required.Always)]
	public required bool IsPublished { get; init; }

	[JsonProperty(PropertyName = "sections", Required = Required.Always)]
	[JsonConverter(typeof(SectionResponseCollectionConverter))]
	public required ICollection<SectionResponse> Sections { get; init; } = [];

	[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Always)]
	public required DateTime CreatedDateUtc { get; init; }

	[JsonProperty(PropertyName = "lastUpdatedDateUtc", Required = Required.Always)]
	public required DateTime LastUpdatedDateUtc { get; init; }
}