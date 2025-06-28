using System.Security.Claims;

namespace Kickoffa.API.Middlewares;

/// <summary>
/// Interface para serviço de geração e validação de tokens JWT
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Gera um token JWT para o usuário
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="email">Email do usuário</param>
    /// <param name="additionalClaims">Claims adicionais (opcional)</param>
    /// <returns>Token JWT gerado</returns>
    string GenerateToken(string userId, string email, IEnumerable<Claim>? additionalClaims = null);

    /// <summary>
    /// Gera um refresh token
    /// </summary>
    /// <returns>Refresh token gerado</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    /// <param name="token">Token a ser validado</param>
    /// <returns>Claims principal se válido, null se inválido</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Extrai o ID do usuário de um token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>ID do usuário ou null se não encontrado</returns>
    string? GetUserIdFromToken(string token);

    /// <summary>
    /// Verifica se um token está expirado
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <returns>True se expirado, false caso contrário</returns>
    bool IsTokenExpired(string token);
}
