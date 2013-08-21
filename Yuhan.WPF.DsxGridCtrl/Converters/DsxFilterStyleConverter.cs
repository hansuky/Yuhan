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
    public class DsxFilterStyleConverter : IValueConverter
    {
        private static ResourceDictionary sStyleResources { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                object _result      = null;
                string _propertyName = parameter.ToString();

                _result = StyleHelper.FindStyleValue(value, _propertyName, DsxDataGrid.FilterStyleProperty, DsxColumn.FilterStyleProperty);
                    
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

                Style _filterStyle = sStyleResources["dsxFilterStyleGray"] as Style;

                if (_filterStyle == null)
                {
                    throw new Exception("if 'dsxFilterStyleGray' is not present in Styles.xaml, a FilterStyle must be set at design time");
                }

                if      (_propertyName.Equals(Control           .BackgroundProperty         .Name))         return _filterStyle.GetStylePropertyValue<Brush>                ("Background",          Brushes.LightGray);
                else if (_propertyName.Equals(Control           .ForegroundProperty         .Name))         return _filterStyle.GetStylePropertyValue<Brush>                ("Foreground",          SystemColors.ControlTextBrush);
                else if (_propertyName.Equals(Border            .BorderBrushProperty        .Name))         return _filterStyle.GetStylePropertyValue<Brush>                ("BorderBrush",         Brushes.Silver);
                else if (_propertyName.Equals(FrameworkElement  .HorizontalAlignmentProperty.Name))         return _filterStyle.GetStylePropertyValue<HorizontalAlignment>  ("HorizontalAlignment", HorizontalAlignment.Left);
                else if (_propertyName.Equals(FrameworkElement  .VerticalAlignmentProperty  .Name))         return _filterStyle.GetStylePropertyValue<VerticalAlignment>    ("VerticalAlignment",   VerticalAlignment.Center);
                else if (_propertyName.Equals(Border            .CornerRadiusProperty       .Name))         return _filterStyle.GetStylePropertyValue<CornerRadius>         ("CornerRadius",        new CornerRadius(2,2,2,2));
                else if (_propertyName.Equals(Border            .BorderThicknessProperty    .Name))         return _filterStyle.GetStylePropertyValue<Thickness>            ("BorderThickness",     new Thickness   (1,1,1,1));
                else if (_propertyName.Equals(Control           .PaddingProperty            .Name))         return _filterStyle.GetStylePropertyValue<Thickness>            ("Padding",             new Thickness   (6,0,6,0));
                else if (_propertyName.Equals(FrameworkElement  .MarginProperty             .Name))         return _filterStyle.GetStylePropertyValue<Thickness>            ("Margin",              new Thickness   (1,0,1,0));

                else if (_propertyName.Equals(DsxFilterStyle    .CriteriaBackgroundProperty.Name))          return _filterStyle.GetStylePropertyValue<Brush>                ("Background",          Brushes.Snow);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
