namespace Kickoffa.API.Domain.Models.Enums
{
	/// <summary>
	/// Status possíveis para um checklist
	/// </summary>
	public enum ChecklistStatus
	{
		/// <summary>
		/// Checklist em desenvolvimento, ainda não publicado
		/// </summary>
		Draft = 0,

		/// <summary>
		/// Checklist publicado e ativo para uso
		/// </summary>
		Active = 1,

		/// <summary>
		/// Checklist concluído pelo cliente
		/// </summary>
		Completed = 2,

		/// <summary>
		/// Checklist arquivado (não mais em uso)
		/// </summary>
		Archived = 3
	}
}