using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Yuhan.WPF.CustomWindow
{
    public partial class WindowButton : Button
    {
        #region DependencyProperties

        /// <summary>
        /// Button content
        /// <remarks>Base button's content property is hidden</remarks>
        /// </summary>
        new public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); RefreshContent(); }
        }

        new public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(WindowButton), new UIPropertyMetadata());

        /// <summary>
        /// Disabled button content
        /// </summary>
        public object ContentDisabled
        {
            get { return GetValue(ContentDisabledProperty); }
            set { SetValue(ContentDisabledProperty, value); RefreshContent(); }
        }

        public static readonly DependencyProperty ContentDisabledProperty =
            DependencyProperty.Register("ContentDisabled", typeof(object), typeof(WindowButton), new UIPropertyMetadata());

        /// <summary>
        /// Corner radius of the button
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusDPProperty); }
            set { SetValue(CornerRadiusDPProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusDPProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(WindowButton), new UIPropertyMetadata(new CornerRadius()));

 
        [System.ComponentModel.Bindable(true)]
        public object ActiveContent
        {
            get { return GetValue(ActiveContentProperty); }
            set { SetValue(ActiveContentProperty, value); }
        }

        public static readonly DependencyProperty ActiveContentProperty =
            DependencyProperty.Register("ActiveContent", typeof(object), typeof(WindowButton), new UIPropertyMetadata());

        #endregion

        /// <summary>
        /// Button default Background 
        /// </summary>
        public virtual Brush BackgroundDefaultValue
        {
            get { return (Brush)FindResource("DefaultBackgroundBrush"); }
        }


        /// <summary>
        /// Instantiate WindowButton class
        /// </summary>
        public WindowButton()
        {
            InitializeComponent();

            this.IsEnabledChanged += (s, e) => RefreshContent();
        }

        /// <summary>
        /// Set's the content of the button according to the current 'IsEnabled' state of the button
        /// </summary>
        protected void RefreshContent()
        {
            // Button is enabled
            if (this.IsEnabled)
                this.ActiveContent = this.Content;
            else
                this.ActiveContent = this.ContentDisabled;
        }
    }
}