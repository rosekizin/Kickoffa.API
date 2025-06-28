using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Contracts.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kickoffa.API.Controllers;

/// <summary>
/// Controller para operações relacionadas ao usuário
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Inicia o processo de alteração de email
    /// </summary>
    /// <param name="request">Dados da solicitação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da operação</returns>
    [HttpPost("change-email/initiate")]
    public async Task<ActionResult<ChangeEmailResponse>> InitiateEmailChange(
        [FromBody] ChangeEmailRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ChangeEmailResponse 
                { 
                    Success = false, 
                    Message = "Usuário não autenticado" 
                });
            }

            var result = await _userService.InitiateEmailChangeAsync(userId.Value, request.NewEmail, cancellationToken);

            if (result.Succeeded)
            {
                _logger.LogInformation("Processo de alteração de email iniciado para usuário {UserId}", userId);
                
                return Ok(new ChangeEmailResponse
                {
                    Success = true,
                    Message = "Email de confirmação enviado com sucesso",
                    EmailSentTo = request.NewEmail
                });
            }

            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Falha ao iniciar alteração de email para usuário {UserId}: {Errors}", userId, errorMessage);

            return BadRequest(new ChangeEmailResponse
            {
                Success = false,
                Message = errorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao iniciar alteração de email");
            return StatusCode(500, new ChangeEmailResponse
            {
                Success = false,
                Message = "Erro interno do servidor"
            });
        }
    }

    /// <summary>
    /// Confirma a alteração de email usando token
    /// </summary>
    /// <param name="request">Dados da confirmação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Resultado da confirmação</returns>
    [HttpPost("change-email/confirm")]
    public async Task<ActionResult<ChangeEmailResponse>> ConfirmEmailChange(
        [FromBody] ConfirmEmailChangeRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new ChangeEmailResponse 
                { 
                    Success = false, 
                    Message = "Usuário não autenticado" 
                });
            }

            var result = await _userService.ConfirmEmailChangeAsync(
                userId.Value, 
                request.NewEmail, 
                request.ConfirmationToken, 
                cancellationToken);

            if (result.Succeeded)
            {
                _logger.LogInformation("Email alterado com sucesso para usuário {UserId}", userId);
                
                return Ok(new ChangeEmailResponse
                {
                    Success = true,
                    Message = "Email alterado com sucesso"
                });
            }

            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Falha ao confirmar alteração de email para usuário {UserId}: {Errors}", userId, errorMessage);

            return BadRequest(new ChangeEmailResponse
            {
                Success = false,
                Message = errorMessage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro interno ao confirmar alteração de email");
            return StatusCode(500, new ChangeEmailResponse
            {
                Success = false,
                Message = "Erro interno do servidor"
            });
        }
    }

    /// <summary>
    /// Verifica se um email está disponível
    /// </summary>
    /// <param name="email">Email a ser verificado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Disponibilidade do email</returns>
    [HttpGet("check-email-availability")]
    public async Task<ActionResult<object>> CheckEmailAvailability(
        [FromQuery] string email, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { available = false, message = "Email é obrigatório" });
            }

            var exists = await _userService.EmailExistsAsync(email, cancellationToken);
            
            return Ok(new { available = !exists });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar disponibilidade do email {Email}", email);
            return StatusCode(500, new { available = false, message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém o ID do usuário atual a partir do token JWT
    /// </summary>
    /// <returns>ID do usuário ou null se não encontrado</returns>
    private long? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }
}
