using System;
using System.Windows;

namespace Yuhan.WPF.TextEditor.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region Event Handlers

        /// <summary>
        /// Forces an update of the FsRichTextBox.Document property.
        /// </summary>
        private void OnForceUpdateClick(object sender, RoutedEventArgs e)
        {
            this.EditBox.UpdateDocumentBindings();
        }

        #endregion
    }
}
