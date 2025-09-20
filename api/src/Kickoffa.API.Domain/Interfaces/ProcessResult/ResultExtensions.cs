using Kickoffa.API.Domain.ProcessResult;

namespace Kickoffa.API.Domain.Interfaces.ProcessResult
{
    public static class ResultAsyncExtensions
    {
        // ---------- SYNC ----------

        public static IResult<TOut> Bind<TIn, TOut>(
            this IResult<TIn> result,
            Func<TIn, IResult<TOut>> func)
        {
            if (result.IsFailure)
                return Result<TOut>.Failure(result.Error!);

            return func(result.Value!);
        }

        public static IResult<TOut> Map<TIn, TOut>(
            this IResult<TIn> result,
            Func<TIn, TOut> func)
        {
            if (result.IsFailure)
                return Result<TOut>.Failure(result.Error!);

            return Result<TOut>.Success(func(result.Value!));
        }

        public static void Match<T>(
            this IResult<T> result,
            Action<T> onSuccess,
            Action<string> onFailure)
        {
            if (result.IsSuccess)
                onSuccess(result.Value!);
            else
                onFailure(result.Error!);
        }

        // ---------- ASYNC ----------

        public static async Task<IResult<TOut>> Bind<TIn, TOut>(
            this Task<IResult<TIn>> task,
            Func<TIn, IResult<TOut>> func)
        {
            var result = await task;
            return result.IsFailure
                ? Result<TOut>.Failure(result.Error!)
                : func(result.Value!);
        }

        public static async Task<IResult<TOut>> BindAsync<TIn, TOut>(
            this Task<IResult<TIn>> task,
            Func<TIn, Task<IResult<TOut>>> func)
        {
            var result = await task;
            return result.IsFailure
                ? Result<TOut>.Failure(result.Error!)
                : await func(result.Value!);
        }

        public static async Task<IResult<TOut>> Map<TIn, TOut>(
            this Task<IResult<TIn>> task,
            Func<TIn, TOut> func)
        {
            var result = await task;
            return result.IsFailure
                ? Result<TOut>.Failure(result.Error!)
                : Result<TOut>.Success(func(result.Value!));
        }

        public static async Task<IResult<TOut>> MapAsync<TIn, TOut>(
            this Task<IResult<TIn>> task,
            Func<TIn, Task<TOut>> func)
        {
            var result = await task;
            if (result.IsFailure)
                return Result<TOut>.Failure(result.Error!);

            var mapped = await func(result.Value!);
            return Result<TOut>.Success(mapped);
        }

        public static async Task Match<T>(
            this Task<IResult<T>> task,
            Action<T> onSuccess,
            Action<string> onFailure)
        {
            var result = await task;
            if (result.IsSuccess)
                onSuccess(result.Value!);
            else
                onFailure(result.Error!);
        }
    }
}