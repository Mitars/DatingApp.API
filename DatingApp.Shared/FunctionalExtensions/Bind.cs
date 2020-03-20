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
        public static Result<TOutput, TError> Bind<TInput, TOutput, TError>(this Result<TInput, TError> result, Func<TInput, TOutput> mapper)
        {
            return Result.Success<TOutput, TError>(mapper(result.Value));
        }

        public static async Task<Result<TOutput, TError>> Bind<TInput, TOutput, TError>(this Task<Result<TInput, TError>> resultTask, Func<TInput, TOutput> mapper)
        {
            var result = await resultTask;
            return Result.Success<TOutput, TError>(mapper(result.Value));
        }

        public static async Task<Result<IEnumerable<TOutput>, TError>> Bind<TInput, TOutput, TError>(this Task<Result<TInput, TError>> resultTask, Func<TInput, IEnumerable<TOutput>> mapper)
        {
            var result = await resultTask;
            return Result.Success<IEnumerable<TOutput>, TError>(mapper(result.Value));
        }
    }
}