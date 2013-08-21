using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Yuhan.WPF.MessageBox.Demo
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DefaultMsgBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxDialog.Show("Instruction Heading[Sample]");
        }

        private void CaptionMsgBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxDialog.Show("Instruction Heading[Sample]", "Caption[Sample]");
        }

        private void YesNoMsgBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxDialog.Show("Instruction Heading[Sample]", "Caption[Sample]", MessageBoxWindowButtons.YesNoCancel);
        }

        private void IConMsgBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxDialog.Show("Instruction Heading[Sample]", "Caption[Sample]", MessageBoxWindowButtons.OKCancel, MessageBoxWindowIcons.Warning);
        }

        private void FullMsgBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxDialog.Show(new MessageBoxViewModel()
            {
                Caption = "Caption[Sample]",
                Header = "Instruction Heading[Sample]",
                HeaderIcon = MessageBoxWindowIcons.Information,
                Description = "Instruction[Sample]",
                Details = "Additional Details Text[Sample]",
                FooterText = "FooterText[Sample]",
                FooterIcon = MessageBoxWindowIcons.Shield,
            });
        }

        private void ProgressMsgBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            ProgressMessageViewModel viewModel = new ProgressMessageViewModel()
            {
                Caption = "Caption[Sample]",
                Header = "Instruction Heading[Sample]",
                HeaderIcon = MessageBoxWindowIcons.Information,
                Description = "Instruction[Sample]",
                Details = "Additional Details Text[Sample]",
                FooterText = "FooterText[Sample]",
                FooterIcon = MessageBoxWindowIcons.Shield,
                IsDialog = false
            };
            MessageBoxDialog.Show(viewModel);
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (obj, evt) =>
            {
                for (int i = 0; i < 100; i++)
                {
                    Thread.Sleep(300);
                    viewModel.Percentage = i;
                }
            };
            worker.RunWorkerAsync();
        }
    }
}
