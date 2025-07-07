namespace Kickoffa.API.Domain.Services
{
	/// <summary>
	/// Serviço para obter informações do usuário atual autenticado.
	/// Fornece acesso seguro aos dados do usuário logado através dos claims JWT.
	/// </summary>
	public interface ICurrentUserService
	{
		/// <summary>
		/// ID do usuário atual autenticado.
		/// Extrai o valor do claim NameIdentifier e converte para long.
		/// </summary>
		/// <value>
		/// ID do usuário se autenticado e válido, null caso contrário.
		/// </value>
		/// <example>
		/// <code>
		/// var userId = _currentUserService.UserId; // 123L ou null
		/// </code>
		/// </example>
		long? UserId { get; }

		/// <summary>
		/// Email do usuário atual autenticado.
		/// Extrai o valor do claim Email.
		/// </summary>
		/// <value>
		/// Email do usuário se disponível, null caso contrário.
		/// </value>
		/// <example>
		/// <code>
		/// var email = _currentUserService.Email; // "user@example.com" ou null
		/// </code>
		/// </example>
		string? Email { get; }

		/// <summary>
		/// Indica se há um usuário autenticado no contexto atual.
		/// Baseado na presença de um UserId válido.
		/// </summary>
		/// <value>
		/// true se há um usuário autenticado com ID válido, false caso contrário.
		/// </value>
		/// <example>
		/// <code>
		/// if (_currentUserService.IsAuthenticated)
		/// {
		///     // Usuário está logado, pode acessar dados protegidos
		/// }
		/// </code>
		/// </example>
		bool IsAuthenticated { get; }

		/// <summary>
		/// Lista de roles (funções) do usuário atual.
		/// Extrai todos os claims do tipo Role.
		/// </summary>
		/// <value>
		/// Coleção de strings com as roles do usuário, vazia se não houver roles.
		/// </value>
		/// <example>
		/// <code>
		/// var roles = _currentUserService.Roles; // ["Admin", "User"] ou []
		/// foreach (var role in roles)
		/// {
		///     Console.WriteLine($"User has role: {role}");
		/// }
		/// </code>
		/// </example>
		IEnumerable<string> Roles { get; }

		/// <summary>
		/// Verifica se o usuário atual possui uma role específica.
		/// Utiliza o método IsInRole do ClaimsPrincipal para verificação.
		/// </summary>
		/// <param name="role">Nome da role a ser verificada (case-sensitive).</param>
		/// <returns>
		/// true se o usuário possui a role especificada, false caso contrário.
		/// </returns>
		/// <example>
		/// <code>
		/// if (_currentUserService.IsInRole("Admin"))
		/// {
		///     // Usuário é administrador, pode acessar funcionalidades admin
		/// }
		/// 
		/// if (_currentUserService.IsInRole("Freelancer"))
		/// {
		///     // Usuário é freelancer, pode criar checklists
		/// }
		/// </code>
		/// </example>
		bool IsInRole(string role);
	}
}