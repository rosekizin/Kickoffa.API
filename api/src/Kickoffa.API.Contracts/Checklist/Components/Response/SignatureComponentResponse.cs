namespace Kickoffa.API.Contracts.Checklist.Components.Response
{
	/// <summary>
	/// Response para componente de assinatura
	/// </summary>
	public sealed record SignatureComponentResponse : ComponentResponse
	{
		public SignatureComponentResponse()
		{
			Type = "signature";
		}
	}
}