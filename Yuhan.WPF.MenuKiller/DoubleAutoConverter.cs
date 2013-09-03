using System;
using System.ComponentModel;
using System.Globalization;

namespace Yuhan.WPF.MenuKiller
{
    public class DoubleAutoConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string) || sourceType == typeof(double))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                double dValue;

                if (String.Compare((string)value, "auto", true) == 0)
                {
                    dValue = Double.NaN;
                }
                else 
                {
                    // Don't catch the exception
                    dValue = Double.Parse((string)value);
                }

                return dValue;
            }

            // No need to handle the trivial case manually:
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

}
