using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Saper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string n;
        public string p;
        public Window thisWindow;
        private ToggleButton lastClickedButton = null;
        public MainWindow()
        {

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            OpenWindowsAsync();

            InitializeComponent();
            nick_u.Content += " " + n;
        }

        private async void OpenWindowsAsync()
        {
            ladowanie ladowanie = new ladowanie();
            ladowanie.ShowDialog();
            n = ladowanie.n;
        }

        private void Poziomy_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton clickedButton = sender as ToggleButton;

            if (clickedButton != null)
            {
                if (clickedButton == lastClickedButton)
                {
                    clickedButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFBEBC");
                }
                else
                {
                    if (lastClickedButton != null)
                    {
                        lastClickedButton.IsChecked = false;
                        lastClickedButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFBEBC");
                    }
                    lastClickedButton = clickedButton;
                    p = lastClickedButton.Name;
                    clickedButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFABAB"); ;
                }
            }
        }

        public void Graj(object sender, RoutedEventArgs e)
        {
            if (p == "latwy")
            {
                Poziom_latwy poziom = new Poziom_latwy();
                thisWindow = this;
                this.Hide();
                poziom.p = p;
                poziom.n = n;
                poziom.window = thisWindow;
                poziom.ShowDialog();
            }
        }

    }
}