using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yuhan.Common.Helpers
{
    public class RuleViolation
    {
        public string ErrorMessage { get; private set; }

        public string propertyName;
        public string PropertyName
        {
            get
            {
                if (propertyName == null)
                    propertyName = "";
                return propertyName;
            }
            private set { propertyName = value; }
        }

        public RuleViolation(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public RuleViolation(string errorMessage, string propertyName)
        {
            ErrorMessage = errorMessage;
            PropertyName = propertyName;
        }
    }
}
