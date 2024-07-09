using LCF_WPF.Models;
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

namespace LCF_WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Défile automatiquement vers le bas lorsque l'utilisateur défile vers le bas.
            if (e.ExtentHeightChange != 0)
            {
                RTBConsole.ScrollToEnd();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // Défile automatiquement vers le bas lors du défilement avec la molette de la souris.
            if (e.Delta > 0)
            {
                RTBConsole.ScrollToVerticalOffset(RTBConsole.VerticalOffset - 1);
            }
            else if (e.Delta < 0)
            {
                RTBConsole.ScrollToVerticalOffset(RTBConsole.VerticalOffset + 1);
            }

            e.Handled = true;
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box)
            {
                if (string.IsNullOrEmpty(box.Text))
                    PlaceholderText.Visibility = Visibility.Visible;
                    //box.Background = (ImageBrush)box.FindResource("messageWatermark");
                else
                    PlaceholderText.Visibility = Visibility.Hidden;
                //box.Background = null;
            }
        }
        /*
private void Button_Click(object sender, RoutedEventArgs e)
{
}
*/
    }
}