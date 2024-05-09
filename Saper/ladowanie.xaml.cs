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
        public string userNick;
        public ladowanie()
        {
            InitializeComponent();
            OtworzOknaAsync();
        }

        private async void OtworzOknaAsync()
        {
            await Task.Delay(5000);
            this.Close();
            logowanie logowanie = new logowanie();        
            logowanie.ShowDialog();
            this.userNick = logowanie.userNick;
        }
    }
}
