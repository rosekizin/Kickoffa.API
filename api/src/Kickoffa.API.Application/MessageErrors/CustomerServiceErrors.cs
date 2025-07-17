using Kickoffa.API.Domain.ProcessResult;
using System.Net;

namespace Kickoffa.API.Application.MessageErrors
{
	/// <summary>
	/// Classe que define os tipos de erro que podem ocorrer no gerenciamento de um Customer
	/// </summary>
	public static class CustomerServiceErrors
	{
		private const string CUSTOMER_NOT_FOUND = "Cliente com id {0} não encontrado";
		private const string CUSTOMER_TYPE_CANNOT_BE_CHANGED = "Não é possível alterar o tipo do cliente após a criação";
		private const string INCOMPATIBLE_REQUEST_TYPE = "Tipo de request incompatível com o tipo do customer existente";

		public static Error CustomerNotFound(long customerId)
		{
			var metadata = new Dictionary<string, object>
			{
				{ "customerId", customerId }
			};

			return new Error(nameof(CUSTOMER_NOT_FOUND), string.Format(CUSTOMER_NOT_FOUND, customerId), HttpStatusCode.NotFound, metadata);
		}

		public static Error CustomerTypeCannotBeChanged(long customerId)
		{
			var metadata = new Dictionary<string, object>
			{
				{ "customerId", customerId }
			};

			return new Error(nameof(CUSTOMER_TYPE_CANNOT_BE_CHANGED), CUSTOMER_TYPE_CANNOT_BE_CHANGED, HttpStatusCode.BadRequest, metadata);
		}

		public static Error IncompatibleRequestType(long customerId)
		{
			var metadata = new Dictionary<string, object>
			{
				{ "customerId", customerId }
			};

			return new Error(nameof(INCOMPATIBLE_REQUEST_TYPE), INCOMPATIBLE_REQUEST_TYPE, HttpStatusCode.BadRequest, metadata);
		}
	}
}