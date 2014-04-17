using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Yuhan.Common.Models;

namespace Yuhan.WPF.ViewModels
{
    public class ViewModelBase : NotifyPropertyChangedBase, IDataErrorInfo
    {
        protected override bool ChangedPropertyChanged<T>(string propertyName, ref T oldValue, ref T newValue)
        {
            Validate(propertyName, newValue);
            return base.ChangedPropertyChanged<T>(propertyName, ref oldValue, ref newValue);
        }

        protected virtual void Validate(string propertyName, object value)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            string error = string.Empty;

            var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>(2);

            bool result = Validator.TryValidateProperty(
                value,
                new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                },
                results);

            if (!result && (value == null || ((value is int || value is long) && (int)value == 0) || (value is decimal && (decimal)value == 0)))
                return;

            if (!result)
            {
                System.ComponentModel.DataAnnotations.ValidationResult validationResult = results.First();
                if (!errorMessages.ContainsKey(propertyName))
                    errorMessages.Add(propertyName, validationResult.ErrorMessage);
            }

            else if (errorMessages.ContainsKey(propertyName))
                errorMessages.Remove(propertyName);
        }

        #region IDataErrorInfo

        public virtual string Error
        {
            get { throw new NotImplementedException(); }
        }

        private Dictionary<string, string> errorMessages = new Dictionary<string, string>();

        public virtual string this[string columnName]
        {
            get
            {
                if (errorMessages.ContainsKey(columnName))
                    return errorMessages[columnName];
                return null;

            }
        }

        #endregion
    }
}
