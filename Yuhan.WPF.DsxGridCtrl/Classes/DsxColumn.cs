using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Reflection;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Globalization;
using System.Windows.Controls.Primitives;
using Microsoft.Windows.Themes;
using System.Windows.Media.Animation;
using System.Collections;

namespace Yuhan.WPF.DsxGridCtrl
{
    #region StyleParts

    [StyleTypedProperty(Property = "HeaderStyle",   StyleTargetType = typeof(DsxHeaderStyle ))]
    [StyleTypedProperty(Property = "FilterStyle",   StyleTargetType = typeof(DsxFilterStyle ))]
    [StyleTypedProperty(Property = "FooterStyle",   StyleTargetType = typeof(DsxFooterStyle ))]
    [StyleTypedProperty(Property = "RowCellStyle",  StyleTargetType = typeof(DsxRowCellStyle))]
    #endregion

    public class DsxColumn : GridViewColumn
    {
        #region events

        internal event EventHandler  FilterTextChanged;

        #endregion

        #region ctors

        public DsxColumn()
        {
        }

        public DsxColumn(GridViewColumn gridViewColumn, DsxDataGrid DataGrid)
        {
            this.DataGrid               = DataGrid;

            this.CellTemplate           = gridViewColumn.CellTemplate;
            this.CellTemplateSelector   = gridViewColumn.CellTemplateSelector;
            this.DisplayMemberBinding   = gridViewColumn.DisplayMemberBinding;
            this.Header                 = gridViewColumn.Header;
            this.Width                  = gridViewColumn.Width;

            DsxColumn _gridViewColumn = gridViewColumn as DsxColumn;

            if (_gridViewColumn != null )
            {
                this.ColumnArea             = _gridViewColumn.ColumnArea;
                this.FieldName              = _gridViewColumn.FieldName;
                this.StringFormat           = _gridViewColumn.StringFormat;

                this.ViewType               = _gridViewColumn.ViewType;
                this.EditType               = _gridViewColumn.EditType;
                this.FilterType             = _gridViewColumn.FilterType;
                this.FooterType             = _gridViewColumn.FooterType;

                this.HeaderStyle            = _gridViewColumn.HeaderStyle;
                this.FilterStyle            = _gridViewColumn.FilterStyle;
                this.FooterStyle            = _gridViewColumn.FooterStyle;
                this.RowCellStyle           = _gridViewColumn.RowCellStyle;

                this.IsSizable              = _gridViewColumn.IsSizable;
                this.IsSortable             = _gridViewColumn.IsSortable;

                this.CellContentSize        = _gridViewColumn.CellContentSize;
                this.CellContentItemsSource = _gridViewColumn.CellContentItemsSource;
                this.CellContentBackground  = _gridViewColumn.CellContentBackground;

                this.CellContentRangeMin    = _gridViewColumn.CellContentRangeMin;
                this.CellContentRangeMax    = _gridViewColumn.CellContentRangeMax;
                this.CellContentIsClickable = _gridViewColumn.CellContentIsClickable;

                this.CellHAlign             = _gridViewColumn.CellHAlign;
                this.CellBackground         = _gridViewColumn.CellBackground;
                this.CellForeground         = _gridViewColumn.CellForeground;

                this.CellFontFamily         = _gridViewColumn.CellFontFamily;
                this.CellFontSize           = _gridViewColumn.CellFontSize;
                this.CellFontWeight         = _gridViewColumn.CellFontWeight;

                this.FilterCriteria         = _gridViewColumn.FilterCriteria;

                InitViewDisplay();
            }
        }
        #endregion

        #region members / properties

        internal GetHandler         ValueGetHandler     { get; set; }
        internal ListSortDirection  SortDirection       { get; set; }
        internal int                ColIndex            { get; set; }
        internal int                ColAreaIndex        { get; set; }
        internal string             CellPartFrameName   { get; set; }

        internal decimal            FooterComputedValue { get; set; }

        internal DsxDataGrid        DataGrid            { get; set; }

        #endregion


        #region Method - InitViewDisplay

        private void InitViewDisplay()
        {
            bool  _isRowFixHeight   = this.DataGrid.ItemFixHeight > 0.0;
            bool  _isRowMinHeight   = this.DataGrid.ItemMinHeight > 0.0;
            bool  _isRowMaxHeight   = this.DataGrid.ItemMaxHeight > 0.0;

            if (_isRowFixHeight && (_isRowMinHeight && _isRowMaxHeight))
            {
                throw new NotSupportedException("either specify ItemHeight or ItemMinHeight/ItemMaxHeight, but not both");
            }

            //  DataTemplate cannot be read, modified and written back
            //  so we cannot decorate the CellTemplate with the missing parts (HeightTracker/GridColumn)

            if (this.CellTemplate == null)
            {
                string  _bindPath       = String.Empty;
                string  _bindFormat     = String.Empty;

                this.CellPartFrameName  = "PART_CellFrame";

                #region prepare binding

                if (this.DisplayMemberBinding == null && !String.IsNullOrEmpty(this.FieldName))
                {
                    _bindPath       = this.FieldName;
                    _bindFormat     = this.StringFormat;
                }
                else
                {
                    _bindPath       = (this.DisplayMemberBinding as Binding).Path.Path;
                    _bindFormat     = this.DisplayMemberBinding.StringFormat;

                    this.DisplayMemberBinding = null;
                }

                BindingBase _valueBinding        = null;
                BindingBase _valueBinding2       = null;

                //  setup the binding to the value
                if (_bindPath.Contains(',') && !String.IsNullOrEmpty(this.StringFormat))
                {
                    this.EditType = EEditType.None;
                    string[] _fieldNames = _bindPath.Split(',');

                    //  we have Multibinding (which forces readonly)
                    MultiBinding _multiBinding = new MultiBinding();
                   
                    foreach(string _fieldName in _fieldNames)
                    {
                        _multiBinding.Bindings.Add( new Binding( _fieldName.Trim() ) );
                    }
                    _multiBinding.Mode  = BindingMode.OneWay;

                    _valueBinding = (BindingBase)_multiBinding;
                }
                else
                {
                    Binding _singleBinding  = null;
                    Binding _singleBinding2 = null;

                    _singleBinding      = new Binding(_bindPath);
                    _singleBinding.Mode = (this.IsEditable || this.ViewType==EViewType.CheckBox) ? BindingMode.TwoWay : BindingMode.OneWay;

                    if (this.ViewType==EViewType.Progress)
                    {
                        _singleBinding2      = new Binding(_bindPath);
                        _singleBinding2.Mode = _singleBinding.Mode;
                    }
                    
                    _valueBinding  = (BindingBase)_singleBinding;
                    _valueBinding2 = (BindingBase)_singleBinding2;
                }
                        
                if (!String.IsNullOrEmpty(_bindFormat))
                {
                    _valueBinding.StringFormat  = _bindFormat;
                }
                #endregion

                #region apply styles

                if (this.CellHAlign == null)
                {
                    this.CellHAlign = (HorizontalAlignment) this.RowCellStyle.GetStylePropertyValue<HorizontalAlignment> (DsxRowCellStyle.CellHAlignProperty.Name, this.DataGrid.RowCellStyle, HorizontalAlignment.Left);
                }
                if (this.CellBackground == null)
                {
                    this.CellBackground = (Brush)this.RowCellStyle.GetStylePropertyValue<Brush>(DsxRowCellStyle.CellBackgroundProperty.Name, this.DataGrid.RowCellStyle, null);
                }
                if (this.CellForeground == null)
                {
                    this.CellForeground = (Brush)this.RowCellStyle.GetStylePropertyValue<Brush>(DsxRowCellStyle.CellForegroundProperty.Name, this.DataGrid.RowCellStyle, null);
                }
                if (this.CellFontFamily == null)
                {
                    this.CellFontFamily = (FontFamily)this.RowCellStyle.GetStylePropertyValue<FontFamily>(DsxRowCellStyle.CellFontFamilyProperty.Name, this.DataGrid.RowCellStyle, null);
                }
                if (this.CellFontSize == null)
                {
                    this.CellFontSize = (double?)this.RowCellStyle.GetStylePropertyValue<double?>(DsxRowCellStyle.CellFontSizeProperty.Name, this.DataGrid.RowCellStyle, null);
                }
                if (this.CellFontWeight == null)
                {
                    this.CellFontWeight = (FontWeight?)this.RowCellStyle.GetStylePropertyValue<FontWeight>(DsxRowCellStyle.CellFontWeightProperty.Name, this.DataGrid.RowCellStyle, null);
                }
                #endregion

                //  the template with the content
                DataTemplate            _cellViewTemplate  = new DataTemplate();
                FrameworkElementFactory _cellViewControl   = null;
                
                //  most setting are applied as value right now

                switch(this.ViewType)
                {
                    #region EViewType.Text

                    case EViewType.Text:
                        _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<TextBlock>), this.CellPartFrameName);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.DsxColumnProperty,  this);

                        _cellViewControl.SetBinding(DsxRowCell<TextBlock>.TextProperty,             _valueBinding);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellHAlignProperty,       this.CellHAlign);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellForegroundProperty,   this.CellForeground);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CanGrowProperty,          !_isRowFixHeight);

                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontFamilyProperty,   this.CellFontFamily);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontSizeProperty,     this.CellFontSize);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontWeightProperty,   this.CellFontWeight);
                        break;

                    #endregion

                    #region EViewType.Integer

                    case EViewType.Integer:
                        _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<TextBlock>), this.CellPartFrameName);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.DsxColumnProperty,  this);

                        _cellViewControl.SetBinding(DsxRowCell<TextBlock>.TextProperty,             _valueBinding);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellHAlignProperty,       this.CellHAlign);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellForegroundProperty,   this.CellForeground);

                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontFamilyProperty,   this.CellFontFamily);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontSizeProperty,     this.CellFontSize);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontWeightProperty,   this.CellFontWeight);
                        break;

                    #endregion

                    #region EViewType.Decimal / Currency

                    case EViewType.Decimal:
                    case EViewType.Currency:
                        _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<TextBlock>), this.CellPartFrameName);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.DsxColumnProperty,  this);

                        if (String.IsNullOrEmpty(_valueBinding.StringFormat) && _valueBinding is Binding)
                        {
                            (_valueBinding as Binding).Converter = new DsxCellDecimalConverter();
                        }
                        _cellViewControl.SetBinding(DsxRowCell<TextBlock>.TextProperty,             _valueBinding);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellHAlignProperty,       this.CellHAlign);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellForegroundProperty,   this.CellForeground);

                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontFamilyProperty,   this.CellFontFamily);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontSizeProperty,     this.CellFontSize);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontWeightProperty,   this.CellFontWeight);
                        break;

                    #endregion

                    #region EViewType.Date

                    case EViewType.Date:
                        _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<TextBlock>), this.CellPartFrameName);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.DsxColumnProperty,  this);

                        if (String.IsNullOrEmpty(_valueBinding.StringFormat) && _valueBinding is Binding)
                        {
                            (_valueBinding as Binding).Converter = new DsxCellDateConverter();
                        }
                        _cellViewControl.SetBinding(DsxRowCell<TextBlock>.TextProperty,             _valueBinding);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellHAlignProperty,       this.CellHAlign);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellForegroundProperty,   this.CellForeground);

                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontFamilyProperty,   this.CellFontFamily);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontSizeProperty,     this.CellFontSize);
                        _cellViewControl.SetValue  (DsxRowCell<TextBlock>.CellFontWeightProperty,   this.CellFontWeight);
                        break;

                    #endregion

                    #region EViewType.Boolean

                    case EViewType.Boolean:
                        _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<BulletChrome>), this.CellPartFrameName);
                        _cellViewControl.SetValue  (DsxRowCell<BulletChrome>.DsxColumnProperty,     this);

                        _cellViewControl.SetBinding(DsxRowCell<BulletChrome>.IsCheckedProperty,     _valueBinding);
                        _cellViewControl.SetValue  (DsxRowCell<BulletChrome>.CellHAlignProperty,    this.CellHAlign);
                        break;

                    #endregion

                    #region EViewType.CheckBox

                    case EViewType.CheckBox:
                        this.EditType = EEditType.None;

                        _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<CheckBox>), this.CellPartFrameName);
                        _cellViewControl.SetValue  (DsxRowCell<CheckBox>.DsxColumnProperty,  this);

                        _cellViewControl.SetBinding(DsxRowCell<CheckBox>.IsCheckedProperty,     _valueBinding);
                        _cellViewControl.SetValue  (DsxRowCell<CheckBox>.CellHAlignProperty,    this.CellHAlign);
                        break;

                    #endregion

                    #region EViewType.Image

                    case EViewType.Image:
                        _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<Image>), this.CellPartFrameName);
                        _cellViewControl.SetValue  (DsxRowCell<Image>.DsxColumnProperty,  this);

                        _cellViewControl.SetBinding(DsxRowCell<Image>.ImgSourceProperty,        _valueBinding);
                        _cellViewControl.SetValue  (DsxRowCell<Image>.CellHAlignProperty,       this.CellHAlign);
                        _cellViewControl.SetValue  (DsxRowCell<Image>.CellContentSizeProperty,  this.CellContentSize);
                        break;

                    #endregion

                    #region EViewType.Progress

                    case EViewType.Progress:
                        _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<DsxCellProgressBar>), this.CellPartFrameName);
                        _cellViewControl.SetValue  (DsxRowCell<DsxCellProgressBar>.DsxColumnProperty,  this);

                        _cellViewControl.SetBinding(DsxRowCell<DsxCellProgressBar>.TextProperty,            _valueBinding);
                        _cellViewControl.SetBinding(DsxRowCell<DsxCellProgressBar>.ValueProperty,           _valueBinding2);
                        _cellViewControl.SetValue  (DsxRowCell<DsxCellProgressBar>.CellHAlignProperty,      this.CellHAlign);
                        _cellViewControl.SetValue  (DsxRowCell<DsxCellProgressBar>.CellForegroundProperty,  this.CellForeground);

                        _cellViewControl.SetValue  (DsxRowCell<DsxCellProgressBar>.CellContentBackgroundProperty,   this.CellContentBackground);
                        _cellViewControl.SetValue  (DsxRowCell<DsxCellProgressBar>.CellRangeMinProperty,            this.CellContentRangeMin);
                        _cellViewControl.SetValue  (DsxRowCell<DsxCellProgressBar>.CellRangeMaxProperty,            this.CellContentRangeMax);

                        _cellViewControl.SetValue  (DsxRowCell<DsxCellProgressBar>.CellFontFamilyProperty,   this.CellFontFamily);
                        _cellViewControl.SetValue  (DsxRowCell<DsxCellProgressBar>.CellFontSizeProperty,     this.CellFontSize);
                        _cellViewControl.SetValue  (DsxRowCell<DsxCellProgressBar>.CellFontWeightProperty,   this.CellFontWeight);

                        break;

                    #endregion

                    #region //EViewType.Button

                    //case EViewType.Button:
                    //    _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<Button>), this.CellPartFrameName);
                    //    _cellViewControl.SetValue  (DsxRowCell<Button>.DsxColumnProperty,  this);

                    //    _cellViewControl.SetValue  (DsxRowCell<Button>.CellHAlignProperty,      this.CellHAlign);
                    //    _cellViewControl.SetValue  (DsxRowCell<Button>.CellForegroundProperty,  this.CellForeground);
                    //    break;

                    #endregion

                    #region //EViewType.ToggleButton

                    //case EViewType.ToggleButton:
                    //    _cellViewControl   = new FrameworkElementFactory(typeof(DsxRowCell<ToggleButton>), this.CellPartFrameName);
                    //    _cellViewControl.SetValue  (DsxRowCell<ToggleButton>.DsxColumnProperty,         this);

                    //    _cellViewControl.SetBinding(DsxRowCell<ToggleButton>.IsCheckedProperty,         _valueBinding);
                    //    _cellViewControl.SetValue  (DsxRowCell<ToggleButton>.CellHAlignProperty,        this.CellHAlign);
                    //    _cellViewControl.SetValue  (DsxRowCell<ToggleButton>.CellForegroundProperty,    this.CellForeground);
                    //    break;

                    #endregion
                }


                _cellViewControl.SetValue  (DsxCellBase.HeightTrackerProperty,          this.DataGrid.HeightTracker);
                _cellViewControl.SetValue  (DsxCellBase.ColumnProperty,                 this);
                

                _cellViewControl.SetValue  (DsxCellBase.CellVBorderIsVisibleProperty,   this.DataGrid.VerticalGridLinesIsVisible);

                _cellViewControl.SetValue  (DsxCellBase.CellVBorderBrushProperty,       this.DataGrid.VerticalGridLinesBrush);
                _cellViewControl.SetValue  (DsxCellBase.CellViewBorderBrushProperty,    this.DataGrid.CellAdornerViewBorderBrush);
                _cellViewControl.SetValue  (DsxCellBase.CellEditableBorderBrushProperty,this.DataGrid.CellAdornerEditableBorderBrush);

                if (this.CellBackground != null)
                {
                    _cellViewControl.SetValue(DsxCellBase.CellBackgroundProperty,       this.CellBackground);

                    //  DataGridItem.IsSelected -> DsxCellBase.Background = Transparent
                    DataTrigger _rowSelectTrigger            = new DataTrigger();
                                _rowSelectTrigger.Binding    = new Binding 
                                                                {
                                                                    Path            = new PropertyPath(ListViewItem.IsSelectedProperty),
                                                                    RelativeSource  = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ListViewItem), 1)
                                                                };
                                _rowSelectTrigger.Value = true;
                                _rowSelectTrigger.Setters.Add( new Setter( DsxCellBase.IsRowSelectedProperty, true, this.CellPartFrameName) );
                    
                    _cellViewTemplate.Triggers.Add(_rowSelectTrigger);
                }

                _cellViewTemplate.VisualTree  = _cellViewControl;
                this.CellTemplate   = _cellViewTemplate;
            }
        }
        #endregion


        #region Method - GetFieldValue<T>

        internal T GetFieldValue<T>(object instance)
        {
            T _result = default(T);

            //  one time we need to create the handler
            if (this.ValueGetHandler == null && instance != null)
            {
                if (!String.IsNullOrEmpty(this.FieldName))
                {
                    PropertyInfo  _propInfo = instance.GetType().GetProperty(this.FieldName, BindingFlags.Instance|BindingFlags.Public);

                    if (_propInfo != null)
                    {
                        this.ValueGetHandler = MethodCompiler.CreateGetHandler(typeof(T), _propInfo);
                    }
                }
            }
            if (this.ValueGetHandler != null)
            {
                _result = (T)this.ValueGetHandler(instance);
            }
            return _result;
        }
        #endregion

        #region Method - FilterCheckValue

        internal bool FilterCheckValue(object instance)
        {
            if (String.IsNullOrEmpty(this.FilterTextValue))
            {
                return true;
            }

            object _fieldValue = GetFieldValue<object>(instance);
            if (_fieldValue == null)
            {
                return false;
            }
            
            bool    _result     = false;
            string  _fldText    = _fieldValue.ToString().ToLower();
            string  _fltText    = this.FilterTextValue.ToLower();


            switch(this.FilterCriteria)
            {
                case EFilterCriteria.Equals:            _result =  _fldText.Equals     (_fltText);   break;
                case EFilterCriteria.NotEquals:         _result = !_fldText.Equals     (_fltText);   break;
                case EFilterCriteria.Contains:          _result =  _fldText.Contains   (_fltText);   break;
                case EFilterCriteria.NotContains:       _result = !_fldText.Contains   (_fltText);   break;
                case EFilterCriteria.StartsWith:        _result =  _fldText.StartsWith (_fltText);   break;
                case EFilterCriteria.EndsWith:          _result =  _fldText.EndsWith   (_fltText);   break;

                case EFilterCriteria.Greater:
                case EFilterCriteria.GreaterOrEqual:
                case EFilterCriteria.Smaller:
                case EFilterCriteria.SmallerOrEqual:
                    
                    switch(this.ViewType)
                    {
                        #region Integer

                        case EViewType.Integer:

                            int _fldValueInt = Convert.ToInt32(_fldText);
                            int _fltValueInt = Convert.ToInt32(_fltText);

                            switch(this.FilterCriteria)
                            {
                                case EFilterCriteria.Greater:           _result = _fldValueInt >  _fltValueInt;   break;
                                case EFilterCriteria.GreaterOrEqual:    _result = _fldValueInt >= _fltValueInt;   break;
                                case EFilterCriteria.Smaller:           _result = _fldValueInt <  _fltValueInt;   break;
                                case EFilterCriteria.SmallerOrEqual:    _result = _fldValueInt <= _fltValueInt;   break;
                            }
                            break;
                        #endregion

                        #region Decimal

                        case EViewType.Decimal:
                        case EViewType.Progress:

                            decimal _fldValueDec = Convert.ToDecimal(_fldText);
                            decimal _fltValueDec = Convert.ToDecimal(_fltText);

                            switch(this.FilterCriteria)
                            {
                                case EFilterCriteria.Greater:           _result = _fldValueDec >  _fltValueDec;   break;
                                case EFilterCriteria.GreaterOrEqual:    _result = _fldValueDec >= _fltValueDec;   break;
                                case EFilterCriteria.Smaller:           _result = _fldValueDec <  _fltValueDec;   break;
                                case EFilterCriteria.SmallerOrEqual:    _result = _fldValueDec <= _fltValueDec;   break;
                            }
                            break;
                        #endregion

                        #region Date

                        case EViewType.Date:

                            DateTime _fldValueDate = Convert.ToDateTime(_fldText);
                            DateTime _fltValueDate = Convert.ToDateTime(_fltText);

                            switch(this.FilterCriteria)
                            {
                                case EFilterCriteria.Greater:           _result = _fldValueDate >  _fltValueDate;   break;
                                case EFilterCriteria.GreaterOrEqual:    _result = _fldValueDate >= _fltValueDate;   break;
                                case EFilterCriteria.Smaller:           _result = _fldValueDate <  _fltValueDate;   break;
                                case EFilterCriteria.SmallerOrEqual:    _result = _fldValueDate <= _fltValueDate;   break;
                            }
                            break;
                        #endregion
                    }
                    break;
            }

            return _result;
        }

        #endregion

        #region Method - FilterUpdateDisplay

        private void FilterUpdateDisplay()
        {
            string _shortSign                   = String.Empty;

            switch(this.FilterCriteria)
            {
                case EFilterCriteria.Equals:            _shortSign = "=";     break;
                case EFilterCriteria.NotEquals:         _shortSign = "<>";    break;
                case EFilterCriteria.Contains:          _shortSign = "*";     break;
                case EFilterCriteria.NotContains:       _shortSign = "!*";    break;
                case EFilterCriteria.StartsWith:        _shortSign = "=*";    break;
                case EFilterCriteria.EndsWith:          _shortSign = "*=";    break;
                case EFilterCriteria.Greater:           _shortSign = ">";     break;
                case EFilterCriteria.GreaterOrEqual:    _shortSign = ">=";    break;
                case EFilterCriteria.Smaller:           _shortSign = "<";     break;
                case EFilterCriteria.SmallerOrEqual:    _shortSign = "<=";    break;
            }

            this.FilterCriteriaSign                 = this.FilterType == EEditType.CheckBox ? String.Empty : _shortSign;

            if (String.IsNullOrEmpty(this.FilterTextValue))
            {
                this.ComputedFilterClearVisibility  = Visibility.Collapsed;
                this.FilterCheckDisplay             = null;
                this.FilterTextDisplay              = String.Empty;
                return;
            }
            
            if (this.FilterType == EEditType.CheckBox)
            {
                this.ComputedFilterClearVisibility  = Visibility.Collapsed;

                if (this.FilterTextValue == Boolean.TrueString)
                {
                    this.FilterCheckDisplay         = true;
                }
                else if (this.FilterTextValue == Boolean.FalseString)
                {
                    this.FilterCheckDisplay         = false;
                }
                else
                {
                    this.FilterCheckDisplay         = null;
                }
                this.FilterTextDisplay              = String.Empty;
            }
            else
            {
                this.ComputedFilterClearVisibility  = Visibility.Visible;
                this.FilterCheckDisplay             = null;


                this.FilterTextDisplay              = string.Format("{0} {1}", this.FilterCriteriaSign, this.FilterTextValue);
            }
        }

        #endregion

        #region Method - ToggleCheck

        internal void ToggleCheck(DsxCellBase curCell)
        {
            DsxRowCell<CheckBox> _checkCell = curCell as DsxRowCell<CheckBox>;

            if (_checkCell != null)
            {
                bool    _newValue = (bool)_checkCell.IsChecked;
                        _newValue = !_newValue;

                System.Diagnostics.Debug.WriteLine("ToggleCheck {0}", _newValue);

                _checkCell.Dispatcher.BeginInvoke( new Action(delegate 
                                                        {   
                                                            _checkCell.IsChecked = _newValue;
                                                        }), 
                                                        DispatcherPriority.Input);
            }
        }
        #endregion


        #region Property - IsEditable

        internal bool IsEditable
        {
            get { return (this.DataGrid.CellEditingIsEnabled && this.EditType != EEditType.None); }
        }
        #endregion

        #region Property - IsFilterActive

        internal bool IsFilterActive
        {
            get { return this.FilterType != EEditType.None; }
        }
        #endregion

        #region Property - IsFooterActive

        internal bool IsFooterActive
        {
            get { return this.FooterType != EFooterType.None; }
        }
        #endregion


        //  reminder: add DP's to be cloned in the constructor

        #region DP - ColumnArea

        public static readonly DependencyProperty ColumnAreaProperty =
            DependencyProperty.Register("ColumnArea", typeof(EArea), typeof(DsxColumn), new PropertyMetadata(EArea.Right));

        public EArea ColumnArea
        {
            get { return (EArea)GetValue(ColumnAreaProperty); }
            set { SetValue(ColumnAreaProperty, value); }
        }
        #endregion

        #region DP - FieldName

        public static readonly DependencyProperty FieldNameProperty =
            DependencyProperty.Register("FieldName", typeof(string), typeof(DsxColumn), new PropertyMetadata(String.Empty));

        public string FieldName
        {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }
        #endregion

        #region DP - StringFormat

        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register("StringFormat", typeof(string), typeof(DsxColumn), new PropertyMetadata(String.Empty));

        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }
        #endregion


        #region DP - ViewType

        public static readonly DependencyProperty ViewTypeProperty =
            DependencyProperty.Register("ViewType", typeof(EViewType), typeof(DsxColumn), new PropertyMetadata(EViewType.Text));

        public EViewType ViewType
        {
            get { return (EViewType)GetValue(ViewTypeProperty); }
            set { SetValue(ViewTypeProperty, value); }
        }
        #endregion

        #region DP - EditType

        public static readonly DependencyProperty EditTypeProperty =
            DependencyProperty.Register("EditType", typeof(EEditType), typeof(DsxColumn), new PropertyMetadata(EEditType.None));

        public EEditType EditType
        {
            get { return (EEditType)GetValue(EditTypeProperty); }
            set { SetValue(EditTypeProperty, value); }
        }
        #endregion

        #region DP - FilterType

        public static readonly DependencyProperty FilterTypeProperty =
            DependencyProperty.Register("FilterType", typeof(EEditType), typeof(DsxColumn), new PropertyMetadata(EEditType.None));

        public EEditType FilterType
        {
            get { return (EEditType)GetValue(FilterTypeProperty); }
            set { SetValue(FilterTypeProperty, value); }
        }
        #endregion

        #region DP - FooterType

        public static readonly DependencyProperty FooterTypeProperty =
            DependencyProperty.Register("FooterType", typeof(EFooterType), typeof(DsxColumn), new PropertyMetadata(EFooterType.None) );

        public EFooterType FooterType
        {
            get { return (EFooterType)GetValue(FooterTypeProperty); }
            set { SetValue(FooterTypeProperty, value); }
        }
        #endregion


        #region DP - HeaderStyle

        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(DsxColumn), new PropertyMetadata(null) );

        public Style HeaderStyle
        {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
        }
        #endregion

        #region DP - FilterStyle

        public static readonly DependencyProperty FilterStyleProperty =
            DependencyProperty.Register("FilterStyle", typeof(Style), typeof(DsxColumn), new PropertyMetadata(null) );

        public Style FilterStyle
        {
            get { return (Style)GetValue(FilterStyleProperty); }
            set { SetValue(FilterStyleProperty, value); }
        }
        #endregion

        #region DP - FooterStyle

        public static readonly DependencyProperty FooterStyleProperty =
            DependencyProperty.Register("FooterStyle", typeof(Style), typeof(DsxColumn), new PropertyMetadata(null) );

        public Style FooterStyle
        {
            get { return (Style)GetValue(FooterStyleProperty); }
            set { SetValue(FooterStyleProperty, value); }
        }
        #endregion

        #region DP - RowCellStyle

        public static readonly DependencyProperty RowCellStyleProperty =
            DependencyProperty.Register("RowCellStyle", typeof(Style), typeof(DsxColumn), new PropertyMetadata(null) );

        public Style RowCellStyle
        {
            get { return (Style)GetValue(RowCellStyleProperty); }
            set { SetValue(RowCellStyleProperty, value); }
        }
        #endregion


        #region DP - IsSizable

        public static readonly DependencyProperty IsSizableProperty =
            DependencyProperty.Register("IsSizable", typeof(bool), typeof(DsxColumn), new PropertyMetadata(true) );

        public bool IsSizable
        {
            get { return (bool)GetValue(IsSizableProperty); }
            set { SetValue(IsSizableProperty, value); }
        }
        #endregion

        #region DP - IsSortable

        public static readonly DependencyProperty IsSortableProperty =
            DependencyProperty.Register("IsSortable", typeof(bool), typeof(DsxColumn), new PropertyMetadata(true) );

        public bool IsSortable
        {
            get { return (bool)GetValue(IsSortableProperty); }
            set { SetValue(IsSortableProperty, value); }
        }
        #endregion


        #region DP - CellContentSize

        public static readonly DependencyProperty CellContentSizeProperty =
            DependencyProperty.Register("CellContentSize", typeof(Size), typeof(DsxColumn), new PropertyMetadata(new Size(16.0,16.0)) );

        public Size CellContentSize
        {
            get { return (Size)GetValue(CellContentSizeProperty); }
            set { SetValue(CellContentSizeProperty, value); }
        }
        #endregion

        #region DP - CellContentItemsSource

        public static readonly DependencyProperty CellContentItemsSourceProperty =
            DependencyProperty.Register("CellContentItemsSource", typeof(IEnumerable), typeof(DsxColumn), new PropertyMetadata(null) );

        public IEnumerable CellContentItemsSource
        {
            get { return (IEnumerable)GetValue(CellContentItemsSourceProperty); }
            set { SetValue(CellContentItemsSourceProperty, value); }
        }
        #endregion

        #region DP - CellContentBackground

        public static readonly DependencyProperty CellContentBackgroundProperty =
            DependencyProperty.Register("CellContentBackground", typeof(Brush), typeof(DsxColumn), new PropertyMetadata(Brushes.Olive));

        public Brush CellContentBackground
        {
            get { return (Brush)GetValue(CellContentBackgroundProperty); }
            set { SetValue(CellContentBackgroundProperty, value); }
        }
        #endregion

        #region DP - CellContentRangeMin

        public static readonly DependencyProperty CellContentRangeMinProperty =
            DependencyProperty.Register("CellContentRangeMin", typeof(double), typeof(DsxColumn), new PropertyMetadata(0.0));

        public double CellContentRangeMin
        {
            get { return (double)GetValue(CellContentRangeMinProperty); }
            set { SetValue(CellContentRangeMinProperty, value); }
        }
        #endregion

        #region DP - CellContentRangeMax

        public static readonly DependencyProperty CellContentRangeMaxProperty =
            DependencyProperty.Register("CellContentRangeMax", typeof(double), typeof(DsxColumn), new PropertyMetadata(100.0));

        public double CellContentRangeMax
        {
            get { return (double)GetValue(CellContentRangeMaxProperty); }
            set { SetValue(CellContentRangeMaxProperty, value); }
        }
        #endregion

        #region DP - CellContentIsClickable

        public static readonly DependencyProperty CellContentIsClickableProperty =
            DependencyProperty.Register("CellContentIsClickable", typeof(bool), typeof(DsxColumn), new PropertyMetadata(false) );

        public bool CellContentIsClickable
        {
            get { return (bool)GetValue(CellContentIsClickableProperty); }
            set { SetValue(CellContentIsClickableProperty, value); }
        }
        #endregion



        #region DP - CellHAlign

        public static readonly DependencyProperty CellHAlignProperty =
            DependencyProperty.Register("CellHAlign", typeof(HorizontalAlignment?), typeof(DsxColumn), new PropertyMetadata(null));

        public HorizontalAlignment? CellHAlign
        {
            get { return (HorizontalAlignment?)GetValue(CellHAlignProperty); }
            set { SetValue(CellHAlignProperty, value); }
        }
        #endregion

        #region DP - CellBackground

        public static readonly DependencyProperty CellBackgroundProperty =
            DependencyProperty.Register("CellBackground", typeof(Brush), typeof(DsxColumn), new PropertyMetadata(null));

        public Brush CellBackground
        {
            get { return (Brush)GetValue(CellBackgroundProperty); }
            set { SetValue(CellBackgroundProperty, value); }
        }
        #endregion

        #region DP - CellForeground

        public static readonly DependencyProperty CellForegroundProperty =
            DependencyProperty.Register("CellForeground", typeof(Brush), typeof(DsxColumn), new PropertyMetadata(null));

        public Brush CellForeground
        {
            get { return (Brush)GetValue(CellForegroundProperty); }
            set { SetValue(CellForegroundProperty, value); }
        }
        #endregion

        #region DP - CellFontFamily

        public static readonly DependencyProperty CellFontFamilyProperty =
            DependencyProperty.Register("CellFontFamily", typeof(FontFamily), typeof(DsxColumn), new PropertyMetadata(null));

        public FontFamily CellFontFamily
        {
            get { return (FontFamily)GetValue(CellFontFamilyProperty); }
            set { SetValue(CellFontFamilyProperty, value); }
        }
        #endregion

        #region DP - CellFontSize

        public static readonly DependencyProperty CellFontSizeProperty =
            DependencyProperty.Register("CellFontSize", typeof(double?), typeof(DsxColumn), new PropertyMetadata(null));

        public double? CellFontSize
        {
            get { return (double?)GetValue(CellFontSizeProperty); }
            set { SetValue(CellFontSizeProperty, value); }
        }
        #endregion

        #region DP - CellFontWeight

        public static readonly DependencyProperty CellFontWeightProperty =
            DependencyProperty.Register("CellFontWeight", typeof(FontWeight?), typeof(DsxColumn), new PropertyMetadata(null));

        public FontWeight? CellFontWeight
        {
            get { return (FontWeight?)GetValue(CellFontWeightProperty); }
            set { SetValue(CellFontWeightProperty, value); }
        }
        #endregion


        #region DP - FooterComputedDisplay

        public static readonly DependencyProperty FooterComputedDisplayProperty =
            DependencyProperty.Register("FooterComputedDisplay", typeof(string), typeof(DsxColumn), new PropertyMetadata(String.Empty) );

        public string FooterComputedDisplay
        {
            get             { return (string)GetValue(FooterComputedDisplayProperty); }
            internal set    { SetValue(FooterComputedDisplayProperty, value); }
        }
        #endregion



        #region DP - FilterCriteria

        public static readonly DependencyProperty FilterCriteriaProperty =
            DependencyProperty.Register("FilterCriteria", typeof(EFilterCriteria), typeof(DsxColumn), new PropertyMetadata(EFilterCriteria.Contains, OnFilterCriteriaChanged));

        public EFilterCriteria FilterCriteria
        {
            get { return (EFilterCriteria)GetValue(FilterCriteriaProperty); }
            set { SetValue(FilterCriteriaProperty, value); }
        }

        private static void OnFilterCriteriaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null || e.NewValue == null)
            {
                return;
            }

            DsxColumn       _context    = (DsxColumn)d;
            EFilterCriteria _newValue   = (EFilterCriteria)e.NewValue;
            EFilterCriteria _oldValue   = (EFilterCriteria)e.OldValue;

            if (_newValue != _oldValue && _context.FilterTextChanged != null)
            {
                _context.FilterTextChanged(_context, null );
            }
            _context.FilterUpdateDisplay();
        }
        #endregion

        #region DP - FilterTextValue

        public static readonly DependencyProperty FilterTextValueProperty =
            DependencyProperty.Register("FilterTextValue", typeof(string), typeof(DsxColumn), new PropertyMetadata(String.Empty, OnFilterTextValueChanged));

        public string FilterTextValue
        {
            get { return (string)GetValue(FilterTextValueProperty); }
            set { SetValue(FilterTextValueProperty, value); }
        }

        private static void OnFilterTextValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
            if (d == null)
            {
                return;
            }

            DsxColumn   _context    = (DsxColumn)d;
            string      _newValue   = (string)e.NewValue;
            string      _oldValue   = (string)e.OldValue;

            if (_newValue != _oldValue && _context.FilterTextChanged != null)
            {
                _context.FilterTextChanged(_context, null );
            }
            _context.FilterUpdateDisplay();
        }
        #endregion

        #region DP - FilterCheckDisplay

        public static readonly DependencyProperty FilterCheckDisplayProperty =
            DependencyProperty.Register("FilterCheckDisplay", typeof(bool?), typeof(DsxColumn), new PropertyMetadata(null));

        public bool? FilterCheckDisplay
        {
            get         { return (bool?)GetValue(FilterCheckDisplayProperty); }
            private set { SetValue(FilterCheckDisplayProperty, value); }
        }
        #endregion

        #region DP - FilterTextDisplay

        public static readonly DependencyProperty FilterTextDisplayProperty =
            DependencyProperty.Register("FilterTextDisplay", typeof(string), typeof(DsxColumn), new PropertyMetadata(String.Empty));

        public string FilterTextDisplay
        {
            get         { return (string)GetValue(FilterTextDisplayProperty); }
            private set { SetValue(FilterTextDisplayProperty, value); }
        }
        #endregion

        #region DP - FilterCriteriaSign

        public static readonly DependencyProperty FilterCriteriaSignProperty =
            DependencyProperty.Register("FilterCriteriaSign", typeof(string), typeof(DsxColumn), new PropertyMetadata("="));

        public string FilterCriteriaSign
        {
            get         { return (string)GetValue(FilterCriteriaSignProperty); }
            private set { SetValue(FilterCriteriaSignProperty, value); }
        }
        #endregion

        #region DP - ComputedFilterClearVisibility

        public static readonly DependencyProperty ComputedFilterClearVisibilityProperty =
            DependencyProperty.Register("ComputedFilterClearVisibility", typeof(Visibility), typeof(DsxColumn), new PropertyMetadata(Visibility.Collapsed));

        public Visibility ComputedFilterClearVisibility
        {
            get         { return (Visibility)GetValue(ComputedFilterClearVisibilityProperty); }
            private set { SetValue(ComputedFilterClearVisibilityProperty, value); }
        }
        #endregion

    }
}
