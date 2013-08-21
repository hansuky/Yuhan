using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;
using Yuhan.WPF.DsxGridCtrl;
using System.Windows.Controls.Primitives;

namespace Yuhan.WPF.DsxGridCtrl.Demo
{
    public partial class MainWindow : Window
    {
        #region ctors

        public MainWindow()
        {
            InitializeComponent();

            LoadDataXml();
        }
        #endregion


        #region members / properties

        private List<Customer>  Customers   { get; set; }

        #endregion


        #region EventConsumer - OnLoadData

        private void OnLoadData(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.ItemsSource = this.Customers;
        }
        #endregion

        #region EventConsumer - OnClearData

        private void OnClearData(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.ItemsSource = null;
        }
        #endregion

        #region EventConsumer - OnSetTheme

        private void OnSetTheme(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.SetTheme( (sender as RadioButton).Content.ToString() );
        }
        #endregion

        #region EventConsumer - OnToggleVLines

        private void OnToggleVLines(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.VerticalGridLinesIsVisible = (bool)(sender as ToggleButton).IsChecked;
        }
        #endregion

        #region EventConsumer - OnToggleHLines

        private void OnToggleHLines(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.HorizontalGridLinesIsVisible = (bool)(sender as ToggleButton).IsChecked;
        }
        #endregion

        #region EventConsumer - OnToggleHeader

        private void OnToggleHeader(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.HeaderVisibility = (bool)(sender as ToggleButton).IsChecked ? EVisibility.Visible : EVisibility.Collapsed;
        }
        #endregion

        #region EventConsumer - OnToggleFilter

        private void OnToggleFilter(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.FilterVisibility = (bool)(sender as ToggleButton).IsChecked ? EVisibility.Auto : EVisibility.Collapsed;
        }
        #endregion

        #region EventConsumer - OnToggleFooter

        private void OnToggleFooter(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.FooterVisibility = (bool)(sender as ToggleButton).IsChecked ? EVisibility.Auto : EVisibility.Collapsed;
        }
        #endregion

        #region EventConsumer - OnToggleCellAdorner

        private void OnToggleCellAdorner(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.CellAdornerIsVisible = (bool)(sender as ToggleButton).IsChecked;
        }
        #endregion

        #region EventConsumer - OnToggleCellEditing

        private void OnToggleCellEditing(object sender, RoutedEventArgs e)
        {
            this.dataGrid1.CellEditingIsEnabled = (bool)(sender as ToggleButton).IsChecked;
        }
        #endregion


        #region Method - LoadDataXml

        private void LoadDataXml()
        {
            Stream          _stream    = Application.GetResourceStream( new Uri("pack://application:,,,/DataXml/Northwind.xml") ).Stream;
            XElement        _xmlRoot   = XElement.Load(_stream, LoadOptions.None);

            this.Customers  = (from xmlCustomer 
                                   in _xmlRoot.Elements("Customers") 
                               select new Customer(xmlCustomer)).ToList();
        }
        #endregion

    }
}
