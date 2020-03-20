using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.Shared.FunctionalExtensions
{
    /// <summary>
    /// Functional programming extension class.
    /// </summary>
    public static partial class FunctionalExtensions
    {
        public static async Task<K> Finally<T, K, E>(this Task<Result<T, E>> resultTask, Func<T, K> success, Func<E, K> failure)
        {
            var result = await resultTask;
            return result.IsSuccess ? success(result.Value) : failure(result.Error);
        }

        public static async Task<Result<None, Error>> None<T>(this Task<Result<T, Error>> resultTask)
        {
            var result = await resultTask;
            if (result.IsSuccess)
                return Result.Success<None, Error>(new None());
            else
                return Result.Failure<None, Error>(result.Error);
        }
    }
}
