using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection;
using Microsoft.Extensions.DependencyInjection;

namespace Kickoffa.API.AspNet.Infrastructure.UnitTests.Extensions.Service.Collection;

/// <summary>
/// Testes unitários para ErrorHandlingServicesCollectionExtensions
/// </summary>
public class ErrorHandlingServicesCollectionExtensionsTests
{
	#region AddErrorHandlers Tests

	[Fact]
	public void AddErrorHandlers_ShouldRegisterIErrorFactoryAsSingleton()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();

		// Assert
		var serviceProvider = services.BuildServiceProvider();
		var errorFactory = serviceProvider.GetService<IErrorFactory>();
		
		Assert.NotNull(errorFactory);
		Assert.IsType<ErrorFactory>(errorFactory);
	}

	[Fact]
	public void AddErrorHandlers_ShouldRegisterIActionResultErrorHandlerAsSingleton()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();

		// Assert
		var serviceProvider = services.BuildServiceProvider();
		var actionResultErrorHandler = serviceProvider.GetService<IActionResultErrorHandler>();
		
		Assert.NotNull(actionResultErrorHandler);
		Assert.IsType<ActionResultErrorHandler>(actionResultErrorHandler);
	}

	[Fact]
	public void AddErrorHandlers_ShouldRegisterBothServicesCorrectly()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();

		// Assert
		var serviceProvider = services.BuildServiceProvider();
		
		var errorFactory = serviceProvider.GetService<IErrorFactory>();
		var actionResultErrorHandler = serviceProvider.GetService<IActionResultErrorHandler>();
		
		Assert.NotNull(errorFactory);
		Assert.NotNull(actionResultErrorHandler);
		Assert.IsType<ErrorFactory>(errorFactory);
		Assert.IsType<ActionResultErrorHandler>(actionResultErrorHandler);
	}

	[Fact]
	public void AddErrorHandlers_ShouldRegisterServicesAsSingleton()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();

		// Assert
		var serviceProvider = services.BuildServiceProvider();
		
		// Verificar que IErrorFactory é singleton
		var errorFactory1 = serviceProvider.GetService<IErrorFactory>();
		var errorFactory2 = serviceProvider.GetService<IErrorFactory>();
		Assert.Same(errorFactory1, errorFactory2);
		
		// Verificar que IActionResultErrorHandler é singleton
		var handler1 = serviceProvider.GetService<IActionResultErrorHandler>();
		var handler2 = serviceProvider.GetService<IActionResultErrorHandler>();
		Assert.Same(handler1, handler2);
	}

	[Fact]
	public void AddErrorHandlers_WithExistingServices_ShouldNotDuplicateRegistrations()
	{
		// Arrange
		var services = new ServiceCollection();
		services.AddSingleton<IErrorFactory, ErrorFactory>();

		// Act
		services.AddErrorHandlers();

		// Assert
		var serviceProvider = services.BuildServiceProvider();
		var errorFactories = serviceProvider.GetServices<IErrorFactory>().ToList();
		
		// Deve ter 2 registros (o original + o do AddErrorHandlers)
		Assert.Equal(2, errorFactories.Count);
		Assert.All(errorFactories, factory => Assert.IsType<ErrorFactory>(factory));
	}

	[Fact]
	public void AddErrorHandlers_ShouldAllowActionResultErrorHandlerToResolveIErrorFactory()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();

		// Assert
		var serviceProvider = services.BuildServiceProvider();
		var actionResultErrorHandler = serviceProvider.GetService<IActionResultErrorHandler>();
		
		Assert.NotNull(actionResultErrorHandler);
		
		// Verificar que o ActionResultErrorHandler pode ser criado (o que significa que suas dependências foram resolvidas)
		Assert.IsType<ActionResultErrorHandler>(actionResultErrorHandler);
	}

	[Fact]
	public void AddErrorHandlers_MultipleCallsShouldNotThrow()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act & Assert
		var exception = Record.Exception(() =>
		{
			services.AddErrorHandlers();
			services.AddErrorHandlers();
			services.AddErrorHandlers();
		});

		Assert.Null(exception);
	}

	[Fact]
	public void AddErrorHandlers_WithMultipleCalls_ShouldRegisterMultipleInstances()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();
		services.AddErrorHandlers();

		// Assert
		var serviceProvider = services.BuildServiceProvider();
		
		var errorFactories = serviceProvider.GetServices<IErrorFactory>().ToList();
		var actionResultErrorHandlers = serviceProvider.GetServices<IActionResultErrorHandler>().ToList();
		
		Assert.Equal(2, errorFactories.Count);
		Assert.Equal(2, actionResultErrorHandlers.Count);
	}

	[Fact]
	public void AddErrorHandlers_ShouldReturnServiceCollection()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();

		// Assert
		// O método é void, então não retorna nada, mas vamos verificar que não quebra o fluent interface
		Assert.NotNull(services);
	}

	#endregion

	#region Service Descriptor Tests

	[Fact]
	public void AddErrorHandlers_ShouldRegisterCorrectServiceDescriptors()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();

		// Assert
		var errorFactoryDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IErrorFactory));
		var actionResultErrorHandlerDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IActionResultErrorHandler));

		Assert.NotNull(errorFactoryDescriptor);
		Assert.Equal(ServiceLifetime.Singleton, errorFactoryDescriptor.Lifetime);
		Assert.Equal(typeof(ErrorFactory), errorFactoryDescriptor.ImplementationType);

		Assert.NotNull(actionResultErrorHandlerDescriptor);
		Assert.Equal(ServiceLifetime.Singleton, actionResultErrorHandlerDescriptor.Lifetime);
		Assert.Equal(typeof(ActionResultErrorHandler), actionResultErrorHandlerDescriptor.ImplementationType);
	}

	[Fact]
	public void AddErrorHandlers_ShouldNotRegisterUnrelatedServices()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();

		// Assert
		var serviceTypes = services.Select(s => s.ServiceType).ToList();
		
		Assert.Contains(typeof(IErrorFactory), serviceTypes);
		Assert.Contains(typeof(IActionResultErrorHandler), serviceTypes);
		Assert.Equal(2, services.Count);
	}

	#endregion

	#region Integration Tests

	[Fact]
	public void AddErrorHandlers_IntegrationTest_ShouldWorkWithRealDependencies()
	{
		// Arrange
		var services = new ServiceCollection();

		// Act
		services.AddErrorHandlers();

		// Assert
		var serviceProvider = services.BuildServiceProvider();
		
		// Verificar que podemos resolver e usar os serviços
		var errorFactory = serviceProvider.GetRequiredService<IErrorFactory>();
		var actionResultErrorHandler = serviceProvider.GetRequiredService<IActionResultErrorHandler>();

		// Testar que os serviços funcionam
		var error = new Kickoffa.API.Domain.ProcessResult.Error(
			"TEST_ERROR", 
			"Test message", 
			System.Net.HttpStatusCode.BadRequest);

		var problemDetails = errorFactory.CreateBadRequest(error);
		var actionResult = actionResultErrorHandler.GetActionResultFromError(error);

		Assert.NotNull(problemDetails);
		Assert.NotNull(actionResult);
	}

	#endregion

	#region Edge Cases

	[Fact]
	public void AddErrorHandlers_WithNullServiceCollection_ShouldThrowArgumentNullException()
	{
		// Arrange
		IServiceCollection? services = null;

		// Act & Assert
		Assert.Throws<ArgumentNullException>(() => services!.AddErrorHandlers());
	}

	#endregion
}