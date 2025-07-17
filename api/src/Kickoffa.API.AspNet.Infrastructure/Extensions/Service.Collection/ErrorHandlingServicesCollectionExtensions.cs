using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Kickoffa.API.AspNet.Infrastructure.Extensions.Service.Collection
{
	public static class ErrorHandlingServicesCollectionExtensions
	{
		public static void AddErrorHandlers(this IServiceCollection services)
		{
			services.AddSingleton<IErrorFactory, ErrorFactory>();
			services.AddSingleton<IActionResultErrorHandler, ActionResultErrorHandler>();
		}
	}
}