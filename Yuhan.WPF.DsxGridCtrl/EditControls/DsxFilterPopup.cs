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

namespace Yuhan.WPF.DsxGridCtrl
{
    #region TemplateParts

    [TemplatePart(Name = cPART_Popup, Type = typeof(Popup))]
    #endregion

    public class DsxFilterPopup : ToggleButton
    {
        #region Consts

        internal const string   cPART_Popup      = "PART_Popup";
        #endregion

        #region ctors

        static DsxFilterPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DsxFilterPopup), new FrameworkPropertyMetadata((typeof(DsxFilterPopup))));
        }

        public DsxFilterPopup(DsxColumn column)
        {
            this.Column     = column;
            this.Column.FilterTextChanged += delegate 
                                                { 
                                                    if (this.PART_Popup != null)
                                                    {
                                                        this.PART_Popup.IsOpen=false; 
                                                    }
                                                };
        }
        #endregion

        #region members / properties

        public DsxColumn    Column      { get; set; }
        public Popup        PART_Popup  { get; set; }
        #endregion

        #region Override - OnApplyTemplate

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_Popup = GetTemplateChild(cPART_Popup) as Popup;
        }
        #endregion
    }
}
