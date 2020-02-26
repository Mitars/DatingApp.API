using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace DatingApp.Shared.FunctionalExtensions
{
    /// <summary>
    /// Functional programming extension class.
    /// </summary>
    public static partial class FunctionalExtensions
    {
        public static Result<K, E> Bind<T, K, E>(this Result<T, E> result, Func<T, K> mapper)
        {
            return Result.Success<K, E>(mapper(result.Value));
        }
        
        public static async Task<Result<K, E>> Bind<T, K, E>(this Task<Result<T, E>> resultTask, Func<T, K> mapper)
        {
            var result = await resultTask;
            return Result.Success<K, E>(mapper(result.Value));
        }

        public static async Task<Result<IEnumerable<K>, E>> Bind<T, K, E>(this Task<Result<T, E>> resultTask, Func<T, IEnumerable<K>> mapper)
        {
            var result = await resultTask;
            return Result.Success<IEnumerable<K>, E>(mapper(result.Value));
        }
    }
}