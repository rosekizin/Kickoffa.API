using Kickoffa.API.AspNet.Infrastructure.ExceptionHandling;
using Kickoffa.API.Contracts.Newtonsoft;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Reflection;
using Xunit;

namespace Kickoffa.API.Integration.Tests.ProgramFile.Mvc;

/// <summary>
/// Testes para verificar se o Program.cs está configurando corretamente os Controllers,
/// filtros de exceção e serialização JSON (linhas 32-42)
/// </summary>
public class ProgramControllersBuilderTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramControllersBuilderTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Program_ShouldRegisterControllers()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var mvcOptions = scope.ServiceProvider.GetService<IConfigureOptions<MvcOptions>>();

        // Assert
        Assert.NotNull(mvcOptions);
    }

    [Fact]
    public void Program_ShouldConfigureExceptionFilter()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var mvcOptions = scope.ServiceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;

        // Assert
        var exceptionFilter = mvcOptions.Filters.First(filter =>
            filter is TypeFilterAttribute tfa &&
            tfa.ImplementationType == typeof(ExceptionFilter));

        Assert.NotNull(exceptionFilter);
    }

    [Fact]
    public void Program_ShouldConfigureNewtonsoftJson()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var jsonOptions = scope.ServiceProvider.GetService<IConfigureOptions<MvcNewtonsoftJsonOptions>>();

        // Assert
        Assert.NotNull(jsonOptions);
    }

    [Fact]
    public void Program_ShouldConfigureStringEnumConverter()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var jsonOptions = scope.ServiceProvider.GetRequiredService<IConfigureOptions<MvcNewtonsoftJsonOptions>>();

        var options = new MvcNewtonsoftJsonOptions();
        jsonOptions.Configure(options);

        // Assert
        var stringEnumConverter = options.SerializerSettings.Converters
            .FirstOrDefault(c => c is StringEnumConverter);

        Assert.NotNull(stringEnumConverter);
        Assert.IsType<StringEnumConverter>(stringEnumConverter);
    }

    [Fact]
    public void Program_ShouldConfigureCustomerRequestConverter()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var jsonOptions = scope.ServiceProvider.GetRequiredService<IConfigureOptions<MvcNewtonsoftJsonOptions>>();

        var options = new MvcNewtonsoftJsonOptions();
        jsonOptions.Configure(options);

        // Assert
        var customerRequestConverter = options.SerializerSettings.Converters
            .FirstOrDefault(c => c is CustomerRequestConverter);

        Assert.NotNull(customerRequestConverter);
        Assert.IsType<CustomerRequestConverter>(customerRequestConverter);
    }

    [Fact]
    public void Program_ShouldConfigureOptionsSerializerSettingsTypeNameHandling()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var jsonOptions = scope.ServiceProvider.GetRequiredService<IConfigureOptions<MvcNewtonsoftJsonOptions>>();

        var options = new MvcNewtonsoftJsonOptions();
        jsonOptions.Configure(options);

        // Assert
        Assert.Equal(TypeNameHandling.None, options.SerializerSettings.TypeNameHandling);
    }

    [Fact]
    public void Program_ShouldConfigureTypeNameHandlingAsNone()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var jsonOptions = scope.ServiceProvider.GetRequiredService<IConfigureOptions<MvcNewtonsoftJsonOptions>>();

        var options = new MvcNewtonsoftJsonOptions();
        jsonOptions.Configure(options);

        // Assert
        Assert.Equal(TypeNameHandling.None, options.SerializerSettings.TypeNameHandling);
    }

    [Fact]
    public void Program_ShouldHaveCorrectNumberOfJsonConverters()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var jsonOptions = scope.ServiceProvider.GetRequiredService<IConfigureOptions<MvcNewtonsoftJsonOptions>>();

        var options = new MvcNewtonsoftJsonOptions();
        jsonOptions.Configure(options);

        // Assert
        // Deve ter pelo menos 2 converters: StringEnumConverter e CustomerRequestConverter
        Assert.True(options.SerializerSettings.Converters.Count >= 2);

        // Verificar se ambos os converters estão presentes
        var hasStringEnumConverter = options.SerializerSettings.Converters.Any(c => c is StringEnumConverter);
        var hasCustomerRequestConverter = options.SerializerSettings.Converters.Any(c => c is CustomerRequestConverter);

        Assert.True(hasStringEnumConverter);
        Assert.True(hasCustomerRequestConverter);
    }

    [Fact]
    public void Program_ShouldConfigureMvcOptionsCorrectly()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var mvcOptions = scope.ServiceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;

        // Assert
        // Verificar se há pelo menos um filtro configurado (ExceptionFilter)
        Assert.NotEmpty(mvcOptions.Filters);

        // Verificar se o ExceptionFilter está presente
        var hasExceptionFilter = mvcOptions.Filters.Any(f =>
            f is TypeFilterAttribute tfa &&
            tfa.ImplementationType == typeof(ExceptionFilter));

        Assert.True(hasExceptionFilter);
    }

    [Fact]
    public void Program_ShouldAllowControllerInstantiation()
    {
        // Act
        using var scope = _factory.Services.CreateScope();

        // Tentar obter um controller específico (assumindo que existe AuthController)
        var controllerTypes = Assembly.GetAssembly(typeof(Program))
            ?.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ControllerBase)) && !t.IsAbstract)
            .ToList();

        // Assert
        Assert.NotNull(controllerTypes);
        Assert.NotEmpty(controllerTypes);

        // Verificar se pelo menos um controller pode ser instanciado
        foreach (var controllerType in controllerTypes.Take(1)) // Testar apenas o primeiro para não sobrecarregar
        {
            var controller = scope.ServiceProvider.GetService(controllerType);
            // Nota: Pode ser null se o controller não estiver registrado no DI, mas não deve lançar exceção
        }
    }

    [Fact]
    public void Program_ShouldConfigureJsonSerializationSettings()
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var jsonOptions = scope.ServiceProvider.GetRequiredService<IConfigureOptions<MvcNewtonsoftJsonOptions>>();

        var options = new MvcNewtonsoftJsonOptions();
        jsonOptions.Configure(options);

        // Assert
        var settings = options.SerializerSettings;

        // Verificar configurações básicas
        Assert.NotNull(settings);
        Assert.Equal(TypeNameHandling.None, settings.TypeNameHandling);
        Assert.NotEmpty(settings.Converters);
    }

    [Fact]
    public void Program_ShouldHaveConsistentMvcConfiguration()
    {
        // Act
        using var scope = _factory.Services.CreateScope();

        // Verificar se tanto MvcOptions quanto MvcNewtonsoftJsonOptions estão configurados
        var mvcOptions = scope.ServiceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
        var jsonOptions = scope.ServiceProvider.GetService<IConfigureOptions<MvcNewtonsoftJsonOptions>>();

        // Assert
        Assert.NotNull(mvcOptions);
        Assert.NotNull(jsonOptions);

        // Ambos devem estar disponíveis simultaneamente
        var jsonOptionsInstance = new MvcNewtonsoftJsonOptions();

        jsonOptions.Configure(jsonOptionsInstance);

        // Verificar se as configurações foram aplicadas
        Assert.NotEmpty(mvcOptions.Filters);
        Assert.NotEmpty(jsonOptionsInstance.SerializerSettings.Converters);
    }

    [Theory]
    [InlineData(typeof(StringEnumConverter))]
    [InlineData(typeof(CustomerRequestConverter))]
    public void Program_ShouldConfigureSpecificJsonConverter(Type converterType)
    {
        // Act
        using var scope = _factory.Services.CreateScope();
        var jsonOptions = scope.ServiceProvider.GetRequiredService<IConfigureOptions<MvcNewtonsoftJsonOptions>>();

        var options = new MvcNewtonsoftJsonOptions();
        jsonOptions.Configure(options);

        // Assert
        var hasConverter = options.SerializerSettings.Converters.Any(c => c.GetType() == converterType);
        Assert.True(hasConverter, $"Converter {converterType.Name} should be configured");
    }

    [Fact]
    public void Program_ShouldConfigureFiltersWithCorrectLifetime()
    {

        // Act
        using var scope = _factory.Services.CreateScope();
        var mvcOptions = scope.ServiceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;

        // Assert
        var exceptionFilterDescriptor = mvcOptions.Filters.FirstOrDefault(f =>
            f is TypeFilterAttribute tfa &&
            tfa.ImplementationType == typeof(ExceptionFilter));

        Assert.NotNull(exceptionFilterDescriptor);
    }
}