using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace Yuhan.WPF.CustomWindow
{
    public class WindowMaximizeButton : WindowButton
    {
        public WindowMaximizeButton()
        {
            // open resource where in XAML are defined icons and colors
            Stream resourceStream = Application.GetResourceStream(new Uri("pack://application:,,,/Yuhan.WPF.CustomWindow;component/ButtonIcons.xaml")).Stream;
            ResourceDictionary resourceDictionary = (ResourceDictionary)XamlReader.Load(resourceStream);

            this.Content = resourceDictionary["WindowButtonMaximizeIcon"];
            this.ContentDisabled = resourceDictionary["WindowButtonMaximizeIconDisabled"];
        }
    }
}
