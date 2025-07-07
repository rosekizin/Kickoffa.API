using Kickoffa.API.Domain.Models.Enums;
using Kickoffa.API.Domain.Models.Components.Base;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <summary>
	/// Component para captura de assinatura digital do cliente
	/// </summary>
	public class SignatureComponent : Component
	{
		public SignatureComponent(long sectionId, string title, int order, string? description, bool isRequired)
			: base(sectionId, title, order, description, isRequired)
		{
		}

		public override ComponentType Type => ComponentType.Signature;
	}
}