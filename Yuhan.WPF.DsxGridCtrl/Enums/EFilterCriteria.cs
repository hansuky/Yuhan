using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.WPF.DsxGridCtrl
{
    public enum EFilterCriteria
    {
        Equals              = 0,
        NotEquals           = 1,
        Contains            = 2,
        NotContains         = 3,
        StartsWith          = 4,
        EndsWith            = 5,
        Greater             = 6,
        GreaterOrEqual      = 7,
        Smaller             = 8,
        SmallerOrEqual      = 9,
    }
}
