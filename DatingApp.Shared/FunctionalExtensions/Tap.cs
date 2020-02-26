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
        public static async Task<Result<T, E>> TapIf<T, E>(this Task<Result<T, E>> resultTask, Func<T, bool> condition, Action<T> action)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            if (condition(result.Value))
                action(result.Value);

            return result;
        }

        public static async Task<Result<T, E>> TapIf<T, K, E>(this Task<Result<T, E>> resultTask, Func<T, bool> condition, Func<T, Task<Result<K, E>>> actionTask)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            var newResult = await actionTask(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (newResult.IsFailure)
                return Result.Failure<T, E>(newResult.Error);

            return result;
        }
        
        public static async Task<Result<T, E>> Tap<T, E>(this Result<T, E> result, Func<T, Task<Result<None, E>>> func)
        {
            if (result.IsFailure)
                return result;

            var newResult = await func(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (newResult.IsFailure)
                return Result.Failure<T, E>(newResult.Error);

            return result;
        }

        public static async Task<Result<T, E>> Tap<T, E>(this Task<Result<T, E>> resultTask, Func<T, Task<Result<None, E>>> func)
        {
            var result = await resultTask;
            if (result.IsFailure)
                return result;

            var newResult = await func(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (newResult.IsFailure)
                return Result.Failure<T, E>(newResult.Error);

            return result;
        }
    }
}