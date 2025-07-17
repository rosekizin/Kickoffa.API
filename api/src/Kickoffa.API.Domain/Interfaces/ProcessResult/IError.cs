using System.Net;

namespace Kickoffa.API.Domain.Interfaces.ProcessResult
{
	public interface IError
	{
		string Code { get; }
		HttpStatusCode HttpStatusCode { get; }
		string Message { get; init; }
		IDictionary<string, object>? Metadata { get; }

		string ToString();
	}
}