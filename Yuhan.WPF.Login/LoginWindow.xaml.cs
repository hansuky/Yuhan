using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using Yuhan.WPF.CustomWindow;
using Yuhan.WPF.Login.ViewModels;
using Microsoft.Win32;
using Yuhan.WPF;

namespace Yuhan.WPF.Login
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : EssentialWindow
    {
        public LoginViewModel ViewModel
        {
            get { return this.FindResource("ViewModel") as LoginViewModel; }
        }

        public Boolean UseCategories
        {
            get { return (Boolean)GetValue(UseCategoriesProperty); }
            set { SetValue(UseCategoriesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseCategories.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseCategoriesProperty =
            DependencyProperty.Register("UseCategories", typeof(Boolean), typeof(LoginWindow), new PropertyMetadata(true));

        public LoginWindow()
        {
            InitializeComponent();
            
            this.Loaded += LoginWindow_Loaded;

            ViewModel.Load();

            ViewModel.Logined += (sender, e) =>
            {
                if (e.Success) this.Close();
                if (Logined != null)
                    Logined(this, e);
            };
        }

        /// <summary>
        /// 로그인 시도 후 발생합니다.
        /// </summary>
        public event LoginEventHandler Logined;

        #region WinAPI
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        private static extern short GetKeyState(int keyCode);

        private bool CapsLock
        {
            get
            {
                return (((ushort)GetKeyState(0x14)) & 0xffff) != 0 &&
                    (((ushort)GetKeyState(0x14)) & 0xffff) != 65408;
            }
        }
        private bool NumLock { get { return (((ushort)GetKeyState(0x90)) & 0xffff) != 0; } }
        private bool ScrollLock { get { return (((ushort)GetKeyState(0x91)) & 0xffff) != 0; } }
        #endregion

        #region Windows Events

        void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey _RegistryKey = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("iMES");
                this.ViewModel.SelectedCategory = Convert.ToString(_RegistryKey.GetValue("Category"));
                this.ViewModel.UserId = Convert.ToString(_RegistryKey.GetValue("UserID"));
                if (!String.IsNullOrEmpty(this.ViewModel.UserId))
                    this.PasswordBox_UserPassword.Focus();
                else this.TextBox_UserId.Focus();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override Decorator GetWindowButtonsPlaceholder()
        {
            return WindowButtonsPlaceholder;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TextBox_UserId_KeyDown(object sender, KeyEventArgs e)
        {
            DisplayCapsLock();
            if (e.Key == Key.Enter) { this.PasswordBox_UserPassword.Focus(); }
            else if(ViewModel.GetUserIDRuleViolations().Count() > 0)
            {
                TextBox element = sender as TextBox;
                element.ToolTip = ViewModel.GetUserIDRuleViolations().First().ErrorMessage;
            }
        }

        private void PasswordBox_UserPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ViewModel.IsValid)
                    ViewModel.Login();
                else
                    MessageBoxDialog.Show(ViewModel.GetRuleViolations());
            }
        }

        private void LayoutRoot_KeyDown(object sender, KeyEventArgs e)
        {
            DisplayCapsLock();
        }

        private void Text_GotFocus(object sender, RoutedEventArgs e)
        {
            DisplayCapsLock();
        }

        private void DisplayCapsLock()
        {
            if (CapsLock)
            {
                this.Details_Expander.IsExpanded = true;
                this.SubInfo.Text = "Capslock이 활성화 돼어있습니다.";
            }
            else
            {
                this.Details_Expander.IsExpanded = false;
                this.SubInfo.Text = String.Empty;
            }
        }

        #endregion


    }


}
