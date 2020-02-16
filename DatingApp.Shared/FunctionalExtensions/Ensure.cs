using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace DatingApp.Shared.FunctionalExtensions
{
    public static partial class FunctionalExtensions
    {
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
                return Result.Failure<T, E>(predicateResult.Error);

            if (predicateResult.Value == null)
                return Result.Failure<T, E>(error);

            return result;
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
                return Result.Failure<T, E>(predicateResult.Error);

            if (predicateResult.Value != null)
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
                return Result.Failure<T, E>(error);

            return result;
        }  

        public static async Task<Result<T, E>> EnsureEqual<T, K, E>(
            this Task<Result<T, E>> resultTask,
            Func<T, Task<Result<K, E>>> predicate,
            Func<T, Task<Result<K, E>>> comparativeValue,
            E error) where K : class 
        {
            var result = await resultTask;

            if (result.IsFailure)
                return result;

            Result<K, E> predicateResult = await predicate(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);

            if (predicateResult.IsFailure)
                return Result.Failure<T, E>(predicateResult.Error);

            Result<K, E> comparativeValueResult = await predicate(result.Value).ConfigureAwait(Result.DefaultConfigureAwait);                

            if (comparativeValueResult.IsFailure)
                return Result.Failure<T, E>(comparativeValueResult.Error);

            if (predicateResult.Value != comparativeValueResult.Value)
                return Result.Failure<T, E>(error);

            return result;
        }  
    }
}