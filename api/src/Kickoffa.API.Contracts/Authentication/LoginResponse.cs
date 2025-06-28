namespace Kickoffa.API.Contracts.Authentication;

/// <summary>
/// Response do login com informações do token
/// </summary>
public sealed record LoginResponse
{
    /// <summary>
    /// Token de acesso JWT
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Tipo do token (sempre "Bearer")
    /// </summary>
    public string TokenType { get; init; } = "Bearer";

    /// <summary>
    /// Tempo de expiração do token em segundos
    /// </summary>
    public required int ExpiresIn { get; init; }

    /// <summary>
    /// Refresh token para renovação
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// ID do usuário autenticado
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Email do usuário autenticado
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Data e hora de emissão do token
    /// </summary>
    public DateTime IssuedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Data e hora de expiração do token
    /// </summary>
    public DateTime ExpiresAt { get; init; }
}
