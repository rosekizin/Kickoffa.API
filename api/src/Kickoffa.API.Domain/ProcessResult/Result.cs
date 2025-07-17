using Kickoffa.API.Domain.Interfaces.ProcessResult;

namespace Kickoffa.API.Domain.ProcessResult
{
	/// <summary>
	/// Representa o resultado de uma operação que pode falhar
	/// </summary>
	public class Result : IResult
	{
		/// <summary>
		/// Indica se a operação foi bem-sucedida
		/// </summary>
		public bool IsSuccess { get; }

		/// <summary>
		/// Indica se a operação falhou
		/// </summary>
		public bool IsFailure => !IsSuccess;

		/// <summary>
		/// Mensagem de erro (vazia se sucesso)
		/// </summary>
		public string Error { get; }

		/// <summary>
		/// Objeto de erro original (null se sucesso ou se não fornecido)
		/// </summary>
		public Error? ErrorObject { get; }

		IError? IResult.ErrorObject => ErrorObject;

		/// <summary>
		/// Construtor protegido para Result
		/// </summary>
		/// <param name="isSuccess">Se a operação foi bem-sucedida</param>
		/// <param name="error">Mensagem de erro</param>
		/// <param name="errorObject">Objeto de erro original</param>
		protected Result(bool isSuccess, string? error, Error? errorObject = null)
		{
			IsSuccess = isSuccess;
			ErrorObject = isSuccess ? null : errorObject;

			// Para sucesso, garantir que error seja vazio
			// Para falha, garantir que error não seja nulo/vazio
			Error = isSuccess
				? string.Empty
				: string.IsNullOrWhiteSpace(error) ? "Erro não especificado" : error;
		}

		/// <summary>
		/// Cria um resultado de sucesso
		/// </summary>
		/// <returns>Result indicando sucesso</returns>
		public static Result Success() => new(true, string.Empty);

		/// <summary>
		/// Cria um resultado de falha
		/// </summary>
		/// <param name="error">Mensagem de erro</param>
		/// <returns>Result indicando falha</returns>
		public static Result Failure(string error) => new(false, error);

		/// <summary>
		/// Cria um resultado de falha com objeto de erro
		/// </summary>
		/// <param name="errorObject">Objeto de erro</param>
		/// <returns>Result indicando falha</returns>
		public static Result Failure(Error errorObject) => new(false, errorObject?.ToString() ?? "Erro não especificado", errorObject);

		/// <summary>
		/// Operador implícito para converter bool em Result
		/// </summary>
		/// <param name="success">Se a operação foi bem-sucedida</param>
		public static implicit operator Result(bool success) => success ? Success() : Failure("Operação falhou");
	}

	/// <summary>
	/// Representa o resultado de uma operação que pode falhar e retorna um valor
	/// </summary>
	/// <typeparam name="T">Tipo do valor retornado</typeparam>
	public class Result<T> : Result, IResult<T>
	{
		/// <summary>
		/// Valor retornado pela operação (default se falha)
		/// </summary>
		public T Value { get; }

		/// <summary>
		/// Construtor privado para Result<T>
		/// </summary>
		/// <param name="isSuccess">Se a operação foi bem-sucedida</param>
		/// <param name="value">Valor retornado</param>
		/// <param name="error">Mensagem de erro</param>
		/// <param name="errorObject">Objeto de erro original</param>
		private Result(bool isSuccess, T value, string? error, Error? errorObject = null) : base(isSuccess, error, errorObject)
		{
			Value = value;
		}

		/// <summary>
		/// Cria um resultado de sucesso com valor
		/// </summary>
		/// <param name="value">Valor a ser retornado</param>
		/// <returns>Result<T> indicando sucesso</returns>
		public static Result<T> Success(T value) => new(true, value, string.Empty);

		/// <summary>
		/// Cria um resultado de falha
		/// </summary>
		/// <param name="error">Mensagem de erro</param>
		/// <returns>Result<T> indicando falha</returns>
		public static new Result<T> Failure(string error) => new(false, default!, error);

		/// <summary>
		/// Cria um resultado de falha com objeto de erro
		/// </summary>
		/// <param name="errorObject">Objeto de erro</param>
		/// <returns>Result<T> indicando falha</returns>
		public static new Result<T> Failure(Error errorObject) => new(false, default!, errorObject?.ToString(), errorObject);

		/// <summary>
		/// Operador implícito para converter T em Result<T>
		/// </summary>
		/// <param name="value">Valor a ser convertido</param>
		public static implicit operator Result<T>(T value) => Success(value);

		public static implicit operator Result<T>(Error error) => Failure(error);
	}

	public static class ResultExtensions
	{
		/// <summary>
		/// Método de extensão para converter Result em Result<T>
		/// </summary>
		/// <param name="result">Result a ser convertido</param>
		public static Result<T> ToGeneric<T>(this Result result)
		{
			return result.IsSuccess
				? Result<T>.Success(default!)
				: Result<T>.Failure(result.Error);
		}
		public static Result<T> ToResult<T>(this T value) => Result<T>.Success(value);
		public static Result<T> ToFailure<T>(this Error error) => Result<T>.Failure(error);
	}
}