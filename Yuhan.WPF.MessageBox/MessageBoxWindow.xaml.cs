using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Yuhan.WPF.CustomWindow;
using Yuhan.WPF.MessageBox;

namespace Yuhan.WPF
{
    public partial class MessageBoxWindow : EssentialWindow
    {
        public MessageBoxViewModel ViewModel
        {
            get
            {
                return this.LayoutRoot.DataContext as MessageBoxViewModel;
            }
            set
            {
                this.LayoutRoot.DataContext = value;
                SetViewModel(value);
            }
        }

        public Boolean IsDialog
        {
            get { return this.ViewModel.IsDialog; }
            set { this.ViewModel.IsDialog = value; }
        }


        public MessageBoxWindow()
        {
            InitializeComponent();
            SetViewModel();
            
            this.DataContextChanged += MessageBoxWindow_DataContextChanged;
        }

        #region Static Properties & Method

        public static MessageBoxWindowResult Show(MessageBoxViewModel viewModel)
        {
            MessageBoxWindow window = new MessageBoxWindow();
            window.ViewModel = viewModel;
            if (viewModel.IsDialog)
                window.ShowDialog();
            else
                window.Show();
            return window.ViewModel.Result;
        }

        #endregion

        protected void SetViewModel()
        {
            if (ViewModel != null)
                SetViewModel(ViewModel);
        }

        public void SetViewModel(MessageBoxViewModel viewModel)
        {
            if (viewModel != null)
            {
                LayoutRoot.SetBinding(FrameworkElement.DataContextProperty, new Binding() { Source = ViewModel });
                switch (viewModel.Buttons)
                {
                    case MessageBoxWindowButtons.OK:
                        btnOK.Visibility = System.Windows.Visibility.Visible;
                        btnYes.Visibility = System.Windows.Visibility.Collapsed;
                        btnNo.Visibility = System.Windows.Visibility.Collapsed;
                        btnCancel.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case MessageBoxWindowButtons.OKCancel:
                        btnOK.Visibility = System.Windows.Visibility.Visible;
                        btnYes.Visibility = System.Windows.Visibility.Collapsed;
                        btnNo.Visibility = System.Windows.Visibility.Collapsed;
                        btnCancel.Visibility = System.Windows.Visibility.Visible;
                        break;
                    case MessageBoxWindowButtons.YesNo:
                        btnOK.Visibility = System.Windows.Visibility.Collapsed;
                        btnYes.Visibility = System.Windows.Visibility.Visible;
                        btnNo.Visibility = System.Windows.Visibility.Visible;
                        btnCancel.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case MessageBoxWindowButtons.YesNoCancel:
                        btnOK.Visibility = System.Windows.Visibility.Collapsed;
                        btnYes.Visibility = System.Windows.Visibility.Visible;
                        btnNo.Visibility = System.Windows.Visibility.Visible;
                        btnCancel.Visibility = System.Windows.Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
        }

        void MessageBoxWindow_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.GetType().Equals(typeof(MessageBoxViewModel)))
                this.ViewModel = e.NewValue as MessageBoxViewModel;
        }

        protected override Decorator GetWindowButtonsPlaceholder()
        {
            return WindowButtonsPlaceholder;
        }

        private void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        private void expAdditionalDetails_Collapsed(object sender, System.Windows.RoutedEventArgs e)
        {
            this.expAdditionalDetails.Header = "See Details";
            this.tbAdditionalDetailsText.Visibility = System.Windows.Visibility.Collapsed;
            this.UpdateLayout();
        }

        private void expAdditionalDetails_Expanded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.expAdditionalDetails.Header = "Hide Details";
            this.tbAdditionalDetailsText.Visibility = System.Windows.Visibility.Visible;
            this.UpdateLayout();
        }

        private void btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "btnOK")
                this.ViewModel.Result = MessageBoxWindowResult.OK;
            else if (button.Name == "btnYes")
                this.ViewModel.Result = MessageBoxWindowResult.Yes;
            else if (button.Name == "btnNo")
                this.ViewModel.Result = MessageBoxWindowResult.No;
            else if (button.Name == "btnCancel")
                this.ViewModel.Result = MessageBoxWindowResult.Cancel;
            else
                this.ViewModel.Result = MessageBoxWindowResult.None;
            this.Close();
        }

        
    }
}
