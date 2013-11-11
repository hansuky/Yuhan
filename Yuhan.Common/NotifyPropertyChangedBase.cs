using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Yuhan.Common.Models
{
    [DataContract]
    public class NotifyPropertyChangedBase : NotificationObject
    {
        #region Methods

        protected virtual bool ChangedPropertyChanged<T>
            (string propertyName, ref T oldValue, ref T newValue)
        {
            if (oldValue == null && newValue == null)
            {
                return false;
            }

            if ((oldValue == null && newValue != null)
                || !oldValue.Equals((T)newValue))
            {
                oldValue = newValue;
                RaisePropertyChanged(propertyName);
                return true;
            }
            
            return false;
        }
        
        #endregion
    }
}
