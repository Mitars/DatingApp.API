namespace DatingApp.API.Dtos
{
    /// <summary>
    /// The pagination header class
    /// </summary>
    public class PaginationHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationHeader"/> class.
        /// </summary>
        /// <param name="currentPage">The page that is being considered.</param>
        /// <param name="itemsPerPage">The size for an individual page.</param>
        /// <param name="totalItems">The total number of items.</param>
        /// <param name="totalPages">The total number of pages.</param>
        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = totalItems;
            this.TotalPages = totalPages;
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
    }
}