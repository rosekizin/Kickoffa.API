using Kickoffa.API.Contracts.Authentication;

namespace Kickoffa.API.Contracts.UnitTests.Authentication;

public class LoginResponseTests
{
    [Fact]
    public void LoginResponse_WithRequiredProperties_ShouldCreateSuccessfully()
    {
        // Arrange
        var userId = "123";
        var email = "test@example.com";

        // Act
        var response = new LoginResponse
        {
            UserId = userId,
            Email = email
        };

        // Assert
        Assert.Equal(userId, response.UserId);
        Assert.Equal(email, response.Email);
        Assert.True(response.Success);
        Assert.Equal("Login realizado com sucesso", response.Message);
        Assert.True(response.LoginAt <= DateTime.UtcNow);
        Assert.True(response.LoginAt > DateTime.UtcNow.AddMinutes(-1));
        Assert.Null(response.Session);
    }

    [Fact]
    public void LoginResponse_WithCustomProperties_ShouldSetCorrectly()
    {
        // Arrange
        var userId = "456";
        var email = "user@domain.com";
        var success = false;
        var message = "Custom message";
        var loginAt = DateTime.UtcNow.AddMinutes(-5);

        // Act
        var response = new LoginResponse
        {
            UserId = userId,
            Email = email,
            Success = success,
            Message = message,
            LoginAt = loginAt
        };

        // Assert
        Assert.Equal(userId, response.UserId);
        Assert.Equal(email, response.Email);
        Assert.Equal(success, response.Success);
        Assert.Equal(message, response.Message);
        Assert.Equal(loginAt, response.LoginAt);
    }

    [Fact]
    public void LoginResponse_WithSessionInfo_ShouldSetCorrectly()
    {
        // Arrange
        var sessionInfo = new SessionInfo
        {
            ExpiresInSeconds = 7200,
            ExpiresAt = DateTime.UtcNow.AddHours(2),
            IsPersistent = true
        };

        // Act
        var response = new LoginResponse
        {
            UserId = "123",
            Email = "test@example.com",
            Session = sessionInfo
        };

        // Assert
        Assert.NotNull(response.Session);
        Assert.Equal(sessionInfo.ExpiresInSeconds, response.Session.ExpiresInSeconds);
        Assert.Equal(sessionInfo.ExpiresAt, response.Session.ExpiresAt);
        Assert.Equal(sessionInfo.IsPersistent, response.Session.IsPersistent);
    }

    [Fact]
    public void LoginResponse_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var response = new LoginResponse
        {
            UserId = "123",
            Email = "test@example.com"
        };

        // Assert
        Assert.True(response.Success);
        Assert.Equal("Login realizado com sucesso", response.Message);
        Assert.True(response.LoginAt <= DateTime.UtcNow);
        Assert.True(response.LoginAt > DateTime.UtcNow.AddMinutes(-1));
        Assert.Null(response.Session);
    }

    [Fact]
    public void LoginResponse_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var loginAt = DateTime.UtcNow;
        var sessionInfo = new SessionInfo
        {
            ExpiresInSeconds = 3600,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsPersistent = false
        };

        var response1 = new LoginResponse
        {
            UserId = "123",
            Email = "test@example.com",
            Success = true,
            Message = "Success",
            LoginAt = loginAt,
            Session = sessionInfo
        };

        var response2 = new LoginResponse
        {
            UserId = "123",
            Email = "test@example.com",
            Success = true,
            Message = "Success",
            LoginAt = loginAt,
            Session = sessionInfo
        };

        // Act & Assert
        Assert.Equal(response1, response2);
        Assert.True(response1 == response2);
        Assert.False(response1 != response2);
    }

    [Fact]
    public void LoginResponse_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var response1 = new LoginResponse
        {
            UserId = "123",
            Email = "test@example.com"
        };

        var response2 = new LoginResponse
        {
            UserId = "456",
            Email = "test@example.com"
        };

        // Act & Assert
        Assert.NotEqual(response1, response2);
        Assert.False(response1 == response2);
        Assert.True(response1 != response2);
    }
}

public class SessionInfoTests
{
    [Fact]
    public void SessionInfo_WithDefaultValues_ShouldSetCorrectly()
    {
        // Act
        var sessionInfo = new SessionInfo();

        // Assert
        Assert.Equal(3600, sessionInfo.ExpiresInSeconds);
        Assert.True(sessionInfo.ExpiresAt <= DateTime.UtcNow.AddHours(1).AddMinutes(1));
        Assert.True(sessionInfo.ExpiresAt > DateTime.UtcNow.AddHours(1).AddMinutes(-1));
        Assert.False(sessionInfo.IsPersistent);
    }

    [Fact]
    public void SessionInfo_WithCustomValues_ShouldSetCorrectly()
    {
        // Arrange
        var expiresInSeconds = 7200;
        var expiresAt = DateTime.UtcNow.AddHours(2);
        var isPersistent = true;

        // Act
        var sessionInfo = new SessionInfo
        {
            ExpiresInSeconds = expiresInSeconds,
            ExpiresAt = expiresAt,
            IsPersistent = isPersistent
        };

        // Assert
        Assert.Equal(expiresInSeconds, sessionInfo.ExpiresInSeconds);
        Assert.Equal(expiresAt, sessionInfo.ExpiresAt);
        Assert.Equal(isPersistent, sessionInfo.IsPersistent);
    }

    [Fact]
    public void SessionInfo_AsRecord_ShouldSupportEquality()
    {
        // Arrange
        var expiresAt = DateTime.UtcNow.AddHours(1);
        
        var session1 = new SessionInfo
        {
            ExpiresInSeconds = 3600,
            ExpiresAt = expiresAt,
            IsPersistent = true
        };

        var session2 = new SessionInfo
        {
            ExpiresInSeconds = 3600,
            ExpiresAt = expiresAt,
            IsPersistent = true
        };

        // Act & Assert
        Assert.Equal(session1, session2);
        Assert.True(session1 == session2);
        Assert.False(session1 != session2);
    }

    [Fact]
    public void SessionInfo_DifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var session1 = new SessionInfo
        {
            ExpiresInSeconds = 3600,
            IsPersistent = false
        };

        var session2 = new SessionInfo
        {
            ExpiresInSeconds = 7200,
            IsPersistent = false
        };

        // Act & Assert
        Assert.NotEqual(session1, session2);
        Assert.False(session1 == session2);
        Assert.True(session1 != session2);
    }

    [Theory]
    [InlineData(1800, false)]
    [InlineData(3600, true)]
    [InlineData(7200, false)]
    [InlineData(86400, true)]
    public void SessionInfo_WithVariousValues_ShouldSetCorrectly(int expiresInSeconds, bool isPersistent)
    {
        // Arrange
        var expiresAt = DateTime.UtcNow.AddSeconds(expiresInSeconds);

        // Act
        var sessionInfo = new SessionInfo
        {
            ExpiresInSeconds = expiresInSeconds,
            ExpiresAt = expiresAt,
            IsPersistent = isPersistent
        };

        // Assert
        Assert.Equal(expiresInSeconds, sessionInfo.ExpiresInSeconds);
        Assert.Equal(expiresAt, sessionInfo.ExpiresAt);
        Assert.Equal(isPersistent, sessionInfo.IsPersistent);
    }
}
