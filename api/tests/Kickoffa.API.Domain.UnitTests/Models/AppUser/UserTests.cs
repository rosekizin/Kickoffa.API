using Kickoffa.API.Domain.Models.AppUser;

namespace Kickoffa.API.Domain.UnitTests.Models.AppUser;

public class UserTests
{
	[Fact]
	public void Constructor_WithValidData_ShouldCreateUser()
	{
		// Arrange
		var email = "joao@example.com";

		// Act
		var user = new User(email);

		// Assert
		Assert.Equal(email, user.Email);
		Assert.Equal(email, user.UserName); // IdentityUser usa UserName
		Assert.Equal(email.ToUpperInvariant(), user.NormalizedEmail);
		Assert.Equal(email.ToUpperInvariant(), user.NormalizedUserName);
		Assert.True(user.CreatedDateUtc <= DateTime.UtcNow);
		Assert.True(user.LastUpdatedDateUtc <= DateTime.UtcNow);
	}

	[Fact]
	public void Constructor_Default_ShouldSetDates()
	{
		// Act
		var user = new User();

		// Assert
		Assert.True(user.CreatedDateUtc <= DateTime.UtcNow);
		Assert.True(user.LastUpdatedDateUtc <= DateTime.UtcNow);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void Constructor_WithInvalidEmail_ShouldThrowArgumentNullException(string invalidEmail)
	{
		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => new User(invalidEmail));
	}

	[Theory]
	[InlineData("user@example.com")]
	[InlineData("test.email@domain.co.uk")]
	[InlineData("user123@test-domain.org")]
	[InlineData("user+tag@example.com")]
	[InlineData("user_name@example.com")]
	public void Constructor_WithValidEmailFormat_ShouldCreateUser(string validEmail)
	{
		// Act
		var user = new User(validEmail);

		// Assert
		Assert.Equal(validEmail, user.Email);
	}

	[Fact]
	public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
	{
		// Arrange
		var user = new User("joao@example.com");
		var newEmail = "joao.silva@example.com";

		// Act
		user.UpdateEmail(newEmail);

		// Assert
		Assert.Equal(newEmail, user.Email);
		Assert.Equal(newEmail, user.UserName);
		Assert.Equal(newEmail.ToUpperInvariant(), user.NormalizedEmail);
		Assert.Equal(newEmail.ToUpperInvariant(), user.NormalizedUserName);
		Assert.True(user.LastUpdatedDateUtc > user.CreatedDateUtc);
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	[InlineData(null)]
	public void UpdateEmail_WithInvalidEmail_ShouldThrowArgumentException(string invalidEmail)
	{
		// Arrange
		var user = new User("joao@example.com");

		// Act & Assert
		Assert.Throws<ArgumentException>(() => user.UpdateEmail(invalidEmail));
	}
}
