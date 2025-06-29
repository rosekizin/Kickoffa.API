using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Items
{
	/// <summary>
	/// Item para captura de assinatura digital do cliente
	/// </summary>
	public class SignatureItem : Item
	{
		public override ItemType Type => ItemType.Signature;
	}
}
