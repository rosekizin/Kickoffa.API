using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Kickoffa.API.Contracts.Authentication;

/// <summary>
/// Request para login de usuário
/// </summary>
public sealed partial record LoginRequest : IValidatableObject
{
    /// <summary>
    /// Email do usuário
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Senha do usuário
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Lembrar do login (opcional)
    /// </summary>
    public bool RememberMe { get; init; } = false;

    /// <summary>
    /// Valida as propriedades do request
    /// </summary>
    /// <param name="validationContext">Contexto de validação</param>
    /// <returns>Resultados da validação</returns>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Validação do Email
        if (string.IsNullOrWhiteSpace(Email))
        {
            results.Add(new ValidationResult("O email é obrigatório", [nameof(Email)]));
        }
        else if (Email.Length > 255)
        {
            results.Add(new ValidationResult("O email deve ter no máximo 255 caracteres", [nameof(Email)]));
        }
        else if (!IsValidEmail(Email))
        {
            results.Add(new ValidationResult("Email deve ter um formato válido", [nameof(Email)]));
        }

        // Validação da Password
        if (string.IsNullOrWhiteSpace(Password))
        {
            results.Add(new ValidationResult("A senha é obrigatória", [nameof(Password)]));
        }
        else if (Password.Length < 6)
        {
            results.Add(new ValidationResult("A senha deve ter pelo menos 6 caracteres", [nameof(Password)]));
        }
        else if (Password.Length > 100)
        {
            results.Add(new ValidationResult("A senha deve ter no máximo 100 caracteres", [nameof(Password)]));
        }

        return results;
    }

    /// <summary>
    /// Valida se o email tem formato válido
    /// </summary>
    /// <param name="email">Email a ser validado</param>
    /// <returns>True se válido</returns>
    private static bool IsValidEmail(string email)
    {
		// Regex que bloqueia pontos consecutivos e outros erros comuns
		var regex = ValidEmail();

		if (!regex.IsMatch(email))
			return false;
		
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

	[GeneratedRegex(@"^(?!.*\.\.)(?!\.)(?!.*\.$)[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")]
	private static partial Regex ValidEmail();
}