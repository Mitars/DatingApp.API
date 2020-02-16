using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.Shared.FunctionalExtensions
{
    public static partial class FunctionalExtensions
    {
        // Success
        public static Result<T, Error> Success<T>(this T value) {
            return Result.Success<T, Error>(value);
        }

        public static Result<IEnumerable<T>, Error> Success<T>(this IEnumerable<T> value) {
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

        public static Result<T, Error> SuccessIf<T, K>(this T value, bool condition, string errorMessage)
            where K : Error
        {
            if (condition)
                return Result.Success<T, Error>(value);

            return Result.Failure<T, Error>((K)Activator.CreateInstance(typeof(K), errorMessage));
        }

        public static Result<T, Error> Failure<T, K>(this T value, string errorMessage)
            where K : Error
        {
            return Result.Failure<T, Error>((K)Activator.CreateInstance(typeof(K), errorMessage));
        }

        public static async Task<Result<T, Error>> Failure<T>(this Task<T> valueTask, string errorMessage)
        {
            var value = await valueTask;
            return Result.Failure<T, Error>(new Error(errorMessage));
        }
    }
}