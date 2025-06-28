using Kickoffa.API.Application.Services.AppUser;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.Domain.Models.AppUser;
using NSubstitute;

namespace Kickoffa.API.Application.UnitTests.Services.AppUser;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _userService = new UserService(_userRepository);
    }

    [Fact]
    public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new UserService(null!));
    }

    [Fact]
    public async Task GetByEmailAndPasswordAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var email = "joao@example.com";
        var password = "123456";
        var expectedUser = new User("João Silva", email, password);
        
        _userRepository.GetByEmailAndPasswordAsync(email, password, Arg.Any<CancellationToken>())
            .Returns(expectedUser);

        // Act
        var result = await _userService.GetByEmailAndPasswordAsync(email, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Email, result.Email);
        await _userRepository.Received(1).GetByEmailAndPasswordAsync(email, password, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByEmailAndPasswordAsync_WithInvalidCredentials_ShouldReturnNull()
    {
        // Arrange
        var email = "joao@example.com";
        var password = "wrongpassword";
        
        _userRepository.GetByEmailAndPasswordAsync(email, password, Arg.Any<CancellationToken>())
            .Returns((Domain.Models.AppUser.User?)null);

        // Act
        var result = await _userService.GetByEmailAndPasswordAsync(email, password);

        // Assert
        Assert.Null(result);
        await _userRepository.Received(1).GetByEmailAndPasswordAsync(email, password, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("", "123456")]
    [InlineData(" ", "123456")]
    [InlineData(null, "123456")]
    [InlineData("joao@example.com", "")]
    [InlineData("joao@example.com", " ")]
    [InlineData("joao@example.com", null)]
    public async Task GetByEmailAndPasswordAsync_WithInvalidParameters_ShouldReturnNull(string email, string password)
    {
        // Act
        var result = await _userService.GetByEmailAndPasswordAsync(email, password);

        // Assert
        Assert.Null(result);
        await _userRepository.DidNotReceive().GetByEmailAndPasswordAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var email = "joao@example.com";
        var expectedUser = new User("João Silva", email, "123456");
        
        _userRepository.GetByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns(expectedUser);

        // Act
        var result = await _userService.GetByEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Email, result.Email);
        await _userRepository.Received(1).GetByEmailAsync(email, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
    {
        // Arrange
        var email = "nonexisting@example.com";
        
        _userRepository.GetByEmailAsync(email, Arg.Any<CancellationToken>())
            .Returns((Domain.Models.AppUser.User?)null);

        // Act
        var result = await _userService.GetByEmailAsync(email);

        // Assert
        Assert.Null(result);
        await _userRepository.Received(1).GetByEmailAsync(email, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull(string invalidEmail)
    {
        // Act
        var result = await _userService.GetByEmailAsync(invalidEmail);

        // Assert
        Assert.Null(result);
        await _userRepository.DidNotReceive().GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var email = "joao@example.com";
        
        _userRepository.EmailExistsAsync(email, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _userService.EmailExistsAsync(email);

        // Assert
        Assert.True(result);
        await _userRepository.Received(1).EmailExistsAsync(email, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EmailExistsAsync_WithNonExistingEmail_ShouldReturnFalse()
    {
        // Arrange
        var email = "nonexisting@example.com";
        
        _userRepository.EmailExistsAsync(email, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _userService.EmailExistsAsync(email);

        // Assert
        Assert.False(result);
        await _userRepository.Received(1).EmailExistsAsync(email, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task EmailExistsAsync_WithInvalidEmail_ShouldReturnFalse(string invalidEmail)
    {
        // Act
        var result = await _userService.EmailExistsAsync(invalidEmail);

        // Assert
        Assert.False(result);
        await _userRepository.DidNotReceive().EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByEmailAndPasswordAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var email = "joao@example.com";
        var password = "123456";
        var cancellationToken = new CancellationToken();
        
        _userRepository.GetByEmailAndPasswordAsync(email, password, cancellationToken)
            .Returns((Domain.Models.AppUser.User?)null);

        // Act
        await _userService.GetByEmailAndPasswordAsync(email, password, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByEmailAndPasswordAsync(email, password, cancellationToken);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var email = "joao@example.com";
        var cancellationToken = new CancellationToken();
        
        _userRepository.GetByEmailAsync(email, cancellationToken)
            .Returns((Domain.Models.AppUser.User?)null);

        // Act
        await _userService.GetByEmailAsync(email, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByEmailAsync(email, cancellationToken);
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var email = "joao@example.com";
        var cancellationToken = new CancellationToken();
        
        _userRepository.EmailExistsAsync(email, cancellationToken)
            .Returns(false);

        // Act
        await _userService.EmailExistsAsync(email, cancellationToken);

        // Assert
        await _userRepository.Received(1).EmailExistsAsync(email, cancellationToken);
    }
}
