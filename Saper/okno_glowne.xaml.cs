using MySql.Data.MySqlClient;
using System;
using System.Collections.ObjectModel;
using System.Reflection.PortableExecutable;
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
    public partial class okno_glowne : Window
    {
        public CustomMessageBox CustomMessageBox = new CustomMessageBox();

        public string userNick;
        public string level;
        public Window window_okno_glowne;
        private ToggleButton lastClickedButton = null;
        public bool czyBezpiecznaStrefa = true;

        public okno_glowne()
        {
            OpenWindowsAsync();
            InitializeComponent();
            window_okno_glowne = this;
            nick_u.Content = userNick;
            Start.IsEnabled = false;
        }
        private async void OpenWindowsAsync()
        {
            ladowanie ladowanie = new ladowanie();
            ladowanie.ShowDialog();
            this.userNick = ladowanie.userNick;
        }
        private void SprawdzWynikiUzytkownika()
        {
            try
            {
                string connectionString = "server=localhost;user id=root;password=;database=saper";
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                string query = "SELECT max(Wynik) FROM rekordy WHERE Poziom = @level AND Nick = @userNick";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@level", level);
                cmd.Parameters.AddWithValue("@userNick", userNick);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    string wynik = result.ToString();
                    wyniki_uzytkownika.Content = wynik;
                }
                else
                {
                    wyniki_uzytkownika.Content = " ";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd " + e);
            }
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
                    level = lastClickedButton.Name;
                    SprawdzWynikiUzytkownika();
                    clickedButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFABAB");
                    Tabela_wynikow();

                    Start.IsEnabled = true;
                }
            }  
        }

        public void Tabela_wynikow()
        {
            List<User> lista = new List<User>();
            try
            {
                string connectionString = "server=localhost;user id=root;password=;database=saper";
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                string query = "SELECT Nick, Wynik FROM rekordy WHERE Poziom = @level ORDER BY Wynik LIMIT 10";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@level", level);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    int i = 1;
                    while (reader.Read())
                    {
                        string nick = reader.GetString(0);
                        string wynik = reader.GetString(1);
                        User user = new User(i.ToString(), nick, wynik);
                        lista.Add(user);
                        i++;
                    }
                }
                user_wyniki.ItemsSource = lista;
            }
            catch (Exception e)
            {
                MessageBox.Show("Błąd " + e);
            }
        }

        public void Graj(object sender, RoutedEventArgs e)
        {
            int bombCount = 0;
            int numberOfButton = 0;
            if (level == "latwy")
            {
                numberOfButton = 10;
                bombCount = 15;
            }
            else if (level == "sredni")
            {
                numberOfButton = 15;
                bombCount = 40;
            }
            else if (level == "trudny")
            {
                numberOfButton = 20;
                bombCount = 100;
            }
   
            poziomy poziom = new poziomy(numberOfButton, bombCount, level, userNick, window_okno_glowne, false);
            this.Hide();
            poziom.ShowDialog();
        }

        private void Ustawienia(object sender, RoutedEventArgs e)
        {
            userNick = CustomMessageBox.MessageBoxUstawienia(userNick, czyBezpiecznaStrefa);
            czyBezpiecznaStrefa = CustomMessageBox.czyBezpiecznaStrefa;
            nick_u.Content = userNick;
        }
    }
}