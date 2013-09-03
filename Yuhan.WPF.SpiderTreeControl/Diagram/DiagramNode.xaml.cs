using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpiderTreeControl.Diagram
{
    public delegate void NodeStateChangedHandler(DiagramNode sender, RoutedEventArgs eventArguments);

    public partial class DiagramNode : UserControl
    {
        public event NodeStateChangedHandler Expanded;
        public event NodeStateChangedHandler Collapsed;
        public event NodeStateChangedHandler Selected;



        private List<DiagramNode> diagramChildren = new List<DiagramNode>();
        private DiagramNode diagramParent;
        private string navigateTo;
        private bool isExpanded = false;


        public static readonly RoutedUICommand expandCommand =
            new RoutedUICommand("expandCommand", "expandCommand", typeof(DiagramNode));

        public static readonly RoutedUICommand collapseCommand =
            new RoutedUICommand("collapseCommand", "collapseCommand", typeof(DiagramNode));


        public DiagramNode(String nodeName, DiagramNode treeParent,
            string imageUrl, string navigateTo, string itemDescription)
        {
            InitializeComponent();

            this.navigateTo = navigateTo;
            this.ToolTip = itemDescription;
            this.diagramParent = treeParent;
            this.NodeName.Content = nodeName;
            if (treeParent != null)
            {
                this.diagramParent.diagramChildren.Add(this);
            }
            this.btnNavigate.Tag = imageUrl;
        }

        protected void OnSelected(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            if (Selected != null)
                Selected(sender, eventArguments);
        }

        protected void OnExpanded(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            OnSelected(sender, eventArguments);
            if (Expanded != null)
                Expanded(this, eventArguments);
        }

        protected void OnCollapsed(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            OnSelected(sender, eventArguments);
            if (Collapsed != null)
                Collapsed(this, eventArguments);
        }



        public List<DiagramNode> DiagramChildren
        {
            get { return diagramChildren; }
            set { diagramChildren = value; }
        }

        private string NavigateTo
        {
            get { return navigateTo; }
            set { navigateTo = value; }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; }
        }


        public Point Location
        {
            get
            {
                return new Point(
                    (double)GetValue(Canvas.LeftProperty) + (double)GetValue(Canvas.ActualWidthProperty) / 2.0,
                    (double)GetValue(Canvas.TopProperty) + (double)GetValue(Canvas.ActualHeightProperty) / 2.0);
            }

            set
            {
                SetValue(Canvas.LeftProperty, value.X - (double)GetValue(Canvas.ActualWidthProperty) / 2.0);
                SetValue(Canvas.TopProperty, value.Y - (double)GetValue(Canvas.ActualHeightProperty) / 2.0);
            }
        }

        public DiagramNode DiagramParent
        {
            get { return diagramParent; }
        }

        public double BoundingCircle
        {
            get
            {
                double size = Math.Max((double)GetValue(Canvas.ActualWidthProperty), (double)GetValue(Canvas.ActualHeightProperty)) / 2;

                return Math.Sqrt(size * size * 2);
            }
        }

        private void btnNavigate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(
                "This could navigate somewhere\r\nsuch as the location pointed to by the navigateTo parameter, which is currently {0}"
                , NavigateTo));
        }

        private void ExpandCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.diagramChildren.Count() > 0;
        }

        private void ExpandCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IsExpanded = true;
            OnExpanded(this, new RoutedEventArgs(e.RoutedEvent));
        }

        private void CollapseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = IsExpanded;
        }

        private void CollapseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IsExpanded = false;
            OnCollapsed(this, new RoutedEventArgs(e.RoutedEvent));
        }

    }
}
