using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Common.Pagination
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }

        public int PageNumber { get; }//This tells the frontend which page the API returned.

        public int PageSize { get; }//This represents the maximum number of products requested per page.

        public int TotalCount { get; }

        public int TotalPages =>//This line calculates how many pages are available.
            (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPreviousPage => PageNumber > 1; //If the user is on page 1:so it is false

        public bool HasNextPage => PageNumber < TotalPages;

        public PagedResult(
            IReadOnlyList<T> items,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            ArgumentNullException.ThrowIfNull(items);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageNumber);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);
            ArgumentOutOfRangeException.ThrowIfNegative(totalCount);

            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
    }
}
