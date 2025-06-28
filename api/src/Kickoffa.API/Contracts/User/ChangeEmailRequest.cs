using System.ComponentModel.DataAnnotations;

namespace Kickoffa.API.Contracts.User;

/// <summary>
/// Contrato para solicitação de alteração de email
/// </summary>
public class ChangeEmailRequest
{
    /// <summary>
    /// Novo email do usuário
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [MaxLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string NewEmail { get; set; } = string.Empty;
}

/// <summary>
/// Contrato para confirmação de alteração de email
/// </summary>
public class ConfirmEmailChangeRequest
{
    /// <summary>
    /// Novo email do usuário
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [MaxLength(255, ErrorMessage = "Email deve ter no máximo 255 caracteres")]
    public string NewEmail { get; set; } = string.Empty;

    /// <summary>
    /// Token de confirmação recebido por email
    /// </summary>
    [Required(ErrorMessage = "Token de confirmação é obrigatório")]
    public string ConfirmationToken { get; set; } = string.Empty;
}

/// <summary>
/// Resposta para solicitação de alteração de email
/// </summary>
public class ChangeEmailResponse
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem descritiva do resultado
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Email para onde foi enviado o token de confirmação (apenas para InitiateEmailChange)
    /// </summary>
    public string? EmailSentTo { get; set; }
}
