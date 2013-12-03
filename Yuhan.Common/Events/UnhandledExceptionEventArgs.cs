using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Common.Events
{
    public class UnhandledExceptionEventArgs : EventArgs
    {
        public String Message { get; set; }
        public String InnerMessage { get; set; }
        private Exception _exception;

        public Exception Exception
        {
            get { return _exception; }
            set
            {
                _exception = value;
                if (_exception != null)
                {
                    Message = _exception.Message;
                    if (_exception.InnerException != null)
                        InnerMessage = _exception.InnerException.Message;
                }

            }
        }

        public UnhandledExceptionEventArgs(Exception exception = null)
        {
            this.Exception = exception;
        }
    }

    public delegate void UnhandledExceptionEventHandler(Object sender, UnhandledExceptionEventArgs e);
}
