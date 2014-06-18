using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Common.Helpers
{
    public class PaginatedList<T> : List<T>, IPager
    where T : class
    {
        public int PageIndex { get; protected set; }
        public int PageSize { get; protected set; }
        public int TotalCount { get; protected set; }
        private int? _TotalPages;
        public int TotalPages
        {
            get
            {
                if (_TotalPages.HasValue)
                    return _TotalPages.Value;
                return (int)Math.Ceiling(TotalCount / (double)PageSize);
            }
            protected set
            {
                _TotalPages = value;
            }
        }

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
