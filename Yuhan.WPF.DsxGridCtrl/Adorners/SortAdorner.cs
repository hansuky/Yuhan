using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.ComponentModel;

namespace Yuhan.WPF.DsxGridCtrl
{
    public class SortAdorner : Adorner
    {
        #region members/properties

        private readonly static Geometry m_AscGeometry  = Geometry.Parse("M 0,4 L 8,4 L 4,0 Z");
        private readonly static Geometry m_DescGeometry = Geometry.Parse("M 0,0 L 8,0 L 4,4 Z");

        public                  ListSortDirection   SortDirection   { get; private set; }
        private                 Brush               PaintBrush      { get; set; }

        #endregion

        #region ctors

        public SortAdorner(UIElement element, ListSortDirection sortDirection) : base(element)
        { 
            this.SortDirection  = sortDirection; 

            this.PaintBrush     = ((element as GridViewColumnHeader).Column as DsxColumn).DataGrid.SortAdornerIndicatorBrush;
        }
        #endregion

        #region Override - OnRender

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 14)
            {
                return;
            }

            drawingContext.PushTransform( new TranslateTransform( AdornedElement.RenderSize.Width - 12, (AdornedElement.RenderSize.Height - 4) / 2));
            drawingContext.DrawGeometry (this.PaintBrush, null, SortDirection == ListSortDirection.Ascending ? m_AscGeometry : m_DescGeometry);

            drawingContext.Pop();
        }
        #endregion
    }
} 