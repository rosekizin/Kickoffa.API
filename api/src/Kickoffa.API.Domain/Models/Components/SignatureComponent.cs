using Kickoffa.API.Domain.Interfaces.Models.Components;
using Kickoffa.API.Domain.Models.Components.Base;
using Kickoffa.API.Domain.Models.Enums;

namespace Kickoffa.API.Domain.Models.Components
{
	/// <summary>
	/// Component para captura de assinatura digital do cliente
	/// </summary>
	public class SignatureComponent : Component, ISignatureComponent
	{
		public SignatureComponent(long sectionId, string title, int order, string? description, bool isRequired)
			: base(sectionId, title, order, description, isRequired)
		{
		}

		public override ComponentType Type => ComponentType.Signature;
	}
}