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
        public static async Task<Result<T, TError>> TapIf<T, TError>(this Task<Result<T, TError>> resultTask, Func<T, bool> condition, Action<T> action)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            if (condition(result.Value))
                action(result.Value);

            return result;
        }

        public static async Task<Result<TInput, TError>> TapIf<TInput, TOutput, TError>(this Task<Result<TInput, TError>> resultTask, Func<TInput, bool> condition, Func<TInput, Task<Result<TOutput, TError>>> actionTask)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            var newResult = await actionTask(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (newResult.IsFailure)
                return Result.Failure<TInput, TError>(newResult.Error);

            return result;
        }
    }
}