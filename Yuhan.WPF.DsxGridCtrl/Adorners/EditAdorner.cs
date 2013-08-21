using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Windows.Themes;
using System.Windows.Input;

namespace Yuhan.WPF.DsxGridCtrl
{
    public class EditAdorner : Adorner
    {
        #region members / properties

        private     DsxDataGrid             DataGrid        { get; set; }
        private     DsxColumn               Column          { get; set; }

        private     DependencyProperty      EditProperty    { get; set; }
        private     DependencyProperty      EditProperty2   { get; set; }

        private     BindingExpressionBase   EditBinding     { get; set; }
        private     BindingExpressionBase   EditBinding2    { get; set; }

        private     Control                 EditCtrl        { get; set; }
        private     object                  EditOrgValue    { get; set; }

        private     Grid                    EditPanel       { get; set; }

        private     VisualCollection        VisualChildren  { get; set; }

        public      bool                    IsFilter        { get; set; }
        #endregion

        #region ctors

        public EditAdorner(DsxDataGrid DataGrid, DsxColumn gridViewColumn, FrameworkElement gridViewCell, bool isFilter) : base( gridViewCell )
        {
            this.IsFilter       = isFilter;

            this.Focusable      = false;

            this.VisualChildren = new VisualCollection(this);

            this.DataGrid       = DataGrid;
            this.Column         = gridViewColumn;

            InitEditing(gridViewCell);

            this.EditCtrl.LostFocus += OnLostFocus;
        }
        #endregion
        
        #region EventConsumer - OnLostFocus

        void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender == e.OriginalSource)
            {
                e.Handled = true;
                //  reset focus
                //  this.EditCtrl.Dispatcher.BeginInvoke( new Action(delegate {   this.EditCtrl.Focus(); }), DispatcherPriority.Input);
                return;
            }
            //  prevent closing if a popup is opened (DatePicker)
            if (e.OriginalSource == this.EditCtrl && !this.EditCtrl.IsKeyboardFocusWithin)
            {
                if (this.EditBinding != null && this.EditBinding.Status == BindingStatus.Active)
                {
                    Validation.ClearInvalid(this.EditBinding);
                    BindingOperations.ClearBinding(this.EditCtrl, this.EditProperty);
                }
                this.DataGrid.RemoveAdorner( (FrameworkElement)this.AdornedElement, this);

                (this.AdornedElement as DsxCellBase).IsEditMode = false;
            }
        }
        #endregion


        #region Override - VisualChildrenCount

        protected override int VisualChildrenCount 
        { 
            get { return this.VisualChildren.Count; } 
        }
        #endregion

        #region Override - GetVisualChild

        protected override Visual GetVisualChild(int index) 
        { 
            return this.VisualChildren[index]; 
        }
        #endregion

        #region Override - ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect    _size = new Rect(finalSize);
            
            this.EditPanel.Arrange(_size);

            return finalSize;
        }
        #endregion

        #region Override - OnRender

        protected override void OnRender(DrawingContext drawingContext)
        {
            Brush   _borderBrush        = this.IsFilter ? this.DataGrid.CellAdornerFilterBorderBrush: this.DataGrid.CellAdornerEditingBorderBrush;
            Pen     _renderPen          = new Pen(_borderBrush, 2);
            double  _renderRadius       = 2.0;
            Rect    _adornedElementRect = new Rect(this.AdornedElement.RenderSize);

            drawingContext.DrawRoundedRectangle(SystemColors.WindowBrush, _renderPen, _adornedElementRect, _renderRadius, _renderRadius);

            base.OnRender(drawingContext);

            _adornedElementRect.Inflate(-1,-1);
            drawingContext.DrawRoundedRectangle(Brushes.Transparent, _renderPen, _adornedElementRect, _renderRadius, _renderRadius);
        }
        #endregion


        #region Method - CancelEditing

        internal void CancelEditing()
        {
            //  revert Value
            this.EditCtrl.SetValue(this.EditProperty, this.EditOrgValue);
            (this.AdornedElement as DsxCellBase).IsEditMode = false;
        }

        #endregion


        #region Method - InitEditing

        private void InitEditing(FrameworkElement gridViewCell)
        {
            DependencyProperty _viewProperty  = null;
            DependencyProperty _viewProperty2 = null;

            switch(this.Column.ViewType)
            {
                case EViewType.Text:  
                case EViewType.Integer:
                case EViewType.Decimal:
                case EViewType.Currency:
                case EViewType.Date:
                    _viewProperty = DsxRowCell<TextBlock>.TextProperty;       
                    break;


                case EViewType.Boolean:
                case EViewType.CheckBox:
                    _viewProperty = DsxRowCell<BulletChrome>.IsCheckedProperty;
                    break;

                case EViewType.Image:   
                    _viewProperty = DsxRowCell<Image>.ImgSourceProperty;       
                    break;

                case EViewType.Progress:   
                    _viewProperty  = DsxRowCell<DsxCellProgressBar>.ValueProperty;
                    _viewProperty2 = DsxRowCell<DsxCellProgressBar>.TextProperty;
                    break;
            }

            //  hosting panel
            this.EditPanel                      = new Grid();
            this.EditPanel.Focusable            = false;
            this.EditPanel.Margin               = new Thickness(3);
            this.EditPanel.Background           = Brushes.Transparent;
            this.EditPanel.HorizontalAlignment  = HorizontalAlignment.Stretch;
            this.EditPanel.VerticalAlignment    = this.IsFilter ? VerticalAlignment.Center : VerticalAlignment.Top;

            EEditType _editType = this.IsFilter ? this.Column.FilterType : this.Column.EditType;

            switch(_editType)
            {
                case EEditType.TextBox:    
                    this.EditProperty = TextBox.TextProperty;

                    TextBox     _textBox                    = new TextBox();
                                _textBox.FocusVisualStyle   = null;
                                _textBox.Margin             = new Thickness(0,1,0,1);
                                _textBox.Padding            = new Thickness(1,-1,1,-1);
                                _textBox.TextWrapping       = this.Column.DataGrid.ItemFixHeight != 0.0 ? TextWrapping.NoWrap : TextWrapping.Wrap;

                    switch(this.Column.CellHAlign)
                    {
                        case HorizontalAlignment.Left:      _textBox.TextAlignment = TextAlignment.Left;    break;
                        case HorizontalAlignment.Right:     _textBox.TextAlignment = TextAlignment.Right;   break;
                        case HorizontalAlignment.Center:    _textBox.TextAlignment = TextAlignment.Center;  break;
                    }

                    this.EditCtrl     = _textBox;
                    break;

                case EEditType.CheckBox:   

                    this.EditProperty = CheckBox.IsCheckedProperty;

                    CheckBox            _checkBox                   = new CheckBox();
                                        _checkBox.IsThreeState      = this.IsFilter;
                                        _checkBox.FocusVisualStyle  = null;
                                        _checkBox.KeyDown += delegate { _checkBox.IsChecked = !_checkBox.IsChecked; };

                    if (this.IsFilter)
                    {
                        _checkBox.IsChecked = null;
                        this.Column.FilterCriteria = EFilterCriteria.Equals;
                    }

                    this.EditCtrl     = _checkBox;

                    this.EditPanel.HorizontalAlignment = (HorizontalAlignment)this.Column.CellHAlign;

                    break;

                case EEditType.DatePicker:

                    this.EditProperty = DsxCellDatePicker.SelectedDateProperty;  

                    DsxCellDatePicker   _datePicker                  = new DsxCellDatePicker();
                                        _datePicker.FocusVisualStyle = null;
                                        _datePicker.Margin           = new Thickness(0);
                                        _datePicker.Padding          = new Thickness(0);
                                        _datePicker.Background       = SystemColors.WindowBrush;

                    this.EditCtrl     = _datePicker;

					if (!this.IsFilter)
					{
                    	this.EditPanel.HorizontalAlignment = (HorizontalAlignment)this.Column.CellHAlign;
					}

                    break;

                case EEditType.ComboBox:

                    this.EditProperty = DsxCellComboBox.SelectedValueProperty;

                    DsxCellComboBox     _comboBox                   = new DsxCellComboBox();
                                        _comboBox.FocusVisualStyle  = null;
                                        _comboBox.Margin            = new Thickness(0);
                                        _comboBox.Padding           = new Thickness(0);
                                        _comboBox.Background        = SystemColors.WindowBrush;
                                        _comboBox.BorderThickness   = new Thickness(0);

                                        _comboBox.IsTextSearchEnabled = true;
                                        _comboBox.IsEditable          = true;

                                        //_comboBox.DisplayMemberPath   = this.Column.CellContentDisplayMemberPath;
                                        _comboBox.ItemsSource         = this.Column.CellContentItemsSource;

                    this.EditCtrl     = _comboBox;

                    break;

                case EEditType.Slider:

                    this.EditProperty  = DsxCellSlider.ValueProperty;
                    this.EditProperty2 = DsxCellSlider.TextProperty;

                    DsxCellSlider       _sliderCtrl                   = new DsxCellSlider();
                                        _sliderCtrl.FocusVisualStyle  = null;
                                        _sliderCtrl.Margin            = new Thickness(2,2,8,0);
                                        _sliderCtrl.Padding           = new Thickness(0);
                                        _sliderCtrl.Foreground        = this.Column.CellForeground;
                                        _sliderCtrl.Background        = SystemColors.WindowBrush;
                                        _sliderCtrl.ContentBackground = this.Column.CellContentBackground;
                                        _sliderCtrl.TextAlignment     = TextAlignment.Right;
                                        _sliderCtrl.BorderThickness   = new Thickness(0);

                    this.EditCtrl     = _sliderCtrl;

                    break;
            }

            this.EditCtrl.HorizontalAlignment   = System.Windows.HorizontalAlignment.Stretch;
            this.EditCtrl.VerticalAlignment     = System.Windows.VerticalAlignment  .Stretch;
            this.EditCtrl.Margin                = new Thickness(0);
            this.EditCtrl.BorderThickness       = new Thickness(0);

            this.EditCtrl.DataContext           = this.IsFilter ? this.Column : gridViewCell.DataContext;
            this.EditCtrl.IsTabStop             =!this.IsFilter;

            // bindings
            BindingExpression   _bindingExpr    = null;
            Binding             _binding        = null;
            BindingExpression   _bindingExpr2   = null;
            Binding             _binding2       = null;

            if (_viewProperty != null)
            {
                if (this.IsFilter)
                {
                    _binding        = new Binding(DsxColumn.FilterTextValueProperty.Name);

                    if (_editType == EEditType.CheckBox)
                    {
                        _binding.FallbackValue = null;
                    }
                    if (_editType == EEditType.DatePicker)
                    {
                        _binding.Converter = new DsxCellDateConverter();
                    }
                }
                else
                {
                    _bindingExpr     = gridViewCell.GetBindingExpression(_viewProperty);
                    _binding         = new Binding(_bindingExpr.ParentBinding.Path.Path);
                }
                _binding.Mode                 = BindingMode.TwoWay;
                _binding.UpdateSourceTrigger  = UpdateSourceTrigger.PropertyChanged;

                this.EditBinding  = this.EditCtrl.SetBinding(this.EditProperty, _binding);
            }

            if (_viewProperty2 != null && !this.IsFilter)
            {
                _bindingExpr2                 = gridViewCell.GetBindingExpression(_viewProperty2);
                _binding2                     = new Binding(_bindingExpr2.ParentBinding.Path.Path);
                _binding2.Mode                = BindingMode.TwoWay;
                _binding2.StringFormat        = this.Column.StringFormat;
                _binding2.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

                this.EditBinding2 = this.EditCtrl.SetBinding(this.EditProperty2, _binding2);
            }

            this.EditOrgValue = this.EditCtrl.GetValue(this.EditProperty);

            //  filter button
            if (this.IsFilter && _editType != EEditType.CheckBox)
            {
                Binding         _clearBinding                       = new Binding("ComputedFilterClearVisibility");
                                _clearBinding.Mode                  = BindingMode.OneWay;
                                _clearBinding.UpdateSourceTrigger   = UpdateSourceTrigger.PropertyChanged;

                DsxFilterPopup  _criteria                           = new DsxFilterPopup(this.Column);
                                _criteria.Focusable                 = false;
                                _criteria.HorizontalAlignment       = HorizontalAlignment.Left;
                                _criteria.SetValue(DockPanel.DockProperty, Dock.Left);

                DsxFilterClear  _clearBtn                           = new DsxFilterClear();
                                _clearBtn.DataContext               = this.EditCtrl.DataContext;

                                _clearBtn.SetBinding(FrameworkElement.VisibilityProperty, _clearBinding);
                                _clearBtn.SetValue  (DockPanel.DockProperty, Dock.Right);

                                _clearBtn.Click += delegate { this.Column.FilterTextValue = String.Empty; };
                
                DockPanel       _dockPanel                          = new DockPanel();

                                _dockPanel.Children.Add(_criteria);
                                _dockPanel.Children.Add(_clearBtn);
                                _dockPanel.Children.Add(this.EditCtrl);

                this.EditPanel.Children.Add(_dockPanel);
            }
            else
            {
                this.EditPanel.Children.Add(this.EditCtrl);
            }
            this.VisualChildren.Add(this.EditPanel);

            switch(this.Column.EditType)
            {
                case EEditType.TextBox:    
                    (this.EditCtrl as TextBox).SelectAll();
                    break;
                case EEditType.CheckBox:
                    break;
                case EEditType.DatePicker:
                    break;
            }

            this.EditCtrl.Dispatcher.BeginInvoke( new Action(delegate 
                                                    {   
                                                        this.EditCtrl.Focus(); 
                                                    }), 
                                                    DispatcherPriority.Input);

        }
        #endregion
    }
} 