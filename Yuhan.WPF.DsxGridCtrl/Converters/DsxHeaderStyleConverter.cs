using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Controls;

namespace Yuhan.WPF.DsxGridCtrl
{
    public class DsxHeaderStyleConverter : IValueConverter
    {
        private static ResourceDictionary sStyleResources { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                object _result      = null;
                string _propertyName = parameter.ToString();

                _result = StyleHelper.FindStyleValue(value, _propertyName, DsxDataGrid.HeaderStyleProperty, DsxColumn.HeaderStyleProperty);
                    
                if (_result != null)
                {
                    return _result;
                }

                bool _isColumn = (value != null);

                // defaults

                if (sStyleResources == null)
                {
                    sStyleResources         = new ResourceDictionary();
                    sStyleResources.Source  = new Uri("/Yuhan.WPF.DsxGridCtrl;component/Themes/Styles.xaml", UriKind.Relative);
                }

                Style _headerStyle = sStyleResources["dsxHeaderStyleGray"] as Style;

                if (_headerStyle == null)
                {
                    throw new Exception("if 'dsxHeaderStyleGray' is not present in Styles.xaml, a HeaderStyle must be set at design time");
                }

                if      (_propertyName.Equals(Control           .BackgroundProperty         .Name))         return _headerStyle.GetStylePropertyValue<Brush>                ("Background",          Brushes.LightGray);
                else if (_propertyName.Equals(Control           .ForegroundProperty         .Name))         return _headerStyle.GetStylePropertyValue<Brush>                ("Foreground",          SystemColors.ControlTextBrush);
                else if (_propertyName.Equals(Border            .BorderBrushProperty        .Name))         return _headerStyle.GetStylePropertyValue<Brush>                ("BorderBrush",         Brushes.DarkGray);
                else if (_propertyName.Equals(FrameworkElement  .HorizontalAlignmentProperty.Name))         return _headerStyle.GetStylePropertyValue<HorizontalAlignment>  ("HorizontalAlignment", HorizontalAlignment.Left);
                else if (_propertyName.Equals(FrameworkElement  .VerticalAlignmentProperty  .Name))         return _headerStyle.GetStylePropertyValue<VerticalAlignment>    ("VerticalAlignment",   VerticalAlignment.Center);
                else if (_propertyName.Equals(Border            .CornerRadiusProperty       .Name))         return _headerStyle.GetStylePropertyValue<CornerRadius>         ("CornerRadius",        new CornerRadius(0,0,0,0));
                else if (_propertyName.Equals(Border            .BorderThicknessProperty    .Name))         return _headerStyle.GetStylePropertyValue<Thickness>            ("BorderThickness",     new Thickness   (0,1,0,1));
                else if (_propertyName.Equals(Control           .PaddingProperty            .Name))         return _headerStyle.GetStylePropertyValue<Thickness>            ("Padding",             new Thickness   (6,0,6,0));
                else if (_propertyName.Equals(FrameworkElement  .MarginProperty             .Name))         return _headerStyle.GetStylePropertyValue<Thickness>            ("Margin",              new Thickness   (0,0,0,0));

                else if (_isColumn && _propertyName.Equals(DsxHeaderStyle.PressedBackgroundProperty.Name))  return _headerStyle.GetStylePropertyValue<Brush>                ("PressedBackground",   Brushes.Gray);
                else if (_isColumn && _propertyName.Equals(DsxHeaderStyle.GripperBrushProperty.Name))       return _headerStyle.GetStylePropertyValue<Brush>                ("GripperBrush",        Brushes.DimGray);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
