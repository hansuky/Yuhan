using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Yuhan.WPF.DsxGridCtrl
{
    public static class StyleExtensions
    {
        public static object GetStylePropertyValue<T>(this Style preferedStyle, String propertyName, Style fallbackStyle, object defaultValue)
        {
            object _result = null;

            if (preferedStyle != null)
            {
                _result = preferedStyle.GetStylePropertyValue<T>(propertyName, null);
            }
            if (_result != null)
            {
                return _result;
            }
            return fallbackStyle.GetStylePropertyValue<T>(propertyName, defaultValue);
        }

        public static object GetStylePropertyValue<T>(this Style styleReference, String propertyName, object defaultValue)
        {
            if (styleReference != null)
            {
                foreach (Setter _setter in styleReference.Setters)
                {
                    if (_setter.Property.Name.Equals(propertyName))
                    {
                        return (T)_setter.Value;
                    }
                }
            }
            return defaultValue;
        }

        public static Setter GetStyleSetter(this Style preferredStyle, DependencyProperty property, Style fallbackStyle)
        {
            Setter _result = null;

            if (preferredStyle != null)
            {
                _result = preferredStyle.GetStyleSetter(property);
            }
            if (_result != null)
            {
                return _result;
            }
            return fallbackStyle.GetStyleSetter(property);
        }

        public static Setter GetStyleSetter(this Style styleReference, DependencyProperty property)
        {
            if (styleReference != null)
            {
                foreach (Setter _setter in styleReference.Setters)
                {
                    if (_setter.Property.Name.Equals(property))
                    {
                        return _setter;
                    }
                }
            }
            return null;
        }

    }
}
