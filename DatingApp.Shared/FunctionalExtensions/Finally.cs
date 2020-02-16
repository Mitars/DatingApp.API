using System;
using System.Collections.Generic;
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
        public static async Task<Result<IEnumerable<K>, E>> AutoMap<T, K, E>(this Task<Result<T, E>> resultTask, Func<T, IEnumerable<K>> mapper)
        {
            var result = await resultTask;
            return Result.Success<IEnumerable<K>, E>(mapper(result.Value));
        }

        public static K Finally<T, K, E>(this Result<T, E> result, Func<Result<T, E>, K> success, Func<Result<T, E>, K> failure)
        {
            return result.IsSuccess ? success(result) : failure(result);
        }

        public static async Task<K> Finally<T, K, E>(this Task<Result<T, E>> resultTask, Func<Result<T, E>, K> success, Func<Result<T, E>, K> failure)
        {
            var result = await resultTask;
            return result.IsSuccess ? success(result) : failure(result);
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
