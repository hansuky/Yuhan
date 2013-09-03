using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MenuKillerApp
{
    /// <summary>
    /// Interaction logic for Window3.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MKPopup.CustomPopupPlacementCallback =
                new CustomPopupPlacementCallback(MKPopupPlacement);
        }
        private void AlwaysCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ApplicationCommandsClose(object sender, ExecutedRoutedEventArgs e)
        {
            BigTextBox.Text += MKRoot.HoverToolTip + "\n";

            // Close();
        }

        private CustomPopupPlacement[] MKPopupPlacement(Size a, Size b, Point c)
        {
            //
            // MKRoot.AlignReferencePoint.

            // TODO: The MenuKiller's align reference report must report when it changes, so we can reposition the
            // Popup... However, it'd be nice to know whether that is possible at all.
            // ISSUE: The popup kills performance, moving it makes matters A LOT worse
            CustomPopupPlacement pl = new CustomPopupPlacement(new Point(0, 0), PopupPrimaryAxis.None);
            CustomPopupPlacement[] arr = new CustomPopupPlacement[1];
            arr[0] = pl;
            return arr;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // MKPopup.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            MKPopup.AllowsTransparency = true;
            MKPopup.StaysOpen = false;
            MKPopup.Width = 700;
            MKPopup.Height= 700;
            MKPopup.IsOpen = true;
            
        }
    }
}
