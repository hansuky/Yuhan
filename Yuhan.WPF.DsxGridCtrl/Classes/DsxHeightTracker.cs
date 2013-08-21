using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Yuhan.WPF.DsxGridCtrl
{
	public class DsxHeightTracker : DependencyObject
    {
        #region ctors

        public DsxHeightTracker(DsxDataGrid dataGrid)
        {
            this.DataGrid   = dataGrid;
            this.RowHeights = new List<double>();
            this.DirtyItems = new List<object>();
        }
        #endregion

        #region members / properties

        private  List<double>       RowHeights  { get; set; }
        private  List<object>       DirtyItems  { get; set; }
        internal DsxDataGrid        DataGrid    { get; set; }

        private  object             TrackerLock = new object();

        #endregion

        #region Method - UpdateLayout

        internal void UpdateLayout()
        {   
            this.DataGrid.Dispatcher.BeginInvoke( new Action(delegate 
                                                        {   
                                                            this.RecalcLayout();
                                                        }), 
                                                        DispatcherPriority.Background);
        }
        #endregion

        #region Method - RecalcLayout

        private void RecalcLayout()
        {
            lock(this.TrackerLock)
            {
                if (this.DataGrid.ItemsSource != null && this.DataGrid.DisplaySource != null)
                {
                    if (this.DirtyItems.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("DirtyItems {0}", this.DirtyItems.Count);

                        List<object> _dirtyItems = new List<object>();
                
                        _dirtyItems = this.DirtyItems.ToList<object>();

                        this.DirtyItems.Clear();

                        foreach (object _dirty in _dirtyItems)
                        {
                            UpdateRowLayout(_dirty, false);
                        }
                    }
                    if (this.DirtyItems.Count > 0)
                    {
                        UpdateLayout();
                    }
                }
            }
        }
        #endregion


        #region Method - TrackItemHeight

        internal double TrackItemHeight(object listItem, double curHeight)
        {
            double _result = curHeight;

            if (this.DataGrid.ItemFixHeight != 0.0)
            {
                _result = this.DataGrid.ItemFixHeight;
            }
            else
            {
                CheckHeightTracker();

                if (this.DataGrid.ItemMinHeight != 0.0 && _result < this.DataGrid.ItemMinHeight)
                {
                    _result = this.DataGrid.ItemMinHeight;
                }
                if (this.DataGrid.ItemMaxHeight != 0.0 && _result > this.DataGrid.ItemMaxHeight)
                {
                    _result = this.DataGrid.ItemMaxHeight;
                }


                int _itmIndex  = this.DataGrid.Items.IndexOf(listItem);
                if (_itmIndex > -1 && this.RowHeights.Count > _itmIndex)
                {
                    double  _rowHeight = this.RowHeights[_itmIndex];

                    if (_result > _rowHeight)
                    {

                        this.RowHeights[_itmIndex] = _result;

                        if (_rowHeight > 0.0)
                        {
                            if (!UpdateRowLayout(listItem))
                            {
                                this.RowHeights[_itmIndex] = 1.0;
                                return 0.0;
                            }
                        }
                    }
                    if (_rowHeight > _result)
                    {
                        _result = _rowHeight;
                    }
                }
            }

            return _result;
        }
        #endregion

        #region Method - ResetItemHeight

        internal void ResetItemHeight(object listItem, double newHeight)
        {
            if (this.DataGrid.ItemFixHeight == 0.0)
            {
                CheckHeightTracker();

                int _itmIndex  = this.DataGrid.Items.IndexOf(listItem);
                if (_itmIndex > -1 && this.RowHeights.Count > _itmIndex)
                {
                    this.RowHeights[_itmIndex] = 1.0;
                }

                UpdateRowLayout(listItem);
            }
        }
        #endregion

        #region Method - UpdateRowLayout

        internal bool UpdateRowLayout(object listItem, bool trackUpdate=true)
        {
            ListViewItem    _listViewItem   = null;

            if (this.DataGrid.IsGridAreaLeft)
            {
                _listViewItem  = (this.DataGrid.PART_ListViewLeft  .ItemContainerGenerator.ContainerFromItem(listItem) as ListViewItem);

                if (!UpdateRowPartLayout(listItem, _listViewItem, trackUpdate))
                {
                    return false;
                }
            }

            if (this.DataGrid.IsGridAreaCenter)
            {
                _listViewItem  = (this.DataGrid.PART_ListViewCenter .ItemContainerGenerator.ContainerFromItem(listItem) as ListViewItem);

                if (!UpdateRowPartLayout(listItem, _listViewItem, trackUpdate))
                {
                    return false;
                }
            }

            if (this.DataGrid.IsGridAreaRight)
            {
                _listViewItem  = (this.DataGrid.PART_ListViewRight .ItemContainerGenerator.ContainerFromItem(listItem) as ListViewItem);

                if (!UpdateRowPartLayout(listItem, _listViewItem, trackUpdate))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Method - UpdateRowPartLayout

        internal bool UpdateRowPartLayout(object listItem, ListViewItem listViewItem, bool trackUpdate=true)
        {
            if (listViewItem == null)
            {
                if (this.DataGrid.ItemFixHeight == 0.0 && this.DataGrid.IsVirtualizing)
                {
                    this.DirtyItems.Add(listItem);
                    if (trackUpdate)
                    {
                        this.UpdateLayout();
                    }
                }
                return false;
            }
            else
            { 
                if (!listViewItem.IsLoaded) 
                { 
                    listViewItem.ApplyTemplate(); 
                } 

                GridViewRowPresenter _gridPresenter = ElementHelper.FindVisualChild<GridViewRowPresenter>(listViewItem);
                UpdateRowArea(_gridPresenter);

                return true;
            }
        }
        #endregion

        #region Method - UpdateRowArea

        internal void UpdateRowArea(GridViewRowPresenter gridPresenter)
        {
            if (gridPresenter == null)
            {
                //  happens due to virutalization
                return;
            }

            ContentPresenter        _cellContent    = null;
            FrameworkElement        _cellElement    = null;

            for (int i=0; i<gridPresenter.Columns.Count; i++)
            {
                _cellContent = VisualTreeHelper.GetChild(gridPresenter, i) as ContentPresenter; 
                _cellElement = ElementHelper.FindVisualChild<FrameworkElement>(_cellContent);
                if (_cellElement != null)
                {
                    _cellElement.InvalidateMeasure();
                    _cellElement.InvalidateArrange();
                    _cellElement.UpdateLayout();
                }
            }
        }
        #endregion

        #region Method - ClearHeightTracker

        internal void ClearHeightTracker()
        {
            this.RowHeights.Clear();
            this.DirtyItems.Clear();
        }
        #endregion

        #region Method - CheckHeightTracker

        internal void CheckHeightTracker()
        {
            if (this.RowHeights.Count != this.DataGrid.Items.Count)
            {
                this.RowHeights = new List<double>(this.DataGrid.Items.Count);

                for(int i=0; i<this.DataGrid.Items.Count; i++)
                {
                    this.RowHeights.Add(0.0);
                }
            }
        }
        #endregion
    }
}
