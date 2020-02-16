using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace DatingApp.Shared.FunctionalExtensions
{
    public static partial class FunctionalExtensions
    {
        public static Result<K, E> AutoMap<T, K, E>(this Result<T, E> result, Func<T, K> mapper)
        {
            return Result.Success<K, E>(mapper(result.Value));
        }
        
        public static async Task<Result<K, E>> AutoMap<T, K, E>(this Task<Result<T, E>> resultTask, Func<T, K> mapper)
        {
            var result = await resultTask;
            return Result.Success<K, E>(mapper(result.Value));
        }
    }
}