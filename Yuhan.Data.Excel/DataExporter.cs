using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Yuhan.Common.Events;

namespace Yuhan.Data
{
    public abstract class DataExporter
    {
        public abstract void Export<T>(IEnumerable<T> collection)
            where T : class, new();

        protected void OnExportCompleted(Object obj)
        {
            if (ExportCompleted != null)
                ExportCompleted(this, new ResultEventArgs() { Object = obj });
        }

        protected void OnExportFailed(Object obj)
        {
            ResultEventArgs arg = new ResultEventArgs(false);
            if (obj.GetType().Equals(typeof(Exception)))
                arg.Exception = obj as Exception;
            if (ExportCompleted != null)
                ExportCompleted(this, arg);
        }

        public event EventHandler<ResultEventArgs> ExportCompleted;
    }
}
