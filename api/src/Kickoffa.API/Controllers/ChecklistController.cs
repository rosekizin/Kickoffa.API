using Kickoffa.API.Application.Interfaces.Checkilists;
using Kickoffa.API.Contracts.Checklist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kickoffa.API.Controllers
{
	/// <summary>
	/// Controller para operações relacionadas a Checklist
	/// </summary>
	[ApiController]
	[Route("api/[controller]")]
	[Produces("application/json")]
	[Authorize]
	public sealed class ChecklistController : ControllerBase
	{
		private readonly IChecklistService _checklistService;
		private readonly ILogger<ChecklistController> _logger;
		private readonly ICreateChecklistService _createChecklistService;

		public ChecklistController(
			IChecklistService checklistService,
			ILogger<ChecklistController> logger,
			ICreateChecklistService createChecklistService)
		{
			_logger = logger;
			_checklistService = checklistService;
			_createChecklistService = createChecklistService;
		}

		/// <summary>
		/// Busca todos os checklists do usuário atual
		/// </summary>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Lista de checklists</returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ChecklistResponse>>> GetAllAsync(CancellationToken cancellationToken)
		{
			var userId = GetCurrentUserId();
			if (userId == null)
			{
				return Unauthorized("Usuário não autenticado");
			}

			var checklists = await _checklistService.GetByOwnerIdAsync(userId.Value, cancellationToken);
			return Ok(checklists);
		}

		/// <summary>
		/// Busca um checklist por ID
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist encontrado</returns>
		[HttpGet("{id:long}")]
		public async Task<ActionResult<ChecklistResponse>> GetByIdAsync([FromRoute] long id, CancellationToken cancellationToken)
		{
			var userId = GetCurrentUserId();
			if (userId == null)
			{
				return Unauthorized("Usuário não autenticado");
			}

			var checklist = await _checklistService.GetByIdAsync(id, cancellationToken);

			// Verificar se o checklist existe e se pertence ao usuário atual
			if (checklist == null)
			{
				return NotFound("Checklist não encontrado");
			}

			if (checklist.OwnerId != userId.Value)
			{
				return Forbid("Acesso negado ao checklist");
			}

			return Ok(checklist);
		}

		/// <summary>
		/// Busca um checklist por slug
		/// </summary>
		/// <param name="slug">Slug do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist encontrado</returns>
		[HttpGet("slug/{slug}")]
		public async Task<ActionResult<ChecklistResponse>> GetBySlugAsync([FromRoute] string slug, CancellationToken cancellationToken)
		{
			var userId = GetCurrentUserId();
			if (userId == null)
			{
				return Unauthorized("Usuário não autenticado");
			}

			var checklist = await _checklistService.GetBySlugAsync(slug, cancellationToken);

			// Verificar se o checklist existe e se pertence ao usuário atual
			if (checklist == null)
			{
				return NotFound("Checklist não encontrado");
			}

			if (checklist.OwnerId != userId.Value)
			{
				return Forbid("Acesso negado ao checklist");
			}

			return Ok(checklist);
		}

		/// <summary>
		/// Cria um novo checklist
		/// </summary>
		/// <param name="request">Dados do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist criado</returns>
		[HttpPost]
		public async Task<ActionResult<ChecklistResponse>> CreateAsync([FromBody] ChecklistRequest request, CancellationToken cancellationToken)
		{
			try
			{
				var userId = GetCurrentUserId();
				if (userId == null)
				{
					return Unauthorized("Usuário não autenticado");
				}

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var checklist = await _createChecklistService.CreateAsync(userId.Value, request, cancellationToken);

				_logger.LogInformation("Checklist criado com sucesso. ID: {ChecklistId}, Usuário: {UserId}", checklist.Id, userId);

				return Created($"/api/checklist/{checklist.Id}", checklist);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao criar checklist para usuário {UserId}", GetCurrentUserId());
				return StatusCode(500, "Erro interno do servidor");
			}
		}

		/// <summary>
		/// Atualiza um checklist existente
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="request">Dados atualizados do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Checklist atualizado</returns>
		[HttpPut("{id:long}")]
		public async Task<ActionResult<ChecklistResponse>> UpdateAsync([FromRoute] long id, [FromBody] ChecklistRequest request, CancellationToken cancellationToken)
		{
			try
			{
				var userId = GetCurrentUserId();
				if (userId == null)
				{
					return Unauthorized("Usuário não autenticado");
				}

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var checklist = await _checklistService.UpdateAsync(id, userId.Value, request, cancellationToken);

				if (checklist == null)
				{
					return NotFound("Checklist não encontrado ou acesso negado");
				}

				_logger.LogInformation("Checklist atualizado com sucesso. ID: {ChecklistId}, Usuário: {UserId}", id, userId);

				return Ok(checklist);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao atualizar checklist {ChecklistId} para usuário {UserId}", id, GetCurrentUserId());
				return StatusCode(500, "Erro interno do servidor");
			}
		}

		/// <summary>
		/// Remove um checklist
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resultado da operação</returns>
		[HttpDelete("{id:long}")]
		public async Task<ActionResult> DeleteAsync([FromRoute] long id, CancellationToken cancellationToken)
		{
			try
			{
				var userId = GetCurrentUserId();
				if (userId == null)
				{
					return Unauthorized("Usuário não autenticado");
				}

				var success = await _checklistService.DeleteAsync(id, userId.Value, cancellationToken);

				if (!success)
				{
					return NotFound("Checklist não encontrado ou acesso negado");
				}

				_logger.LogInformation("Checklist removido com sucesso. ID: {ChecklistId}, Usuário: {UserId}", id, userId);

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao remover checklist {ChecklistId} para usuário {UserId}", id, GetCurrentUserId());
				return StatusCode(500, "Erro interno do servidor");
			}
		}

		/// <summary>
		/// Publica um checklist
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resultado da operação</returns>
		[HttpPost("{id:long}/publish")]
		public async Task<ActionResult> PublishAsync([FromRoute] long id, CancellationToken cancellationToken)
		{
			try
			{
				var userId = GetCurrentUserId();
				if (userId == null)
				{
					return Unauthorized("Usuário não autenticado");
				}

				var success = await _checklistService.PublishAsync(id, userId.Value, cancellationToken);

				if (!success)
				{
					return NotFound("Checklist não encontrado ou acesso negado");
				}

				_logger.LogInformation("Checklist publicado com sucesso. ID: {ChecklistId}, Usuário: {UserId}", id, userId);

				return Ok(new { message = "Checklist publicado com sucesso" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao publicar checklist {ChecklistId} para usuário {UserId}", id, GetCurrentUserId());
				return StatusCode(500, "Erro interno do servidor");
			}
		}

		/// <summary>
		/// Despublica um checklist
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Resultado da operação</returns>
		[HttpPost("{id:long}/unpublish")]
		public async Task<ActionResult> UnpublishAsync([FromRoute] long id, CancellationToken cancellationToken)
		{
			try
			{
				var userId = GetCurrentUserId();
				if (userId == null)
				{
					return Unauthorized("Usuário não autenticado");
				}

				var success = await _checklistService.UnpublishAsync(id, userId.Value, cancellationToken);

				if (!success)
				{
					return NotFound("Checklist não encontrado ou acesso negado");
				}

				_logger.LogInformation("Checklist despublicado com sucesso. ID: {ChecklistId}, Usuário: {UserId}", id, userId);

				return Ok(new { message = "Checklist despublicado com sucesso" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao despublicar checklist {ChecklistId} para usuário {UserId}", id, GetCurrentUserId());
				return StatusCode(500, "Erro interno do servidor");
			}
		}

		/// <summary>
		/// Regenera o token de acesso de um checklist
		/// </summary>
		/// <param name="id">ID do checklist</param>
		/// <param name="cancellationToken">Token de cancelamento</param>
		/// <returns>Novo token de acesso</returns>
		[HttpPost("{id:long}/regenerate-token")]
		public async Task<ActionResult> RegenerateAccessTokenAsync([FromRoute] long id, CancellationToken cancellationToken)
		{
			try
			{
				var userId = GetCurrentUserId();
				if (userId == null)
				{
					return Unauthorized("Usuário não autenticado");
				}

				var newToken = await _checklistService.RegenerateAccessTokenAsync(id, userId.Value, cancellationToken);

				if (newToken == null)
				{
					return NotFound("Checklist não encontrado ou acesso negado");
				}

				_logger.LogInformation("Token de acesso regenerado com sucesso. ID: {ChecklistId}, Usuário: {UserId}", id, userId);

				return Ok(new { accessToken = newToken });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Erro ao regenerar token do checklist {ChecklistId} para usuário {UserId}", id, GetCurrentUserId());
				return StatusCode(500, "Erro interno do servidor");
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
}