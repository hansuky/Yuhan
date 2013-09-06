using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Yuhan.WPF.TextEditor.Demo
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            var mainWindowViewModel = new MainWindowViewModel();
            mainWindow.DataContext = mainWindowViewModel;
            mainWindow.Show();
        }
    }
}
