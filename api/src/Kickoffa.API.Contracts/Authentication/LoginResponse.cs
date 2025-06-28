namespace Kickoffa.API.Contracts.Authentication;

/// <summary>
/// Response do login usando ASP.NET Core Identity
/// Autenticação gerenciada por cookies HttpOnly
/// </summary>
public sealed record LoginResponse
{
    /// <summary>
    /// ID do usuário autenticado
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Email do usuário autenticado
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Indica se o login foi bem-sucedido
    /// </summary>
    public bool Success { get; init; } = true;

    /// <summary>
    /// Mensagem de sucesso
    /// </summary>
    public string Message { get; init; } = "Login realizado com sucesso";

    /// <summary>
    /// Data e hora do login
    /// </summary>
    public DateTime LoginAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Informações sobre a sessão (opcional)
    /// </summary>
    public SessionInfo? Session { get; init; }
}

/// <summary>
/// Informações sobre a sessão do usuário
/// </summary>
public sealed record SessionInfo
{
    /// <summary>
    /// Tempo de expiração da sessão em segundos
    /// </summary>
    public int ExpiresInSeconds { get; init; } = 3600; // 1 hora

    /// <summary>
    /// Data e hora de expiração da sessão
    /// </summary>
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddHours(1);

    /// <summary>
    /// Indica se a sessão é persistente (Remember Me)
    /// </summary>
    public bool IsPersistent { get; init; }
}
