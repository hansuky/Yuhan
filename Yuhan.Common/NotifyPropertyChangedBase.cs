using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Yuhan.Common.Models
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Memebers
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Methods
        //protected virtual bool ChangedPropertyChanged<T>
        //    (T property, ref T oldValue, ref T newValue)
        //{
        //    return ChangedPropertyChanged<T>
        //        //(property.GetType().UnderlyingSystemType.Name, ref oldValue, ref newValue);
        //        (ObjectExtension.GetPropertyName(() => property), ref oldValue, ref newValue);
        //}

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
        protected virtual void RaisePropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //protected virtual void RaisePropertyChanged<T>(T property)
        //{
        //    RaisePropertyChanged(property.GetType().UnderlyingSystemType.Name);
        //}

        #endregion
    }
}
