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
        public static Result<T, Error> Success<T>(this T value)
        {
            return Result.Success<T, Error>(value);
        }

        public static Result<IEnumerable<T>, Error> Success<T>(this IEnumerable<T> value)
        {
            return Result.Success<IEnumerable<T>, Error>(value);
        }

        public static async Task<Result<T, Error>> Success<T>(this Task<T> valueTask)
        {
            var value = await valueTask;
            return Result.Success<T, Error>(value);
        }

        public static async Task<Result<IEnumerable<T>, Error>> Success<T>(this Task<IList<T>> valueTask)
        {
            var value = await valueTask;
            return Result.Success<IEnumerable<T>, Error>(value);
        }

        public static async Task<Result<IEnumerable<T>, Error>> Success<T>(this Task<List<T>> valueTask)
        {
            var value = await valueTask;
            return Result.Success<IEnumerable<T>, Error>(value);
        }

        public static Result<TInput, Error> SuccessIf<TInput, TOutput>(this TInput value, bool condition, string errorMessage)
            where TOutput : Error
        {
            if (condition)
                return Result.Success<TInput, Error>(value);

            return Result.Failure<TInput, Error>((TOutput)Activator.CreateInstance(typeof(TOutput), errorMessage));
        }
    }
}