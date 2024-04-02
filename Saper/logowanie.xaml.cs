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
        public string n;
        public CustomMessageBox CustomMessageBox = new CustomMessageBox();

        public logowanie()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();

            CustomMessageBox.MessageBoxYesNo("Użytkownik o podanym nicku już istnieje. Chcesz kontynuować jego grę?", (result) =>
            {
                if (result)
                {
                    this.Close();
                }
                else
                {
                    //jesli nie
                }
            });
        }

        public void Zaloguj(object sender, RoutedEventArgs e)
        {
            string connectionString = "server=localhost;user id=root;password=;database=saper";
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                n = nick.Text;
                string query = "SELECT COUNT(*) FROM rekordy WHERE Nick = @n";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@n", n);

                int rowCount = Convert.ToInt32(cmd.ExecuteScalar());

                if (rowCount > 0)
                {
                    CustomMessageBox.MessageBoxYesNo("Użytkownik o podanym nicku już istnieje. Chcesz kontynuować jego grę?", (result) =>
                    {
                        if (result)
                        {
                            this.Close();
                        }
                        else
                        {
                            //jesli nie
                        }
                    });
                }
                else
                {
                    string dodajQuery = "INSERT INTO rekordy (Nick) VALUES (@n)";
                    MySqlCommand dodajCmd = new MySqlCommand(dodajQuery, conn);
                    dodajCmd.Parameters.AddWithValue("@n", n);

                    dodajCmd.ExecuteNonQuery();

                    CustomMessageBox.MessageBoxOk("Dodano: " + n);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.MessageBoxOk("Błąd logowania: " + ex.Message);
                
            }
            finally
            {
                conn.Close();
            }
        }



    }

}
