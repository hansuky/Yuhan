using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Yuhan.WPF.CustomWindow
{
    /*
     * Adorner - each UI element in WPF has a top layer where can be added adorner controls
     * Thumb - used to drag elements, returns drag delta
     */
    
    // Implements custom window resizing when default resizing is disabled (Aero disabled)
    class WindowResizingAdorner : Adorner
    {
        [Flags]
        enum Position 
        { 
            Top = 0x1,
            Bottom = 0x2,
            Left = 0x8,
            Right = 0x10
        }

        // Width of thumb resizer
        const int ThumbThickness = 8;

        // Stores Thumbs
        VisualCollection visualChildren;
        WindowThumb[] thumbs;

        Window _window;
        Point _mouseStartPosition;
        Point _windowStartPosition;
        Size _windowStartSize;

        /// <summary>
        /// Instantiates WindowResizingAdorner class
        /// </summary>
        /// <param name="element">Control into which's adorer layer will be this adorner added</param>
        /// <param name="window"></param>
        public WindowResizingAdorner(UIElement element, Window window)
            : base(element)
        {
            _window = window;

            // must be instantiated first before WindowThumbs are created and added
            visualChildren = new VisualCollection(element);
            thumbs = new WindowThumb[8];

            // * if you change the order, you have to change indexing in ArrangeOverride() method
            thumbs[0] = CreateThumb(Position.Left | Position.Top, Cursors.SizeNWSE);
            thumbs[1] = CreateThumb(Position.Right | Position.Top, Cursors.SizeNESW);
            thumbs[2] = CreateThumb(Position.Left | Position.Bottom, Cursors.SizeNESW);
            thumbs[3] = CreateThumb(Position.Right | Position.Bottom, Cursors.SizeNWSE);
            thumbs[4] = CreateThumb(Position.Left, Cursors.SizeWE);
            thumbs[5] = CreateThumb(Position.Top, Cursors.SizeNS);
            thumbs[6] = CreateThumb(Position.Right, Cursors.SizeWE);
            thumbs[7] = CreateThumb(Position.Bottom, Cursors.SizeNS);
        }

        /// <summary>
        /// Auxilliary method for creating thumbs
        /// </summary>
        /// <param name="position">Thumb position in the window</param>
        /// <returns>Returns created WindowThumb</returns>
        WindowThumb CreateThumb(Position position, Cursor cursor)
        {
            WindowThumb thumb = new WindowThumb();
            thumb.Position = position;
            thumb.DragStarted += new DragStartedEventHandler(Thumb_DragStarted);
            thumb.DragDelta += new DragDeltaEventHandler(Thumb_DragDelta);
            thumb.Cursor = cursor;

            visualChildren.Add(thumb);

            return thumb;
        }

        // called when thumb drag started (window resize started)
        void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            WindowThumb thumb = (WindowThumb)sender;

            // store settings of the window, will be used to resize and move the window
            _mouseStartPosition = PointToScreen(Mouse.GetPosition(_window));
            _windowStartPosition = new Point(_window.Left, _window.Top);
            _windowStartSize = new Size(_window.Width, _window.Height);
        }

        // Called whenever thumb dragged (window resizing)
        void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            WindowThumb thumb = (WindowThumb)sender;

            // calculate mouse delta
            Point position = PointToScreen(Mouse.GetPosition(_window));
            double deltaX = position.X - _mouseStartPosition.X;
            double deltaY = position.Y - _mouseStartPosition.Y;

            // horizontal resize
            if ((thumb.Position & Position.Left) == Position.Left)
            {
                this.SetWindowWidth(_windowStartSize.Width - deltaX);
                _window.Left = _windowStartPosition.X + deltaX;
            }
            else if ((thumb.Position & Position.Right) == Position.Right)
                this.SetWindowWidth(_windowStartSize.Width + deltaX);
            
            // vertical resize
            if ((thumb.Position & Position.Top) == Position.Top)
            {
                this.SetWindowHeight(_windowStartSize.Height - deltaY);
                _window.Top = _windowStartPosition.Y + deltaY;
            }
            else if ((thumb.Position & Position.Bottom) == Position.Bottom)
                this.SetWindowHeight(_windowStartSize.Height + deltaY);
        }

        /// <summary>
        /// Auxiliary method for setting Window width
        /// </summary>
        /// <param name="width">New window width</param>
        void SetWindowWidth(double width)
        {
            if (width < 2 * ThumbThickness)
                width = 2 * ThumbThickness;

            _window.Width = width;
        }

        /// <summary>
        /// Auxiliary method for setting Window height
        /// </summary>
        /// <param name="height">New window hright</param>
        void SetWindowHeight(double height)
        {
            if (height < 2 * ThumbThickness)
                height = 2 * ThumbThickness;

            _window.Height = height;
        }

        // Arrange the Adorners.
        protected override Size ArrangeOverride(Size finalSize)
        {
            // DesiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;

            thumbs[0].Arrange(new Rect(0, 0, ThumbThickness, ThumbThickness));
            thumbs[1].Arrange(new Rect(this.DesiredSize.Width - ThumbThickness, 0, ThumbThickness, ThumbThickness));
            thumbs[2].Arrange(new Rect(0, this.DesiredSize.Height - ThumbThickness, ThumbThickness, ThumbThickness));
            thumbs[3].Arrange(new Rect(this.DesiredSize.Width - ThumbThickness, this.DesiredSize.Height - ThumbThickness, ThumbThickness, ThumbThickness));
            thumbs[4].Arrange(new Rect(0, ThumbThickness, ThumbThickness, this.DesiredSize.Height - (2 * ThumbThickness)));
            thumbs[5].Arrange(new Rect(ThumbThickness, 0, this.DesiredSize.Width - (2 * ThumbThickness), ThumbThickness));
            thumbs[6].Arrange(new Rect(this.DesiredSize.Width - ThumbThickness, ThumbThickness, ThumbThickness, this.DesiredSize.Height - (2 * ThumbThickness)));
            thumbs[7].Arrange(new Rect(ThumbThickness, this.DesiredSize.Height - ThumbThickness, this.DesiredSize.Width - (2 * ThumbThickness), ThumbThickness));

            // Return the final size.
            return finalSize;
        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }


        // Used for resizing a window as a dragging point
        class WindowThumb : Thumb
        {
            public Position Position { get; set; }

            public WindowThumb()
            {
                FrameworkElementFactory borderFactory = new FrameworkElementFactory(typeof(Border));
                borderFactory.SetValue(Border.BackgroundProperty, Brushes.Transparent);

                ControlTemplate template = new ControlTemplate(typeof(WindowThumb));
                template.VisualTree = borderFactory;

                this.Template = template;
            }
        }
    }
}
