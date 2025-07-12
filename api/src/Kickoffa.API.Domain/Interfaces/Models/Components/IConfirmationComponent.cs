namespace Kickoffa.API.Domain.Interfaces.Models.Components
{
	/// <summary>
	/// Componente de confirmação com texto personalizado
	/// </summary>
	public interface IConfirmationComponent : IComponent
	{
		/// <summary>
		/// Texto de confirmação que será exibido junto com o checkbox
		/// </summary>
		string? ConfirmationText { get; }

		/// <summary>
		/// Updates the basic properties of the object, including setting a confirmation text.
		/// </summary>
		/// <param name="confirmationText">The text to be used for confirmation. Can be <see langword="null"/> or empty, in which case no confirmation text is set.</param>
		void UpdateBasicProperties(string? confirmationText);
	}
}