using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.Application.Services.AppUser;
using Kickoffa.API.Domain.Models.AppUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kickoffa.API.Application.UnitTests.Services.AppUser;

public class UserServiceTests
{
	private readonly IUserManagerWrapper _userManagerWrapper;
	private readonly ISignInManagerWrapper _signInManagerWrapper;
	private readonly IEmailService _emailService;
	private readonly ILogger<UserService> _logger;
	private readonly UserService _userService;
	private readonly CancellationToken _cancellationToken;

	public UserServiceTests()
	{
		_cancellationToken = new();
		_userManagerWrapper = Substitute.For<IUserManagerWrapper>();
		_signInManagerWrapper = Substitute.For<ISignInManagerWrapper>();
		_emailService = Substitute.For<IEmailService>();
		_logger = Substitute.For<ILogger<UserService>>();
		_userService = new UserService(_userManagerWrapper, _signInManagerWrapper, _emailService, _logger);
	}

	[Fact]
	public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnUser()
	{
		// Arrange
		var email = "joao@example.com";
		var password = "123456";
		var expectedUser = new User(email);

		_userManagerWrapper.FindByEmailAsync(email).Returns(expectedUser);
		_signInManagerWrapper.CheckPasswordSignInAsync(expectedUser, password, false).Returns(SignInResult.Success);

		// Act
		var result = await _userService.AuthenticateAsync(email, password, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expectedUser.Email, result.Email);
	}

	[Fact]
	public async Task AuthenticateAsync_WithInvalidCredentials_ShouldReturnNull()
	{
		// Arrange
		var email = "joao@example.com";
		var password = "wrongpassword";
		var user = new User(email);

		_userManagerWrapper.FindByEmailAsync(email).Returns(user);
		_signInManagerWrapper.CheckPasswordSignInAsync(user, password, false).Returns(SignInResult.Failed);

		// Act
		var result = await _userService.AuthenticateAsync(email, password, _cancellationToken);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task AuthenticateAsync_WithNonExistingUser_ShouldReturnNull()
	{
		// Arrange
		var email = "nonexisting@example.com";
		var password = "123456";

		_userManagerWrapper.FindByEmailAsync(email).Returns((User?)null);

		// Act
		var result = await _userService.AuthenticateAsync(email, password, _cancellationToken);

		// Assert
		Assert.Null(result);
		await _signInManagerWrapper.DidNotReceive().CheckPasswordSignInAsync(Arg.Any<User>(), Arg.Any<string>(), Arg.Any<bool>());
	}

	[Fact]
	public async Task CreateUserAsync_WithValidData_ShouldReturnSuccessResult()
	{
		// Arrange
		var email = "joao@example.com";
		var password = "123456";
		var successResult = IdentityResult.Success;

		_userManagerWrapper.CreateAsync(Arg.Any<User>(), password).Returns(successResult);
		_userManagerWrapper.AddToRoleAsync(Arg.Any<User>(), "freelancer").Returns(IdentityResult.Success);

		// Act
		var result = await _userService.CreateUserAsync(email, password, _cancellationToken);

		// Assert
		Assert.True(result.Succeeded);
		await _userManagerWrapper.Received(1).CreateAsync(Arg.Any<User>(), password);
		await _userManagerWrapper.Received(1).AddToRoleAsync(Arg.Any<User>(), "freelancer");
	}

	[Fact]
	public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
	{
		// Arrange
		var email = "joao@example.com";
		var expectedUser = new User(email);

		_userManagerWrapper.FindByEmailAsync(email).Returns(expectedUser);

		// Act
		var result = await _userService.GetByEmailAsync(email, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expectedUser.Email, result.Email);
	}

	[Fact]
	public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
	{
		// Arrange
		var userId = 123L;
		var expectedUser = new User("joao@example.com");

		_userManagerWrapper.FindByIdAsync(userId.ToString()).Returns(expectedUser);

		// Act
		var result = await _userService.GetByIdAsync(userId, _cancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal(expectedUser.Email, result.Email);
	}

	[Fact]
	public async Task InitiateEmailChangeAsync_WithValidData_ShouldReturnSuccessResult()
	{
		// Arrange
		var userId = 123L;
		var newEmail = "joao.santos@example.com";
		var user = new User("joao@example.com");
		var token = "test-token";

		_userManagerWrapper.FindByIdAsync(userId.ToString()).Returns(user);
		_userManagerWrapper.FindByEmailAsync(newEmail).Returns((User?)null);
		_userManagerWrapper.GenerateChangeEmailTokenAsync(user, newEmail).Returns(token);
		_emailService.SendEmailChangeConfirmationAsync(newEmail, Arg.Any<string>(), token, Arg.Any<CancellationToken>()).Returns(true);

		// Act
		var result = await _userService.InitiateEmailChangeAsync(userId, newEmail, _cancellationToken);

		// Assert
		Assert.True(result.Succeeded);
		await _userManagerWrapper.Received(1).GenerateChangeEmailTokenAsync(user, newEmail);
		await _emailService.Received(1).SendEmailChangeConfirmationAsync(newEmail, Arg.Any<string>(), token, Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task ConfirmEmailChangeAsync_WithValidData_ShouldReturnSuccessResult()
	{
		// Arrange
		var userId = 123L;
		var newEmail = "joao.santos@example.com";
		var token = "test-token";
		var user = new User("joao@example.com");

		_userManagerWrapper.FindByIdAsync(userId.ToString()).Returns(user);
		_userManagerWrapper.FindByEmailAsync(newEmail).Returns((User?)null);
		_userManagerWrapper.ChangeEmailAsync(user, newEmail, token).Returns(IdentityResult.Success);
		_userManagerWrapper.SetUserNameAsync(user, newEmail).Returns(IdentityResult.Success);
		_userManagerWrapper.UpdateAsync(user).Returns(IdentityResult.Success);
		_emailService.SendEmailChangeNotificationAsync(Arg.Any<string>(), newEmail, Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

		// Act
		var result = await _userService.ConfirmEmailChangeAsync(userId, newEmail, token, _cancellationToken);

		// Assert
		Assert.True(result.Succeeded);
		await _userManagerWrapper.Received(1).ChangeEmailAsync(user, newEmail, token);
		await _userManagerWrapper.Received(1).SetUserNameAsync(user, newEmail);
	}

	[Fact]
	public async Task ConfirmEmailChangeAsync_WithInvalidToken_ShouldReturnFailure()
	{
		// Arrange
		var userId = 123L;
		var newEmail = "joao.santos@example.com";
		var invalidToken = "invalid-token";
		var user = new User("joao@example.com");

		_userManagerWrapper.FindByIdAsync(userId.ToString()).Returns(user);
		_userManagerWrapper.FindByEmailAsync(newEmail).Returns((User?)null);
		_userManagerWrapper.ChangeEmailAsync(user, newEmail, invalidToken)
			.Returns(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

		// Act
		var result = await _userService.ConfirmEmailChangeAsync(userId, newEmail, invalidToken, _cancellationToken);

		// Assert
		Assert.False(result.Succeeded);
		Assert.Contains("Invalid token", result.Errors.Select(e => e.Description));
	}

	[Fact]
	public async Task ChangePasswordAsync_WithValidData_ShouldReturnSuccessResult()
	{
		// Arrange
		var userId = 123L;
		var currentPassword = "oldPassword";
		var newPassword = "newPassword";
		var user = new User("joao@example.com");

		_userManagerWrapper.FindByIdAsync(userId.ToString()).Returns(user);
		_userManagerWrapper.ChangePasswordAsync(user, currentPassword, newPassword).Returns(IdentityResult.Success);

		// Act
		var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword, _cancellationToken);

		// Assert
		Assert.True(result.Succeeded);
		await _userManagerWrapper.Received(1).ChangePasswordAsync(user, currentPassword, newPassword);
	}

	[Fact]
	public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
	{
		// Arrange
		var email = "joao@example.com";
		var user = new User(email);

		_userManagerWrapper.FindByEmailAsync(email).Returns(user);

		// Act
		var result = await _userService.EmailExistsAsync(email, _cancellationToken);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public async Task EmailExistsAsync_WithNonExistingEmail_ShouldReturnFalse()
	{
		// Arrange
		var email = "nonexisting@example.com";

		_userManagerWrapper.FindByEmailAsync(email).Returns((User?)null);

		// Act
		var result = await _userService.EmailExistsAsync(email, _cancellationToken);

		// Assert
		Assert.False(result);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public async Task AuthenticateAsync_WithInvalidEmail_ShouldReturnNull(string? invalidEmail)
	{
		// Act
#pragma warning disable CS8604 // Possible null reference argument.
		var result = await _userService.AuthenticateAsync(invalidEmail, "password", _cancellationToken);
#pragma warning restore CS8604 // Possible null reference argument.

		// Assert
		Assert.Null(result);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull(string? invalidPassword)
	{
		// Act
#pragma warning disable CS8604 // Possible null reference argument.
		var result = await _userService.AuthenticateAsync("email@example.com", invalidPassword, _cancellationToken);
#pragma warning restore CS8604 // Possible null reference argument.

		// Assert
		Assert.Null(result);
	}
}