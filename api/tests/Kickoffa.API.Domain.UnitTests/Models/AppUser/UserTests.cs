using Kickoffa.API.Domain.Models.AppUser;

namespace Kickoffa.API.Domain.UnitTests.Models.AppUser;

public class UserTests
{
	[Fact]
	public void Constructor_WithValidData_ShouldCreateUser()
	{
		// Arrange
		var name = "João Silva";
		var email = "joao@example.com";
		var password = "123456";
		var role = "freelancer";

		// Act
		var user = new User(name, email, password, role);

		// Assert
		Assert.Equal(name, user.Name);
		Assert.Equal(email, user.Email);
		Assert.Equal(password, user.Password);
		Assert.Equal(role, user.Role);
		Assert.NotEqual(Guid.Empty, user.Id);
		Assert.True(user.CreatedDateUtc <= DateTime.UtcNow);
		Assert.True(user.LastUpdatedDateUtc <= DateTime.UtcNow);
	}

	[Fact]
	public void Constructor_WithDefaultRole_ShouldSetFreelancerRole()
	{
		// Arrange
		var name = "João Silva";
		var email = "joao@example.com";
		var password = "123456";

		// Act
		var user = new User(name, email, password);

		// Assert
		Assert.Equal("freelancer", user.Role);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void Constructor_WithInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
	{
		// Arrange
		var name = "João Silva";
		var password = "123456";

		// Act & Assert
		var exception = Assert.Throws<ArgumentException>(() => new User(name, invalidEmail, password));
		Assert.Equal("Email é obrigatório (Parameter 'email')", exception.Message);
	}

	[Theory]
	[InlineData("invalid-email")]
	[InlineData("@example.com")]
	[InlineData("user@")]
	[InlineData("user.example.com")]
	[InlineData("user@.com")]
	[InlineData("user@example.")]
	public void Constructor_WithInvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
	{
		// Arrange
		var name = "João Silva";
		var password = "123456";

		// Act & Assert
		var exception = Assert.Throws<ArgumentException>(() => new User(name, invalidEmail, password));
		Assert.Equal("Email deve ter um formato válido (Parameter 'email')", exception.Message);
	}

	[Theory]
	[InlineData("user@example.com")]
	[InlineData("test.email@domain.co.uk")]
	[InlineData("user123@test-domain.org")]
	[InlineData("user+tag@example.com")]
	[InlineData("user_name@example.com")]
	public void Constructor_WithValidEmailFormat_ShouldCreateUser(string validEmail)
	{
		// Arrange
		var name = "João Silva";
		var password = "123456";

		// Act
		var user = new User(name, validEmail, password);

		// Assert
		Assert.Equal(validEmail, user.Email);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void Constructor_WithInvalidName_ShouldThrowArgumentNullException(string invalidName)
	{
		// Arrange
		var email = "joao@example.com";
		var password = "123456";

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new User(invalidName, email, password));
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void Constructor_WithInvalidPassword_ShouldThrowArgumentNullException(string invalidPassword)
	{
		// Arrange
		var name = "João Silva";
		var email = "joao@example.com";

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new User(name, email, invalidPassword));
	}

	[Fact]
	public void UpdatePassword_WithValidPassword_ShouldUpdatePassword()
	{
		// Arrange
		var user = new User("João Silva", "joao@example.com", "123456");
		var newPassword = "newPassword123";

		// Act
		user.UpdatePassword(newPassword);

		// Assert
		Assert.Equal(newPassword, user.Password);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void UpdatePassword_WithInvalidPassword_ShouldThrowArgumentException(string invalidPassword)
	{
		// Arrange
		var user = new User("João Silva", "joao@example.com", "123456");

		// Act & Assert
		var exception = Assert.Throws<ArgumentException>(() => user.UpdatePassword(invalidPassword));
		Assert.Equal("Password é obrigatório (Parameter 'newPassword')", exception.Message);
	}

	[Fact]
	public void UpdateName_WithValidName_ShouldUpdateName()
	{
		// Arrange
		var user = new User("João Silva", "joao@example.com", "123456");
		var newName = "João Santos";

		// Act
		user.UpdateName(newName);

		// Assert
		Assert.Equal(newName, user.Name);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void UpdateName_WithInvalidName_ShouldThrowArgumentException(string invalidName)
	{
		// Arrange
		var user = new User("João Silva", "joao@example.com", "123456");

		// Act & Assert
		var exception = Assert.Throws<ArgumentException>(() => user.UpdateName(invalidName));
		Assert.Equal("Nome é obrigatório (Parameter 'newName')", exception.Message);
	}

	[Fact]
	public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
	{
		// Arrange
		var user = new User("João Silva", "joao@example.com", "123456");
		var newEmail = "joao.santos@example.com";

		// Act
		user.UpdateEmail(newEmail);

		// Assert
		Assert.Equal(newEmail, user.Email);
	}

	[Theory]
	[InlineData("invalid-email")]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void UpdateEmail_WithInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
	{
		// Arrange
		var user = new User("João Silva", "joao@example.com", "123456");

		// Act & Assert
		Assert.Throws<ArgumentException>(() => user.UpdateEmail(invalidEmail));
	}
}
