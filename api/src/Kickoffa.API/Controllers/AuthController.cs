using Kickoffa.API.Middlewares;
using Kickoffa.API.Contracts.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kickoffa.API.Controllers;

/// <summary>
/// Controller para operações de autenticação
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IJwtTokenService jwtTokenService, ILogger<AuthController> logger)
    {
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login do usuário
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Token de acesso e informações do usuário</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Implementar validação real de usuário/senha
            // Por enquanto, vamos usar credenciais fixas para demonstração
            if (request.Email != "admin@kickoffa.com" || request.Password != "123456")
            {
                _logger.LogWarning("Tentativa de login inválida para email: {Email}", request.Email);
                return Unauthorized(new { message = "Email ou senha inválidos" });
            }

            // Gerar token JWT
            var userId = Guid.NewGuid().ToString(); // TODO: Buscar ID real do usuário
            var accessToken = _jwtTokenService.GenerateToken(userId, request.Email);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Configurar cookies HttpOnly seguros
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Não acessível via JavaScript - proteção XSS
                Secure = Request.IsHttps, // Apenas HTTPS em produção
                SameSite = SameSiteMode.Strict, // Proteção CSRF
                Expires = DateTime.UtcNow.AddHours(1), // Expiração do token
                Path = "/" // Disponível em toda aplicação
            };

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7), // Refresh token expira em 7 dias
                Path = "/"
            };

            // Definir cookies seguros
            Response.Cookies.Append("access_token", accessToken, cookieOptions);
            Response.Cookies.Append("refresh_token", refreshToken, refreshCookieOptions);

            var response = new LoginResponse
            {
                AccessToken = accessToken, // Ainda retornamos para compatibilidade
                RefreshToken = refreshToken,
                ExpiresIn = 3600, // 1 hora em segundos
                UserId = userId,
                Email = request.Email,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            _logger.LogInformation("Login realizado com sucesso para usuário: {Email}", request.Email);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login para email: {Email}", request.Email);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Valida se o token atual é válido
    /// </summary>
    /// <returns>Informações do usuário se token válido</returns>
    [HttpGet("validate")]
    [Authorize]
    public ActionResult ValidateToken()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            {
                return Unauthorized(new { message = "Token inválido" });
            }

            return Ok(new
            {
                userId,
                email,
                isValid = true,
                validatedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante validação do token");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Realiza logout do usuário (invalidação do token)
    /// </summary>
    /// <returns>Confirmação de logout</returns>
    [HttpPost("logout")]
    [Authorize]
    public ActionResult Logout()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Limpar cookies HttpOnly
            var expiredCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(-1), // Expirar cookie
                Path = "/"
            };

            Response.Cookies.Append("access_token", "", expiredCookieOptions);
            Response.Cookies.Append("refresh_token", "", expiredCookieOptions);

            // TODO: Implementar blacklist de tokens ou invalidação no banco
            // Por enquanto, apenas limpeza dos cookies

            _logger.LogInformation("Logout realizado para usuário: {UserId}", userId);

            return Ok(new { message = "Logout realizado com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante logout");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}
