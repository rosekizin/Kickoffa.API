using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Services.AppUser;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Security.Claims;
using Xunit;

namespace Kickoffa.API.Application.UnitTests.Services.AppUser
{
	/// <summary>
	/// Testes unitários para CurrentUserService
	/// </summary>
	public class CurrentUserServiceTests
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly CurrentUserService _currentUserService;

		public CurrentUserServiceTests()
		{
			_httpContextAccessor = Substitute.For<IHttpContextAccessor>();
			_currentUserService = new CurrentUserService(_httpContextAccessor);
		}

		[Fact]
		public void UserId_WithValidClaim_ShouldReturnUserId()
		{
			// Arrange
			var expectedUserId = 123L;
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, expectedUserId.ToString())
			};
			var identity = new ClaimsIdentity(claims);
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.UserId;

			// Assert
			Assert.Equal(expectedUserId, result);
		}

		[Fact]
		public void UserId_WithInvalidClaim_ShouldReturnNull()
		{
			// Arrange
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "invalid-id")
			};
			var identity = new ClaimsIdentity(claims);
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.UserId;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void UserId_WithoutClaim_ShouldReturnNull()
		{
			// Arrange
			var identity = new ClaimsIdentity();
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.UserId;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void UserId_WithoutHttpContext_ShouldReturnNull()
		{
			// Arrange
			_httpContextAccessor.HttpContext.Returns((HttpContext?)null);

			// Act
			var result = _currentUserService.UserId;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void Email_WithValidClaim_ShouldReturnEmail()
		{
			// Arrange
			var expectedEmail = "test@example.com";
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Email, expectedEmail)
			};
			var identity = new ClaimsIdentity(claims);
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.Email;

			// Assert
			Assert.Equal(expectedEmail, result);
		}

		[Fact]
		public void Email_WithoutClaim_ShouldReturnNull()
		{
			// Arrange
			var identity = new ClaimsIdentity();
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.Email;

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void IsAuthenticated_WithValidUserId_ShouldReturnTrue()
		{
			// Arrange
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, "123")
			};
			var identity = new ClaimsIdentity(claims);
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.IsAuthenticated;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsAuthenticated_WithoutUserId_ShouldReturnFalse()
		{
			// Arrange
			var identity = new ClaimsIdentity();
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.IsAuthenticated;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void Roles_WithValidClaims_ShouldReturnRoles()
		{
			// Arrange
			var expectedRoles = new[] { "Admin", "User" };
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Role, "Admin"),
				new Claim(ClaimTypes.Role, "User")
			};
			var identity = new ClaimsIdentity(claims);
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.Roles;

			// Assert
			Assert.Equal(expectedRoles, result);
		}

		[Fact]
		public void Roles_WithoutClaims_ShouldReturnEmpty()
		{
			// Arrange
			var identity = new ClaimsIdentity();
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.Roles;

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void IsInRole_WithValidRole_ShouldReturnTrue()
		{
			// Arrange
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Role, "Admin")
			};
			var identity = new ClaimsIdentity(claims);
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.IsInRole("Admin");

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsInRole_WithInvalidRole_ShouldReturnFalse()
		{
			// Arrange
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Role, "User")
			};
			var identity = new ClaimsIdentity(claims);
			var principal = new ClaimsPrincipal(identity);
			var httpContext = new DefaultHttpContext { User = principal };
			
			_httpContextAccessor.HttpContext.Returns(httpContext);

			// Act
			var result = _currentUserService.IsInRole("Admin");

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void IsInRole_WithoutHttpContext_ShouldReturnFalse()
		{
			// Arrange
			_httpContextAccessor.HttpContext.Returns((HttpContext?)null);

			// Act
			var result = _currentUserService.IsInRole("Admin");

			// Assert
			Assert.False(result);
		}
	}
}