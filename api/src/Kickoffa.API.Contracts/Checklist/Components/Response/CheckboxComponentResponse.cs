namespace Kickoffa.API.Contracts.Checklist.Components.Response
{
	/// <summary>
	/// Response para componente de checkbox
	/// </summary>
	public sealed record CheckboxComponentResponse : ComponentResponse
	{
		public CheckboxComponentResponse()
		{
			Type = "checkbox";
		}
	}
}