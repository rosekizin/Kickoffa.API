using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Checklist.Components.Response
{
	/// <summary>
	/// Response para componente de texto
	/// </summary>
	public sealed record TextComponentResponse : ComponentResponse
	{
		[JsonProperty(PropertyName = "placeholder", Required = Required.Default)]
		public string? Placeholder { get; init; }

		[JsonProperty(PropertyName = "maxLength", Required = Required.Default)]
		public int? MaxLength { get; init; }

		public TextComponentResponse()
		{
			Type = "text";
		}
	}
}