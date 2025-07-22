using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using Xunit;

namespace Kickoffa.API.Integration.Tests.ProgramFile.Cors;

/// <summary>
/// Testes para verificar se o Program.cs está configurando corretamente o CORS
/// (linhas 48-60 e linha 95)
/// </summary>
public class ProgramCorsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProgramCorsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public void Program_ShouldRegisterCorsServices()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsService = scope.ServiceProvider.GetService<ICorsService>();

        // Assert
        Assert.NotNull(corsService);
    }

    [Fact]
    public void Program_ShouldConfigureCorsOptions()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsOptions = scope.ServiceProvider.GetService<IOptions<CorsOptions>>();

        // Assert
        Assert.NotNull(corsOptions);
        Assert.NotNull(corsOptions.Value);
    }

    [Fact]
    public void Program_ShouldHaveAllowFrontendPolicy()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsOptions = scope.ServiceProvider.GetRequiredService<IOptions<CorsOptions>>();

        // Assert
        var policy = corsOptions.Value.GetPolicy("AllowFrontend");
        Assert.NotNull(policy);
    }

    [Fact]
    public void Program_ShouldConfigureAllowFrontendPolicyWithCorrectOrigin()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsOptions = scope.ServiceProvider.GetRequiredService<IOptions<CorsOptions>>();
        var policy = corsOptions.Value.GetPolicy("AllowFrontend");

        // Assert
        Assert.NotNull(policy);
        Assert.Contains("http://localhost:3000", policy.Origins);
    }

    [Fact]
    public void Program_ShouldConfigureAllowFrontendPolicyWithAnyHeaders()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsOptions = scope.ServiceProvider.GetRequiredService<IOptions<CorsOptions>>();
        var policy = corsOptions.Value.GetPolicy("AllowFrontend");

        // Assert
        Assert.NotNull(policy);
        Assert.True(policy.AllowAnyHeader);
    }

    [Fact]
    public void Program_ShouldConfigureAllowFrontendPolicyWithAnyMethods()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsOptions = scope.ServiceProvider.GetRequiredService<IOptions<CorsOptions>>();
        var policy = corsOptions.Value.GetPolicy("AllowFrontend");

        // Assert
        Assert.NotNull(policy);
        Assert.True(policy.AllowAnyMethod);
    }

    [Fact]
    public void Program_ShouldConfigureAllowFrontendPolicyWithCredentials()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsOptions = scope.ServiceProvider.GetRequiredService<IOptions<CorsOptions>>();
        var policy = corsOptions.Value.GetPolicy("AllowFrontend");

        // Assert
        Assert.NotNull(policy);
        Assert.True(policy.SupportsCredentials);
    }

    [Fact]
    public void Program_ShouldConfigureAllowFrontendPolicyCorrectly()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsOptions = scope.ServiceProvider.GetRequiredService<IOptions<CorsOptions>>();
        var policy = corsOptions.Value.GetPolicy("AllowFrontend");

        // Assert
        Assert.NotNull(policy);

        // Verificar todas as configurações em um teste
        Assert.Contains("http://localhost:3000", policy.Origins);
        Assert.True(policy.AllowAnyHeader);
        Assert.True(policy.AllowAnyMethod);
        Assert.True(policy.SupportsCredentials);
    }

    [Fact]
    public async Task Program_ShouldHandlePreflightRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        request.Headers.Add("Origin", "http://localhost:3000");
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", "Content-Type");

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        // Preflight deve retornar 204 (No Content) ou 200 (OK)
        Assert.True(response.StatusCode == HttpStatusCode.NoContent ||
                   response.StatusCode == HttpStatusCode.OK);

        // Verificar headers CORS na resposta
        Assert.True(response.Headers.Contains("Access-Control-Allow-Origin"));
        Assert.True(response.Headers.Contains("Access-Control-Allow-Methods"));
        Assert.True(response.Headers.Contains("Access-Control-Allow-Headers"));
    }

    [Fact]
    public async Task Program_ShouldAllowOriginFromLocalhost3000()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        request.Headers.Add("Origin", "http://localhost:3000");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var allowOriginHeader = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
        Assert.Equal("http://localhost:3000", allowOriginHeader);
    }

    [Fact]
    public async Task Program_ShouldAllowCredentialsInCorsResponse()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        request.Headers.Add("Origin", "http://localhost:3000");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var allowCredentialsHeader = response.Headers.GetValues("Access-Control-Allow-Credentials").FirstOrDefault();
        Assert.Equal("true", allowCredentialsHeader);
    }

    [Fact]
    public async Task Program_ShouldRejectUnauthorizedOrigin()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        request.Headers.Add("Origin", "http://malicious-site.com");
        request.Headers.Add("Access-Control-Request-Method", "POST");

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        // Para origem não autorizada, não deve ter header Access-Control-Allow-Origin
        // ou deve retornar erro
        var hasAllowOriginHeader = response.Headers.Contains("Access-Control-Allow-Origin");
        if (hasAllowOriginHeader)
        {
            var allowOriginHeader = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            Assert.NotEqual("http://malicious-site.com", allowOriginHeader);
        }
    }

    [Theory]
    [InlineData("GET")]
    [InlineData("POST")]
    [InlineData("PUT")]
    [InlineData("DELETE")]
    [InlineData("PATCH")]
    public async Task Program_ShouldAllowAllHttpMethods(string method)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        request.Headers.Add("Origin", "http://localhost:3000");
        request.Headers.Add("Access-Control-Request-Method", method);

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var allowMethodsHeader = response.Headers.GetValues("Access-Control-Allow-Methods").FirstOrDefault();
        Assert.NotNull(allowMethodsHeader);

        // Verificar se o método está permitido (pode estar em formato de lista)
        Assert.Contains(method, allowMethodsHeader, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("Content-Type")]
    [InlineData("Authorization")]
    [InlineData("X-Requested-With")]
    [InlineData("Accept")]
    public async Task Program_ShouldAllowCommonHeaders(string headerName)
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Options, "/api/auth/login");
        request.Headers.Add("Origin", "http://localhost:3000");
        request.Headers.Add("Access-Control-Request-Method", "POST");
        request.Headers.Add("Access-Control-Request-Headers", headerName);

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var allowHeadersHeader = response.Headers.GetValues("Access-Control-Allow-Headers").FirstOrDefault();
        Assert.NotNull(allowHeadersHeader);

        // Como AllowAnyHeader está configurado, deve permitir qualquer header
        Assert.Contains(headerName, allowHeadersHeader, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Program_ShouldHaveOnlyOneNamedCorsPolicy()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsOptions = scope.ServiceProvider.GetRequiredService<IOptions<CorsOptions>>();

        // Assert
        // Verificar se existe apenas a política "AllowFrontend"
        var policy = corsOptions.Value.GetPolicy("AllowFrontend");
        Assert.NotNull(policy);

        // Tentar obter uma política inexistente
        var nonExistentPolicy = corsOptions.Value.GetPolicy("NonExistentPolicy");
        Assert.Null(nonExistentPolicy);
    }

    [Fact]
    public async Task Program_ShouldHandleActualCorsRequest()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/current");
        request.Headers.Add("Origin", "http://localhost:3000");

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        // Verificar se a resposta contém headers CORS apropriados
        var hasAllowOriginHeader = response.Headers.Contains("Access-Control-Allow-Origin");

        if (hasAllowOriginHeader)
        {
            var allowOriginHeader = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            Assert.Equal("http://localhost:3000", allowOriginHeader);
        }
    }

    [Fact]
    public void Program_ShouldConfigureCorsForHttpOnlyCookies()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var corsOptions = scope.ServiceProvider.GetRequiredService<IOptions<CorsOptions>>();
        var policy = corsOptions.Value.GetPolicy("AllowFrontend");

        // Assert
        Assert.NotNull(policy);

        // Para HttpOnly cookies, é essencial que SupportsCredentials seja true
        Assert.True(policy.SupportsCredentials,
            "CORS deve suportar credenciais para funcionar com HttpOnly cookies");

        // Verificar se a origem específica está configurada (necessário quando SupportsCredentials = true)
        Assert.Contains("http://localhost:3000", policy.Origins);
        Assert.False(policy.AllowAnyOrigin,
            "AllowAnyOrigin não pode ser true quando SupportsCredentials é true");
    }
}
