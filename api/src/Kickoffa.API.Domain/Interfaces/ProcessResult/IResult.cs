namespace Kickoffa.API.Domain.Interfaces.ProcessResult
{
	public interface IResult
	{
		bool IsSuccess { get; }
		bool IsFailure { get; }
		string Error { get; }
		IError? ErrorObject { get; }
	}

	public interface IResult<out T> : IResult
	{
		T Value { get; }
	}
}