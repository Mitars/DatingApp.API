using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace DatingApp.Shared.FunctionalExtensions
{
    /// <summary>
    /// Functional programming extension class.
    /// </summary>
    public static partial class FunctionalExtensions
    {
        public static async Task<Result<T, TError>> EnsureNotNull<T, TError>(
            this Task<Result<T, TError>> resultTask,
            TError error)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            if (result.Value == null)
                return Result.Failure<T, TError>(error);

            return result;
        }

        public static async Task<Result<TInput, TError>> EnsureNotNull<TInput, TOutput, TError>(
            this Task<Result<TInput, TError>> resultTask,
            Func<TInput, Task<Result<TOutput, TError>>> predicate,
            TError error)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            Result<TOutput, TError> predicateResult = await predicate(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (predicateResult.IsFailure)
                return Result.Failure<TInput, TError>(predicateResult.Error);

            if (predicateResult.Value == null)
                return Result.Failure<TInput, TError>(error);

            return result;
        }

        public static async Task<Result<TInput, TError>> EnsureNull<TInput, TOutput, TError>(
            this Result<TInput, TError> result,
            Func<TInput, Task<Result<TOutput, TError>>> predicate,
            TError error)
        {
            if (result.IsFailure)
                return result;

            Result<TOutput, TError> predicateResult = await predicate(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (predicateResult.IsFailure)
                return Result.Failure<TInput, TError>(predicateResult.Error);

            if (predicateResult.Value != null)
                return Result.Failure<TInput, TError>(error);

            return result;
        }

        public static async Task<Result<T, TError>> Ensure<T, TError>(
            this Task<Result<T, TError>> resultTask,
            Func<Result<T, TError>, bool> predicate,
            TError error)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            if (predicate(result))
                return Result.Failure<T, TError>(error);

            return result;
        }
    }
}