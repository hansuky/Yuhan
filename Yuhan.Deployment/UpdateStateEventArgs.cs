using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Deployment
{
    public class UpdateStateEventArgs : EventArgs
    {
        public UpdateStateType StateType { get; set; }
        public Exception Exception { get; set; }
        public String Message { get; set; }

        public UpdateStateEventArgs(Exception ex = null)
        {
            if (ex != null)
            {
                StateType = UpdateStateType.Failed;
                Exception = ex;
            }
        }
    }

    public enum UpdateStateType
    {
        Updated,
        ReleasedVersion,
        UpdateAvailable,
        IsUpdateRequired,
        Failed,
    }

    public delegate void UpdateStateEventHandler(Object sender, UpdateStateEventArgs e);
}
