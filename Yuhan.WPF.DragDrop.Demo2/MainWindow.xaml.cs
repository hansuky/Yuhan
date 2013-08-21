using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Yuhan.WPF.DragDrop.DragDropFramework;
using Yuhan.WPF.DragDrop.DragDropFrameworkData;

namespace Yuhan.WPF.DragDrop.Demo2
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : DragDropWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            // Used by TabControl, TreeView and ListBox.
            // This data consumer allows items to be created
            // from a file or files dragged from Windows Explorer.
            FileDropConsumer fileDropDataConsumer =
                new FileDropConsumer(new string[] {
                    "FileDrop",
                    "FileNameW",
                });

            #region T A B   C O N T R O L
            // Data Provider
            TabControlDataProvider<TabControl, TabItem> tabControlDataProvider =
                new TabControlDataProvider<TabControl, TabItem>("TabItemObject");

            // Data Consumer
            TabControlDataConsumer<TabControl, TabItem> tabControlDataConsumer =
                new TabControlDataConsumer<TabControl, TabItem>(new string[] { "TabItemObject" });

            // Drag Managers
            DragManager dragHelperTabControl0 = new DragManager(this.docTabControl0, tabControlDataProvider);
            DragManager dragHelperTabControl1 = new DragManager(this.docTabControl1, tabControlDataProvider);

            // Drop Managers
            DropManager dropHelperTabControl0 = new DropManager(this.docTabControl0,
                new IDataConsumer[] {
                    tabControlDataConsumer,
                    fileDropDataConsumer
                });
            DropManager dropHelperTabControl1 = new DropManager(this.docTabControl1,
                new IDataConsumer[] {
                    tabControlDataConsumer,
                    fileDropDataConsumer
                });
            #endregion

            #region T R E E   V I E W
            // Data Provider
            TreeViewDataProvider<ItemsControl, TreeViewItem> treeViewDataProvider =
                new TreeViewDataProvider<ItemsControl, TreeViewItem>("TreeViewItemObject");

            // Data Consumer
            TreeViewDataConsumer<ItemsControl, TreeViewItem> treeViewDataConsumer =
                new TreeViewDataConsumer<ItemsControl, TreeViewItem>(new string[] { "TreeViewItemObject" });

            // Data Consumer of ListBoxItems
            ListBoxItemToTreeViewItem<ListBox, ListBoxItem> listBoxItemToTreeViewItem =
                new ListBoxItemToTreeViewItem<ListBox, ListBoxItem>(new string[] { "ListBoxItemObject" });

            // Drag Managers
            DragManager dragHelperTreeView0 = new DragManager(this.treeView0, treeViewDataProvider);
            DragManager dragHelperTreeView1 = new DragManager(this.treeView1, treeViewDataProvider);

            // Drop Managers
            DropManager dropHelperTreeView0 = new DropManager(this.treeView0,
                new IDataConsumer[] {
                    treeViewDataConsumer,
                    listBoxItemToTreeViewItem,
                    fileDropDataConsumer,
                });
            DropManager dropHelperTreeView1 = new DropManager(this.treeView1,
                new IDataConsumer[] {
                    treeViewDataConsumer,
                    listBoxItemToTreeViewItem,
                    fileDropDataConsumer,
                });
            #endregion

            #region L I S T   B O X
            // Data Provider
            ListBoxDataProvider<ListBox, ListBoxItem> listBoxDataProvider =
                new ListBoxDataProvider<ListBox, ListBoxItem>("ListBoxItemObject");

            // Data Consumer
            ListBoxDataConsumer<ListBox, ListBoxItem> listBoxDataConsumer =
                new ListBoxDataConsumer<ListBox, ListBoxItem>(new string[] { "ListBoxItemObject" });

            // Data Consumer of TreeViewItems
            TreeViewItemToListBoxItem<ItemsControl, TreeViewItem> treeViewItemToListBoxItem =
                new TreeViewItemToListBoxItem<ItemsControl, TreeViewItem>(new string[] { "TreeViewItemObject" });

            // Drag Managers
            DragManager dragHelperListBox0 = new DragManager(this.listBox0, listBoxDataProvider);
            DragManager dragHelperListBox1 = new DragManager(this.listBox1, listBoxDataProvider);

            // Drop Managers
            DropManager dropHelperListBox0 = new DropManager(this.listBox0,
                new IDataConsumer[] {
                    listBoxDataConsumer,
                    treeViewItemToListBoxItem,
                    fileDropDataConsumer,
                });
            DropManager dropHelperListBox1 = new DropManager(this.listBox1,
                new IDataConsumer[] {
                    listBoxDataConsumer,
                    treeViewItemToListBoxItem,
                    fileDropDataConsumer,
                });
            #endregion

            #region T R A S H
            // Data Consumer
            TrashConsumer trashConsumer = new TrashConsumer(new string[] {
                "TabItemObject",
                "TreeViewItemObject",
                "ListBoxItemObject",
                "CanvasTextBlockObject",
                "CanvasRectangleObject",
                "CanvasButtonObject",
                "ToolbarButtonObject",
            });

            // Drop Manager
            DropManager dropHelperListBoxItemTrash = new DropManager(this.trash, trashConsumer);
            #endregion

            #region C A N V A S
            // Data Providers/Consumers
            CanvasDataProvider<Canvas, TextBlock> canvasTextBlockDataProvider =
                new CanvasDataProvider<Canvas, TextBlock>("CanvasTextBlockObject");

            CanvasDataConsumer<Canvas, TextBlock> canvasTextBlockDataConsumer =
                new CanvasDataConsumer<Canvas, TextBlock>(new string[] { "CanvasTextBlockObject" });

            CanvasDataProvider<Canvas, Rectangle> canvasRectangleDataProvider =
                new CanvasDataProvider<Canvas, Rectangle>("CanvasRectangleObject");

            CanvasDataConsumer<Canvas, Rectangle> canvasRectangleDataConsumer =
                new CanvasDataConsumer<Canvas, Rectangle>(new string[] { "CanvasRectangleObject" });

            CanvasDataProvider<Canvas, Button> canvasButtonDataProvider =
                new CanvasDataProvider<Canvas, Button>("CanvasButtonObject");

            CanvasDataConsumer<Canvas, Button> canvasButtonDataConsumer =
                new CanvasDataConsumer<Canvas, Button>(new string[] { "CanvasButtonObject" });

            // Data Consumer of Toolbar Buttons
            ToolbarButtonToCanvasButton<ToolBar, Button> toolbarButtonToCanvasButton =
                new ToolbarButtonToCanvasButton<ToolBar, Button>(new string[] { "ToolbarButtonObject" });

            // Data consumer of System.Strings (creates a TextBlock)
            StringToCanvasTextBlock systemStringToCanvasTextBlock =
                new StringToCanvasTextBlock(new string[] { "System.String" });

            // Drag Managers
            DragManager dragHelperCanvas0 = new DragManager(this.canvas0,
                new IDataProvider[] {
                    canvasTextBlockDataProvider,
                    canvasRectangleDataProvider,
                    canvasButtonDataProvider,
                });
            DragManager dragHelperCanvas1 = new DragManager(this.canvas1,
                new IDataProvider[] {
                    canvasTextBlockDataProvider,
                    canvasRectangleDataProvider,
                    canvasButtonDataProvider,
                });

            // Drop Managers
            DropManager dropHelperCanvas0 = new DropManager(this.canvas0,
                new IDataConsumer[] {
                    canvasTextBlockDataConsumer,
                    canvasRectangleDataConsumer,
                    canvasButtonDataConsumer,
                    toolbarButtonToCanvasButton,
                    systemStringToCanvasTextBlock,
                });
            DropManager dropHelperCanvas1 = new DropManager(this.canvas1,
                new IDataConsumer[] {
                    canvasTextBlockDataConsumer,
                    canvasRectangleDataConsumer,
                    canvasButtonDataConsumer,
                    toolbarButtonToCanvasButton,
                    systemStringToCanvasTextBlock,
                });
            #endregion

            #region T O O L B A R
            // Data Provider
            ToolBarDataProvider<ToolBar, Button> toolBarButtonDataProvider =
                new ToolBarDataProvider<ToolBar, Button>("ToolbarButtonObject");

            // Data Consumer
            ToolBarDataConsumer<ToolBar, Button> toolBarButtonDataConsumer =
                new ToolBarDataConsumer<ToolBar, Button>(new string[] { "ToolbarButtonObject" });

            // Data Consumer of Canvas Buttons
            CanvasButtonToToolbarButton<Canvas, Button> canvasButtonToToolbarButton =
                new CanvasButtonToToolbarButton<Canvas, Button>(new string[] { "CanvasButtonObject" });

            // Drag Managers
            DragManager dragHelperToolBar0 = new DragManager(this.toolBar0,
                new IDataProvider[] {
                    toolBarButtonDataProvider,
                });
            DragManager dragHelperToolBar1 = new DragManager(this.toolBar1,
                new IDataProvider[] {
                    toolBarButtonDataProvider,
                });

            // Drop Managers
            DropManager dropHelperToolBar0 = new DropManager(this.toolBar0,
                new IDataConsumer[] {
                    toolBarButtonDataConsumer,
                    canvasButtonToToolbarButton,
                });
            DropManager dropHelperToolBar1 = new DropManager(this.toolBar1,
                new IDataConsumer[] {
                    toolBarButtonDataConsumer,
                    canvasButtonToToolbarButton,
                });
            #endregion
        }
    }
}
