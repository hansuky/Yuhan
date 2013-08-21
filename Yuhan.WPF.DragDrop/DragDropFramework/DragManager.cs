#define PRINT2BUFFER    // Monitor event entry/exit
#define PRINT2OUTPUT    // Output interesting information to Visual Studio's Output window

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls;



namespace Yuhan.WPF.DragDrop.DragDropFramework
{
    /// <summary>
    /// A representation of a data object that is
    /// dragged and dropped using this framework
    /// </summary>
    public interface IDataProvider
    {

        /// <summary>
        /// Called by drag-and-drop framework to initialize the class
        /// </summary>
        void Init();

        /// <summary>
        /// Return true to add an adorner to the dragged object
        /// </summary>
        bool AddAdorner { get; }

        /// <summary>
        /// Return true to capture the mouse while dragging
        /// </summary>
        bool NeedsCaptureMouse { get; }

        /// <summary>
        /// Returns the drag operations supported by this data object provider
        /// </summary>
        DragDropEffects AllowedEffects { get; }

        /// <summary>
        /// Returns the actions used by this data object provider
        /// </summary>
        DataProviderActions DataProviderActions { get; }

        /// <summary>
        /// Returns true when the specified source container, source object
        /// and original source object are supported by this data object provider.
        /// Saves the parameters in SourceContainer, SourceObject and
        /// OriginalSourceObject, respectively, when initFlag is true.
        /// </summary>
        /// <param name="initFlag">When true, initialize the class and source/container values</param>
        /// <param name="dragSourceContainer">Mouse event <code>sender</code></param>
        /// <param name="dragSourceObject">Mouse event args <code>Source</code></param>
        /// <param name="dragOriginalSourceObject">Mouse event args <code>Source</code></param>
        /// <returns>True for a supported container and object; false otherwise</returns>
        bool IsSupportedContainerAndObject(bool initFlag, object dragSourceContainer, object dragSourceObject, object dragOriginalSourceObject);

        /// <summary>
        /// The adorner (when used)
        /// </summary>
        DefaultAdorner DragAdorner { get; set; }

        /// <summary>
        /// Point where LeftMouseDown occurred,
        /// relative to the drag source object's origin
        /// </summary>
        Point StartPosition { get; set; }

        /// <summary>
        /// Drag source container, e.g. TabControl.
        /// </summary>
        object SourceContainer { get; set; }

        /// <summary>
        /// Drag source object, e.g. TabItem
        /// </summary>
        object SourceObject { get; set; }

        /// <summary>
        /// OriginalSource from MouseButtonEventArgs
        /// </summary>
        object OriginalSourceObject { get; set; }

        /// <summary>
        /// Sets the data passed to WPF
        /// DragDrop.DoDragDrop()
        /// </summary>
        /// <param name="data"></param>
        void SetData(ref DataObject data);

        /// <summary>
        /// Saves EscapePressed and KeyStates when
        /// QueryContinueDrag is defined in DataProviderActions.
        /// Provide your own method if you wish; making sure
        /// to define QueryContinueDrag in DataProviderActions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e);

        /// <summary>
        /// Provide your own method for displaying
        /// the correct cursor during a drag.
        /// Make sure to define GiveFeedback in DataProviderActions.
        /// </summary>
        /// <param name="sender">GiveFeedback event sender</param>
        /// <param name="e">GiveFeedback event arguments</param>
        void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e);

        /// <summary>
        /// Called after DragDrop.DoDragDrop() returns.
        /// Typically during a file move, for example, the file is deleted here.
        /// However, when moving a TabItem from one TabControl to another the
        /// source TabItem must be unparented from the source TabControl
        /// before it can be added to the destination TabControl.
        /// So most of the time when moving items between item controls,
        /// this method isn't used.
        /// Provide your own method if you wish; making sure
        /// to define DoDragDrop_Done in DataProviderActions.
        /// </summary>
        /// <param name="resultEffects">The drop operation that was performed</param>
        void DoDragDrop_Done(DragDropEffects resultEffects);

        /// <summary>
        /// Provide your own method to remove the source object from its container.
        /// This method is typically called when the source object is dropped and
        /// must be removed from its container.
        /// </summary>
        void Unparent();
    }


    [Flags]
    public enum DataProviderActions
    {
        QueryContinueDrag = 0x01,   // Call IDataProvider.DragSource_QueryContinueDrag
        GiveFeedback      = 0x02,   // Call IDataProvider.DragSource_GiveFeedback
        DoDragDrop_Done   = 0x04,   // Call IDataProvider.DoDragDrop_Done

        None              = 0x00,
    }


    /// <summary>
    /// Manage drag events for IDataProviders
    /// </summary>
    public class DragManager
    {
        private UIElement _dragSource;
        private IDataProvider[] _dragDropObjects;

        private IDataProvider _dragDropObject;
        private Point _startPosition;
        private bool _dragInProgress;

        /// <summary>
        /// Manage dragging data object from <code>dragSource</code> FrameworkElement.
        /// Hook various PreviewMouse* events in order to determine when a drag starts.
        /// </summary>
        /// <param name="dragSource">The FrameworkElement which contains objects to be dragged</param>
        /// <param name="dragDropObject">Object to be dragged, implementing IDataProvider</param>
        public DragManager(FrameworkElement dragSource, IDataProvider dragDropObject)
            : this(dragSource, new IDataProvider[] { dragDropObject })
        {
        }

        /// <summary>
        /// Manage dragging data object from <code>dragSource</code> FrameworkElement.
        /// Hook various PreviewMouse* events in order to determine when a drag starts.
        /// </summary>
        /// <param name="dragSource">The FrameworkElement which contains objects to be dragged</param>
        /// <param name="dragDropObjects">Array of objects to be dragged, implementing IDataProvider</param>
        public DragManager(FrameworkElement dragSource, IDataProvider[] dragDropObjects) {
            this._dragSource = dragSource;
            Debug.Assert(dragSource != null, "dragSource cannot be null");
            this._dragDropObjects = dragDropObjects;

            this._dragSource.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonDown);
            this._dragSource.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(DragSource_PreviewMouseMove);
            this._dragSource.PreviewMouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(DragSource_PreviewMouseLeftButtonUp);
        }

        /// <summary>
        /// Check for a supported SourceContainer/SourceObject.
        /// If found, get ready for a possible drag operation.
        /// </summary>
        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            foreach(IDataProvider dragDropObject in this._dragDropObjects) {
                if(dragDropObject.IsSupportedContainerAndObject(true, sender, e.Source, e.OriginalSource)) {
                    Debug.Assert(sender.Equals(this._dragSource));

                    this._dragDropObject = dragDropObject;

                    this._startPosition = e.GetPosition(sender as IInputElement);

                    this._dragDropObject.StartPosition = e.GetPosition(e.Source as IInputElement);

                    if(this._dragDropObject.NeedsCaptureMouse)
                        this._dragSource.CaptureMouse();

                    break;
                }
            }
        }

        /// <summary>
        /// If the mouse is moved (dragged) a minimum distance
        /// over a supported SourceContainer/SourceObject,
        /// initiate a drag operation.
        /// </summary>
        private void DragSource_PreviewMouseMove(object sender, MouseEventArgs e) {
            if((this._dragDropObject != null) && !this._dragInProgress && this._dragDropObject.IsSupportedContainerAndObject(false, sender, e.Source, e.OriginalSource)) {
                Point currentPosition = e.GetPosition(sender as IInputElement);
                if(((Math.Abs(currentPosition.X - this._startPosition.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(currentPosition.Y - this._startPosition.Y) > SystemParameters.MinimumVerticalDragDistance))) {

                    // NOTE:
                    //      While dragging a ListBoxItem, another one can be selected
                    //      This doesn't seem to happen with TreeView or TabControl
                    if(sender is ListBox)
                        this._dragDropObject.SourceObject = e.Source;

                    this._dragInProgress = true;

                    if(this._dragDropObject.AddAdorner) {
                        this._dragDropObject.DragAdorner = new DefaultAdorner(
                            (UIElement)Application.Current.MainWindow.Content,
                            (UIElement)this._dragDropObject.SourceObject,
                            this._dragDropObject.StartPosition);
                        System.Windows.Media.Visual visual = Application.Current.MainWindow.Content as Visual;
                        AdornerLayer.GetAdornerLayer(visual).Add(this._dragDropObject.DragAdorner);
                    }

                    DragDropEffects resultEffects = DoDragDrop_Start(e);

                    if(this._dragDropObject.NeedsCaptureMouse)
                        this._dragSource.ReleaseMouseCapture();

                    this.DoDragDrop_Done(resultEffects);

                    if(this._dragDropObject.AddAdorner) {
                        AdornerLayer.GetAdornerLayer((Visual)Application.Current.MainWindow.Content).Remove(this._dragDropObject.DragAdorner);
                    }

                    Mouse.OverrideCursor = null;

                    this._dragDropObject = null;
                    this._dragInProgress = false;
                }
            }
        }

        /// <summary>
        /// When MouseLeftButtonUp event occurs, abandon
        /// any drag operation that may be in progress
        /// </summary>
        private void DragSource_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if(this._dragDropObject != null) {
                if(this._dragDropObject.NeedsCaptureMouse)
                    this._dragSource.ReleaseMouseCapture();
                this._dragDropObject = null;
                this._dragInProgress = false;
            }
        }

        /// <summary>
        /// Gather keyboard key state information
        /// and optionally abort a drag operation
        /// </summary>
        private void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
#if PRINT2BUFFER
            ((DragDropWindow)Application.Current.MainWindow).buf0.Append('q');
#endif

            if((this._dragDropObject.DataProviderActions & DataProviderActions.QueryContinueDrag) != 0)
                this._dragDropObject.DragSource_QueryContinueDrag(sender, e);

#if PRINT2OUTPUT
            Debug.WriteLine(
                "q handled=" + e.Handled.ToString()
                + " action=" + e.Action.ToString()
                + " sender=" + sender.GetType().ToString()
                + " Source=" + e.Source.GetType().ToString()
                + " OriginalSource=" + e.OriginalSource.GetType().ToString()
                + " KeyStates=" + e.KeyStates.ToString()
                );
#endif

#if PRINT2BUFFER
            ((DragDropWindow)Application.Current.MainWindow).buf1.Append('q');
#endif
        }

        /// <summary>
        /// Display the appropriate drag cursor based on
        /// DragDropEffects returned within the DropManager
        /// </summary>
        private void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
#if PRINT2BUFFER
            ((DragDropWindow)Application.Current.MainWindow).buf0.Append('g');
#endif

            if(this._dragDropObject.AddAdorner) {
                Point point = Utilities.Win32GetCursorPos();
                DefaultAdorner dragAdorner = this._dragDropObject.DragAdorner;
                dragAdorner.SetMousePosition(dragAdorner.AdornedElement.PointFromScreen(point));
            }

            if((this._dragDropObject.DataProviderActions & DataProviderActions.GiveFeedback) != 0)
                this._dragDropObject.DragSource_GiveFeedback(sender, e);

#if PRINT2OUTPUT
            Debug.WriteLine(
                "g handled=" + e.Handled.ToString()
                + " sender=" + sender.GetType().ToString()
                + " Source=" + e.Source.GetType().ToString()
                + " OriginalSource=" + e.OriginalSource.GetType().ToString()
                + " Effects=" + e.Effects.ToString()
                );
#endif

#if PRINT2BUFFER
            ((DragDropWindow)Application.Current.MainWindow).buf1.Append('g');
#endif
        }

        /// <summary>
        /// Prepare for and begin a drag operation.
        /// Hook the events needed by the data provider.
        /// </summary>
        private DragDropEffects DoDragDrop_Start(MouseEventArgs e) {
            DragDropEffects resultEffects = DragDropEffects.None;

            DataObject data = new DataObject();
            this._dragDropObject.SetData(ref data);

            bool hookQueryContinueDrag = false;
            bool hookGiveFeedback = false;

            if((this._dragDropObject.DataProviderActions & DataProviderActions.QueryContinueDrag) != 0)
                hookQueryContinueDrag = true;

            if((this._dragDropObject.DataProviderActions & DataProviderActions.GiveFeedback) != 0)
                hookGiveFeedback = true;

            if(this._dragDropObject.AddAdorner)
                hookGiveFeedback = true;

            QueryContinueDragEventHandler queryContinueDrag = null;
            GiveFeedbackEventHandler giveFeedback = null;

            if(hookQueryContinueDrag) {
                queryContinueDrag = new QueryContinueDragEventHandler(DragSource_QueryContinueDrag);
                this._dragSource.QueryContinueDrag += queryContinueDrag;
            }
            if(hookGiveFeedback) {
                giveFeedback = new GiveFeedbackEventHandler(DragSource_GiveFeedback);
                this._dragSource.GiveFeedback += giveFeedback;
            }

            try {
                // NOTE:  Set 'dragSource' to desired value (dragSource or item being dragged)
                //		  'dragSource' is passed to QueryContinueDrag as Source and OriginalSource
                DependencyObject dragSource;
                dragSource = this._dragSource;
                //dragSource = this._dragDropObject.Item;
                resultEffects = System.Windows.DragDrop.DoDragDrop(dragSource, data, this._dragDropObject.AllowedEffects);
            }
            catch {
                Debug.WriteLine("DragDrop.DoDragDrop threw an exception");
            }

            if(queryContinueDrag != null)
                this._dragSource.QueryContinueDrag -= queryContinueDrag;
            if(giveFeedback != null)
                this._dragSource.GiveFeedback -= giveFeedback;

            return resultEffects;
        }

        /// <summary>
        /// Called after DragDrop.DoDragDrop() returns.
        /// Typically during a file move, for example, the file is deleted here.
        /// However, when moving a TabItem from one TabControl to another the
        /// source TabItem must be unparented from the source TabControl
        /// before it can be added to the destination TabControl.
        /// So most of the time when moving items between item controls,
        /// this method isn't used.
        /// </summary>
        /// <param name="resultEffects">The drop operation that was performed</param>
        private void DoDragDrop_Done(DragDropEffects resultEffects) {
            if((this._dragDropObject.DataProviderActions & DataProviderActions.DoDragDrop_Done) != 0)
                this._dragDropObject.DoDragDrop_Done(resultEffects);

#if PRINT2BUFFER
            Debug.WriteLine("buf0: " + ((DragDropWindow)Application.Current.MainWindow).buf0.ToString());
            Debug.WriteLine("buf1: " + ((DragDropWindow)Application.Current.MainWindow).buf1.ToString());
            bool buffersSame = (((DragDropWindow)Application.Current.MainWindow).buf0.ToString().CompareTo(((DragDropWindow)Application.Current.MainWindow).buf1.ToString()) == 0);
            if(buffersSame)
                Debug.WriteLine("buf0 and buf1 are the same");
            Debug.Assert(buffersSame, "Possible reentrancy issue(s) -- make sure event code is short");
            ((DragDropWindow)Application.Current.MainWindow).buf0 = new StringBuilder("");
            ((DragDropWindow)Application.Current.MainWindow).buf1 = new StringBuilder("");
#endif
        }
    }
}
