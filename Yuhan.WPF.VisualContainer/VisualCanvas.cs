using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Yuhan.WPF.VisualContainer
{
    public class VisualCanvas : Selector
    {
        public Boolean IsEditable
        {
            get { return (Boolean)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(Boolean), typeof(VisualCanvas), new PropertyMetadata(false, IsEditableChanged));

        private static void IsEditableChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) { }



        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(VisualCanvas), new PropertyMetadata(null));



        public Stretch ImageStrech
        {
            get { return (Stretch)GetValue(ImageStrechProperty); }
            set { SetValue(ImageStrechProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageStrech.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageStrechProperty =
            DependencyProperty.Register("ImageStrech", typeof(Stretch), typeof(VisualCanvas), new PropertyMetadata(Stretch.Uniform));


        #region ItemContainerStyle

        public String LeftFieldName
        {
            get { return (String)GetValue(LeftFieldNameProperty); }
            set { SetValue(LeftFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftFieldNameProperty =
            DependencyProperty.Register("LeftFieldName",
            typeof(String), typeof(VisualCanvas),
            new PropertyMetadata("X", OnLeftFieldNameChanged));

        protected static void OnLeftFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VisualCanvas).InitItemContainerStyle();
        }

        public String TopFieldName
        {
            get { return (String)GetValue(TopFieldNameProperty); }
            set { SetValue(TopFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopFieldNameProperty =
            DependencyProperty.Register("TopFieldName",
            typeof(String), typeof(VisualCanvas),
            new PropertyMetadata("Y", OnTopFieldNameChanged));

        protected static void OnTopFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VisualCanvas).InitItemContainerStyle();
        }

        public String RightFieldName
        {
            get { return (String)GetValue(RightFieldNameProperty); }
            set { SetValue(RightFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightFieldNameProperty =
            DependencyProperty.Register("RightFieldName",
            typeof(String), typeof(VisualCanvas),
            new PropertyMetadata(null, OnRightFieldNameChanged));

        protected static void OnRightFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VisualCanvas).InitItemContainerStyle();
        }

        public String BottomFieldName
        {
            get { return (String)GetValue(BottomFieldNameProperty); }
            set { SetValue(BottomFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BottomFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BottomFieldNameProperty =
            DependencyProperty.Register("BottomFieldName",
            typeof(String), typeof(VisualCanvas),
            new PropertyMetadata(null, OnBottomFieldNameChanged));

        protected static void OnBottomFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as VisualCanvas).InitItemContainerStyle();
        }

        public void InitItemContainerStyle()
        {
            this.ItemContainerStyle = GenerateItemContainerStyle();
        }

        protected virtual Style GenerateItemContainerStyle()
        {
            Style style = new Style();
            style.Setters.Add(new Setter()
            {
                Property = Canvas.LeftProperty,
                Value = new Binding(LeftFieldName)
            });
            style.Setters.Add(new Setter()
            {
                Property = Canvas.TopProperty,
                Value = new Binding(TopFieldName)
            });
            style.Setters.Add(new Setter()
            {
                Property = Canvas.RightProperty,
                Value = new Binding(RightFieldName)
            });
            style.Setters.Add(new Setter()
            {
                Property = Canvas.BottomProperty,
                Value = new Binding(BottomFieldName)
            });
            return style;
        }

        #region Items


        public String WidthFieldName
        {
            get { return (String)GetValue(WidthFieldNameProperty); }
            set { SetValue(WidthFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WidthFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidthFieldNameProperty =
            DependencyProperty.Register("WidthFieldName",
            typeof(String), typeof(VisualCanvas),
            new PropertyMetadata("Width", OnWidthFieldNameChanged));

        protected static void OnWidthFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }



        public String HeightFieldName
        {
            get { return (String)GetValue(HeightFieldNameProperty); }
            set { SetValue(HeightFieldNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeightFieldName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeightFieldNameProperty =
            DependencyProperty.Register("HeightFieldName",
            typeof(String), typeof(VisualCanvas),
            new PropertyMetadata("Height", OnHeightFieldNameChanged));

        protected static void OnHeightFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }

        #endregion

        #endregion

        private Boolean IsDragStart { get; set; }
        private Object NewItem { get; set; }
        private VisualCanvasItem newCanvasItem;
        private VisualCanvasItem NewCanvasItem
        {
            get
            {
                if (NewItem != null)
                    return this.ItemContainerGenerator.ContainerFromItem(NewItem) as VisualCanvasItem;
                else
                    return newCanvasItem;
            }
            set
            {
                newCanvasItem = value;
            }
        }
        private Point StartMousePoint { get; set; }
        private Point CurrentMousePoint { get; set; }

        public VisualCanvas()
            : base()
        {
            this.IsDragStart = false;
            this.Resources.MergedDictionaries.Add(
                new ResourceDictionary()
                {
                    Source = new Uri("pack://application:,,,/Yuhan.WPF.VisualContainer;component/Resources/VisualCanvas.xaml")
                });
            this.MouseLeftButtonDown += VisualCanvas_MouseLeftButtonDown;
            this.MouseMove += VisualCanvas_MouseMove;
            this.MouseLeftButtonUp += VisualCanvas_MouseLeftButtonUp;
        }

        #region Override Methods
        protected override DependencyObject GetContainerForItemOverride()
        {
            if (IsDragStart)
            {
                VisualCanvasItem item = new VisualCanvasItem();
                item.SetBinding(VisualCanvasItem.WidthProperty, new Binding(this.WidthFieldName) { Mode = BindingMode.TwoWay });
                item.SetBinding(VisualCanvasItem.HeightProperty, new Binding(this.HeightFieldName) { Mode = BindingMode.TwoWay });
                return item;
            }
            return base.GetContainerForItemOverride();

        }
        #endregion

        void VisualCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsDragStart)
            {
                IsDragStart = false;
                NewCanvasItem.SetCurrentValue(VisualCanvasItem.IsDrawingProperty, false);
                NewItem = null;
                NewCanvasItem = null;
            }
        }

        void VisualCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || NewCanvasItem == null)
                return;
            if (IsDragStart)
            {
                CurrentMousePoint = e.GetPosition(sender as IInputElement);
                Debug.WriteLine(CurrentMousePoint);
                var x = Math.Min(CurrentMousePoint.X, StartMousePoint.X);
                var y = Math.Min(CurrentMousePoint.Y, StartMousePoint.Y);

                var width = Math.Max(CurrentMousePoint.X, StartMousePoint.X) - x;
                var height = Math.Max(CurrentMousePoint.Y, StartMousePoint.Y) - y;

                NewCanvasItem.SetCurrentValue(FrameworkElement.WidthProperty, width);
                NewCanvasItem.SetCurrentValue(FrameworkElement.HeightProperty, height);

                Canvas.SetLeft(NewCanvasItem, x);
                Canvas.SetTop(NewCanvasItem, y);
                
                //IEditableCollectionViewAddNewItem items = this.Items;
                //items.CommitNew();
                //items.CommitEdit();

            }
        }

        void VisualCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsEditable)
            {
                this.IsDragStart = true;
                IEditableCollectionViewAddNewItem items = this.Items;
                StartMousePoint = e.GetPosition(sender as IInputElement);
                Debug.WriteLine(StartMousePoint);
                if (items.CanAddNewItem)
                {
                    NewItem = items.AddNew();
                    items.CommitNew();
                }
                else
                {
                    var newItem = this.GetContainerForItemOverride();
                    
                    int index = this.Items.Add(newItem);
                    NewCanvasItem = this.Items[index] as VisualCanvasItem;
                }
                Canvas.SetLeft(NewCanvasItem, StartMousePoint.X);
                Canvas.SetTop(NewCanvasItem, StartMousePoint.Y);
                NewCanvasItem.SetCurrentValue(VisualCanvasItem.IsDrawingProperty, true);
            }
        }
    }
}
