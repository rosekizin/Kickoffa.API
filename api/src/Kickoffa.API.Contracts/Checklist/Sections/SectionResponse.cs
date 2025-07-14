using Kickoffa.API.Contracts.Checklist.Components;
using Kickoffa.API.Contracts.Newtonsoft;
using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Checklist.Sections;

/// <summary>
/// Classe base para responses de seções
/// </summary>
public abstract record SectionResponse
{
	[JsonProperty(PropertyName = "id", Required = Required.Always)]
	public required long Id { get; init; }

	[JsonProperty(PropertyName = "checklistId", Required = Required.Always)]
	public required long ChecklistId { get; init; }

	[JsonProperty(PropertyName = "title", Required = Required.Always)]
	public required string Title { get; init; }

	[JsonProperty(PropertyName = "type", Required = Required.Always)]
	public required string Type { get; init; }

	[JsonProperty(PropertyName = "order", Required = Required.Always)]
	public required int Order { get; init; }

	[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Always)]
	public required DateTime CreatedDateUtc { get; init; }

	[JsonProperty(PropertyName = "lastUpdatedDateUtc", Required = Required.Always)]
	public required DateTime LastUpdatedDateUtc { get; init; }
}

/// <summary>
/// Response para seção de briefing
/// </summary>
public sealed record BriefingSectionResponse : SectionResponse
{
	[JsonProperty(PropertyName = "contentJson", Required = Required.Default)]
	public string? ContentJson { get; init; }

	[JsonProperty(PropertyName = "contentHtml", Required = Required.Default)]
	public string? ContentHtml { get; init; }

	[JsonProperty(PropertyName = "contentLastUpdated", Required = Required.Default)]
	public DateTime? ContentLastUpdated { get; init; }

    public BriefingSectionResponse()
    {
        Type = "briefing";
    }
}

/// <summary>
/// Response para seção de checklist
/// </summary>
public sealed record ChecklistSectionResponse : SectionResponse
{
	[JsonProperty(PropertyName = "components", Required = Required.Always)]
	[JsonConverter(typeof(ComponentResponseCollectionConverter))]
    public ICollection<ComponentResponse>? Components { get; init; }

    public ChecklistSectionResponse()
    {
        Type = "checklist";
    }
}