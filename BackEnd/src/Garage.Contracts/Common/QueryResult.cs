using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.Common
{
    public class QueryResult<T> where T : class
    {
        public IEnumerable<T> Items { get; private set; }

        /// <summary>
        /// Gets total number of items (useful when paging is used, otherwise 0)
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// Gets current page nubmer used to get items (useful when paging is used)
        /// </summary>
        public int PageNumber { get; private set; }

        /// <summary>
        /// Get page size used to get items (useful when paging is used)
        /// </summary>
        public int PageSize { get; private set; }


        public QueryResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            if (totalCount < 0)
                throw new ArgumentOutOfRangeException("totalCount", totalCount, "Incorrect value.");

            Items = items ?? throw new ArgumentNullException("items");
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public void AddItems(IEnumerable<T> items)
        {
            this.Items = items;
        }
    }
}

