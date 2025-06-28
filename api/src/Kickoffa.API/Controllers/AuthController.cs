using Kickoffa.API.Contracts.Authentication;
using Kickoffa.API.Application.Services.AppUser;
using Kickoffa.API.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kickoffa.API.Controllers;

/// <summary>
/// Controller para operações de autenticação usando ASP.NET Core Identity
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ISignInManagerWrapper _signInManagerWrapper;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ISignInManagerWrapper signInManagerWrapper, ILogger<AuthController> logger)
    {
        _userService = userService;
        _signInManagerWrapper = signInManagerWrapper;
        _logger = logger;
    }

    /// <summary>
    /// Realiza login do usuário usando ASP.NET Core Identity
    /// </summary>
    /// <param name="request">Dados de login</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Informações do usuário autenticado</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
			// SenhaSuperSegura123!

			// Buscar usuário por email
			var user = await _userService.GetByEmailAsync(request.Email, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("Tentativa de login com email inexistente: {Email}", request.Email);
                return Unauthorized(new { message = "Email ou senha inválidos" });
            }

            // Fazer login usando SignInManager do Identity
            var rememberMe = request.RememberMe;
            var result = await _signInManagerWrapper.PasswordSignInAsync(
                user,
                request.Password,
                rememberMe,
                lockoutOnFailure: true);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Tentativa de login inválida para email: {Email}. Motivo: {Reason}",
                    request.Email,
                    result.IsLockedOut ? "Conta bloqueada" :
                    result.IsNotAllowed ? "Login não permitido" : "Credenciais inválidas");

                return Unauthorized(new { message = "Email ou senha inválidos" });
            }

            // O ASP.NET Core Identity já gerencia cookies de autenticação automaticamente
            var response = new LoginResponse
            {
                UserId = user.Id.ToString(),
                Email = user.Email ?? string.Empty,
                Success = true,
                Message = "Login realizado com sucesso",
                LoginAt = DateTime.UtcNow,
                Session = new SessionInfo
                {
                    ExpiresInSeconds = rememberMe ? 604800 : 3600, // 7 dias ou 1 hora
                    ExpiresAt = DateTime.UtcNow.Add(rememberMe ? TimeSpan.FromDays(7) : TimeSpan.FromHours(1)),
                    IsPersistent = rememberMe
                }
            };

            _logger.LogInformation("Login realizado com sucesso para usuário: {Email}", user.Email);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante o login para email: {Email}", request.Email);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Valida se o usuário está autenticado e retorna informações da sessão
    /// </summary>
    /// <returns>Informações do usuário e sessão se autenticado</returns>
    [HttpGet("validate")]
    [Authorize]
    public ActionResult ValidateSession()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "Usuário não autenticado" });
            }

            return Ok(new
            {
                userId,
                email,
                isAuthenticated = true,
                validatedAt = DateTime.UtcNow,
                sessionInfo = new
                {
                    authenticationMethod = "Cookie",
                    issuedAt = User.FindFirst("iat")?.Value,
                    expiresAt = User.FindFirst("exp")?.Value
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro durante validação de autenticação");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém informações do usuário atual
    /// </summary>
    /// <returns>Dados do usuário autenticado</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out var userIdLong))
            {
                return Unauthorized(new { message = "Usuário não autenticado" });
            }

            var user = await _userService.GetByIdAsync(userIdLong, cancellationToken);
            if (user == null)
            {
                return NotFound(new { message = "Usuário não encontrado" });
            }

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                userName = user.UserName,
                emailConfirmed = user.EmailConfirmed,
                createdAt = user.CreatedDateUtc,
                lastUpdated = user.LastUpdatedDateUtc
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter dados do usuário atual");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Realiza logout do usuário usando ASP.NET Core Identity
    /// </summary>
    /// <returns>Confirmação de logout</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> LogoutAsync()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Fazer logout usando SignInManager do Identity
            await _signInManagerWrapper.SignOutAsync();

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
