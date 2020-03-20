using System;
using System.Collections.Generic;

namespace DatingApp.Shared.FunctionalExtensions
{
    /// <summary>
    /// The Linq extension methods.
    /// </summary>
    public static class Linq
    {
        /// <summary>
        /// Iterates over the given array and executes the specified action.
        /// </summary>
        /// <param name="array">The array over which to iterate.</param>
        /// <param name="action">The action to perform.</param>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The updated list.</returns>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> array, Action<T> action)
        {
            foreach (var element in array)
                action(element);

            return array;
        }
    }
}