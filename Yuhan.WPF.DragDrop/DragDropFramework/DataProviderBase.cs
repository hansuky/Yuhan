#define PRINT2OUTPUT

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Xml;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;



namespace Yuhan.WPF.DragDrop.DragDropFramework
{

    /// <summary>
    /// This class provides some default implementations for
    /// IDataProvider that can be used by most derived classes.
    /// This class defines a data object that can be dragged.
    /// </summary>
    /// <typeparam name="TSourceContainer">Type of the source container, e.g. TabControl</typeparam>
    /// <typeparam name="TSourceObject">Type of the source object, e.g. TabItem</typeparam>
    public abstract class DataProviderBase<TSourceContainer, TSourceObject> : IDataProvider
        where TSourceContainer : UIElement
        where TSourceObject : UIElement
    {

        /// <summary>
        /// Create a Data Provider for specified SourceContainer/SourceObject
        /// identified by the specified data format string
        /// </summary>
        /// <param name="dataFormatString">Identifies the data object</param>
        public DataProviderBase(string dataFormatString) {
            Debug.Assert((dataFormatString != null) && (dataFormatString.Length > 0), "dataFormatString cannot be null and must not be an empty string");
            this.SourceDataFormat = dataFormatString;
        }

        /// <summary>
        /// Called by drag-and-drop framework to initialize the class
        /// </summary>
        public void Init() {
            this._keyStates = null;
            this._escapePressed = null;
        }

        /// <summary>
        /// Name of the dragged data object
        /// </summary>
        public string SourceDataFormat { get; private set; }

        /// <summary>
        /// Return true to add an adorner to the dragged object
        /// </summary>
        public virtual bool AddAdorner { get { return false; } }

        /// <summary>
        /// Return true to capture the mouse while dragging
        /// </summary>
        public virtual bool NeedsCaptureMouse { get { return false; } }

        /// <summary>
        /// Returns the drag operations supported by this data object provider
        /// </summary>
        public abstract DragDropEffects AllowedEffects {
            get;
            //{
            //    return
            //        DragDropEffects.Copy |
            //        DragDropEffects.Scroll |
            //        DragDropEffects.Move |
            //        DragDropEffects.Link |
            //
            //        DragDropEffects.None;
            //}
        }

        /// <summary>
        /// Returns the actions used by this data object provider
        /// </summary>
        public abstract DataProviderActions DataProviderActions {
            get;
            //{
            //    return
            //        //DataProviderActions.QueryContinueDrag |
            //        //DataProviderActions.GiveFeedback |
            //        //DataProviderActions.DoDragDrop_Done |
            //
            //        DataProviderActions.None;
            //}
        }

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
        public virtual bool IsSupportedContainerAndObject(bool initFlag, object dragSourceContainer, object dragSourceObject, object dragOriginalSourceObject) {
            // Init DataProvider variables
            if(initFlag) {
                this.Init();
                this.SourceContainer = dragSourceContainer;
                this.SourceObject = dragSourceObject;
                this.OriginalSourceObject = dragOriginalSourceObject;
            }

            return
                (dragSourceObject is TSourceObject) &&
                (dragSourceContainer is TSourceContainer);
        }

        /// <summary>
        /// The adorner (when used)
        /// </summary>
        public DefaultAdorner DragAdorner { get; set; }

        /// <summary>
        /// Point where LeftMouseDown occurred,
        /// relative to the drag source object's origin
        /// </summary>
        public Point StartPosition { get; set; }

        /// <summary>
        /// Drag source container, e.g. TabControl
        /// </summary>
        public object SourceContainer { get; set; }

        /// <summary>
        /// Drag source object, e.g. TabItem
        /// </summary>
        public object SourceObject { get; set; }

        /// <summary>
        /// OriginalSource from MouseButtonEventArgs
        /// </summary>
        public object OriginalSourceObject { get; set; }

        /// <summary>
        /// KeyStates saved from QueryContinueDrag
        /// </summary>
        private DragDropKeyStates? _keyStates = null;
        public DragDropKeyStates KeyStates {
            get {
                if(this._keyStates != null)
                    return (DragDropKeyStates)this._keyStates;
                else
                    throw new NotImplementedException("No KeyState value to return");
            }
        }

        /// <summary>
        /// EscapePressed saved from QueryContinueDrag
        /// </summary>
        private bool? _escapePressed = null;
        public bool EscapePressed {
            get {
                if(this._escapePressed != null)
                    return (bool)this._escapePressed;
                else
                    throw new NotImplementedException("No EscapePressed value to return");
            }
        }

        /// <summary>
        /// Sets the data passed to WPF
        /// DragDrop.DoDragDrop()
        /// </summary>
        /// <param name="data"></param>
        public virtual void SetData(ref DataObject data) {
            System.Diagnostics.Debug.Assert(data.GetDataPresent(this.SourceDataFormat) == false, "Shouldn't set data more than once");
            data.SetData(this.SourceDataFormat, this);
        }

        /// <summary>
        /// Saves EscapePressed and KeyStates when
        /// QueryContinueDrag is defined in DataProviderActions.
        /// Provide your own method if you wish; making sure
        /// to define QueryContinueDrag in DataProviderActions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
            this._escapePressed = e.EscapePressed;
            this._keyStates = e.KeyStates;
        }

        /// <summary>
        /// Provide your own method for displaying
        /// the correct cursor during a drag.
        /// Make sure to define GiveFeedback in DataProviderActions.
        /// </summary>
        /// <param name="sender">GiveFeedback event sender</param>
        /// <param name="e">GiveFeedback event arguments</param>
        public virtual void DragSource_GiveFeedback(object sender, GiveFeedbackEventArgs e) {
            throw new NotImplementedException("GiveFeedback not implemented");
        }

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
        public virtual void DoDragDrop_Done(DragDropEffects resultEffects) {
            throw new NotImplementedException("DoDragDropFinished not implemented");
        }

        /// <summary>
        /// Provide your own method to remove the source object from its container.
        /// This method is typically called when the source object is dropped and
        /// must be removed from its container.
        /// </summary>
        public virtual void Unparent() {
            throw new NotImplementedException("Unparent not implemented");
        }
    }
}
