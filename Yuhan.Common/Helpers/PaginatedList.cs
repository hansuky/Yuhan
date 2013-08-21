using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Common.Helpers
{
    public class PaginatedList<T> : List<T>, IPager
    where T : class
    {
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get { return (int)Math.Ceiling(TotalCount / (double)PageSize); } }

        public PaginatedList(IEnumerable<T> source, int pageIndex = 0, int pageSize = 10)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = source.Count();

            this.AddRange(source.Skip((PageIndex - 1) * PageSize).Take(PageSize));
        }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 0); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex + 1 < TotalPages); }
        }
    }
}
