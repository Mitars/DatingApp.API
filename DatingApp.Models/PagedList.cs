using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Models
{
    /// <summary>
    /// The paged list class.
    /// </summary>
    /// <typeparam name="T">The type contained within the paged list.</typeparam>
    public class PagedList<T> : List<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class.
        /// </summary>
        /// <param name="items">The items which are paged.</param>
        /// <param name="currentPage">The page that is being considered.</param>
        /// <param name="itemsPerPage">The size for an individual page.</param>
        /// <param name="totalItems">The total number of items.</param>
        public PagedList(List<T> items, int currentPage, int itemsPerPage, int totalItems)
        {
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = totalItems;
            this.TotalPages = (totalItems / (itemsPerPage - 1)) + 1;
            this.AddRange(items);
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets or sets the number of elements per page.
        /// </summary>
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the total number of items.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Creates the list of items.
        /// </summary>
        /// <param name="source">The source queryable list of items.</param>
        /// <param name="pageNumber">The page that is being considered.</param>
        /// <param name="pageSize">The size for an individual page.</param>
        /// <returns></returns>
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var totalItems = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, pageNumber, pageSize, totalItems);
        }
    }
}