using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Yuhan.WPF.MenuKiller
{
    /// <summary>
    /// </summary>
    public partial class MenuKiller : ContentControl, ICustomAlignedControl
    {
        static MenuKiller()    
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuKiller),
                new FrameworkPropertyMetadata(typeof(MenuKiller)));
        }

        void HandleMouseHover(object sender, RoutedEventArgs rea)
        {
            if (null != rea)
            {
                MenuKillerItem mki = rea.OriginalSource as MenuKillerItem;

                if (null != mki)
                {
                    HoverToolTip = mki.RootToolTip;
                }
            }
        }

        [Category("Content")]
        [Description("Provides a rich tooltip for the menu killer item which is under the mouse currently")]
        public static readonly DependencyProperty HoverToolTipProperty =
            DependencyProperty.Register(
                "HoverToolTip",
                typeof(object),
                typeof(MenuKiller),
                new PropertyMetadata(null));

        public object HoverToolTip
        {
            get
            {
                return (object)GetValue(HoverToolTipProperty);
            }
            set
            {
                SetValue(HoverToolTipProperty, value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AddHandler(MenuKillerItem.MouseHoverEvent, new RoutedEventHandler(HandleMouseHover));
        }

        /*
        static readonly DependencyProperty AlignReferencePointProperty =
            DependencyProperty.Register("AlignReferencePoint", typeof(Point), typeof(MenuKiller),
            new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsArrange, new PropertyChangedCallback(AlignReferencePointDPChanged)));

        void AlignReferencePointDPChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            // dependencyObject.
        }*/

        #region ICustomAlignedControl Members
        public Point AlignReferencePoint
        {
            get
            {
                MenuKillerItem rootItem = this.Content as MenuKillerItem;
                if (null != rootItem)
                {
                    return rootItem.AlignReferencePoint;
                }

                return new Point();
            }
        }
        #endregion
    }
}
