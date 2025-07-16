using Newtonsoft.Json;

namespace Kickoffa.API.Contracts.Checklist.Components.Response
{
	/// <summary>
	/// Response para componente de confirmação
	/// </summary>
	public sealed record ConfirmationComponentResponse : ComponentResponse
	{
		[JsonProperty(PropertyName = "confirmationText", Required = Required.Default)]
		public string? ConfirmationText { get; init; }

		public ConfirmationComponentResponse()
		{
			Type = "confirmation";
		}
	}
}