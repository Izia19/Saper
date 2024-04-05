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
using System.Windows.Shapes;

namespace Saper
{
    /// <summary>
    /// Logika interakcji dla klasy ladowanie.xaml
    /// </summary>
    public partial class ladowanie : Window
    {
        public ladowanie()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            OpenWindowsAsync();
        }

        private async void OpenWindowsAsync()
        {
            await Task.Delay(5000);
            this.Hide();
            logowanie logowanie = new logowanie(this);
            logowanie.ShowDialog();
        }
    }
}
