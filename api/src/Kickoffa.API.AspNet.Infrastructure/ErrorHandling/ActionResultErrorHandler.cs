using Kickoffa.API.Domain.Interfaces.ProcessResult;
using Microsoft.AspNetCore.Mvc;

namespace Kickoffa.API.AspNet.Infrastructure.ErrorHandling
{
	public interface IActionResultErrorHandler
	{
		IActionResult GetActionResultFromError(IError error);
	}

	public class ActionResultErrorHandler : IActionResultErrorHandler
	{
		private readonly IErrorFactory _errorFactory;

		public ActionResultErrorHandler(IErrorFactory errorFactory)
		{
			_errorFactory = errorFactory;
		}

		public IActionResult GetActionResultFromError(IError error)
		{
			var statusCode = (int)error.HttpStatusCode;

			return statusCode switch
			{
				400 => new BadRequestObjectResult(_errorFactory.CreateBadRequest(error)),
				404 => new NotFoundObjectResult(_errorFactory.CreateNotFound(error)),
				_ => new ObjectResult(_errorFactory.CreateInternalServerError(error)) { StatusCode = 500 }
			};
		}
	}
}