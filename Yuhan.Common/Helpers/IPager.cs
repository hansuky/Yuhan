using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Common.Helpers
{
    public interface IPager
    {
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
    }
}
