namespace Seed.Domain.Pagination
{
    using System;

    /// <summary> Wrapper for common pagination parameters </summary>
    public class PaginationParameters
    {
        private const int DefaultPageSize = 15;
        private const int MaxPageSize = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationParameters"/> class.
        /// </summary>
        /// <param name="pageSize"> Number of results per page </param>
        /// <param name="pageNumber"> Requested page number </param>
        public PaginationParameters(int pageSize, int pageNumber)
        {
            PageSize = pageSize <= 0 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);
            PageNumber = pageNumber <= 0 ? 1 : pageNumber;
        }

        /// <summary> Number of results per page </summary>
        public int PageSize { get; }

        /// <summary> Requested page number </summary>
        public int PageNumber { get; }
    }
}
