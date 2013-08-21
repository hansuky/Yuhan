using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Yuhan.WPF.CustomWindow
{
    public enum WindowButtonState { Normal, Disabled, None }

    // This is converter of 'Caption' property. It enables to simply set Caption="string value"
    // in the Xaml definition and the convertor then converts the text into a TextBlock
    public class TypeConverterStringToUIElement : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) ? true : false;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            // unsupported type
            if (value.GetType() != typeof(string))
                return base.ConvertFrom(context, culture, value);

            // string
            TextBlock textBlock = new TextBlock();
            textBlock.Text = (string)value;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(3, 0, 0, 0);

            return textBlock;
        }
    }
}