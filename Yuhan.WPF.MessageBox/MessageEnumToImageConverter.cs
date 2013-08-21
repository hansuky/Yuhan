using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Yuhan.WPF.MessageBox
{
    public class MessageEnumToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null &&
                value.GetType().Equals(typeof(MessageBoxWindowIcons)))
            {
                MessageBoxWindowIcons icon = (MessageBoxWindowIcons)value;
                switch (icon)
                {
                    case MessageBoxWindowIcons.None:
                        return null;
                    case MessageBoxWindowIcons.Information:
                        return "Images/Information.png";
                    case MessageBoxWindowIcons.Question:
                        return "Images/Question.png";
                    case MessageBoxWindowIcons.Shield:
                        return "Images/Shield.png";
                    case MessageBoxWindowIcons.Stop:
                        return "Images/Stop.png";
                    case MessageBoxWindowIcons.Warning:
                        return "Images/Warning.png";
                    default:
                        break;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
