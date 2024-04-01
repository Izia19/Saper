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

namespace Saper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string n;
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
    }
}