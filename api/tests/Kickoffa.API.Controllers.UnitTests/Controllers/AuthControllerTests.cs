using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Contracts.Authentication;
using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kickoffa.API.Controllers.UnitTests.Controllers;

public class AuthControllerTests
{
    private readonly IUserService _userService;
    private readonly ISignInManagerWrapper _signInManagerWrapper;
    private readonly ILogger<AuthController> _logger;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _userService = Substitute.For<IUserService>();
        _signInManagerWrapper = Substitute.For<ISignInManagerWrapper>();
        _logger = Substitute.For<ILogger<AuthController>>();
        _authController = new AuthController(_userService, _signInManagerWrapper, _logger);
        
        // Mock HttpContext para testes
        _authController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnOk()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "password123",
            RememberMe = false
        };

        var user = new User("test@example.com");
        _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
        _signInManagerWrapper.PasswordSignInAsync(user, request.Password, false, true).Returns(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Act
        var result = await _authController.LoginAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.Equal(user.Id.ToString(), response.UserId);
        Assert.Equal(user.Email, response.Email);
        Assert.True(response.Success);
        Assert.NotNull(response.Session);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password123",
            RememberMe = false
        };

        _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var result = await _authController.LoginAsync(request, CancellationToken.None);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "wrongpassword",
            RememberMe = false
        };

        var user = new User("test@example.com");
        _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
        _signInManagerWrapper.PasswordSignInAsync(user, request.Password, false, true).Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await _authController.LoginAsync(request, CancellationToken.None);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public async Task LogoutAsync_ShouldReturnOk()
    {
        // Arrange
        _signInManagerWrapper.SignOutAsync().Returns(Task.CompletedTask);

        // Act
        var result = await _authController.LogoutAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        await _signInManagerWrapper.Received(1).SignOutAsync();
    }

    [Fact]
    public void ValidateSession_WithValidUser_ShouldReturnOk()
    {
        // Arrange
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "123"),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, "test@example.com")
        };
        
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "test");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        
        _authController.ControllerContext.HttpContext.User = principal;

        // Act
        var result = _authController.ValidateSession();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public void ValidateSession_WithoutUserId_ShouldReturnUnauthorized()
    {
        // Arrange
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, "test@example.com")
            // Sem NameIdentifier
        };
        
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "test");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        
        _authController.ControllerContext.HttpContext.User = principal;

        // Act
        var result = _authController.ValidateSession();

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.NotNull(unauthorizedResult.Value);
    }

    [Fact]
    public async Task GetCurrentUser_WithValidUser_ShouldReturnUserData()
    {
        // Arrange
        var userId = 123L;
        var user = new User("test@example.com");
        
        var claims = new[]
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
        };
        
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "test");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);
        
        _authController.ControllerContext.HttpContext.User = principal;
        _userService.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _authController.GetCurrentUser(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }
}
