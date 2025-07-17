using Kickoffa.API.AspNet.Infrastructure.ErrorHandling;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Kickoffa.API.AspNet.Infrastructure.ExceptionHandling
{
	/// <summary>
	/// Global Exception handler para capturar para tratamento global e logging.
	/// </summary>
	public sealed class GlobalExceptionHandler : IExceptionHandler
	{
		public readonly IErrorFactory _errorFactory;
		public readonly ILogger<GlobalExceptionHandler> _logger;

		public GlobalExceptionHandler(
			IErrorFactory errorFactory,
			ILogger<GlobalExceptionHandler> logger)
		{
			_errorFactory = errorFactory;
			_logger = logger;
		}

		public async ValueTask<bool> TryHandleAsync(
			HttpContext httpContext,
			Exception exception,
			CancellationToken cancellationToken)
		{
			_logger.LogError(exception, "An unhandled exception occurred while processing the request.");

			var problemDetails = _errorFactory.CreateInternalServerError();

			httpContext.Response.StatusCode = problemDetails.Status!.Value;

			await httpContext
				.Response
				.WriteAsJsonAsync(problemDetails, cancellationToken);

			return true;
		}
	}
}