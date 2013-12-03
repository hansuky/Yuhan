using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Data.Excel
{
    public class ExcelAttribute : Attribute
    {
        public ExcelAttribute(Boolean isDisplay = true)
            : base()
        {
            this.IsDisplay = isDisplay;
        }
        public ExcelAttribute(String headerName)
            : base()
        {
            this.IsDisplay = true;
            this.HeaderName = headerName;
        }

        public Boolean IsDisplay { get; set; }
        public virtual String HeaderName { get; set; }
    }
}
