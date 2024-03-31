using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using MySql.Data.MySqlClient;


namespace Saper
{
    /// <summary>
    /// Logika interakcji dla klasy logowanie.xaml
    /// </summary>
    public partial class logowanie : Window
    {
        public string n; // Nie przypisuj wartości tutaj

        public logowanie()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        public void Zaloguj(object sender, RoutedEventArgs e)
        {
            string connectionString = "server=localhost;user id=root;password=;database=saper";
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                n = nick.Text; // Pobierz wartość nick.Text po otwarciu okna i wpisaniu tekstu przez użytkownika
                string query = "SELECT COUNT(*) FROM rekordy WHERE Nick = @n";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@n", n);

                int rowCount = Convert.ToInt32(cmd.ExecuteScalar());

                if (rowCount > 0)
                {
                    MessageBoxResult result = MessageBox.Show("Użytkownik o podanym nicku już istnieje. Chcesz kontynuować jego gry?", "", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        this.Close();
                    }
                    else
                    {
                        // Można zaproponować jakiś nick ale aktualnie jestem zbyt leniwa żeby to zrobić 
                    }
                }
                else
                {
                    string dodajQuery = "INSERT INTO rekordy (Nick) VALUES (@n)";
                    MySqlCommand dodajCmd = new MySqlCommand(dodajQuery, conn);
                    dodajCmd.Parameters.AddWithValue("@n", n);

                    dodajCmd.ExecuteNonQuery();

                    MessageBox.Show("Dodano nowego użytkownika: " + n);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd logowania: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

    }

}
