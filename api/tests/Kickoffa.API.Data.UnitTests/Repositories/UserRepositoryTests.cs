using Microsoft.EntityFrameworkCore;
using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Data.EntityFramework.Mapping;
using Kickoffa.API.Data.Repositories;
using Kickoffa.API.Domain.Models;
using NSubstitute;
using Kickoffa.API.Domain.Models.AppUser;

namespace Kickoffa.API.Data.UnitTests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly KickoffaDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<KickoffaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var customerMapping = Substitute.For<ICustomerEntityFrameworkMapping>();
        var userMapping = Substitute.For<IUserEntityFrameworkMapping>();

        _context = new KickoffaDbContext(options, customerMapping, userMapping);
        _repository = new UserRepository(_context);

        // Configurar mapeamentos básicos para o teste
        customerMapping.Map(Arg.Any<ModelBuilder>())/*.Returns(x => { })*/;
        userMapping.Map(Arg.Any<ModelBuilder>())/*.Returns(x => { })*/;
    }

    [Fact]
    public async Task GetByEmailAndPasswordAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "123456");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAndPasswordAsync("joao@example.com", "123456");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAndPasswordAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "123456");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAndPasswordAsync("invalid@example.com", "123456");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAndPasswordAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "123456");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAndPasswordAsync("joao@example.com", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetByEmailAndPasswordAsync_WithInvalidEmailParameter_ShouldReturnNull(string invalidEmail)
    {
        // Act
        var result = await _repository.GetByEmailAndPasswordAsync(invalidEmail, "123456");

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetByEmailAndPasswordAsync_WithInvalidPasswordParameter_ShouldReturnNull(string invalidPassword)
    {
        // Act
        var result = await _repository.GetByEmailAndPasswordAsync("joao@example.com", invalidPassword);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAndPasswordAsync_WithCaseInsensitiveEmail_ShouldReturnUser()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "123456");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAndPasswordAsync("JOAO@EXAMPLE.COM", "123456");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetByEmailAsync_WithValidEmail_ShouldReturnUser()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "123456");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("joao@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "123456");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync("invalid@example.com");

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task GetByEmailAsync_WithInvalidEmailParameter_ShouldReturnNull(string invalidEmail)
    {
        // Act
        var result = await _repository.GetByEmailAsync(invalidEmail);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "123456");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync("joao@example.com");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithNonExistingEmail_ShouldReturnFalse()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "123456");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync("nonexisting@example.com");

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task EmailExistsAsync_WithInvalidEmailParameter_ShouldReturnFalse(string invalidEmail)
    {
        // Act
        var result = await _repository.EmailExistsAsync(invalidEmail);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task EmailExistsAsync_WithCaseInsensitiveEmail_ShouldReturnTrue()
    {
        // Arrange
        var user = new User("João Silva", "joao@example.com", "123456");
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.EmailExistsAsync("JOAO@EXAMPLE.COM");

        // Assert
        Assert.True(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
