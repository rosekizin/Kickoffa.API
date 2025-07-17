using Kickoffa.API.Domain.Interfaces.ProcessResult;
using System.Net;

namespace Kickoffa.API.Domain.ProcessResult
{
	public class Error : IError
	{
		public string Code { get; init; }
		public HttpStatusCode HttpStatusCode { get; init; }
		public string Message { get; init; }
		public IDictionary<string, object>? Metadata { get; init; }

		public Error(string code, string message, HttpStatusCode httpStatusCode, IDictionary<string, object>? metadata = null)
		{
			Code = code;
			Message = message;
			Metadata = metadata;
			HttpStatusCode = httpStatusCode;
		}

		public override string ToString() => $"{Code}: {Message}";
	}
}