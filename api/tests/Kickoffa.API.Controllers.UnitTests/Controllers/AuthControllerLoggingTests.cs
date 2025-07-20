using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Contracts.Authentication;
using Kickoffa.API.Controllers;
using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kickoffa.API.Controllers.UnitTests.Controllers;

/// <summary>
/// Testes específicos para verificar se os logs do AuthController estão sendo chamados corretamente
/// </summary>
public class AuthControllerLoggingTests
{

    //private readonly IUserService _userService;
    //private readonly ISignInManagerWrapper _signInManagerWrapper;
    //private readonly ILogger<AuthController> _logger;
    //private readonly AuthController _authController;

    //public AuthControllerLoggingTests()
    //{
    //    _userService = Substitute.For<IUserService>();
    //    _signInManagerWrapper = Substitute.For<ISignInManagerWrapper>();
    //    _logger = Substitute.For<ILogger<AuthController>>();
    //    _authController = new AuthController(_userService, _signInManagerWrapper, _logger);
        
    //    // Mock HttpContext para testes
    //    _authController.ControllerContext = new ControllerContext
    //    {
    //        HttpContext = new DefaultHttpContext()
    //    };
    //}

    //[Fact]
    //public async Task LoginAsync_ShouldLogLoginAttempt_WhenCalled()
    //{
    //    // Arrange
    //    var request = new LoginRequest
    //    {
    //        Email = "test@example.com",
    //        Password = "password123",
    //        RememberMe = false
    //    };

    //    var user = new User("test@example.com");
    //    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
    //    _signInManagerWrapper.PasswordSignInAsync(user, request.Password, false, true).Returns(SignInResult.Success);

    //    // Act
    //    await _authController.LoginAsync(request, CancellationToken.None);

    //    // Assert
    //    _logger.Received(1).LogInformation("🔐 Tentativa de login iniciada para email: {Email}", request.Email);
    //}

    //[Fact]
    //public async Task LoginAsync_ShouldLogSuccessfulLogin_WhenCredentialsAreValid()
    //{
    //    // Arrange
    //    var request = new LoginRequest
    //    {
    //        Email = "test@example.com",
    //        Password = "password123",
    //        RememberMe = false
    //    };

    //    var user = new User("test@example.com") { Id = Guid.NewGuid() };
    //    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
    //    _signInManagerWrapper.PasswordSignInAsync(user, request.Password, false, true).Returns(SignInResult.Success);

    //    // Act
    //    await _authController.LoginAsync(request, CancellationToken.None);

    //    // Assert
    //    _logger.Received(1).LogInformation("✅ Login realizado com sucesso para usuário: {UserId} ({Email})", 
    //        user.Id, user.Email);
    //}

    //[Fact]
    //public async Task LoginAsync_ShouldLogWarning_WhenUserNotFound()
    //{
    //    // Arrange
    //    var request = new LoginRequest
    //    {
    //        Email = "nonexistent@example.com",
    //        Password = "password123",
    //        RememberMe = false
    //    };

    //    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

    //    // Act
    //    await _authController.LoginAsync(request, CancellationToken.None);

    //    // Assert
    //    _logger.Received(1).LogWarning("Tentativa de login com email inexistente: {Email}", request.Email);
    //}

    //[Fact]
    //public async Task LoginAsync_ShouldLogWarning_WhenCredentialsAreInvalid()
    //{
    //    // Arrange
    //    var request = new LoginRequest
    //    {
    //        Email = "test@example.com",
    //        Password = "wrongpassword",
    //        RememberMe = false
    //    };

    //    var user = new User("test@example.com");
    //    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
    //    _signInManagerWrapper.PasswordSignInAsync(user, request.Password, false, true).Returns(SignInResult.Failed);

    //    // Act
    //    await _authController.LoginAsync(request, CancellationToken.None);

    //    // Assert
    //    _logger.Received(1).LogWarning("Tentativa de login inválida para email: {Email}. Motivo: {Reason}",
    //        request.Email, "Credenciais inválidas");
    //}

    //[Fact]
    //public async Task LoginAsync_ShouldLogWarning_WhenAccountIsLockedOut()
    //{
    //    // Arrange
    //    var request = new LoginRequest
    //    {
    //        Email = "test@example.com",
    //        Password = "password123",
    //        RememberMe = false
    //    };

    //    var user = new User("test@example.com");
    //    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
    //    _signInManagerWrapper.PasswordSignInAsync(user, request.Password, false, true).Returns(SignInResult.LockedOut);

    //    // Act
    //    await _authController.LoginAsync(request, CancellationToken.None);

    //    // Assert
    //    _logger.Received(1).LogWarning("Tentativa de login inválida para email: {Email}. Motivo: {Reason}",
    //        request.Email, "Conta bloqueada");
    //}

    //[Fact]
    //public async Task LoginAsync_ShouldLogWarning_WhenLoginIsNotAllowed()
    //{
    //    // Arrange
    //    var request = new LoginRequest
    //    {
    //        Email = "test@example.com",
    //        Password = "password123",
    //        RememberMe = false
    //    };

    //    var user = new User("test@example.com");
    //    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
    //    _signInManagerWrapper.PasswordSignInAsync(user, request.Password, false, true).Returns(SignInResult.NotAllowed);

    //    // Act
    //    await _authController.LoginAsync(request, CancellationToken.None);

    //    // Assert
    //    _logger.Received(1).LogWarning("Tentativa de login inválida para email: {Email}. Motivo: {Reason}",
    //        request.Email, "Login não permitido");
    //}

    //[Fact]
    //public async Task LoginAsync_ShouldLogBothAttemptAndSuccess_WhenLoginIsSuccessful()
    //{
    //    // Arrange
    //    var request = new LoginRequest
    //    {
    //        Email = "test@example.com",
    //        Password = "password123",
    //        RememberMe = false
    //    };

    //    var user = new User("test@example.com") { Id = Guid.NewGuid() };
    //    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
    //    _signInManagerWrapper.PasswordSignInAsync(user, request.Password, false, true).Returns(SignInResult.Success);

    //    // Act
    //    await _authController.LoginAsync(request, CancellationToken.None);

    //    // Assert
    //    // Verificar se ambos os logs foram chamados
    //    _logger.Received(1).LogInformation("🔐 Tentativa de login iniciada para email: {Email}", request.Email);
    //    _logger.Received(1).LogInformation("✅ Login realizado com sucesso para usuário: {UserId} ({Email})", 
    //        user.Id, user.Email);
    //}

    //[Fact]
    //public async Task LoginAsync_ShouldLogAttemptAndWarning_WhenLoginFails()
    //{
    //    // Arrange
    //    var request = new LoginRequest
    //    {
    //        Email = "test@example.com",
    //        Password = "wrongpassword",
    //        RememberMe = false
    //    };

    //    var user = new User("test@example.com");
    //    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
    //    _signInManagerWrapper.PasswordSignInAsync(user, request.Password, false, true).Returns(SignInResult.Failed);

    //    // Act
    //    await _authController.LoginAsync(request, CancellationToken.None);

    //    // Assert
    //    // Verificar se ambos os logs foram chamados
    //    _logger.Received(1).LogInformation("🔐 Tentativa de login iniciada para email: {Email}", request.Email);
    //    _logger.Received(1).LogWarning("Tentativa de login inválida para email: {Email}. Motivo: {Reason}",
    //        request.Email, "Credenciais inválidas");
    //}

    //[Theory]
    //[InlineData(true)]
    //[InlineData(false)]
    //public async Task LoginAsync_ShouldLogSuccessRegardlessOfRememberMe(bool rememberMe)
    //{
    //    // Arrange
    //    var request = new LoginRequest
    //    {
    //        Email = "test@example.com",
    //        Password = "password123",
    //        RememberMe = rememberMe
    //    };

    //    var user = new User("test@example.com") { Id = Guid.NewGuid() };
    //    _userService.GetByEmailAsync(request.Email, Arg.Any<CancellationToken>()).Returns(user);
    //    _signInManagerWrapper.PasswordSignInAsync(user, request.Password, rememberMe, true).Returns(SignInResult.Success);

    //    // Act
    //    await _authController.LoginAsync(request, CancellationToken.None);

    //    // Assert
    //    _logger.Received(1).LogInformation("✅ Login realizado com sucesso para usuário: {UserId} ({Email})", 
    //        user.Id, user.Email);
    //}
}
