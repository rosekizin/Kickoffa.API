using Kickoffa.API.Domain.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Kickoffa.API.Application.Services.AppUser
{
	/// <summary>
	/// Implementação do serviço para obter informações do usuário atual
	/// </summary>
	public class CurrentUserService : ICurrentUserService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public long? UserId => GetUserIdFromClaims();

		public string? Email => _httpContextAccessor.HttpContext?.User
			?.FindFirst(ClaimTypes.Email)?.Value;

		public bool IsAuthenticated => UserId.HasValue;

		public IEnumerable<string> Roles => _httpContextAccessor.HttpContext?.User
			?.FindAll(ClaimTypes.Role)
			?.Select(c => c.Value) ?? Enumerable.Empty<string>();

		public bool IsInRole(string role)
		{
			return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
		}

		private long? GetUserIdFromClaims()
		{
			var userIdClaim = _httpContextAccessor.HttpContext?.User
				?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrEmpty(userIdClaim))
				return null;

			return long.TryParse(userIdClaim, out var userId) ? userId : null;
		}
	}
}