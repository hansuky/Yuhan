using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Common.Events
{
    public class TransactionEventArgs : EventArgs
    {
        public Boolean IsSuccess { get; set; }

        public String Message { get; set; }

        public TransactionEventArgs(Boolean isSuccess = true, String message = null)
            : base()
        {
            this.IsSuccess = true;
            this.Message = message;
        }

        public TransactionEventArgs(String message)
            :base()
        {
            this.IsSuccess = true;
            this.Message = message;
        }
    }

    public delegate void TransactionEventHandler(Object sender, TransactionEventArgs e);
}
