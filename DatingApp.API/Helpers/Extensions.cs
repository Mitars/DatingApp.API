using System;
using DatingApp.API.Dtos;
using DatingApp.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API.Helpers
{
    /// <summary>
    /// The extensions helper class.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Generates the password hash and salt using the specified password.
        /// </summary>
        /// <param name="password">The password used to generate a hash and salt.</param>
        /// <returns>A tuple with the password hash and salt.</returns>
        public static (byte[] passwordHash, byte[] passwordSalt) GeneratePasswordHashSalt(this string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                byte[] passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                byte[] passwordSalt = hmac.Key;
                return (passwordHash, passwordSalt);
            }
        }

        /// <summary>
        /// Adds pagination to the HTTP response.
        /// </summary>
        /// <param name="response">The HTTP response to which to add pagination.</param>
        /// <param name="pagedList">The paged list.</param>
        /// <typeparam name="T">The type of data.</typeparam>
        public static void AddPagination<T>(this HttpResponse response, PagedList<T> pagedList) {
            response.AddPagination(pagedList.CurrentPage, pagedList.ItemsPerPage, pagedList.TotalItems, pagedList.TotalPages);
        }

        /// <summary>
        /// Adds pagination to the response.
        /// </summary>
        /// <param name="response">The HTTP response.</param>
        /// <param name="currentPage">The current page.</param>
        /// <param name="itemsPerPage">The number of items per page.</param>
        /// <param name="totalItems">The total number of items.</param>
        /// <param name="totalPages">The number of pages.</param>
        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }

        /// <summary>
        /// Gets the age using the specified date time.
        /// </summary>
        /// <param name="theDateTime">The date time which should be converted to the age.</param>
        /// <returns>The age.</returns>
        public static int Age(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            if (theDateTime.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            return age;
        }
    }
}
