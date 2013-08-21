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
    public class DsxFooterStyleConverter : IValueConverter
    {
        private static ResourceDictionary sStyleResources { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                object _result      = null;
                string _propertyName = parameter.ToString();

                _result = StyleHelper.FindStyleValue(value, _propertyName, DsxDataGrid.FooterStyleProperty, DsxColumn.FooterStyleProperty);
                    
                if (_result != null)
                {
                    return _result;
                }

                // defaults

                if (sStyleResources == null)
                {
                    sStyleResources         = new ResourceDictionary();
                    sStyleResources.Source  = new Uri("/Yuhan.WPF.DsxGridCtrl;component/Themes/Styles.xaml", UriKind.Relative);
                }

                Style _footerStyle = sStyleResources["dsxFooterStyleGray"] as Style;

                if (_footerStyle == null)
                {
                    throw new Exception("if 'dsxFooterStyleGray' is not present in Styles.xaml, a FooterStyle must be set at design time");
                }

                if      (_propertyName.Equals(Control           .BackgroundProperty         .Name))         return _footerStyle.GetStylePropertyValue<Brush>                ("Background",          Brushes.WhiteSmoke);
                else if (_propertyName.Equals(Control           .ForegroundProperty         .Name))         return _footerStyle.GetStylePropertyValue<Brush>                ("Foreground",          SystemColors.ControlTextBrush);
                else if (_propertyName.Equals(Border            .BorderBrushProperty        .Name))         return _footerStyle.GetStylePropertyValue<Brush>                ("BorderBrush",         Brushes.DarkGray);
                else if (_propertyName.Equals(FrameworkElement  .HorizontalAlignmentProperty.Name))         return _footerStyle.GetStylePropertyValue<HorizontalAlignment>  ("HorizontalAlignment", HorizontalAlignment.Left);
                else if (_propertyName.Equals(FrameworkElement  .VerticalAlignmentProperty  .Name))         return _footerStyle.GetStylePropertyValue<VerticalAlignment>    ("VerticalAlignment",   VerticalAlignment.Center);
                else if (_propertyName.Equals(Border            .CornerRadiusProperty       .Name))         return _footerStyle.GetStylePropertyValue<CornerRadius>         ("CornerRadius",        new CornerRadius(0,0,0,0));
                else if (_propertyName.Equals(Border            .BorderThicknessProperty    .Name))         return _footerStyle.GetStylePropertyValue<Thickness>            ("BorderThickness",     new Thickness   (0,1,0,1));
                else if (_propertyName.Equals(Control           .PaddingProperty            .Name))         return _footerStyle.GetStylePropertyValue<Thickness>            ("Padding",             new Thickness   (6,0,6,0));
                else if (_propertyName.Equals(FrameworkElement  .MarginProperty             .Name))         return _footerStyle.GetStylePropertyValue<Thickness>            ("Margin",              new Thickness   (0,0,0,0));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
