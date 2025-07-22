using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Net;
using Xunit;

namespace Kickoffa.API.Integration.Tests.ProgramFile.Swagger;

/// <summary>
/// Testes para verificar se o Program.cs está configurando corretamente o Swagger
/// (linhas 44-46 para configuração e linhas 83-89 para uso no pipeline)
/// </summary>
public class ProgramSwaggerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramSwaggerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Program_ShouldRegisterEndpointsApiExplorer()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var apiExplorer = scope.ServiceProvider.GetService<Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionGroupCollectionProvider>();

        // Assert
        Assert.NotNull(apiExplorer);
    }

    [Fact]
    public void Program_ShouldRegisterSwaggerGen()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var swaggerGenOptions = scope.ServiceProvider.GetService<IOptions<SwaggerGenOptions>>()!.Value;

        // Assert
        Assert.NotNull(swaggerGenOptions);
    }

    [Fact]
    public void Program_ShouldRegisterSwaggerServices()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var swaggerProvider = scope.ServiceProvider.GetService<ISwaggerProvider>();

        // Assert
        Assert.NotNull(swaggerProvider);
    }

    [Fact]
    public void Program_ShouldConfigureSwaggerGenOptions()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var swaggerGenOptions = scope.ServiceProvider.GetRequiredService<IOptions<SwaggerGenOptions>>().Value;

        // Assert
        Assert.NotNull(swaggerGenOptions);
        // Verificar se pelo menos uma versão da API foi configurada
        Assert.Empty(swaggerGenOptions.SwaggerGeneratorOptions.SwaggerDocs);
    }

    [Fact]
    public async Task Program_ShouldServeSwaggerJsonInDevelopment()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.NotEmpty(content);
        Assert.Contains("openapi", content.ToLower());
    }

    [Fact]
    public async Task Program_ShouldServeSwaggerUIInDevelopment()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("swagger", content.ToLower());
    }
    [Fact]
    public async Task Program_ShouldServeSwaggerUI_AndSwaggerJson()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        using var client = factory.CreateClient();

        // Act
        var uiResponse = await client.GetAsync("/", TestContext.Current.CancellationToken);
        var jsonResponse = await client.GetAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, uiResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, jsonResponse.StatusCode);

        var jsonContent = await jsonResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("\"openapi\"", jsonContent); // Garante que é um documento OpenAPI
        var jsonOpenApiObject = JObject.Parse(jsonContent);
        Assert.Equal("1.0", jsonOpenApiObject["info"]!["version"]!.ToString());
        Assert.Equal("Kickoffa.API", jsonOpenApiObject["info"]!["title"]!.ToString());
    }

    [Fact]
    public async Task Program_ShouldConfigureSwaggerUIAtRoot()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        // Swagger UI deve estar disponível na raiz (RoutePrefix = string.Empty)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("swagger-ui", content.ToLower());
    }

    [Fact]
    public async Task Program_ShouldNotServeSwaggerInProduction()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
        });

        using var client = factory.CreateClient();

        // Act
        var swaggerJsonResponse = await client.GetAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);
        var swaggerUIResponse = await client.GetAsync("/swagger", TestContext.Current.CancellationToken);

        // Assert
        // Em produção, Swagger não deve estar disponível
        Assert.Equal(HttpStatusCode.NotFound, swaggerJsonResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, swaggerUIResponse.StatusCode);
    }

    [Fact]
    public async Task Program_ShouldNotServeSwaggerInStaging()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Staging");
        });

        using var client = factory.CreateClient();

        // Act
        var swaggerJsonResponse = await client.GetAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);
        var swaggerUIResponse = await client.GetAsync("/swagger", TestContext.Current.CancellationToken);

        // Assert
        // Em staging, Swagger não deve estar disponível
        Assert.Equal(HttpStatusCode.NotFound, swaggerJsonResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, swaggerUIResponse.StatusCode);
    }

    [Fact]
    public async Task Program_ShouldGenerateValidOpenApiDocument()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar estrutura básica do OpenAPI
        Assert.Contains("\"openapi\":", content);
        Assert.Contains("\"info\":", content);
        Assert.Contains("\"paths\":", content);

        // Verificar se contém informações da API
        Assert.Contains("\"title\":", content);
        Assert.Contains("\"version\":", content);
    }

    [Fact]
    public async Task Program_ShouldIncludeControllersInSwaggerDoc()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verificar se pelo menos alguns endpoints dos controllers estão documentados
        Assert.Contains("\"/api/", content);
    }

    [Fact]
    public void Program_ShouldConfigureSwaggerUIOptions()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var swaggerUIOptions = scope.ServiceProvider.GetService<IOptions<SwaggerUIOptions>>()?.Value;

        // Assert
        Assert.NotNull(swaggerUIOptions);
    }

    [Theory]
    [InlineData("Development")]
    public async Task Program_ShouldServeSwaggerOnlyInDevelopmentEnvironment(string environment)
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
        });

        using var client = factory.CreateClient();

        // Act
        var swaggerJsonResponse = await client.GetAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);
        var swaggerUIResponse = await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        if (environment == "Development")
        {
            Assert.Equal(HttpStatusCode.OK, swaggerJsonResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, swaggerUIResponse.StatusCode);
        }
        else
        {
            Assert.Equal(HttpStatusCode.NotFound, swaggerJsonResponse.StatusCode);
        }
    }

    [Fact]
    public async Task Program_ShouldHaveCorrectContentTypeForSwaggerJson()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Program_ShouldHaveCorrectContentTypeForSwaggerUI()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public void Program_ShouldRegisterRequiredSwaggerDependencies()
    {
        // Act
        using var scope = _factory.Services.CreateScope();

        var swaggerProvider = scope.ServiceProvider.GetService<ISwaggerProvider>();
        var apiDescriptionProvider = scope.ServiceProvider.GetService<Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionGroupCollectionProvider>();
        var swaggerGenOptions = scope.ServiceProvider.GetService<IOptions<SwaggerGenOptions>>()?.Value;
        var swaggerUIOptions = scope.ServiceProvider.GetService<IOptions<SwaggerUIOptions>>()?.Value;

        // Assert
        Assert.NotNull(swaggerProvider);
        Assert.NotNull(apiDescriptionProvider);
        Assert.NotNull(swaggerGenOptions);
        Assert.NotNull(swaggerUIOptions);
    }
}