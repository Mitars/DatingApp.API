using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using DatingApp.Shared.ErrorTypes;

namespace DatingApp.Shared
{
    /// <summary>
    /// Functional programming extension class.
    /// </summary>
    public static class FunctionalExtensions
    {
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
            {
                return Result.Success<T, Error>(value);
            }

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
        
        public static async Task<Result<T, E>> EnsureNull<T, K, E>(
            this Result<T, E> result,
            Func<T, Task<Result<K, E>>> predicate,
            E error)
        {
            if (result.IsFailure)
                return result;

            Result<K, E> predicateResult = await predicate(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (predicateResult.IsFailure)
            {
                var result2 = Result.Failure<T, E>(predicateResult.Error);
                return result2;
            }

            if (predicateResult.Value != null)
                return Result.Failure<T, E>(error);

            return result;
        }
        
        public static async Task<Result<T, E>> EnsureNotNull<T, K, E>(
            this Task<Result<T, E>> resultTask,
            Func<T, Task<Result<K, E>>> predicate,
            E error)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            Result<K, E> predicateResult = await predicate(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (predicateResult.IsFailure)
            {
                var result2 = Result.Failure<T, E>(predicateResult.Error);
                return result2;
            }

            if (predicateResult.Value == null)
                return Result.Failure<T, E>(error);

            return result;
        }
        
        public static async Task<Result<T, E>> Ensure<T, E>(
            this Task<Result<T, E>> resultTask,
            Func<Result<T, E>, bool> predicate,
            E error)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            if (predicate(result))
            {
                var result2 = Result.Failure<T, E>(error);
                return result2;
            }

            return result;
        }  

        public static async Task<Result<T, E>> EnsureEqual<T, K, E>(
            this Task<Result<T, E>> resultTask,
            Func<T, Task<Result<K, E>>> predicate,
            Func<T, Task<Result<K, E>>> comparativeValue,
            E error)
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            Result<K, E> predicateResult = await predicate(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (predicateResult.IsFailure)
            {
                var result2 = Result.Failure<T, E>(predicateResult.Error);
                return result2;
            }

            if (predicateResult.Value == null)
                return Result.Failure<T, E>(error);

            return result;
        }  
     
        public static Result<K, E> AutoMap<T, K, E>(this Result<T, E> result, Func<T, K> mapper)
        {
            return Result.Success<K, E>(mapper(result.Value));
        }

        public static async Task<Result<K, E>> AutoMap<T, K, E>(this Task<Result<T, E>> resultTask, Func<T, K> mapper)
        {
            var result = await resultTask;
            return Result.Success<K, E>(mapper(result.Value));
        }

        public static async Task<Result<IEnumerable<K>, E>> AutoMap<T, K, E>(this Task<Result<T, E>> resultTask, Func<T, IEnumerable<K>> mapper)
        {
            var result = await resultTask;
            return Result.Success<IEnumerable<K>, E>(mapper(result.Value));
        }

        public static K Finally<T, K, E>(this Result<T, E> result, Func<Result<T, E>, K> func, Func<Result<T, E>, K> func2)
        {
                return result.IsSuccess ? func(result) : func2(result);
        }

        public static async Task<K> Finally<T, K, E>(this Task<Result<T, E>> result, Func<Result<T, E>, K> func, Func<Result<T, E>, K> func2)
        {
            var a = await result;
            return a.IsSuccess ? func(a) : func2(a);
        }

        public static async Task<Result<T, E>> TapIf<T, E>(this Task<Result<T, E>> resultTask, Func<T, bool> condition, Action<T> action)
        {
            var result = await resultTask;
            if (result.IsSuccess && condition(result.Value))
            {
                action(result.Value);
            }

            return result;
        }

        public static async Task<Result<None, Error>> DropResult<T>(this Task<Result<T, Error>> result)
        {
            if ((await result).IsSuccess)
            {
                return Result.Success<None, Error>(new None());
            } else {
                return Result.Failure<None, Error>((await result).Error);
            }
        }

        public static async Task<Result<T, E>> Tap<T, E>(this Result<T, E> result, Func<T, Task<Result<None, E>>> func)
        {
            if (result.IsSuccess)
            {
                var potato = await func(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);
                if (potato.IsFailure)
                {
                    return Result.Failure<T, E>(potato.Error);
                }
            }

            return result;
        }

        public static async Task<Result<T, E>> Tap<T, E>(this Task<Result<T, E>> resultTask, Func<T, Task<Result<None, E>>> func)
        {
            var result = await resultTask;
            if (result.IsSuccess)
            {
                var potato = await func(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);
                if (potato.IsFailure)
                {
                    return Result.Failure<T, E>(potato.Error);
                }
            }

            return result;
        }

        // public static async Task<Result<T, E>> Tap<T, E>(this Task<Result<T, E>> resultTask, Action<T> func)
        // {
        //     var result = await resultTask;
        //     if (result.IsSuccess)
        //     {
        //         func(result.Value);
        //     }

        //     return result;
        // }
                
        public static async Task<Result<T, E>> TapZ<T, E, _>(
            this Result<T, E> result,
            Func<T, Task<Result<_, E>>> predicate)
        {
            if (result.IsSuccess)
            {
                Result<_, E> predicateResult = await predicate(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);
                if (predicateResult.IsFailure)
                {
                    return Result.Failure<T, E>(predicateResult.Error);
                }
            }

            return result;
        }   
    }
}