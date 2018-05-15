using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seed.Domain.Pagination
{
    /// <summary> Wrapper for results exposed as pages </summary>
    /// <typeparam name="T"> Object type contained inside page's collection </typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedResult{T}"/> class.
        /// </summary>
        /// <param name="values"> Values contained in this particular page </param>
        /// <param name="totalPages"> Number of pages available for the original query </param>
        /// <param name="totalItems"> Number of total items in the original query </param>
        internal PaginatedResult(IList<T> values, int totalPages, int totalItems)
        {
            Values = values;
            TotalPages = totalPages;
            TotalItems = totalItems;
        }

        /// <summary> Values contained in this particular page </summary>
        public IList<T> Values { get; }

        /// <summary> Number of pages available for the original query </summary>
        public int TotalPages { get; }

        /// <summary> Number of items available for the original query </summary>
        public int TotalItems { get; }

        internal static async Task<PaginatedResult<T>> FromQueryAsync(IQueryable<T> query, PaginationParameters parameters)
        {
            var count = await query.CountAsync();
            var values = await query.Skip((parameters.PageNumber - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync();
            var totalPages = (int)Math.Ceiling((double)count / parameters.PageSize);
            return new PaginatedResult<T>(values, totalPages, count);
        }
    }
}
