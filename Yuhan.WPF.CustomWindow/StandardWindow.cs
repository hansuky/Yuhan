using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Yuhan.WPF.CustomWindow
{
    public class StandardWindow : Window
    {
        bool _aeroEnabled;

        protected WindowButtonState _minimizeButtonState;
        protected WindowButtonState _maximizeButtonState;                       // including restore button
        protected WindowButtonState _closeButtonState;

        protected WindowMinimizeButton _minimizeButton;
        protected WindowRestoreButton _restoreButton;
        protected WindowMaximizeButton _maximizeButton;
        protected WindowCloseButton _closeButton;

        Border _captionControl;
        Border _contentWindowBackgroundBorder;
        Border _contentWindowBorder;
        Border _windowBorderBorder;
        bool _contentExtend;
        
        // window caption doesn't support doubleclick; this stores the time of last click
        // and upon this can be decided if last two clicks is or is not a double click
        int _lastMouseCaptionClick;

        // implements resizing window when aero is off
        WindowResizingAdorner _resizingAdorner;                                 

        #region Properties
        /// <summary>
        /// Can be assigned on initialization if the window.
        /// Changing value doesn't make effect on already opened window.
        /// </summary>
        public int CaptionHeight { get; set; }

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

        /// <summary>
        /// Content of the caption
        /// </summary>
        [TypeConverter(typeof(TypeConverterStringToUIElement))]
        public UIElement Caption
        {
            get { return _captionControl.Child; }
            set { _captionControl.Child = value; }
        }

        /// <summary>
        /// If true, window content is extended into caption area.
        /// Cannot be changed when window is already loaded.
        /// </summary>
        public bool ContentExtend
        {
            get { return _contentExtend; }
            set { _contentExtend = value; }
        }

        #region Content Property
        /// <summary>
        /// Window content
        /// <remarks>Hides base window class 'Content' property</remarks>
        /// </summary>
        public new object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public new static readonly DependencyProperty ContentProperty;

        // called when 'Content' property changed
        static void ContentChangedCallback(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            StandardWindow window = (StandardWindow)property;
            window._contentWindowBorder.Child = (UIElement)args.NewValue;
        }
        #endregion

        #region Background Property
        /// <summary>
        /// Window background
        /// <remarks>Hides base window class 'Background' property</remarks>
        /// </summary>
        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set
            {
                SetValue(BackgroundProperty, value);
                _contentWindowBackgroundBorder.Background = value;
            }
        }

        public new static readonly DependencyProperty BackgroundProperty;

        // called when 'Background' property changed
        static void BackgroundChangedCallback(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            StandardWindow window = (StandardWindow)property;
            window._contentWindowBackgroundBorder.Background = (Brush)args.NewValue;
        }
        #endregion

        /// <summary>
        /// Minimize, Maximize or Restore window
        /// </summary>
        public new virtual WindowState WindowState 
        {
            get { return base.WindowState; }
            set { SetWindowState(value); }
        }
        #endregion

        // This enables to use the CustomWindow in Visual Studio design mode. If DependencyProperties are used, the design mode does not work then.
        static StandardWindow()
        {
            // this checks whether application runs in design mode or not; if not the DependencyProperties are initialized
            if (System.Reflection.Assembly.GetEntryAssembly() != null)
            {
                ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(StandardWindow), new UIPropertyMetadata(null, new PropertyChangedCallback(ContentChangedCallback)));
                BackgroundProperty = DependencyProperty.Register("Background", typeof(object), typeof(StandardWindow), new UIPropertyMetadata(Brushes.Transparent, new PropertyChangedCallback(BackgroundChangedCallback)));
            }                     
        }

        /// <summary>
        /// Instantiate StandardWindow class
        /// </summary>
        public StandardWindow()
        {
            InitializeContentControls();

            this.CaptionHeight = (int)SystemParameters.CaptionHeight;
            _contentExtend = false;

            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        /// <summary>
        /// Pre-defined controls of the window
        /// </summary>
        protected void InitializeContentControls()
        {
            this.WindowStyle = WindowStyle.None;
            base.Background = Brushes.Transparent;

            //
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

            // put stack into border
            Border buttonsBorder = new Border();
            buttonsBorder.BorderThickness = new Thickness(0, 1, 0, 0);
            buttonsBorder.BorderBrush = new SolidColorBrush(new Color() { R = 118, G = 124, B = 132, A = 255 });
            buttonsBorder.VerticalAlignment = VerticalAlignment.Top;
            buttonsBorder.Child = buttonsStackPanel;
            buttonsBorder.VerticalAlignment = VerticalAlignment.Top;
            buttonsBorder.HorizontalAlignment = HorizontalAlignment.Right;

            //
            // Caption
            _captionControl = new Border();
            _captionControl.MouseMove += new System.Windows.Input.MouseEventHandler(OnWindowDragMove);
            _captionControl.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnCaptionBarClick);
            DockPanel.SetDock(_captionControl, Dock.Top);

            //
            // Window
            _contentWindowBackgroundBorder = new Border();
            _contentWindowBackgroundBorder.Background = Brushes.White;
            
            DockPanel windowDockPanel = new DockPanel();
            windowDockPanel.Children.Add(_captionControl);
            windowDockPanel.Children.Add(_contentWindowBackgroundBorder);

            // all wrap into grid
            _contentWindowBorder = new Border();

            Grid topGrid = new Grid();
            topGrid.Children.Add(windowDockPanel);
            topGrid.Children.Add(_contentWindowBorder);
            topGrid.Children.Add(buttonsBorder);

            base.Content = topGrid;
        }

        // called when window is loaded; extends Aero glass effect
        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            // set caption height
            _captionControl.Height = CaptionHeight;

            if (_contentExtend)
                _contentWindowBorder.Margin = new Thickness(0);
            else
                _contentWindowBorder.Margin = new Thickness(0, CaptionHeight, 0, 0);

            // bug? I don't know why, but when aero is disabled and _captionControl.Height is set,
            // the windowDockPanel doesn't react on the _captionControl size change.
            // Only after window is resized the dockpanel reacts on the change.
            this.Height = this.Height + 1;
            this.Height = this.Height - 1;

            // try to enable aero
            _aeroEnabled = Graphics.InicializeAero(this, CaptionHeight);

            // aero is enabled => caption background color managed by aero; otherwise set system color
            if (_aeroEnabled)
                _captionControl.Background = Brushes.Transparent;

            // if aero is not enabled => create default border around the window, disable default resizing, add custom resizing
            else
            {
                _captionControl.Background = SystemColors.ActiveCaptionBrush;

                // disconnect current content
                object content = base.Content;
                base.Content = null;

                // window border
                _windowBorderBorder = new Border();
                _windowBorderBorder.BorderThickness = new Thickness(5);
                _windowBorderBorder.Child = (UIElement)content;
                _windowBorderBorder.BorderBrush = SystemColors.ActiveCaptionBrush;

                // white strip arround the window
                Border borderWhite = new Border();

                borderWhite.BorderThickness = new Thickness(1);
                borderWhite.Child = _windowBorderBorder;
                borderWhite.BorderBrush = Brushes.White;

                // black strip arround the window
                Border borderBlack = new Border();
                borderBlack.BorderThickness = new Thickness(1); 
                borderBlack.Child = borderWhite;
                borderBlack.BorderBrush = Brushes.Black;

                // put the content back
                base.Content = borderBlack;

                // switch off resizing => it hides the ugly gray border => resizing must be implemented again though
                this.ResizeMode = ResizeMode.NoResize;

                // create window resizing
                _resizingAdorner = new WindowResizingAdorner((UIElement)base.Content, this);
                AdornerLayer.GetAdornerLayer((UIElement)this.Content).Add(_resizingAdorner);
            }

            // set system properties
            TextBlock.SetForeground(_captionControl, SystemColors.ActiveCaptionTextBrush);
            TextBlock.SetFontFamily(_captionControl, SystemFonts.CaptionFontFamily);
            TextBlock.SetFontSize(_captionControl, SystemFonts.CaptionFontSize);
            TextBlock.SetFontStyle(_captionControl, SystemFonts.CaptionFontStyle);
            TextBlock.SetFontWeight(_captionControl, SystemFonts.CaptionFontWeight);

            this.StateChanged += new EventHandler(StandardWindow_StateChanged);

            // buttons on active window and not active window differ => manage
            this.Activated += new EventHandler(OnStandardWindowActivated);
            this.Deactivated += new EventHandler(OnStandardWindowDeactivated);

            // refresh state
            OnStateChanged(new EventArgs());
        }

        // called when Minimize button clicked
        protected virtual void OnButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // called when Restore button clicked
        protected virtual void OnButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        // called when Maximize button clicked
        protected virtual void OnButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        // called when Close button clicked
        protected virtual void OnButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        ////
        //// Window State implementation
        ////

        void SetWindowState(WindowState state)
        {
            // Maximized
            if (state == WindowState.Maximized)
            {
                // Maximizied window in WPF with WindowStyle is set to 'None' overlaps taskbar.
                // This method implements maximilization that does not overlap taskbar.
                Point size = Graphics.GetMonitorSize(this);

                // set maximal size
                this.MaxHeight = size.Y;
                this.ResizeMode = ResizeMode.NoResize;                              // set no resize to compute the right size of the window

                // ^^ must be called before state is changed

                // maximize window
                base.WindowState = WindowState.Maximized;
            }
            // Normal
            else if (state == WindowState.Normal)
            {
                base.WindowState = WindowState.Normal;
            }
            //
            // Minimized
            else if (state == WindowState.Minimized)
            {
                base.WindowState = WindowState.Minimized;
            }
        }

        // called when state of the window changed to minimized, normal or maximized
        protected void StandardWindow_StateChanged(object sender, EventArgs e)
        {
            // Normal
            if (this.WindowState == WindowState.Normal)
            {
                _restoreButton.Visibility = Visibility.Collapsed;
                TextBlock.SetForeground(_captionControl, Brushes.Black);   // title color in normal window is black

                // Aero enabled
                if (_aeroEnabled)
                {
                    // SetMaximizeWindow() changed resizing mode => change back
                    _captionControl.Background = Brushes.Transparent;
                    this.ResizeMode = ResizeMode.CanResize;
                }
                // Aero disabled
                else
                {
                    // display resizing layer
                    _resizingAdorner.Visibility = Visibility.Visible;
                }

                // if Maximize button state is 'None' (button is explicitly hidden by developer) => do not make visible
                if (_maximizeButtonState != WindowButtonState.None)
                    _maximizeButton.Visibility = Visibility.Visible;
            }
            // Maximized
            else if (this.WindowState == WindowState.Maximized)
            {
                _maximizeButton.Visibility = Visibility.Collapsed;
                TextBlock.SetForeground(_captionControl, Brushes.White);

                // Aero enabled
                if (_aeroEnabled)
                {
                    // when aero is enabled and ResizeMode is 'NoResize', then border
                    // of the window is automatically transparent including caption bar => set color
                    _captionControl.Background = new SolidColorBrush(Color.FromRgb(12, 32, 50));
                }
                // Aero disabled
                else
                {
                    // hide resizing layer
                    _resizingAdorner.Visibility = Visibility.Hidden;
                }

                // if Maximize button state is 'None' (button is explicitly hidden by developer) => do not make visible
                if (_maximizeButtonState != WindowButtonState.None)
                    _restoreButton.Visibility = Visibility.Visible;
            }
        }

        // called whenever window dragged
        protected virtual void OnWindowDragMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                this.DragMove();
        }

        // called whenever clicked on caption bar
        protected virtual void OnCaptionBarClick(object sender, MouseButtonEventArgs e)
        {
            // Double-click on window caption triggers window state change.
            // caption control doesn't provide double-click so it is implemented
            // here by storing time of last click, comparing it with current time
            // and eventually changing the state
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (Environment.TickCount - _lastMouseCaptionClick < 400)
                {
                    if (this.WindowState == WindowState.Maximized)
                        this.WindowState = WindowState.Normal;

                    else if (this.WindowState == WindowState.Normal)
                        this.WindowState = WindowState.Maximized;

                    _lastMouseCaptionClick = 0;
                }

                _lastMouseCaptionClick = Environment.TickCount;
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

        
        ////
        //// Active / Not active Window
        //// manages window buttons that differ in active and non-active states
        ////
        protected virtual void OnStandardWindowActivated(object sender, EventArgs e)
        {
            // set caption text
            if (this.WindowState == WindowState.Maximized)
                TextBlock.SetForeground(_captionControl, Brushes.White);                        // caption text color when window is maximized and active
            else
                TextBlock.SetForeground(_captionControl, SystemColors.ActiveCaptionTextBrush);  // caption text color when window is active

            if (!_aeroEnabled)
            {
                _captionControl.Background = SystemColors.ActiveCaptionBrush;                   // caption background color is managed automatically by Aero if enabled
                _windowBorderBorder.BorderBrush = SystemColors.ActiveCaptionBrush;
            }
        }

        protected virtual void OnStandardWindowDeactivated(object sender, EventArgs e)
        {
            // set caption text color
            TextBlock.SetForeground(_captionControl, SystemColors.InactiveCaptionTextBrush);    // caption text color when window is inactive

            if (!_aeroEnabled)
            {
                _captionControl.Background = SystemColors.InactiveCaptionBrush;                 // caption background color is managed automatically by Aero if enabled
                _windowBorderBorder.BorderBrush = SystemColors.InactiveCaptionBrush;
            }
        }
    }
}
