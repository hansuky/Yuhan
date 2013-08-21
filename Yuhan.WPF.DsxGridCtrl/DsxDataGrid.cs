using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Timers;
using System.Windows.Threading;
using System.Linq;
using Microsoft.Windows.Themes;

//  Christoph Brändle (dialogik.ch)
//  Article published: 

namespace Yuhan.WPF.DsxGridCtrl
{
    #region TemplateParts

    [TemplatePart(Name = cPART_ListViewLeft,        Type = typeof(ListView))]
    [TemplatePart(Name = cPART_ListViewCenter,      Type = typeof(ListView))]
    [TemplatePart(Name = cPART_ListViewRight,       Type = typeof(ListView))]

    [TemplatePart(Name = cPART_GridViewLeft,        Type = typeof(DsxGridView))]
    [TemplatePart(Name = cPART_GridViewCenter,      Type = typeof(DsxGridView))]
    [TemplatePart(Name = cPART_GridViewRight,       Type = typeof(DsxGridView))]

    #endregion

    #region StyleParts
    [StyleTypedProperty(Property = "HeaderStyle",   StyleTargetType = typeof(DsxHeaderStyle ))]
    [StyleTypedProperty(Property = "FilterStyle",   StyleTargetType = typeof(DsxFilterStyle ))]
    [StyleTypedProperty(Property = "FooterStyle",   StyleTargetType = typeof(DsxFooterStyle ))]
    [StyleTypedProperty(Property = "RowCellStyle",  StyleTargetType = typeof(DsxRowCellStyle))]

    #endregion

    public class DsxDataGrid : Selector
    {
        #region Consts

        internal const string   cPART_ListViewLeft      = "PART_ListViewLeft";
        internal const string   cPART_ListViewCenter    = "PART_ListViewCenter";
        internal const string   cPART_ListViewRight     = "PART_ListViewRight";

        internal const string   cPART_GridViewLeft      = "PART_GridViewLeft";
        internal const string   cPART_GridViewCenter    = "PART_GridViewCenter";
        internal const string   cPART_GridViewRight     = "PART_GridViewRight";

        internal const string   cPART_FilterCheck       = "PART_FilterCheck";
        internal const string   cPART_FilterText        = "PART_FilterText";

        #endregion

        #region ctors

        static DsxDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DsxDataGrid), new FrameworkPropertyMetadata((typeof(DsxDataGrid))));
        }

        public DsxDataGrid()
        {
            this.Columns            = new GridViewColumnCollection();
            this.InnerColumns       = new List<DsxColumn>();
            this.FilterColumns      = new List<DsxColumn>();
            this.FooterColumns      = new List<DsxColumn>();
            this.HeightTracker      = new DsxHeightTracker(this);

            this.Loaded             += OnLoaded;
            this.SelectionChanged   += OnSelectionChanged;
            this.SizeChanged        += OnSizeChanged;

            this.IsSynchronizedWithCurrentItem = true;
        }
      
        #endregion

        #region members / properties

        internal ListView               PART_ListViewLeft       { get; set; }
        internal ListView               PART_ListViewCenter     { get; set; }
        internal ListView               PART_ListViewRight      { get; set; }

        internal DsxGridView            PART_GridViewLeft       { get; set; }
        internal DsxGridView            PART_GridViewCenter     { get; set; }
        internal DsxGridView            PART_GridViewRight      { get; set; }

        private ScrollViewer            ScrollViewerLeft        { get; set; }
        private ScrollViewer            ScrollViewerRight       { get; set; }
        private ScrollViewer            ScrollViewerCenter      { get; set; }

        private List<DsxColumn>         InnerColumns            { get; set; }
        private List<DsxColumn>         FilterColumns           { get; set; }
        private List<DsxColumn>         FooterColumns           { get; set; }

        private Timer                   FilterWaitTimer         { get; set; }

        private GridViewColumnHeader    SortHeader              { get; set; }
        private SortAdorner             SortAdorner             { get; set; }
        private DsxColumn               SortColumn              { get; set; }

        internal ICollectionView        DisplaySource           { get; set; }

        private DsxColumn               LastFocusColumn         { get; set; }
        private DsxCellBase             LastFocusCell           { get; set; }
        private DsxCellBase             LastEditCell            { get; set; }
        private object                  LastFocusRow            { get; set; }
        private Adorner                 LastAdorner             { get; set; }

        internal bool                   IsGridAreaLeft          { get; set; }
        internal bool                   IsGridAreaCenter        { get; set; }
        internal bool                   IsGridAreaRight         { get; set; }

        internal int                    AreaLeftColCount        { get; set; }
        internal int                    AreaCenterColCount      { get; set; }
        internal int                    AreaRightColCount       { get; set; }

        #endregion


        #region Override - OnApplyTemplate

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_ListViewLeft      = GetTemplateChild(cPART_ListViewLeft)      as ListView;
            this.PART_ListViewCenter    = GetTemplateChild(cPART_ListViewCenter)    as ListView;
            this.PART_ListViewRight     = GetTemplateChild(cPART_ListViewRight)     as ListView;

            this.PART_GridViewLeft      = GetTemplateChild(cPART_GridViewLeft)      as DsxGridView;
            this.PART_GridViewCenter    = GetTemplateChild(cPART_GridViewCenter)    as DsxGridView;
            this.PART_GridViewRight     = GetTemplateChild(cPART_GridViewRight)     as DsxGridView;

            this.PART_ListViewLeft      .AlternationCount = this.AlternatingRowBrushes.Count;
            this.PART_ListViewCenter    .AlternationCount = this.AlternatingRowBrushes.Count;
            this.PART_ListViewRight     .AlternationCount = this.AlternatingRowBrushes.Count;

            CreateColumnLayout();
        }
        #endregion

        #region Method - CreateColumnLayout

        private void CreateColumnLayout()
        {
            DsxColumn   _gridViewColumn;
            int         _gridColCount = 0;

            if (!this.IsInitialized)
            {
                return;
            }

            if (this.InnerColumns.Count > 0)
            {
                if (this.IsGridAreaLeft)
                {
                    this.PART_ListViewLeft .RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnGridViewColumnHeaderClicked));
                }
                if (this.IsGridAreaCenter)
                {
                    this.PART_ListViewCenter.RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnGridViewColumnHeaderClicked));
                }
                if (this.IsGridAreaRight)
                {
                    this.PART_ListViewRight .RemoveHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnGridViewColumnHeaderClicked));
                }

                this.PART_GridViewLeft  .Columns.Clear();
                this.PART_GridViewCenter.Columns.Clear();
                this.PART_GridViewRight .Columns.Clear();

                this.InnerColumns .Clear();
                this.FilterColumns.Clear();
                this.FooterColumns.Clear();
            }

            //  map the columns (we need DsxColumns)
            foreach(GridViewColumn _column in this.Columns)
            {
                _gridViewColumn = new DsxColumn(_column, this);
                
                //  filtering
                if (_gridViewColumn.IsFilterActive)
                {
                    _gridViewColumn.FilterTextChanged += OnColumnFilterTextChanged;
                    this.FilterColumns.Add(_gridViewColumn);
                }

                //  summaries
                if (_gridViewColumn.IsFooterActive)
                {
                    this.FooterColumns.Add(_gridViewColumn);
                }

                //  columns
                DsxGridView _gridView = GetGridView(_gridViewColumn.ColumnArea);

                            _gridViewColumn.ColAreaIndex = _gridView.Columns.Count;
                            _gridViewColumn.ColIndex     = _gridColCount;
                            _gridColCount++;

                            _gridView.Columns.Add(_gridViewColumn);     

                this.InnerColumns.Add(_gridViewColumn);
            }

            this.AreaLeftColCount   = this.PART_GridViewLeft  .Columns.Count;
            this.AreaCenterColCount = this.PART_GridViewCenter.Columns.Count;
            this.AreaRightColCount  = this.PART_GridViewRight .Columns.Count;

            this.IsGridAreaLeft     = this.AreaLeftColCount   > 0;
            this.IsGridAreaCenter   = this.AreaCenterColCount > 0;
            this.IsGridAreaRight    = this.AreaRightColCount  > 0;

            if (this.IsGridAreaLeft)
            {
                this.PART_ListViewLeft.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnGridViewColumnHeaderClicked));
            }
            else
            {
                this.AreaLeftWidth          = new GridLength(0.0);
                this.SplitterLeftIsSizing   = false;
                this.SplitterLeftWidth      = new GridLength(0.0);
            }

            if (this.IsGridAreaCenter)
            {
                this.PART_ListViewCenter.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnGridViewColumnHeaderClicked));
            }
            else
            {
                this.AreaCenterWidth        = new GridLength(0.0);
                this.SplitterRightIsSizing  = false;
                this.SplitterRightWidth     = new GridLength(0.0);
                this.AreaRightWidth         = new GridLength(this.AreaRightWidth.Value, GridUnitType.Star);
            }

            if (this.IsGridAreaRight)
            {
                this.PART_ListViewRight.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(OnGridViewColumnHeaderClicked));
            }
            else if (this.Columns.Count>0)
            {
                throw new NotSupportedException("at least one column must be in the right area");
            }

            //  check the limitations
            if (this.IsGridAreaLeft || this.IsGridAreaCenter)
            {
                if (this.IsVirtualizing)
                {
                    throw new NotSupportedException("Multiple areas and IsVirtualizing are not allowed: turn off IsVirtualizing");
                }
                if (this.ItemFixHeight == 0.0 || this.ItemMinHeight != 0.0 || this.ItemMaxHeight != 0.0)
                {
                    throw new NotSupportedException("Multiple areas and variable ItemHeight are not allowed: set ItemFixHeight and turn off ItemMinHeight/ItemMaxHeight");
                }
            }

        }
        #endregion


        #region EventConsumer - OnLoaded

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            //  usually don't check for TemplatePart
            this.ScrollViewerLeft       = (VisualTreeHelper.GetChild(this.PART_ListViewLeft,   0) as Decorator).Child as ScrollViewer;
            this.ScrollViewerCenter     = (VisualTreeHelper.GetChild(this.PART_ListViewCenter, 0) as Decorator).Child as ScrollViewer;
            this.ScrollViewerRight      = (VisualTreeHelper.GetChild(this.PART_ListViewRight,  0) as Decorator).Child as ScrollViewer;

            DsxFilterClear  _clearBtn = ElementHelper.FindVisualChild<DsxFilterClear>(this.ScrollViewerRight, "PART_dsxFilterBtn");
            
            if (_clearBtn != null)
            {
                _clearBtn.Click += delegate { ResetFilters(); };
            }


            this.ScrollViewerLeft  .SetValue(ScrollSynchronizer.VScrollGroupProperty, "listFull");
            this.ScrollViewerCenter.SetValue(ScrollSynchronizer.VScrollGroupProperty, "listFull");
            this.ScrollViewerRight .SetValue(ScrollSynchronizer.VScrollGroupProperty, "listFull");

            this.PART_ListViewLeft  .PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            this.PART_ListViewCenter.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            this.PART_ListViewRight .PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;

            this.PART_ListViewLeft  .PreviewGotKeyboardFocus    += OnPreviewGotKeyboardFocus;
            this.PART_ListViewCenter.PreviewGotKeyboardFocus    += OnPreviewGotKeyboardFocus;
            this.PART_ListViewRight .PreviewGotKeyboardFocus    += OnPreviewGotKeyboardFocus;

            this.PART_ListViewLeft  .PreviewKeyDown             += OnPreviewKeyDown;
            this.PART_ListViewCenter.PreviewKeyDown             += OnPreviewKeyDown;
            this.PART_ListViewRight .PreviewKeyDown             += OnPreviewKeyDown;

            //  force calculate initial display
            this.HasData = false;
        }
        #endregion

        #region EventConsumer - OnSizeChanged

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.HeightChanged)
            {
                this.HeightTracker.UpdateLayout();
            }
        }
        #endregion

        #region EventConsumer - OnColumnFilterTextChanged

        void OnColumnFilterTextChanged(object sender, EventArgs e)
        {
            UpdateAdorner(null);
            FilterWaitQueue();
        }
        #endregion

        #region Callback - CheckColumnFilters

        private bool CheckColumnFilters(object listItem)
        {
            foreach(DsxColumn _column in this.FilterColumns)
            {
                if (!_column.FilterCheckValue(listItem))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion


        #region EventConsumer - OnGridViewColumnHeaderClicked

        private void OnGridViewColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            GridViewColumnHeader _gridViewHeader = e.OriginalSource as GridViewColumnHeader;

            if (_gridViewHeader != null) // && _gridViewHeader.Tag!=null && _gridViewHeader.Tag.ToString().Equals("Hdr"))
            {
                //  we have header, filter and footer, all are GridViewHeaderRowPresenter's
                //  so we get click from all of them and we need to filter out
                Border _visualBorder = ElementHelper.FindVisualChild<Border>(_gridViewHeader);

                if (_visualBorder == null || !_visualBorder.Name.Contains("Header"))
                {
                    return;
                }

                DsxColumn    _gridViewColumn = _gridViewHeader.Column as DsxColumn;

                SetSortColumn(_gridViewColumn, _gridViewHeader);
            }
        }
        #endregion

        #region EventConsumer - OnSelectionChanged

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.IsEnabled || !this.CellAdornerIsVisible || e == null || e.AddedItems.Count == 0)
            {
                return;
            }

            object  _rowItem = e.AddedItems[0];

            if (this.LastFocusRow != _rowItem && this.LastFocusColumn != null)
            {
                if (e.OriginalSource != GetListView(this.LastFocusColumn.ColumnArea))
                {
                    return;
                }
                SelectCellViewElement(_rowItem);
            }
        }
        #endregion


        #region EventConsumer - OnPreviewMouseLeftButtonDown

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement _hitElement = e.OriginalSource as FrameworkElement;

            if (!this.CellAdornerIsVisible || _hitElement == null)
            {
                return;
            }


            if (_hitElement is BulletChrome || _hitElement is DsxRowCell<CheckBox>)
            {
                if (this.AllowCheckAnyTime)
                {
                    DsxCellBase _parentCell = _hitElement as DsxCellBase;
                    
                    if (_parentCell == null)
                    {
                        _parentCell = ElementHelper.FindLogicalParent<DsxCellBase>(_hitElement);
                    }

                    if (_parentCell != null && _parentCell.Column != null && _parentCell.Column.ViewType==EViewType.CheckBox)
                    {
                        _parentCell.Column.ToggleCheck(_parentCell);
                        e.Handled = true;
                        return;
                    }
                }
            }

            if (_hitElement is Border && !(_hitElement is DsxCellBase))
            {
                //  try to find which cell we did hit
                //  because all cells should be top aligned, try to find dirty
                Point            _point      = e.GetPosition((UIElement)sender);
                                 _point.Y    = 10;
                HitTestResult    _hitResult  = VisualTreeHelper.HitTest(_hitElement, _point);
               
                if (_hitResult != null && _hitResult.VisualHit is FrameworkElement)
                {
                    _hitElement = (FrameworkElement)_hitResult.VisualHit;
                }
                else
                {
                    return;
                }
            }

            if (_hitElement == this.LastEditCell && this.LastEditCell.IsEditMode)
            {
                return;
            }

            FrameworkElement _element = UpdateAdorner(_hitElement as FrameworkElement);
            
            SetCellFocus (_element);
        }   
        #endregion

        #region EventConsumer - OnPreviewGotKeyboardFocus

        void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!this.CellAdornerIsVisible)
            {
                return;
            }

            if ((e.OriginalSource as FrameworkElement).IsFocused)
            {
                return;
            }

            FrameworkElement _focusElement = e.OriginalSource as FrameworkElement;

            this.UpdateAdorner(_focusElement);
        }
        #endregion

        #region EventConsumer - OnPreviewKeyDown

        void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!this.CellAdornerIsVisible || this.LastFocusColumn == null || this.LastFocusCell == null || this.LastFocusRow == null || this.SelectedItem == null)
            {
                return;
            }


            bool _isEditing = false;

            if (this.LastEditCell != null && this.LastEditCell.IsEditMode)
            {
                _isEditing = true;
            }

            if (!_isEditing)
            {
                if (e.Key == Key.Space && this.AllowCheckAnyTime)
                {
                    if (    this.LastFocusCell != null 
                         && this.LastFocusColumn != null 
                         && this.LastFocusColumn.ViewType == EViewType.CheckBox)
                    {
                        e.Handled = true;
                        this.LastFocusColumn.ToggleCheck(this.LastFocusCell);
                    }
                    return;
                }

                if (e.Key == Key.F2 && this.CellEditingIsEnabled)
                {
                    if (    this.LastFocusCell != null 
                         && this.LastFocusColumn != null 
                         && this.LastFocusColumn.IsEditable)
                    {
                        e.Handled = true;
                        SetEditCell(this.LastFocusColumn, this.LastFocusCell);
                    }
                    return;
                }
            }

            if (_isEditing && this.CellEditingIsEnabled)
            {
                if (e.Key == Key.Escape)
                {
                    e.Handled = true;
                    if (this.LastAdorner != null && this.LastAdorner is EditAdorner)
                    {
                        (this.LastAdorner as EditAdorner).CancelEditing();
                    }
                }
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                }
                
                if (    e.Key == Key.Enter  ||  e.Key == Key.Escape
                    ||  e.Key == Key.Up     ||  e.Key == Key.Down
                    ||  e.Key == Key.PageUp ||  e.Key == Key.PageDown)
                {
                    this.LastEditCell.Dispatcher.BeginInvoke( new Action(delegate 
                                                            {   
                                                                this.LastEditCell.Focus();
                                                                RemoveAdorner(this.LastEditCell, this.LastAdorner);
                                                            }), 
                                                            DispatcherPriority.Input);
                }
                return;
            }


            if ((e.Key == Key.Left) && this.LastFocusColumn.ColIndex > 0)
            {
                e.Handled = true;

                if (this.LastFocusColumn.ColAreaIndex == 0)
                {
                    if (this.LastFocusColumn.ColumnArea == EArea.Left)
                    {
                        return;
                    }
                    else if (this.LastFocusColumn.ColumnArea == EArea.Center && !this.IsGridAreaLeft)
                    {
                        return;
                    }
                    else if (this.LastFocusColumn.ColumnArea == EArea.Right &&  !this.IsGridAreaCenter && !this.IsGridAreaLeft)
                    {
                        return;
                    }
                }

                GridView _curArea = GetGridView(this.LastFocusColumn.ColumnArea);

                //  Ctrl-Left
                if ( (Keyboard.Modifiers & ModifierKeys.Control) > 0 )
                {
                    if (this.LastFocusColumn.ColumnArea == EArea.Right)
                    {
                        if (this.IsGridAreaCenter)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Center).Columns[this.AreaCenterColCount-1];
                        }
                        else if (this.IsGridAreaLeft)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Left).Columns[this.AreaLeftColCount-1];
                        }
                    }
                    else if (this.LastFocusColumn.ColumnArea == EArea.Center)
                    {
                        if (this.IsGridAreaLeft)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Left).Columns[this.AreaLeftColCount-1];
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (this.LastFocusColumn.ColAreaIndex > 0)
                    {
                        UpdateAdorner(null);
                        this.LastFocusColumn = (DsxColumn)_curArea.Columns[this.LastFocusColumn.ColAreaIndex-1];
                    }
                    else if (this.LastFocusColumn.ColumnArea == EArea.Right)
                    {
                        if (this.IsGridAreaCenter)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Center).Columns[this.AreaCenterColCount-1];
                        }
                        else if (this.IsGridAreaLeft)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Left).Columns[this.AreaLeftColCount-1];
                        }
                    }
                    else if (this.LastFocusColumn.ColumnArea == EArea.Center)
                    {
                        if (this.IsGridAreaLeft)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Left).Columns[this.AreaLeftColCount-1];
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                SelectCellViewElement(this.SelectedItem);
            }
            else if ((e.Key == Key.Right))
            {
                e.Handled = true;

                if (this.LastFocusColumn.ColumnArea == EArea.Right && this.LastFocusColumn.ColAreaIndex == (this.AreaRightColCount-1))
                {
                    return;
                }
                GridView _curArea = GetGridView(this.LastFocusColumn.ColumnArea);

                //  Ctrl-Right
                if ( (Keyboard.Modifiers & ModifierKeys.Control) > 0 )
                {
                    if (this.LastFocusColumn.ColumnArea == EArea.Left)
                    {
                        if (this.IsGridAreaCenter)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Center).Columns[0];
                        }
                        else if (this.IsGridAreaRight)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Right).Columns[0];
                        }
                    }
                    else if (this.LastFocusColumn.ColumnArea == EArea.Center)
                    {
                        if (this.IsGridAreaRight)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Right).Columns[0];
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (this.LastFocusColumn.ColAreaIndex < (_curArea.Columns.Count-1))
                    {
                        UpdateAdorner(null);
                        this.LastFocusColumn = (DsxColumn)_curArea.Columns[this.LastFocusColumn.ColAreaIndex+1];
                    }
                    else if (this.LastFocusColumn.ColumnArea == EArea.Left)
                    {
                        if (this.IsGridAreaCenter)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Center).Columns[0];
                        }
                        else if (this.IsGridAreaRight)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Right).Columns[0];
                        }
                    }
                    else if (this.LastFocusColumn.ColumnArea == EArea.Center)
                    {
                        if (this.IsGridAreaRight)
                        {
                            UpdateAdorner(null);
                            this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Right).Columns[0];
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                SelectCellViewElement(this.SelectedItem);
            }
            else if (e.Key == Key.Home && this.LastFocusColumn.ColIndex > 0)
            {
                e.Handled = true;
                UpdateAdorner(null);
                this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Left).Columns[0];
                SelectCellViewElement(this.SelectedItem);
            }
            else if (e.Key == Key.End && this.LastFocusColumn.ColIndex < this.InnerColumns.Count-1 )
            {
                e.Handled = true;
                UpdateAdorner(null);
                this.LastFocusColumn = (DsxColumn)GetGridView(EArea.Right).Columns[ ( GetGridView(EArea.Right).Columns.Count-1) ];
                SelectCellViewElement(this.SelectedItem);
            }
        }
        #endregion


        #region Override - OnItemSourceChanged

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            RecalcDisplaySource();
        }

        #endregion

        #region Method - RebuildLayout
		
		private void RebuildLayout()
		{
            IEnumerable _itemsSource = this.ItemsSource;

            if (_itemsSource != null)
            {
                this.ItemsSource = null;
            }
            if (this.SortColumn != null)
            {
                this.SortColumn = null;
            }

            CreateColumnLayout();

            if (_itemsSource != null)
            {
                this.ItemsSource = _itemsSource;
            }

            SetSortColumn(this.SortField);
		}
        #endregion
		

        #region Method - SelectCellViewElement

        private void SelectCellViewElement(object rowItem)
        {
            ListView        _listView = GetListView(this.LastFocusColumn.ColumnArea);
            DsxGridView     _gridView = GetGridView(this.LastFocusColumn.ColumnArea);
            ListViewItem    _listItem = _listView.ItemContainerGenerator.ContainerFromItem(rowItem) as ListViewItem; 
            if (_listItem != null) 
            { 
                if (!_listItem.IsLoaded) 
                { 
                    _listItem.ApplyTemplate(); 
                } 
                GridViewRowPresenter _rowPresenter = ElementHelper.FindVisualChild<GridViewRowPresenter>(_listItem); 

                if(_rowPresenter != null) 
                { 
                    if (VisualTreeHelper.GetChildrenCount(_rowPresenter)>0)
                    {
                        ContentPresenter    _templatedParent = VisualTreeHelper.GetChild(_rowPresenter, this.LastFocusColumn.ColAreaIndex) as ContentPresenter; 
                        DataTemplate        _dataTemplate    = this.LastFocusColumn.CellTemplate; 

                        if(_dataTemplate != null && _templatedParent != null) 
                        { 
                            if (_dataTemplate.VisualTree != null)
                            {
                                FrameworkElement _element  = _dataTemplate.FindName(this.LastFocusColumn.CellPartFrameName, _templatedParent) as FrameworkElement; 
                                SetCellFocus(_element);
                            }
                            else if (!String.IsNullOrEmpty(this.LastFocusColumn.CellPartFrameName))
                            {
                                FrameworkElement _element  = ElementHelper.FindVisualChild<FrameworkElement>(_templatedParent, this.LastFocusColumn.CellPartFrameName);
                                SetCellFocus(_element);
                            }
                            else
                            {
                                try
                                {
                                    FrameworkElement _element  = ElementHelper.FindVisualChild<FrameworkElement>(_templatedParent);
                                    SetCellFocus(_element);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        } 
                    }
                } 
            } 
        }
        #endregion

        #region Method - RemoveAdorner

        internal void RemoveAdorner(FrameworkElement gridViewCell, Adorner cellViewAdorner)
        {
            if (cellViewAdorner != null && gridViewCell != null)
            {
                AdornerLayer.GetAdornerLayer(gridViewCell).Remove(cellViewAdorner);
            }
            if (cellViewAdorner == this.LastAdorner)
            {
                this.LastAdorner = null;
            }
            if (gridViewCell == this.LastEditCell)
            {
                if (this.LastEditCell != null)
                {
                    this.LastEditCell.IsEditMode = false;
                    this.LastEditCell = null;
                }
            }
        }
        #endregion

        #region Method - AddEditAdorner

        internal void AddEditAdorner()
        {
            if (this.LastAdorner != null)
            {
                RemoveAdorner(this.LastFocusCell, this.LastAdorner);
            }
            
            this.LastEditCell            = this.LastFocusCell;
            this.LastEditCell.IsEditMode = true;

            if (this.LastFocusColumn != null && (this.LastFocusColumn.EditType != EEditType.CellTemplate || this.LastEditCell is DsxFilterCheckCell || this.LastEditCell is DsxFilterTextCell))
            {
                this.LastAdorner = new EditAdorner(this, this.LastFocusColumn, this.LastEditCell, (this.LastFocusCell.Name == DsxDataGrid.cPART_FilterText || this.LastFocusCell.Name == cPART_FilterCheck ));
                if (this.LastEditCell != null)
                {
                    AdornerLayer.GetAdornerLayer(this.LastEditCell).Add(this.LastAdorner);
                }
            }
            else
            {
                //  let the celltemplate trigger do its work
                LastEditCell.Dispatcher.BeginInvoke( new Action(delegate 
                                                        {   
                                                            this.LastEditCell.SetCellEditFocus(this.LastEditCell.Child); 
                                                        }), 
                                                        DispatcherPriority.Input);
            }
        }
        #endregion

        #region Method - SetEditCell

        public void SetEditCell(DsxColumn gridViewColumn, FrameworkElement gridViewCell)
        {
            if (this.LastFocusColumn != gridViewColumn || this.LastFocusCell != gridViewCell)
            {
                throw new Exception("SetEditCell");
            }
            else
            {
                AddEditAdorner();
            }
        }
        #endregion

        #region Method - SetCellFocus

        private void SetCellFocus(FrameworkElement focusElement)
        {
            FrameworkElement _focusElement = focusElement;

            if (_focusElement != null && !_focusElement.Focusable && _focusElement.Parent != null)
            {
                if ((_focusElement.Parent as FrameworkElement).Focusable)
                {
                    _focusElement = (FrameworkElement)_focusElement.Parent;
                }
            }

            if (_focusElement != null && !_focusElement.IsFocused && !_focusElement.IsKeyboardFocusWithin)
            {
                if (_focusElement is DsxCellBase)
                {
                    if ((_focusElement as DsxCellBase).IsEditMode)
                    {
                        return;
                    }
                }
                _focusElement.Dispatcher.BeginInvoke( new Action(delegate 
                                                        {   
                                                            _focusElement.Focus(); 
                                                        }), 
                                                        DispatcherPriority.Input);
            }
        }
        #endregion

        #region Method - UpdateAdorner

        private FrameworkElement UpdateAdorner(FrameworkElement focusElement)
        {
            if (focusElement == null && this.LastAdorner is EditAdorner && (this.LastAdorner as EditAdorner).IsFilter)
            {
                return this.LastFocusCell;
            }

            FrameworkElement    _focusElement = focusElement;
            DsxCellBase         _focusCell    = focusElement as DsxCellBase;

            //  1st test
            if (this.LastFocusCell != null && this.LastFocusCell == _focusCell)
            {
                return this.LastFocusCell;
            }

            //  compare the right level
            if (_focusCell == null && _focusElement != null)
            {
                while (_focusCell == null && (_focusElement.Parent != null || _focusElement.TemplatedParent != null))
                {
                    if (_focusElement.Parent != null)
                    {
                        _focusElement   = _focusElement.Parent as FrameworkElement;
                        _focusCell      = _focusElement as DsxCellBase;
                    }
                    else if (_focusElement.TemplatedParent != null)
                    {
                        _focusElement   = _focusElement.TemplatedParent as FrameworkElement;
                        _focusCell      = _focusElement as DsxCellBase;
                    }
                }
                if (_focusCell == null)
                {
                    return this.LastFocusCell;
                }

                //  2nd test
                if (this.LastFocusCell != null && this.LastFocusCell == _focusCell)
                {
                    return this.LastFocusCell;
                }
            }

            //  filter
            if (_focusCell != null && _focusCell != this.LastFocusCell && (_focusCell.Name==cPART_FilterText || _focusCell.Name==cPART_FilterCheck) )
            {
                if (this.LastFocusCell != null)
                {
                    RemoveAdorner(this.LastFocusCell, this.LastAdorner);
                    this.LastFocusCell = null;
                }

                this.LastFocusRow    = null;
                this.LastFocusColumn = (_focusElement.TemplatedParent as GridViewColumnHeader).Column as DsxColumn;
                this.LastFocusCell   = _focusCell;
                if (this.LastFocusCell.Column == null)
                {
                    this.LastFocusCell.Column = this.LastFocusColumn;
                }


                AddEditAdorner();
                return this.LastFocusCell;
            }


            //  Cells with Celltemplate dont have the GridViewColumn set
            if (_focusCell != null && _focusCell.Column == null)
            {
                ContentPresenter _columnPresenter;
                ContentPresenter _contentPresenter = _focusCell.TemplatedParent as ContentPresenter;

                if (_contentPresenter != null)
                {
                    GridViewRowPresenter _rowPresenter = _contentPresenter.Parent as GridViewRowPresenter;
                    if (_rowPresenter != null)
                    {
                        int _colCount = _rowPresenter.Columns.Count;
                        for (int i=0; i < _colCount; i++)
                        {
                            _columnPresenter = VisualTreeHelper.GetChild(_rowPresenter, i) as ContentPresenter; 
                            if (_columnPresenter == _contentPresenter)
                            {
                                _focusCell.Column = _rowPresenter.Columns[i] as DsxColumn;
                                this.HeightTracker.UpdateRowArea(_rowPresenter);
                                break;
                            }
                        }
                    }
                }

                if (_focusCell.Column == null)
                {
                    throw new Exception("focusCell.Column could not be resolved");
                }
            }

            //  needed for cross-selection change row/col
            if (this.LastFocusCell != null && this.LastFocusCell != _focusElement)
            {
                RemoveAdorner(this.LastFocusCell, this.LastAdorner);

                if (focusElement == null || (focusElement is ListViewItem))
                {
                    this.LastFocusRow  = null;
                    this.LastFocusCell = null;
                }
            }

            if (_focusElement != null && this.LastFocusCell != _focusElement)
            {
                RemoveAdorner(this.LastFocusCell, this.LastAdorner);

                if (this.LastFocusCell != null)
                {
                    this.LastFocusCell.IsEditMode = false;
                }

                //  timer
                this.LastFocusRow    = this.SelectedItem;
                this.LastFocusColumn = _focusCell.Column;
                this.LastFocusCell   = _focusCell;
            }
            return this.LastFocusCell;
        }
        #endregion


        #region Method - SetSortColumn

        private void SetSortColumn(string sortField)
        {
            if (this.SortColumn != null && this.SortColumn.FieldName.Equals(SortField))
            {
                return;
            }

            DsxColumn _sortColumn = null;

            foreach(DsxColumn _column in this.InnerColumns)
            {
                if (_column.FieldName.Equals(SortField))
                {
                    _sortColumn = _column;
                    break;
                }
            }
            
            if (_sortColumn != null && _sortColumn != this.SortColumn)
            {
                ListView                    _listView        = GetListView(_sortColumn.ColumnArea);
                GridViewHeaderRowPresenter  _headerPresenter = ElementHelper.FindVisualChild<GridViewHeaderRowPresenter>(_listView, "PART_HeaderRow");
                GridViewColumnHeader        _gridViewHeader  = null;
                int                         _childrenCount   = VisualTreeHelper.GetChildrenCount(_headerPresenter);  

                //  enumeration through the children is needed, since the colAreaIndex is not matching the Children Position
                for (int i = 0; i < _childrenCount; i++)  
                {    
                    DependencyObject _child = VisualTreeHelper.GetChild(_headerPresenter, i);

                    _gridViewHeader = _child as GridViewColumnHeader;

                    if (_gridViewHeader != null && _gridViewHeader.Column != null && _gridViewHeader.Column == _sortColumn)
                    {
                        break;
                    }
                }

                SetSortColumn(_sortColumn, _gridViewHeader);
            }
        }

        private void SetSortColumn(DsxColumn sortColumn, GridViewColumnHeader sortHeader)
        {
            if (sortColumn != null && sortColumn.IsSortable && sortHeader != null)
            {
                //  remove an existing adorner
                if (this.SortHeader != null)
                {
                    AdornerLayer _adorner = AdornerLayer.GetAdornerLayer(this.SortHeader);
                    if (_adorner != null)
                    {
                        _adorner.Remove(this.SortAdorner);
                    }
                }

                if (this.SortColumn != sortColumn)
                {
                    sortColumn.SortDirection = ListSortDirection.Ascending;
                }
                else
                {
                    sortColumn.SortDirection = sortColumn.SortDirection == ListSortDirection.Ascending 
                                                                            ? ListSortDirection.Descending
                                                                            : ListSortDirection.Ascending;
                }

                this.SortHeader   = sortHeader;
                this.SortColumn   = sortColumn;
                this.SortField    = SortColumn.FieldName;

                //  create adorner
                if (this.SortHeader != null)
                {
                    this.SortAdorner = new SortAdorner(this.SortHeader, sortColumn.SortDirection);
                    AdornerLayer.GetAdornerLayer(this.SortHeader).Add(this.SortAdorner);
                }
                
                RecalcDisplaySource();
            }
        }
        #endregion


        #region Method - FilterWaitQueue

        private void FilterWaitQueue()
        {
            if (this.FilterWaitTimer == null)
            {
                this.FilterWaitTimer = new Timer(200);
                this.FilterWaitTimer.AutoReset = false;
                this.FilterWaitTimer.Elapsed += delegate 
                { 
                    this.Dispatcher.BeginInvoke( new Action(delegate{ RecalcDisplaySource(); }), DispatcherPriority.Normal);
                };
            }
            else
            {
                this.FilterWaitTimer.Stop();
            }
            this.FilterWaitTimer.Start();
        }
        #endregion

        #region Method - RecalcDisplaySource

        private void RecalcDisplaySource()
        {
            if (this.ItemsSource == null && this.HasData != true)
            {
                return;
            }

            UpdateAdorner(null);

            this.DisplaySource = CollectionViewSource.GetDefaultView( this.ItemsSource );

            this.IsEnabled = false;

            //  this enforces the ListViewLayoutManager to resize properly
            this.PART_ListViewLeft   .IsEnabled = false;
            this.PART_ListViewCenter .IsEnabled = false;
            this.PART_ListViewRight  .IsEnabled = false;

            if (this.DisplaySource != null)
            {
                //  filter, sorting
                using(this.DisplaySource.DeferRefresh())
                {
                    if (this.FilterColumns.Count>0)
                    {
                        bool _filterActive = false;
                        foreach(DsxColumn _filterColumn in this.FilterColumns)
                        {
                            if (!String.IsNullOrEmpty(_filterColumn.FilterTextValue))
                            {
                                _filterActive = true;
                                break;
                            }
                        }
                        this.ComputedFilterClearVisibility = _filterActive ? Visibility.Visible : Visibility.Hidden;
                        this.DisplaySource.Filter                = this.CheckColumnFilters;
                    }
                
                    this.DisplaySource.SortDescriptions.Clear();
                
                    if (this.SortColumn != null)
                    {
                        this.DisplaySource.SortDescriptions.Add(new SortDescription(this.SortColumn.FieldName, this.SortColumn.SortDirection));
                    }
                }
            }

            //  summarize
            if (this.FooterColumns.Count>0)
            {
                foreach(DsxColumn _column in this.FooterColumns)
                {
                    _column.FooterComputedValue = 0.0M;
                }

                if (this.DisplaySource != null)
                {
                    decimal _itemValue  = 0.0M;
                    int     _count      = 0;
                    
                    foreach(object _item in this.DisplaySource)
                    {
                        _count++;

                        foreach(DsxColumn _column in this.FooterColumns)
                        {
                            switch(_column.FooterType)
                            {
                                case EFooterType.Sum:
                                case EFooterType.Avg:
                                     _column.FooterComputedValue += _column.GetFieldValue<decimal>(_item);
                                    break;
                                case EFooterType.Min:
                                    _itemValue = _column.GetFieldValue<decimal>(_item);
                                    _column.FooterComputedValue = (_column.FooterComputedValue != 0.0M && _itemValue < _column.FooterComputedValue) ? _column.FooterComputedValue : _itemValue;
                                    break;
                                case EFooterType.Max:
                                    _itemValue = _column.GetFieldValue<decimal>(_item);
                                    _column.FooterComputedValue = (_itemValue > _column.FooterComputedValue) ? _itemValue : _column.FooterComputedValue;
                                    break;
                                case EFooterType.Count:
                                    break;
                            }
                        }
                    }
                    foreach(DsxColumn _column in this.FooterColumns)
                    {
                        if (_column.FooterType == EFooterType.Avg && _column.FooterComputedValue!=0)
                        {
                            _column.FooterComputedValue /= _count;
                            _column.FooterComputedDisplay = string.Format("{0:F2}", _column.FooterComputedValue);
                        }
                        else if (_column.FooterType == EFooterType.Count)
                        {
                            _column.FooterComputedDisplay = string.Format("{0}", _count);
                        }
                        else
                        {
                            _column.FooterComputedDisplay = string.Format("{0:F2}", _column.FooterComputedValue);
                        }
                    }
                }
            }

            this.HeightTracker.ClearHeightTracker();
            this.HeightTracker.CheckHeightTracker();

            //  apply data
            if (this.IsGridAreaLeft)
            {
                this.PART_ListViewLeft.ItemsSource = this.DisplaySource;
                this.PART_ListViewLeft.IsEnabled = true;
            }
            if (this.IsGridAreaCenter)
            {
                this.PART_ListViewCenter.ItemsSource = this.DisplaySource;
                this.PART_ListViewCenter.IsEnabled = true;
            }
            if (this.IsGridAreaRight)
            {
                this.PART_ListViewRight.ItemsSource = this.DisplaySource;
                this.PART_ListViewRight.IsEnabled = true;
            }

            this.HasData   = this.ItemsSource != null;
            this.IsEnabled = true;

            this.HeightTracker.UpdateLayout();
        }
        #endregion


        #region Method - ResetFilters

        private void ResetFilters()
        {
            foreach(DsxColumn _column in this.FilterColumns)
            {
                _column.FilterTextValue = String.Empty;
            }
            RecalcDisplaySource();
        }
        #endregion

        #region Method - GetListView

        private ListView GetListView(EArea columnArea)
        {
            switch(columnArea)
            {
                case EArea.Left:       return this.PART_ListViewLeft;
                case EArea.Center:     return this.PART_ListViewCenter;
                case EArea.Right:      return this.PART_ListViewRight;
            }
            return null;
        }
        #endregion

        #region Method - GetGridView

        private DsxGridView GetGridView(EArea columnArea)
        {
            switch(columnArea)
            {
                case EArea.Left:       return this.PART_GridViewLeft;
                case EArea.Center:     return this.PART_GridViewCenter;
                case EArea.Right:      return this.PART_GridViewRight;
            }
            return null;
        }
        #endregion


        #region Method - SetTheme

        public void SetTheme(string themeColor)
        {
            this.Style       = (Style)this.TryFindResource("dsxDataGridStyle"+themeColor);

            this.HeaderStyle = (Style)this.TryFindResource("dsxHeaderStyle"  +themeColor);
            this.FilterStyle = (Style)this.TryFindResource("dsxFilterStyle"  +themeColor);
            this.FooterStyle = (Style)this.TryFindResource("dsxFooterStyle"  +themeColor);
			
			RebuildLayout();
        }
        #endregion



        #region DP - CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(DsxDataGrid), new PropertyMetadata(new CornerRadius(2)) );

        public GridLength CornerRadius
        {
            get { return (GridLength)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        #endregion


        #region DP - AreaLeftWidth

        public static readonly DependencyProperty AreaLeftWidthProperty =
            DependencyProperty.Register("AreaLeftWidth", typeof(GridLength), typeof(DsxDataGrid), new PropertyMetadata(new GridLength(100, GridUnitType.Pixel)) );

        /// <summary>
        /// Typically the sum of all columns in the left area + 2
        /// </summary>
        public GridLength AreaLeftWidth
        {
            get { return (GridLength)GetValue(AreaLeftWidthProperty); }
            set { SetValue(AreaLeftWidthProperty, value); }
        }
        #endregion

        #region DP - AreaCenterWidth

        public static readonly DependencyProperty AreaCenterWidthProperty =
            DependencyProperty.Register("AreaCenterWidth", typeof(GridLength), typeof(DsxDataGrid), new PropertyMetadata(new GridLength(100, GridUnitType.Star)) );

        /// <summary>
        /// Typically not set (=100*)
        /// </summary>
        public GridLength AreaCenterWidth
        {
            get { return (GridLength)GetValue(AreaCenterWidthProperty); }
            set { SetValue(AreaCenterWidthProperty, value); }
        }
        #endregion

        #region DP - AreaRightWidth

        public static readonly DependencyProperty AreaRightWidthProperty =
            DependencyProperty.Register("AreaRightWidth", typeof(GridLength), typeof(DsxDataGrid), new PropertyMetadata(new GridLength(100, GridUnitType.Pixel)) );

        /// <summary>
        /// Typically the sum of all columns in the right area + 22
        /// </summary>
        public GridLength AreaRightWidth
        {
            get { return (GridLength)GetValue(AreaRightWidthProperty); }
            set { SetValue(AreaRightWidthProperty, value); }
        }
        #endregion


        #region DP - SplitterLeftWidth

        public static readonly DependencyProperty SplitterLeftWidthProperty =
            DependencyProperty.Register("SplitterLeftWidth", typeof(GridLength), typeof(DsxDataGrid), new PropertyMetadata(new GridLength(3.0)) );

        public GridLength SplitterLeftWidth
        {
            get { return (GridLength)GetValue(SplitterLeftWidthProperty); }
            set { SetValue(SplitterLeftWidthProperty, value); }
        }
        #endregion

        #region DP - SplitterRightWidth

        public static readonly DependencyProperty SplitterRightWidthProperty =
            DependencyProperty.Register("SplitterRightWidth", typeof(GridLength), typeof(DsxDataGrid), new PropertyMetadata(new GridLength(3.0)) );

        public GridLength SplitterRightWidth
        {
            get { return (GridLength)GetValue(SplitterRightWidthProperty); }
            set { SetValue(SplitterRightWidthProperty, value); }
        }
        #endregion

        #region DP - SplitterLeftBackground

        public static readonly DependencyProperty SplitterLeftBackgroundProperty =
            DependencyProperty.Register("SplitterLeftBackground", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata(Brushes.DarkGray) );

        public Brush SplitterLeftBackground
        {
            get { return (Brush)GetValue(SplitterLeftBackgroundProperty); }
            set { SetValue(SplitterLeftBackgroundProperty, value); }
        }
        #endregion

        #region DP - SplitterRightBackground

        public static readonly DependencyProperty SplitterRightBackgroundProperty =
            DependencyProperty.Register("SplitterRightBackground", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata(Brushes.DarkGray) );

        public Brush SplitterRightBackground
        {
            get { return (Brush)GetValue(SplitterRightBackgroundProperty); }
            set { SetValue(SplitterRightBackgroundProperty, value); }
        }
        #endregion

        #region DP - SplitterLeftIsSizing

        public static readonly DependencyProperty SplitterLeftIsSizingProperty =
            DependencyProperty.Register("SplitterLeftIsSizing", typeof(bool), typeof(DsxDataGrid), new PropertyMetadata(false) );

        public bool SplitterLeftIsSizing
        {
            get { return (bool)GetValue(SplitterLeftIsSizingProperty); }
            set { SetValue(SplitterLeftIsSizingProperty, value); }
        }
        #endregion

        #region DP - SplitterRightIsSizing

        public static readonly DependencyProperty SplitterRightIsSizingProperty =
            DependencyProperty.Register("SplitterRightIsSizing", typeof(bool), typeof(DsxDataGrid), new PropertyMetadata(false) );

        public bool SplitterRightIsSizing
        {
            get { return (bool)GetValue(SplitterRightIsSizingProperty); }
            set { SetValue(SplitterRightIsSizingProperty, value); }
        }
        #endregion


        #region DP - CellEditingIsEnabled

        public static readonly DependencyProperty CellEditingIsEnabledProperty =
            DependencyProperty.Register("CellEditingIsEnabled", typeof(bool), typeof(DsxDataGrid), new PropertyMetadata(false) );

        public bool CellEditingIsEnabled
        {
            get { return (bool)GetValue(CellEditingIsEnabledProperty); }
            set { SetValue(CellEditingIsEnabledProperty, value); }
        }
        #endregion


        #region DP - CellAdornerIsVisible

        public static readonly DependencyProperty CellAdornerIsVisibleProperty =
            DependencyProperty.Register("CellAdornerIsVisible", typeof(bool), typeof(DsxDataGrid), new PropertyMetadata(false) );

        public bool CellAdornerIsVisible
        {
            get { return (bool)GetValue(CellAdornerIsVisibleProperty); }
            set { SetValue(CellAdornerIsVisibleProperty, value); }
        }
        #endregion

        #region DP - CellAdornerViewBorderBrush

        public static readonly DependencyProperty CellAdornerViewBorderBrushProperty =
            DependencyProperty.Register("CellAdornerViewBorderBrush", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata( Brushes.DimGray) );

        public Brush CellAdornerViewBorderBrush
        {
            get { return (Brush)GetValue(CellAdornerViewBorderBrushProperty); }
            set { SetValue(CellAdornerViewBorderBrushProperty, value); }
        }
        #endregion

        #region DP - CellAdornerEditableBorderBrush

        public static readonly DependencyProperty CellAdornerEditableBorderBrushProperty =
            DependencyProperty.Register("CellAdornerEditableBorderBrush", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata( Brushes.DarkGray) );

        public Brush CellAdornerEditableBorderBrush
        {
            get { return (Brush)GetValue(CellAdornerEditableBorderBrushProperty); }
            set { SetValue(CellAdornerEditableBorderBrushProperty, value); }
        }
        #endregion

        #region DP - CellAdornerEditingBorderBrush

        public static readonly DependencyProperty CellAdornerEditingBorderBrushProperty =
            DependencyProperty.Register("CellAdornerEditingBorderBrush", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata( new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF404040"))) );

        public Brush CellAdornerEditingBorderBrush
        {
            get { return (Brush)GetValue(CellAdornerEditingBorderBrushProperty); }
            set { SetValue(CellAdornerEditingBorderBrushProperty, value); }
        }
        #endregion

        #region DP - CellAdornerFilterBorderBrush

        public static readonly DependencyProperty CellAdornerFilterBorderBrushProperty =
            DependencyProperty.Register("CellAdornerFilterBorderBrush", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata( Brushes.DimGray) );

        public Brush CellAdornerFilterBorderBrush
        {
            get { return (Brush)GetValue(CellAdornerFilterBorderBrushProperty); }
            set { SetValue(CellAdornerFilterBorderBrushProperty, value); }
        }
        #endregion

        #region DP - SortAdornerIndicatorBrush

        public static readonly DependencyProperty SortAdornerIndicatorBrushProperty =
            DependencyProperty.Register("SortAdornerIndicatorBrush", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata( Brushes.DimGray) );

        public Brush SortAdornerIndicatorBrush
        {
            get { return (Brush)GetValue(SortAdornerIndicatorBrushProperty); }
            set { SetValue(SortAdornerIndicatorBrushProperty, value); }
        }
        #endregion



        #region DP - AllowCheckAnyTime

        public static readonly DependencyProperty AllowCheckAnyTimeProperty =
            DependencyProperty.Register("AllowCheckAnyTime", typeof(bool), typeof(DsxDataGrid), new PropertyMetadata(false) );

        public bool AllowCheckAnyTime
        {
            get { return (bool)GetValue(AllowCheckAnyTimeProperty); }
            set { SetValue(AllowCheckAnyTimeProperty, value); }
        }
        #endregion


        #region DP - SortField

        public static readonly DependencyProperty SortFieldProperty =
            DependencyProperty.Register("SortField", typeof(string), typeof(DsxDataGrid), new PropertyMetadata(null, OnSortFieldChanged) );

        public string SortField
        {
            get { return (string)GetValue(SortFieldProperty); }
            set { SetValue(SortFieldProperty, value); }
        }

        private static void OnSortFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null)
            {
                return;
            }

            DsxDataGrid  _context    = (DsxDataGrid)d;
            string       _newValue   = (string)e.NewValue;
            string       _oldValue   = (string)e.OldValue;

            if (_newValue != _oldValue && _context.IsInitialized)
            {
                _context.SetSortColumn(_newValue);
            }
        }

        #endregion


        #region DP - HeaderHeight

        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register("HeaderHeight", typeof(double), typeof(DsxDataGrid), new PropertyMetadata(22.0) );

        public double HeaderHeight
        {
            get { return (double)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }
        #endregion

        #region DP - FilterHeight

        public static readonly DependencyProperty FilterHeightProperty =
            DependencyProperty.Register("FilterHeight", typeof(double), typeof(DsxDataGrid), new PropertyMetadata(22.0) );

        public double FilterHeight
        {
            get { return (double)GetValue(FilterHeightProperty); }
            set { SetValue(FilterHeightProperty, value); }
        }
        #endregion

        #region DP - ItemFixHeight

        public static readonly DependencyProperty ItemFixHeightProperty =
            DependencyProperty.Register("ItemFixHeight", typeof(double), typeof(DsxDataGrid), new PropertyMetadata(20.0) );

        public double ItemFixHeight
        {
            get { return (double)GetValue(ItemFixHeightProperty); }
            set { SetValue(ItemFixHeightProperty, value); }
        }
        #endregion

        #region DP - ItemMinHeight

        public static readonly DependencyProperty ItemMinHeightProperty =
            DependencyProperty.Register("ItemMinHeight", typeof(double), typeof(DsxDataGrid), new PropertyMetadata(0.0) );

        public double ItemMinHeight
        {
            get { return (double)GetValue(ItemMinHeightProperty); }
            set { SetValue(ItemMinHeightProperty, value); }
        }
        #endregion

        #region DP - ItemMaxHeight

        public static readonly DependencyProperty ItemMaxHeightProperty =
            DependencyProperty.Register("ItemMaxHeight", typeof(double), typeof(DsxDataGrid), new PropertyMetadata(0.0) );

        public double ItemMaxHeight
        {
            get { return (double)GetValue(ItemMaxHeightProperty); }
            set { SetValue(ItemMaxHeightProperty, value); }
        }
        #endregion

        #region DP - FooterHeight

        public static readonly DependencyProperty FooterHeightProperty =
            DependencyProperty.Register("FooterHeight", typeof(double), typeof(DsxDataGrid), new PropertyMetadata(21.0) );

        public double FooterHeight
        {
            get { return (double)GetValue(FooterHeightProperty); }
            set { SetValue(FooterHeightProperty, value); }
        }
        #endregion


        #region DP - Columns

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(GridViewColumnCollection), typeof(DsxDataGrid), new PropertyMetadata(null));

        public GridViewColumnCollection Columns
        {
            get { return (GridViewColumnCollection)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }
        #endregion

        #region DP - HeightTracker

        public static readonly DependencyProperty HeightTrackerProperty =
            DependencyProperty.Register("HeightTracker", typeof(DsxHeightTracker), typeof(DsxDataGrid), new PropertyMetadata(null) );

        public DsxHeightTracker HeightTracker
        {
            get         { return (DsxHeightTracker)GetValue(HeightTrackerProperty); }
            private set { SetValue(HeightTrackerProperty, value); }
        }
        #endregion


        #region DP - ComputedHeaderVisibility

        public static readonly DependencyProperty ComputedHeaderVisibilityProperty =
            DependencyProperty.Register("ComputedHeaderVisibility", typeof(Visibility), typeof(DsxDataGrid), new PropertyMetadata(Visibility.Visible));

        public Visibility ComputedHeaderVisibility
        {
            get         { return (Visibility)GetValue(ComputedHeaderVisibilityProperty); }
            private set { SetValue(ComputedHeaderVisibilityProperty, value); }
        }
        #endregion

        #region DP - ComputedFilterVisibility

        public static readonly DependencyProperty ComputedFilterVisibilityProperty =
            DependencyProperty.Register("ComputedFilterVisibility", typeof(Visibility), typeof(DsxDataGrid), new PropertyMetadata(Visibility.Visible));

        public Visibility ComputedFilterVisibility
        {
            get         { return (Visibility)GetValue(ComputedFilterVisibilityProperty); }
            private set { SetValue(ComputedFilterVisibilityProperty, value); }
        }
        #endregion

        #region DP - ComputedFilterClearVisibility

        public static readonly DependencyProperty ComputedFilterClearVisibilityProperty =
            DependencyProperty.Register("ComputedFilterClearVisibility", typeof(Visibility), typeof(DsxDataGrid), new PropertyMetadata(Visibility.Hidden));

        public Visibility ComputedFilterClearVisibility
        {
            get         { return (Visibility)GetValue(ComputedFilterClearVisibilityProperty); }
            private set { SetValue(ComputedFilterClearVisibilityProperty, value); }
        }
        #endregion

        #region DP - ComputedFooterVisibility

        public static readonly DependencyProperty ComputedFooterVisibilityProperty =
            DependencyProperty.Register("ComputedFooterVisibility", typeof(Visibility), typeof(DsxDataGrid), new PropertyMetadata(Visibility.Visible));

        public Visibility ComputedFooterVisibility
        {
            get         { return (Visibility)GetValue(ComputedFooterVisibilityProperty); }
            private set { SetValue(ComputedFooterVisibilityProperty, value); }
        }
        #endregion



        #region DP - HeaderVisibility

        public static readonly DependencyProperty HeaderVisibilityProperty =
            DependencyProperty.Register("HeaderVisibility", typeof(EVisibility), typeof(DsxDataGrid), new PropertyMetadata(EVisibility.Collapsed, OnGridLayoutPartChanged));

        public EVisibility HeaderVisibility
        {
            get { return (EVisibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }
        #endregion

        #region DP - FilterVisibility

        public static readonly DependencyProperty FilterVisibilityProperty =
            DependencyProperty.Register("FilterVisibility", typeof(EVisibility), typeof(DsxDataGrid), new PropertyMetadata(EVisibility.Collapsed, OnGridLayoutPartChanged));

        public EVisibility FilterVisibility
        {
            get { return (EVisibility)GetValue(FilterVisibilityProperty); }
            set { SetValue(FilterVisibilityProperty, value); }
        }
        #endregion

        #region DP - FooterVisibility

        public static readonly DependencyProperty FooterVisibilityProperty =
            DependencyProperty.Register("FooterVisibility", typeof(EVisibility), typeof(DsxDataGrid), new PropertyMetadata(EVisibility.Collapsed, OnGridLayoutPartChanged));

        public EVisibility FooterVisibility
        {
            get { return (EVisibility)GetValue(FooterVisibilityProperty); }
            set { SetValue(FooterVisibilityProperty, value); }
        }
        #endregion


        #region DP - HorizontalScrollbarVisibility
        public static readonly DependencyProperty HorizontalScrollbarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollbarVisibility", typeof(ScrollBarVisibility), typeof(DsxDataGrid), new PropertyMetadata(ScrollBarVisibility.Auto));

        public ScrollBarVisibility HorizontalScrollbarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollbarVisibilityProperty); }
            set { SetValue(HorizontalScrollbarVisibilityProperty, value); }
        }
        #endregion

        #region DP - VerticalScrollbarVisibility

        public static readonly DependencyProperty VerticalScrollbarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollbarVisibility", typeof(ScrollBarVisibility), typeof(DsxDataGrid), new PropertyMetadata(ScrollBarVisibility.Visible));

        public ScrollBarVisibility VerticalScrollbarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollbarVisibilityProperty); }
            set { SetValue(VerticalScrollbarVisibilityProperty, value); }
        }
        #endregion

       

        #region DP - HorizontalGridLinesIsVisible

        public static readonly DependencyProperty HorizontalGridLinesIsVisibleProperty =
            DependencyProperty.Register("HorizontalGridLinesIsVisible", typeof(bool), typeof(DsxDataGrid), new PropertyMetadata(false, OnGridLayoutPartChanged) );

        public bool HorizontalGridLinesIsVisible
        {
            get { return (bool)GetValue(HorizontalGridLinesIsVisibleProperty); }
            set { SetValue(HorizontalGridLinesIsVisibleProperty, value); }
        }
        #endregion

        #region DP - VerticalGridLinesIsVisible

        public static readonly DependencyProperty VerticalGridLinesIsVisibleProperty =
            DependencyProperty.Register("VerticalGridLinesIsVisible", typeof(bool), typeof(DsxDataGrid), new PropertyMetadata(false, OnGridLayoutPartChanged) );

        public bool VerticalGridLinesIsVisible
        {
            get { return (bool)GetValue(VerticalGridLinesIsVisibleProperty); }
            set { SetValue(VerticalGridLinesIsVisibleProperty, value); }
        }
        #endregion

        #region DP - HorizontalGridLinesBrush

        public static readonly DependencyProperty HorizontalGridLinesBrushProperty =
            DependencyProperty.Register("HorizontalGridLinesBrush", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata( Brushes.DimGray) );

        public Brush HorizontalGridLinesBrush
        {
            get { return (Brush)GetValue(HorizontalGridLinesBrushProperty); }
            set { SetValue(HorizontalGridLinesBrushProperty, value); }
        }
        #endregion

        #region DP - VerticalGridLinesBrush

        public static readonly DependencyProperty VerticalGridLinesBrushProperty =
            DependencyProperty.Register("VerticalGridLinesBrush", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata( Brushes.DimGray) );

        public Brush VerticalGridLinesBrush
        {
            get { return (Brush)GetValue(VerticalGridLinesBrushProperty); }
            set { SetValue(VerticalGridLinesBrushProperty, value); }
        }
        #endregion

        #region DP - SelectedRowBrush

        public static readonly DependencyProperty SelectedRowBrushProperty =
            DependencyProperty.Register("SelectedRowBrush", typeof(Brush), typeof(DsxDataGrid), new PropertyMetadata( new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE8E8E8"))) );

        public Brush SelectedRowBrush
        {
            get { return (Brush)GetValue(SelectedRowBrushProperty); }
            set { SetValue(SelectedRowBrushProperty, value); }
        }
        #endregion

        #region DP - AlternatingRowBrushes

        public static readonly DependencyProperty AlternatingRowBrushesProperty =
            DependencyProperty.Register("AlternatingRowBrushes", typeof(List<Brush>), typeof(DsxDataGrid), new PropertyMetadata(new List<Brush>() ) );

        public List<Brush> AlternatingRowBrushes
        {
            get { return (List<Brush>)GetValue(AlternatingRowBrushesProperty); }
            set { SetValue(AlternatingRowBrushesProperty, value); }
        }
        #endregion


        #region DP - IsVirtualizing

        public static readonly DependencyProperty IsVirtualizingProperty =
            DependencyProperty.Register("IsVirtualizing", typeof(bool), typeof(DsxDataGrid), new PropertyMetadata(true));

        public bool IsVirtualizing
        {
            get { return (bool)GetValue(IsVirtualizingProperty); }
            set { SetValue(IsVirtualizingProperty, value); }
        }
        #endregion

        #region DP - HasData

        public static readonly DependencyProperty HasDataProperty =
            DependencyProperty.Register("HasData", typeof(bool?), typeof(DsxDataGrid), new PropertyMetadata(null, OnHasDataChanged));

        public bool? HasData
        {
            get         { return (bool?)GetValue(HasDataProperty); }
            private set { SetValue(HasDataProperty, value); }
        }

        private static void OnHasDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxDataGrid     _context    = (DsxDataGrid)d;
            bool?           _newValue   = (bool?)e.NewValue;
            bool?           _oldValue   = (bool?)e.OldValue;

            switch(_context.HeaderVisibility)
            {
                case EVisibility.Auto:       _context.ComputedHeaderVisibility = (bool)_newValue ? Visibility.Visible : Visibility.Hidden;   break;
                case EVisibility.Visible:    _context.ComputedHeaderVisibility = Visibility.Visible;                                   break;
                case EVisibility.Hidden:     _context.ComputedHeaderVisibility = Visibility.Hidden;                                    break;
                case EVisibility.Collapsed:  _context.ComputedHeaderVisibility = Visibility.Collapsed;                                 break;
            }

            switch(_context.FilterVisibility)
            {
                case EVisibility.Auto:       _context.ComputedFilterVisibility = (bool)_newValue ? Visibility.Visible : Visibility.Hidden;   break;
                case EVisibility.Visible:    _context.ComputedFilterVisibility = Visibility.Visible;                                   break;
                case EVisibility.Hidden:     _context.ComputedFilterVisibility = Visibility.Hidden;                                    break;
                case EVisibility.Collapsed:  _context.ComputedFilterVisibility = Visibility.Collapsed;                                 break;
            }

            switch(_context.FooterVisibility)
            {
                case EVisibility.Auto:       _context.ComputedFooterVisibility = (bool)_newValue ? Visibility.Visible : Visibility.Hidden;   break;
                case EVisibility.Visible:    _context.ComputedFooterVisibility = Visibility.Visible;                                   break;
                case EVisibility.Hidden:     _context.ComputedFooterVisibility = Visibility.Hidden;                                    break;
                case EVisibility.Collapsed:  _context.ComputedFooterVisibility = Visibility.Collapsed;                                 break;
            }

            if (!String.IsNullOrEmpty(_context.SortField))
            {
                _context.SetSortColumn(_context.SortField);
            }
        }
        #endregion


        #region DP - HeaderStyle

        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(DsxDataGrid), new PropertyMetadata(null, OnHeaderStyleChanged) );

        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }

        private static void OnHeaderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null)
            {
                return;
            }

            DsxDataGrid _context    = (DsxDataGrid)d;
            Style       _newValue   = (Style)e.NewValue;
            Style       _oldValue   = (Style)e.OldValue;

            if (_newValue != _oldValue)
            {
                //  because the applied style wont change (see DsxHeaderStyleConverter)
                //  we have to make sure the style is re-evaluted
                Style _style;

                _style = _context.PART_GridViewLeft  .ColumnHeaderContainerStyle;
                         _context.PART_GridViewLeft  .ColumnHeaderContainerStyle = null;
                         _context.PART_GridViewLeft  .ColumnHeaderContainerStyle = _style;

                _style = _context.PART_GridViewCenter.ColumnHeaderContainerStyle;
                         _context.PART_GridViewCenter.ColumnHeaderContainerStyle = null;
                         _context.PART_GridViewCenter.ColumnHeaderContainerStyle = _style;
                
                _style = _context.PART_GridViewRight .ColumnHeaderContainerStyle;
                         _context.PART_GridViewRight .ColumnHeaderContainerStyle = null;
                         _context.PART_GridViewRight .ColumnHeaderContainerStyle = _style;

                Border   _fillThumb = ElementHelper.FindVisualChild<Border>(_context.ScrollViewerRight, "PART_HeaderThumb");
                         
                         _fillThumb.GetBindingExpression(Border.CornerRadiusProperty   ).UpdateTarget();
                         _fillThumb.GetBindingExpression(Border.BackgroundProperty     ).UpdateTarget();
                         _fillThumb.GetBindingExpression(Border.BorderBrushProperty    ).UpdateTarget();
                         _fillThumb.GetBindingExpression(Border.PaddingProperty        ).UpdateTarget();
                         _fillThumb.GetBindingExpression(Border.BorderThicknessProperty).UpdateTarget();

            }
        }
        #endregion

        #region DP - FilterStyle

        public static readonly DependencyProperty FilterStyleProperty =
            DependencyProperty.Register("FilterStyle", typeof(Style), typeof(DsxDataGrid), new PropertyMetadata(null, OnFilterStyleChanged) );

        public Style FilterStyle
        {
            get { return (Style)GetValue(FilterStyleProperty); }
            set { SetValue(FilterStyleProperty, value); }
        }

        private static void OnFilterStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null)
            {
                return;
            }

            DsxDataGrid _context    = (DsxDataGrid)d;
            Style       _newValue   = (Style)e.NewValue;
            Style       _oldValue   = (Style)e.OldValue;

            if (_newValue != _oldValue)
            {
                //  because the applied style wont change (see DsxFilterStyleConverter)
                //  we have to make sure the style is re-evaluted
                Style _style;

                _style = _context.PART_GridViewLeft  .ColumnFilterContainerStyle;
                         _context.PART_GridViewLeft  .ColumnFilterContainerStyle = null;
                         _context.PART_GridViewLeft  .ColumnFilterContainerStyle = _style;

                _style = _context.PART_GridViewCenter.ColumnFilterContainerStyle;
                         _context.PART_GridViewCenter.ColumnFilterContainerStyle = null;
                         _context.PART_GridViewCenter.ColumnFilterContainerStyle = _style;
                
                _style = _context.PART_GridViewRight .ColumnFilterContainerStyle;
                         _context.PART_GridViewRight .ColumnFilterContainerStyle = null;
                         _context.PART_GridViewRight .ColumnFilterContainerStyle = _style;

            }
        }

        #endregion

        #region DP - FooterStyle

        public static readonly DependencyProperty FooterStyleProperty =
            DependencyProperty.Register("FooterStyle", typeof(Style), typeof(DsxDataGrid), new PropertyMetadata(null, OnFooterStyleChanged) );

        public Style FooterStyle
        {
            get { return (Style)GetValue(FooterStyleProperty); }
            set { SetValue(FooterStyleProperty, value); }
        }

        private static void OnFooterStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null)
            {
                return;
            }

            DsxDataGrid _context    = (DsxDataGrid)d;
            Style       _newValue   = (Style)e.NewValue;
            Style       _oldValue   = (Style)e.OldValue;

            if (_newValue != _oldValue)
            {
                //  because the applied style wont change (see DsxFooterStyleConverter)
                //  we have to make sure the style is re-evaluted
                Style _style;

                _style = _context.PART_GridViewLeft  .ColumnFooterContainerStyle;
                         _context.PART_GridViewLeft  .ColumnFooterContainerStyle = null;
                         _context.PART_GridViewLeft  .ColumnFooterContainerStyle = _style;

                _style = _context.PART_GridViewCenter.ColumnFooterContainerStyle;
                         _context.PART_GridViewCenter.ColumnFooterContainerStyle = null;
                         _context.PART_GridViewCenter.ColumnFooterContainerStyle = _style;
                
                _style = _context.PART_GridViewRight .ColumnFooterContainerStyle;
                         _context.PART_GridViewRight .ColumnFooterContainerStyle = null;
                         _context.PART_GridViewRight .ColumnFooterContainerStyle = _style;

                Border   _fillThumb = ElementHelper.FindVisualChild<Border>(_context.ScrollViewerRight, "PART_FooterThumb");

                         _fillThumb.GetBindingExpression(Border.CornerRadiusProperty   ).UpdateTarget();
                         _fillThumb.GetBindingExpression(Border.BackgroundProperty     ).UpdateTarget();
                         _fillThumb.GetBindingExpression(Border.BorderBrushProperty    ).UpdateTarget();
                         _fillThumb.GetBindingExpression(Border.PaddingProperty        ).UpdateTarget();
                         _fillThumb.GetBindingExpression(Border.BorderThicknessProperty).UpdateTarget();
            }
        }
        #endregion

        #region DP - RowCellStyle

        public static readonly DependencyProperty RowCellStyleProperty =
            DependencyProperty.Register("RowCellStyle", typeof(Style), typeof(DsxDataGrid), new PropertyMetadata(null) );

        public Style RowCellStyle
        {
            get { return (Style)GetValue(RowCellStyleProperty); }
            set { SetValue(RowCellStyleProperty, value); }
        }
        #endregion


        #region DP Callback - OnGridLayoutPartChanged

        private static void OnGridLayoutPartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxDataGrid _context        = (DsxDataGrid)d;
			
			_context.RebuildLayout();
        }
        #endregion
	}
}
