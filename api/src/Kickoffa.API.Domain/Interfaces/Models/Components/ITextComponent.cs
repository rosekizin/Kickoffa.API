namespace Kickoffa.API.Domain.Interfaces.Models.Components
{
	/// <summary>
	/// Componente de campo de texto para entrada de dados pelo cliente
	/// </summary>
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

		/// <summary>
		/// Update básico das propriedades do componente de texto
		/// </summary>
		/// <param name="maxLength"></param>
		/// <param name="placeholder"></param>
		void UpdateBasicProperties(int? maxLength, string? placeholder);
	}
}