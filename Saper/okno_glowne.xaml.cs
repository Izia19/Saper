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
        public string poziom;
        public Window oknoGlowne;
        private ToggleButton ostatnioKliknietyPrzycisk = null;
        public bool czyBezpiecznaStrefa = true;

        public okno_glowne()
        {
            OtworzOknaAsync();
            InitializeComponent();
            oknoGlowne = this;
            nick_u.Content = userNick;
            Start.IsEnabled = false;
        }
        private async void OtworzOknaAsync()
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
                string maxScore = "SELECT max(Wynik) FROM rekordy WHERE Poziom = @level AND Nick = @userNick";
                MySqlCommand cmd = new MySqlCommand(maxScore, conn);
                cmd.Parameters.AddWithValue("@level", poziom);
                cmd.Parameters.AddWithValue("@userNick", userNick);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    string wynik = result.ToString();
                    wyniki_uzytkownika.Content = "Rekord: " + wynik;
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
        private void Poziomy(object sender, RoutedEventArgs e)
        {
            ToggleButton kliknietyPrzycisk = sender as ToggleButton;

            if (kliknietyPrzycisk != null)
            {
                if (kliknietyPrzycisk == ostatnioKliknietyPrzycisk)
                {
                    kliknietyPrzycisk.Background = (Brush)new BrushConverter().ConvertFrom("#FFBEBC");
                }
                else
                {
                    if (ostatnioKliknietyPrzycisk != null)
                    {
                        ostatnioKliknietyPrzycisk.IsChecked = false;
                        ostatnioKliknietyPrzycisk.Background = (Brush)new BrushConverter().ConvertFrom("#FFBEBC");
                    }
                    ostatnioKliknietyPrzycisk = kliknietyPrzycisk;
                    poziom = ostatnioKliknietyPrzycisk.Name;
                    SprawdzWynikiUzytkownika();
                    kliknietyPrzycisk.Background = (Brush)new BrushConverter().ConvertFrom("#FFABAB");
                    TabelaWynikow();

                    Start.IsEnabled = true;
                }
            }  
        }
        public void TabelaWynikow()
        {
            List<User> lista = new List<User>();
            try
            {
                string connectionString = "server=localhost;user id=root;password=;database=saper";
                MySqlConnection conn = new MySqlConnection(connectionString);
                conn.Open();
                string bestScore = "SELECT Nick, Wynik FROM rekordy WHERE Poziom = @level ORDER BY Wynik LIMIT 10";
                MySqlCommand cmd = new MySqlCommand(bestScore, conn);
                cmd.Parameters.AddWithValue("@level", poziom);

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
            int iloscBomb = 0;
            int iloscPrzyciskow = 0;
            if (this.poziom == "latwy")
            {
                iloscPrzyciskow = 10;
                iloscBomb = 15;
            }
            else if (this.poziom == "sredni")
            {
                iloscPrzyciskow = 15;
                iloscBomb = 40;
            }
            else if (this.poziom == "trudny")
            {
                iloscPrzyciskow = 20;
                iloscBomb = 100;
            }
   
            poziomy poziomy = new poziomy(iloscPrzyciskow, iloscBomb, poziom, userNick, oknoGlowne, czyBezpiecznaStrefa);
            this.Hide();
            poziomy.ShowDialog();
        }
        private void Ustawienia(object sender, RoutedEventArgs e)
        {
            userNick = CustomMessageBox.MessageBoxUstawienia(userNick, czyBezpiecznaStrefa);
            czyBezpiecznaStrefa = CustomMessageBox.czyBezpiecznaStrefa;
            nick_u.Content = userNick;
        }
    }
}