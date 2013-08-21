using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yuhan.WPF.CustomWindow
{
    public abstract class EssentialWindow : Window
    {
        protected WindowButtonState _minimizeButtonState;
        protected WindowButtonState _maximizeButtonState;           // including restore button
        protected WindowButtonState _closeButtonState;

        protected WindowMinimizeButton _minimizeButton;
        protected WindowRestoreButton _restoreButton;
        protected WindowMaximizeButton _maximizeButton;
        protected WindowCloseButton _closeButton;

        protected UIElement _windowButtons;

        #region Properties
        /// <summary>
        /// Miximize button state
        /// </summary>
        public WindowButtonState MinimizeButtonState
        {
            get { return _minimizeButtonState; }
            set { _minimizeButtonState = value; OnWindowButtonStateChange(value, _minimizeButton); }
        }

        /// <summary>
        /// Maximize button state
        /// </summary>
        public WindowButtonState MaximizeButtonState
        {
            get { return _maximizeButtonState; }
            set
            {
                _maximizeButtonState = value;
                OnWindowButtonStateChange(value, _restoreButton);
                OnWindowButtonStateChange(value, _maximizeButton);
            }
        }

        /// <summary>
        /// Close button state
        /// </summary>
        public WindowButtonState CloseButtonState
        {
            get { return _closeButtonState; }
            set { _closeButtonState = value; OnWindowButtonStateChange(value, _closeButton); }
        }
        #endregion


        //
        // Constructors
        //
        public EssentialWindow()
        {
            _windowButtons = GenerateWindowButtons();

            this.WindowStyle = WindowStyle.None;

            this.Loaded += new RoutedEventHandler(OnLoaded);
            this.StateChanged += new EventHandler(StandardWindow_StateChanged);

            // buttons on active window and not active window differ => manage
            this.Activated += new EventHandler(OnStandardWindowActivated);
            this.Deactivated += new EventHandler(OnStandardWindowDeactivated);
        }


        //
        // Methods
        //

        protected abstract Decorator GetWindowButtonsPlaceholder();

        /// <summary>
        /// Pre-create window buttons
        /// </summary>
        private UIElement GenerateWindowButtons()
        {
            // Buttons
            _minimizeButton = new WindowMinimizeButton();
            _minimizeButton.Click += new RoutedEventHandler(OnButtonMinimize_Click);

            _restoreButton = new WindowRestoreButton();
            _restoreButton.Click += new RoutedEventHandler(OnButtonRestore_Click);
            _restoreButton.Margin = new Thickness(-1, 0, 0, 0);

            _maximizeButton = new WindowMaximizeButton();
            _maximizeButton.Click += new RoutedEventHandler(OnButtonMaximize_Click);
            _maximizeButton.Margin = new Thickness(-1, 0, 0, 0);

            _closeButton = new WindowCloseButton();
            _closeButton.Click += new RoutedEventHandler(OnButtonClose_Click);
            _closeButton.Margin = new Thickness(-1, 0, 0, 0);

            // put buttons into StackPanel
            StackPanel buttonsStackPanel = new StackPanel();
            buttonsStackPanel.Orientation = Orientation.Horizontal;
            buttonsStackPanel.Children.Add(_minimizeButton);
            buttonsStackPanel.Children.Add(_restoreButton);
            buttonsStackPanel.Children.Add(_maximizeButton);
            buttonsStackPanel.Children.Add(_closeButton);

            return buttonsStackPanel;
        }

        // called when window is loaded; extends Aero glass effect
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            // put Window Buttons into placeholder
            Decorator placeholder = GetWindowButtonsPlaceholder();

            if (placeholder == null)
                throw new NotSupportedException("Placeholder must be created already in the initialization of the Window");

            placeholder.Child = _windowButtons;

            // refresh state
            OnStateChanged(new EventArgs());
        }

        /// <summary>
        /// Called when Minimize button clicked
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event args</param>
        protected virtual void OnButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Called when Restore button clicked
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event args</param>
        protected virtual void OnButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Called when Maximize button clicked
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event args</param>
        protected virtual void OnButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// Called when Close button clicked
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event args</param>
        protected virtual void OnButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        // called when state of the window changed to minimized, normal or maximized
        void StandardWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this._restoreButton.Visibility = Visibility.Collapsed;

                // if Maximize button state is 'None' => do not make visible
                if (_maximizeButtonState != WindowButtonState.None)
                    this._maximizeButton.Visibility = Visibility.Visible;
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                this._maximizeButton.Visibility = Visibility.Collapsed;

                // if Maximize button state is 'None' => do not make visible
                if (_maximizeButtonState != WindowButtonState.None)
                    this._restoreButton.Visibility = Visibility.Visible;
            }
        }

        // hepler function
        protected virtual void OnWindowButtonStateChange(WindowButtonState state, WindowButton button)
        {
            switch (state)
            {
                case WindowButtonState.Normal:
                    button.Visibility = Visibility.Visible;
                    button.IsEnabled = true;
                    break;

                case WindowButtonState.Disabled:
                    button.Visibility = Visibility.Visible;
                    button.IsEnabled = false;
                    break;

                case WindowButtonState.None:
                    button.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        //
        // Active / Not active Window
        // manages window buttons that differ in those states
        //
        protected virtual void OnStandardWindowActivated(object sender, EventArgs e)
        {
            _minimizeButton.Background = _minimizeButton.BackgroundDefaultValue;
            _maximizeButton.Background = _maximizeButton.BackgroundDefaultValue;
            _restoreButton.Background = _restoreButton.BackgroundDefaultValue;
            _closeButton.Background = _closeButton.BackgroundDefaultValue;
        }

        protected virtual void OnStandardWindowDeactivated(object sender, EventArgs e)
        {
            _minimizeButton.Background = Brushes.Transparent;
            _maximizeButton.Background = Brushes.Transparent;
            _restoreButton.Background = Brushes.Transparent;
            _closeButton.Background = Brushes.Transparent;
        }
    }
}
