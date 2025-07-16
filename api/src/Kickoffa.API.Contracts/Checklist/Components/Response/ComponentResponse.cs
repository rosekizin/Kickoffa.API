using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Checklist.Components.Response
{
	/// <summary>
	/// Classe base para responses de componentes
	/// </summary>
	public abstract record ComponentResponse
	{
		[JsonProperty(PropertyName = "id", Required = Required.Always)]
		public required long Id { get; init; }

		[JsonProperty(PropertyName = "sectionId", Required = Required.Always)]
		public required long SectionId { get; init; }

		[JsonProperty(PropertyName = "title", Required = Required.Always)]
		public required string Title { get; init; }

		[JsonProperty(PropertyName = "description", Required = Required.Default)]
		public string? Description { get; init; }

		[JsonProperty(PropertyName = "type", Required = Required.Always)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
		public string Type { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

		[JsonProperty(PropertyName = "isRequired", Required = Required.Always)]
		public required bool IsRequired { get; init; }

		[JsonProperty(PropertyName = "order", Required = Required.Always)]
		public required int Order { get; init; }

		[JsonProperty(PropertyName = "status", Required = Required.Default)]
		public ComponentStatusResponse? Status { get; init; }

		[JsonProperty(PropertyName = "createdDateUtc", Required = Required.Always)]
		public required DateTime CreatedDateUtc { get; init; }

		[JsonProperty(PropertyName = "lastUpdatedDateUtc", Required = Required.Always)]
		public required DateTime LastUpdatedDateUtc { get; init; }
	}
}