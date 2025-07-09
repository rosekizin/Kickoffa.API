namespace Kickoffa.API.Domain.Interfaces.Models.Components
{
	public interface ITextComponent : IComponent
	{
		/// <summary>
		/// Texto de placeholder para o campo
		/// </summary>
		string? Placeholder { get; }

		/// <summary>
		/// Limite máximo de caracteres
		/// </summary>
		int? MaxLength { get; }
	}
}