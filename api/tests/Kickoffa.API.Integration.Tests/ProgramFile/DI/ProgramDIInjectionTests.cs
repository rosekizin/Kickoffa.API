using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.AspNet.Infrastructure.Configuration.Data;
using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Kickoffa.API.AspNet.Infrastructure.ExceptionHandling;
using Kickoffa.API.Data.EntityFramework.Context;
using Kickoffa.API.Domain.Models.AppUser;
using Kickoffa.API.Domain.Repositories;
using Kickoffa.API.Domain.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using Xunit;

namespace Kickoffa.API.Integration.Tests.ProgramFile.DI;

/// <summary>
/// Testes para verificar se o Program.cs está configurando corretamente a Dependency Injection
/// (linhas 60-76: Database, Repositories, Identity, Services, Handlers)
/// </summary>
public class ProgramDIInjectionTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramDIInjectionTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Test"); // <- define o ambiente "Test"
        });
    }

    #region Database Configuration Tests (Line 61)

    [Fact]
    public void Program_ShouldRegister_KickoffaDbContext()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<KickoffaDbContext>();

        // Assert
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void Program_ShouldConfigureDatabaseWithCorrectLifetime()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var dbContext1 = scope.ServiceProvider.GetService<KickoffaDbContext>();
        var dbContext2 = scope.ServiceProvider.GetService<KickoffaDbContext>();

        // Assert
        Assert.NotNull(dbContext1);
        Assert.NotNull(dbContext2);
        // DbContext deve ser Scoped, então deve ser a mesma instância no mesmo scope
        Assert.Same(dbContext1, dbContext2);
    }

    #endregion

    #region Repositories Tests (Line 62)

    [Fact]
    public void Program_ShouldRegisterCustomerRepository()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var customerRepository = scope.ServiceProvider.GetService<ICustomerRepository>();

        // Assert
        Assert.NotNull(customerRepository);
    }

    [Fact]
    public void Program_ShouldRegisterChecklistRepository()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var checklistRepository = scope.ServiceProvider.GetService<IChecklistRepository>();

        // Assert
        Assert.NotNull(checklistRepository);
    }

    [Fact]
    public void Program_ShouldRegisterFileTypeRepository()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var fileTypeRepository = scope.ServiceProvider.GetService<IFileTypeRepository>();

        // Assert
        Assert.NotNull(fileTypeRepository);
    }

    #endregion

    #region Identity Configuration Tests (Line 67)

    [Fact]
    public void Program_ShouldRegisterUserManager()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetService<UserManager<User>>();

        // Assert
        Assert.NotNull(userManager);
    }

    [Fact]
    public void Program_ShouldRegisterSignInManager()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var signInManager = scope.ServiceProvider.GetService<SignInManager<User>>();

        // Assert
        Assert.NotNull(signInManager);
    }

    [Fact]
    public void Program_ShouldRegisterUserManagerWrapper()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var userManagerWrapper = scope.ServiceProvider.GetService<IUserManagerWrapper>();

        // Assert
        Assert.NotNull(userManagerWrapper);
    }

    [Fact]
    public void Program_ShouldRegisterSignInManagerWrapper()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var signInManagerWrapper = scope.ServiceProvider.GetService<ISignInManagerWrapper>();

        // Assert
        Assert.NotNull(signInManagerWrapper);
    }

    #endregion

    #region Application Services Tests (Line 70)

    [Fact]
    public void Program_ShouldRegisterUserService()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var userService = scope.ServiceProvider.GetService<IUserService>();

        // Assert
        Assert.NotNull(userService);
    }

    [Fact]
    public void Program_ShouldRegisterCustomerService()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var customerService = scope.ServiceProvider.GetService<ICustomerService>();

        // Assert
        Assert.NotNull(customerService);
    }

    [Fact]
    public void Program_ShouldRegisterEmailService()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var emailService = scope.ServiceProvider.GetService<IEmailService>();

        // Assert
        Assert.NotNull(emailService);
    }

    [Fact]
    public void Program_ShouldRegisterFileTypeService()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var fileTypeService = scope.ServiceProvider.GetService<IFileTypeService>();

        // Assert
        Assert.NotNull(fileTypeService);
    }

    [Fact]
    public void Program_ShouldRegisterCurrentUserService()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var currentUserService = scope.ServiceProvider.GetService<ICurrentUserService>();

        // Assert
        Assert.NotNull(currentUserService);
    }

    [Fact]
    public void Program_ShouldRegisterApplicationServicesWithScopedLifetime()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var userService1 = scope.ServiceProvider.GetService<IUserService>();
        var userService2 = scope.ServiceProvider.GetService<IUserService>();

        // Assert
        Assert.NotNull(userService1);
        Assert.NotNull(userService2);
        // Application services devem ser Scoped
        Assert.Same(userService1, userService2);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Program_ShouldHaveConsistentDependencyGraph()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        
        // Tentar resolver uma cadeia de dependências complexa
        var userService = scope.ServiceProvider.GetService<IUserService>();
        var customerService = scope.ServiceProvider.GetService<ICustomerService>();
        var customerRepository = scope.ServiceProvider.GetService<ICustomerRepository>();

        // Assert
        Assert.NotNull(userService);
        Assert.NotNull(customerService);
        Assert.NotNull(customerRepository);
    }

    [Fact]
    public void Program_ShouldResolveServicesWithoutCircularDependencies()
    {
        // Act & Assert - Não deve lançar exceção
        using var scope = _factory.Services.CreateScope();
        
        // Tentar resolver todos os serviços principais
        var services = new[]
        {
            typeof(IUserService),
            typeof(ICustomerService),
            typeof(IEmailService),
            typeof(IFileTypeService),
            typeof(ICurrentUserService),
            typeof(ICustomerRepository),
            typeof(IChecklistRepository),
            typeof(IFileTypeRepository)
        };

        foreach (var serviceType in services)
        {
            var service = scope.ServiceProvider.GetService(serviceType);
            Assert.NotNull(service);
        }
    }

    [Fact]
    public void Program_ShouldRegisterAllRequiredDependencies()
    {
        // Act
        using var scope = _factory.Services.CreateScope();

        // Assert - Verificar se todas as dependências principais estão registradas
        var requiredServices = new[]
        {
            // Database
            typeof(KickoffaDbContext),
            
            // Repositories
            typeof(ICustomerRepository),
            typeof(IChecklistRepository),
            typeof(IFileTypeRepository),
            
            // Identity
            typeof(UserManager<User>),
            typeof(SignInManager<User>),
            typeof(IUserManagerWrapper),
            typeof(ISignInManagerWrapper),
            
            // Application Services
            typeof(IUserService),
            typeof(ICustomerService),
            typeof(IEmailService),
            typeof(IFileTypeService),
            typeof(ICurrentUserService),
            
            // Error Handlers
            typeof(IErrorFactory),
            typeof(IActionResultErrorHandler)
        };

        foreach (var serviceType in requiredServices)
        {
            var service = scope.ServiceProvider.GetService(serviceType);
            Assert.NotNull(service);
        }
    }

    [Fact]
    public async Task GlobalExceptionHandler_ShouldHandleException()
    {
        // Arrange
        using var scope1 = _factory.Services.CreateScope();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/crash/fail", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        // Verifique que há JSON com os campos esperados
        Assert.False(string.IsNullOrWhiteSpace(content));

        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

        Assert.NotNull(problemDetails);
        Assert.Equal(500, problemDetails.Status);
        Assert.Equal("Internal Server Error", problemDetails.Title);
        Assert.Equal("INTERNAL_SERVER_ERROR", problemDetails.Extensions["code"]);
        Assert.Equal("Ocorreu um erro inesperado. Contate o suporte.", problemDetails.Detail);
    }

    [Fact]
    public void Program_ShouldHaveCorrectServiceLifetimes()
    {
        // Act
        using var scope1 = _factory.Services.CreateScope();
        using var scope2 = _factory.Services.CreateScope();
        
        // Scoped services - devem ser iguais dentro do mesmo scope, diferentes entre scopes
        var userService1Scope1 = scope1.ServiceProvider.GetService<IUserService>();
        var userService2Scope1 = scope1.ServiceProvider.GetService<IUserService>();
        var userServiceScope2 = scope2.ServiceProvider.GetService<IUserService>();

        // Assert
        Assert.NotNull(userService1Scope1);
        Assert.NotNull(userService2Scope1);
        Assert.NotNull(userServiceScope2);
        
        // Mesmo scope = mesma instância
        Assert.Same(userService1Scope1, userService2Scope1);
        
        // Scopes diferentes = instâncias diferentes
        Assert.NotSame(userService1Scope1, userServiceScope2);
    }

    #endregion
}