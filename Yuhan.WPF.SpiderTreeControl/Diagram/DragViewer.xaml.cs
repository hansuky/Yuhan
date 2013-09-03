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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpiderTreeControl.Diagram
{
    /// <summary>
    /// Interaction logic for DragViewer.xaml
    /// </summary>
    public partial class DragViewer : UserControl
    {

        public DragViewer()
        {
            InitializeComponent();
            this.Loaded+=delegate
            {
                LoadDiagramNodes();
            };
        }

        public void LoadDiagramNodes()
        {

            DiagramNode root = new DiagramNode("Root", null, "../Images/DiagramRootNode.png", "Dummy1View","this is the root node");

            DiagramNode a = new DiagramNode("A", root, "../Images/DiagramNode.png", "Dummy1View", "this is node A");
            DiagramNode b = new DiagramNode("B", root, "../Images/DiagramNode.png", "Dummy1View", "this is node B");
            DiagramNode c = new DiagramNode("C", root, "../Images/DiagramNode.png", "Dummy1View", "this is node C");
            DiagramNode d = new DiagramNode("D", root, "../Images/DiagramNode.png", "Dummy1View", "this is node D");
            DiagramNode e = new DiagramNode("E", root, "../Images/DiagramNode.png", "Dummy1View", "this is node E");
            DiagramNode f = new DiagramNode("F", root, "../Images/DiagramNode.png", "Dummy1View", "this is node F");

            DiagramNode a1 = new DiagramNode("A.1", a, "../Images/DiagramNode.png", "Dummy1View", "this is node A.1");
            DiagramNode a2 = new DiagramNode("A.2", a, "../Images/DiagramNode.png", "Dummy1View", "this is node A.2");
            DiagramNode a3 = new DiagramNode("A.3", a, "../Images/DiagramNode.png", "Dummy1View", "this is node A.3");
            DiagramNode a4 = new DiagramNode("A.4", a, "../Images/DiagramNode.png", "Dummy1View", "this is node A.4");

            DiagramNode a1a = new DiagramNode("A.1.A", a1, "../Images/DiagramNode.png", "Dummy1View", "this is node A.1.A");
            DiagramNode a1b = new DiagramNode("A.1.B", a1, "../Images/DiagramNode.png", "Dummy1View", "this is node A.1.B");
            DiagramNode a1c = new DiagramNode("A.1.C", a1, "../Images/DiagramNode.png", "Dummy1View", "this is node A.1.C");
            DiagramNode a1d = new DiagramNode("A.1.D", a1, "../Images/DiagramNode.png", "Dummy1View", "this is node A.1.D");

            DiagramNode a1a1 = new DiagramNode("A.1.A.1", a1a, "../Images/DiagramNode.png", "Dummy1View", "this is node A.1.A.1");

            DiagramNode b1 = new DiagramNode("B.1", b, "../Images/DiagramNode.png", "Dummy1View", "this is node B.1");
            DiagramNode b2 = new DiagramNode("B.2", b, "../Images/DiagramNode.png", "Dummy1View", "this is node B.2");
            DiagramNode b3 = new DiagramNode("B.3", b, "../Images/DiagramNode.png", "Dummy1View", "this is node B.3");
            DiagramNode b4 = new DiagramNode("B.4", b, "../Images/DiagramNode.png", "Dummy1View", "this is node B.4");

            DiagramNode b1a = new DiagramNode("B.1.A", b1, "../Images/DiagramNode.png", "Dummy1View", "this is node B.1.A");
            DiagramNode b1b = new DiagramNode("B.1.B", b1, "../Images/DiagramNode.png", "Dummy1View", "this is node B.1.B");


            DiagramNode c1 = new DiagramNode("C.1", c, "../Images/DiagramNode.png", "Dummy1View", "this is node C.1");
            DiagramNode c2 = new DiagramNode("C.2", c, "../Images/DiagramNode.png", "Dummy1View", "this is node C.2");
            DiagramNode c3 = new DiagramNode("C.3", c, "../Images/DiagramNode.png", "Dummy1View", "this is node C.3");
            DiagramNode c4 = new DiagramNode("C.4", c, "../Images/DiagramNode.png", "Dummy1View", "this is node C.4");

            DiagramNode d1 = new DiagramNode("D.1", d, "../Images/DiagramNode.png", "Dummy1View", "this is node D.1");
            DiagramNode d2 = new DiagramNode("D.2", d, "../Images/DiagramNode.png", "Dummy1View", "this is node D.2");
            DiagramNode d3 = new DiagramNode("D.3", d, "../Images/DiagramNode.png", "Dummy1View", "this is node D.3");
            DiagramNode d4 = new DiagramNode("D.4", d, "../Images/DiagramNode.png", "Dummy1View", "this is node D.4");

            DiagramNode e1 = new DiagramNode("E.1", e, "../Images/DiagramNode.png", "Dummy1View", "this is node E.1");
            DiagramNode e2 = new DiagramNode("E.2", e, "../Images/DiagramNode.png", "Dummy1View", "this is node E.2");
            DiagramNode e3 = new DiagramNode("E.3", e, "../Images/DiagramNode.png", "Dummy1View", "this is node E.3");
            DiagramNode e4 = new DiagramNode("E.4", e, "../Images/DiagramNode.png", "Dummy1View", "this is node E.4");

            DiagramNode f1 = new DiagramNode("F.1", f, "../Images/DiagramNode.png", "Dummy1View", "this is node F.1");
            DiagramNode f2 = new DiagramNode("F.2", f, "../Images/DiagramNode.png", "Dummy1View", "this is node F.2");
            DiagramNode f3 = new DiagramNode("F.3", f, "../Images/DiagramNode.png", "Dummy1View", "this is node F.3");
            DiagramNode f4 = new DiagramNode("F.4", f, "../Images/DiagramNode.png", "Dummy1View", "this is node F.4");
            DiagramNode f5 = new DiagramNode("F.5", f, "../Images/DiagramNode.png", "Dummy1View", "this is node F.5");
            DiagramNode f6 = new DiagramNode("F.6", f, "../Images/DiagramNode.png", "Dummy1View", "this is node F.6");


            diagramViewer.RootNode = root;
            diagramViewer.FrictionScrollViewer = this.sv;
        }



  



 
    }
}
