using Kickoffa.API.Application.Factories;
using Kickoffa.API.Application.Services;
using Kickoffa.API.Application.Services.AppUser;
using Microsoft.Extensions.DependencyInjection;

namespace Kickoffa.API.AspNet.Infrastructure.Extensions.ServiceCollection;

/// <summary>
/// Extensões para configuração dos serviços da Application
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona todos os serviços da camada Application
    /// </summary>
    /// <param name="services">Collection de serviços</param>
    /// <returns>IServiceCollection para chaining</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Registrar factories
        services.AddScoped<ICreateCustomerFactory, CreateCustomerFactory>();

        // Registrar services
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }

    /// <summary>
    /// Adiciona apenas os factories da Application
    /// </summary>
    /// <param name="services">Collection de serviços</param>
    /// <returns>IServiceCollection para chaining</returns>
    public static IServiceCollection AddApplicationFactories(this IServiceCollection services)
    {
        services.AddScoped<ICreateCustomerFactory, CreateCustomerFactory>();
        return services;
    }

    /// <summary>
    /// Adiciona apenas os services da Application
    /// </summary>
    /// <param name="services">Collection de serviços</param>
    /// <returns>IServiceCollection para chaining</returns>
    public static IServiceCollection AddApplicationBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
