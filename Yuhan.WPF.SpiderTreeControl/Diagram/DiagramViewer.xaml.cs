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
    public partial class DiagramViewer : UserControl
    {
        private DiagramNode rootNode;
        private FrictionScrollViewer frictionScrollViewer = null;

        public DiagramViewer()
        {
            InitializeComponent();
        }

        public DiagramNode RootNode
        {
            set 
            { 
                rootNode = value;
                BaseCanvas.Children.Add(rootNode);

                rootNode.Location = new Point(
                    (double)GetValue(Canvas.ActualWidthProperty) / 2.0,
                    (double)GetValue(Canvas.ActualHeightProperty) / 2.0);



                rootNode.Selected += new NodeStateChangedHandler(NodeSelected);
                rootNode.Expanded += new NodeStateChangedHandler(NodeExpanded);
                rootNode.Collapsed += new NodeStateChangedHandler(NodeCollapsed);
            }
        }




        private double CalculateStartAngle(DiagramNode node)
        {
            if (node == rootNode)
            {
                return 0.0;
            }
            else
            {
                Vector parentToNode = node.Location - node.DiagramParent.Location;
                parentToNode.Normalize();
                Vector leftToRight = new Vector(1.0, 0.0);
                double angle = Vector.AngleBetween(parentToNode, leftToRight);
                if (angle < 0)
                    angle = 360 - angle;

                if (node.DiagramChildren.Count > 1)
                {
                    if (node.Location.Y < node.DiagramParent.Location.Y)
                    {
                        if (node.Location.X > node.DiagramParent.Location.X)
                            angle -= 180;
                        else
                            angle -= 270;
                    }
                    else
                    {
                        angle -= 90;
                    }
                }
                
                return (Math.PI * angle / 180.0);
            }
        }



        private void NodeSelected(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            if (frictionScrollViewer != null)
                frictionScrollViewer.AutoScrollTarget = sender.Location;
            //throw new NotImplementedException();
        }



        private void NodeExpanded(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            rootNode.Location = new Point(
                (double)GetValue(Canvas.ActualWidthProperty) / 2.0,
                (double)GetValue(Canvas.ActualHeightProperty) / 2.0);


            MakeChildrenVisible(sender);

            if (sender.DiagramParent != null)
            {
                sender.DiagramParent.Visibility = Visibility.Visible;
                foreach (DiagramNode sibling in sender.DiagramParent.DiagramChildren)
                {
                    if (sibling != sender)
                        sibling.Visibility = Visibility.Collapsed;
                }
                if (sender.DiagramParent.DiagramParent != null)
                    sender.DiagramParent.DiagramParent.Visibility = Visibility.Collapsed;
            }

            if (sender.DiagramChildren.Count > 0)
            {
                double startAngle = CalculateStartAngle(sender);
                double angleBetweenChildren = (sender == rootNode ? Math.PI * 2.0 : Math.PI) / ((double)sender.DiagramChildren.Count - 0);

                double legDistance = CalculateLegDistance(sender, angleBetweenChildren);

                for (int i = 0; i < sender.DiagramChildren.Count; ++i)
                {
                    DiagramNode child = sender.DiagramChildren[i];
                    child.Selected += new NodeStateChangedHandler(NodeSelected);
                    child.Expanded += new NodeStateChangedHandler(NodeExpanded);
                    child.Collapsed += new NodeStateChangedHandler(NodeCollapsed);

                    Point parentLocation = sender.Location;

                    child.Location = new Point(
                        parentLocation.X + Math.Cos(startAngle + angleBetweenChildren * (double)i) * legDistance,
                        parentLocation.Y + Math.Sin(startAngle + angleBetweenChildren * (double)i) * legDistance);

                    foreach (DiagramNode childsChild in child.DiagramChildren)
                    {
                        childsChild.Visibility = Visibility.Collapsed;
                    }
                }
            }

            BaseCanvas.InvalidateArrange();
            BaseCanvas.UpdateLayout();
            BaseCanvas.InvalidateVisual();
        }

        private static double CalculateLegDistance(DiagramNode sender, double angleBetweenChildren)
        {
            double legDistance = 1.0;
            double childToChildMinDistance = 1.0;
            foreach (DiagramNode child in sender.DiagramChildren)
            {
                legDistance = Math.Max(legDistance, sender.BoundingCircle + child.BoundingCircle);
                foreach (DiagramNode otherChild in sender.DiagramChildren)
                {
                    if (otherChild != child)
                    {
                        childToChildMinDistance = Math.Max(childToChildMinDistance, child.BoundingCircle + otherChild.BoundingCircle);
                    }
                }
            }

            legDistance = Math.Max(
                legDistance,
                (childToChildMinDistance / 2.0) / Math.Sin(angleBetweenChildren / 2.0));
            return legDistance;
        }

        private void MakeChildrenVisible(DiagramNode sender)
        {
            foreach (DiagramNode child in sender.DiagramChildren)
            {
                if (!BaseCanvas.Children.Contains(child))
                    BaseCanvas.Children.Add(child);
                else
                    child.Visibility = Visibility.Visible;
            }

            BaseCanvas.InvalidateArrange();
            BaseCanvas.UpdateLayout();
            BaseCanvas.InvalidateVisual();
        }

        private void NodeCollapsed(DiagramNode sender, RoutedEventArgs eventArguments)
        {
            foreach (DiagramNode child in sender.DiagramChildren)
            {
                child.Visibility = Visibility.Collapsed;
                foreach (DiagramNode grandChildren in child.DiagramChildren)
                {
                    grandChildren.Visibility = Visibility.Collapsed;
                }
            }
            BaseCanvas.InvalidateArrange();
            BaseCanvas.UpdateLayout();
            BaseCanvas.InvalidateVisual();
        }

        public FrictionScrollViewer FrictionScrollViewer
        {
            set { 
                frictionScrollViewer = value;
                if (frictionScrollViewer != null)
                    frictionScrollViewer.ScrollToCenterTarget(rootNode.Location);
            }
        }
   }
}
