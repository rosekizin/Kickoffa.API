using Kickoffa.API.Domain.Interfaces.ProcessResult;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Kickoffa.API.AspNet.Infrastructure.ErrorHandling
{
	public interface IErrorFactory
	{
		ProblemDetails CreateNotFound(IError error);
		ProblemDetails CreateNotFound(string error);

		ProblemDetails CreateBadRequest(IError error);

		ProblemDetails CreateBadRequest(string error);
		
		ProblemDetails CreateInternalServerError(IError error);

		ProblemDetails CreateInternalServerError();
	}

	public class ErrorFactory : IErrorFactory
	{
		private const string NOT_FOUND = "Not Found";
		private const string BAD_REQUEST = "Bad Request";
		private const string INTERNAL_SERVER_ERROR = "Internal Server Error";

		public ProblemDetails CreateBadRequest(string error)
		{
			var problemDetails = new ProblemDetails
			{
				Title = BAD_REQUEST,
				Status = (int)HttpStatusCode.BadRequest,
				Detail = error
            };

			return problemDetails;
		}

		public ProblemDetails CreateBadRequest(IError error)
		{
			var problemDetails = new ProblemDetails
			{
				Title = BAD_REQUEST,
				Status = (int)error.HttpStatusCode,
				Detail = string.IsNullOrWhiteSpace(error.Message) ? "O recurso tem inconsistências." : error.Message
			};

			AddExtensions(problemDetails, error);

			return problemDetails;
		}

		public ProblemDetails CreateNotFound(IError error)
		{
			var problemDetails = new ProblemDetails
			{
				Title = NOT_FOUND,
				Status = (int)error.HttpStatusCode,
				Detail = string.IsNullOrWhiteSpace(error.Message) ? "O recurso não foi encontrado." : error.Message
			};

			AddExtensions(problemDetails, error);

			return problemDetails;
		}

		public ProblemDetails CreateNotFound(string error)
		{
			var problemDetails = new ProblemDetails
			{
				Title = NOT_FOUND,
				Status = (int)HttpStatusCode.NotFound,
				Detail = error
            };

			return problemDetails;
		}

		public ProblemDetails CreateInternalServerError(IError error)
		{
			var problemDetails = new ProblemDetails
			{
				Title = INTERNAL_SERVER_ERROR,
				Status = (int)error.HttpStatusCode,
				Detail = string.IsNullOrWhiteSpace(error.Message) ? "Ocorreu um erro inesperado. Contate o suporte." : error.Message
			};

			AddExtensions(problemDetails, error);

			return problemDetails;
		}

		public ProblemDetails CreateInternalServerError()
		{
			var problemDetails = new ProblemDetails
			{
				Title = INTERNAL_SERVER_ERROR,
				Status = (int)HttpStatusCode.InternalServerError,
				Detail = "Ocorreu um erro inesperado. Contate o suporte."
			};

			problemDetails.Extensions.TryAdd("code", "INTERNAL_SERVER_ERROR");

			return problemDetails;
		}

		private static void AddExtensions(ProblemDetails problemDetails, IError error)
		{

			if(error.Metadata is not null)
				problemDetails.Extensions.TryAdd("metadata", error.Metadata);
		}
	}
}