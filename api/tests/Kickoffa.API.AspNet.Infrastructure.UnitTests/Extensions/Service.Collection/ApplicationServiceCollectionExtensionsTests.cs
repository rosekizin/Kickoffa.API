using Kickoffa.API.Application.Interfaces;
using Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection;
using Kickoffa.API.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.Extensions.Service.Collection;

public class ApplicationServiceCollectionExtensionsTests
{
    [Fact]
    public void AddApplicationServices_ShouldRegisterServicesWithCorrectLifetime()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices();

        // Assert
        var userServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserService));
        var customerServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ICustomerService));
        var emailServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IEmailService));
        var fileTypeServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IFileTypeService));
        var currentUserServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ICurrentUserService));
        var userManagerWrapperServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IUserManagerWrapper));
        var signInManagerWrapperServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ISignInManagerWrapper));
        var createCustomerFactoryServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(ICreateCustomerFactory));
        var httpContextAccessorDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IHttpContextAccessor));

        Assert.NotNull(userServiceDescriptor);
        Assert.NotNull(customerServiceDescriptor);
        Assert.NotNull(emailServiceDescriptor);
        Assert.NotNull(fileTypeServiceDescriptor);
        Assert.NotNull(currentUserServiceDescriptor);
        Assert.NotNull(userManagerWrapperServiceDescriptor);
        Assert.NotNull(signInManagerWrapperServiceDescriptor);
        Assert.NotNull(createCustomerFactoryServiceDescriptor);
        Assert.NotNull(httpContextAccessorDescriptor);

        // Verificar se são registrados como Scoped
        Assert.Equal(ServiceLifetime.Scoped, userServiceDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, customerServiceDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, emailServiceDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, fileTypeServiceDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, currentUserServiceDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, userManagerWrapperServiceDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, signInManagerWrapperServiceDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, createCustomerFactoryServiceDescriptor.Lifetime);
        Assert.Equal(ServiceLifetime.Singleton, httpContextAccessorDescriptor.Lifetime);
    }

    [Fact]
    public void AddIdentityConfiguration_ShouldConfigureIdentityServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddIdentityConfiguration();

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verificar se Identity foi configurado (não podemos testar completamente sem DbContext)
        var identityServices = services.Where(s => s.ServiceType.Name.Contains("Identity")).ToList();
        Assert.NotEmpty(identityServices);
    }
}