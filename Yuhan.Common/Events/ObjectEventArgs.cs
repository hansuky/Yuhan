using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Common.Events
{
    public class ResultEventArgs : EventArgs
    {
        private Boolean isSuccess;

        public Boolean IsSuccess
        {
            get { return isSuccess; }
            set
            {
                isSuccess = value;
                if (value)
                    this.Exception = null;
            }
        }


        public Object Object { get; set; }

        private Exception exception;

        public Exception Exception
        {
            get { return exception; }
            set
            {
                exception = value;
                if (value != null)
                    this.IsSuccess = false;
            }
        }


        public ResultEventArgs(Boolean isSuccess = true)
            : base()
        {
            this.IsSuccess = isSuccess;
        }
        public ResultEventArgs(Exception exception)
            : base()
        {
            this.Exception = exception;
            this.IsSuccess = false;
        }
    }
}
