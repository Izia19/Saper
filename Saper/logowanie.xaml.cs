using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        public string userNick;
        public CustomMessageBox CustomMessageBox = new CustomMessageBox();
        public Random random = new Random();

        public logowanie()
        {
            InitializeComponent();
        }

        public void Zaloguj(object sender, RoutedEventArgs e)
        {
            string connectionString = "server=localhost;user id=root;password=;database=saper";
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                userNick = nick.Text;
                string countNick = "SELECT COUNT(*) FROM rekordy WHERE Nick = @userNick";
                MySqlCommand cmd = new MySqlCommand(countNick, conn);
                cmd.Parameters.AddWithValue("@userNick", userNick);

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
                            bool jestNick = true;
                            while (jestNick)
                            {
                                string countNick2 = "SELECT COUNT(*) FROM rekordy WHERE Nick = @userNick";
                                MySqlCommand cmd = new MySqlCommand(countNick2, conn);
                                cmd.Parameters.AddWithValue("@userNick", userNick);

                                int rowCount = Convert.ToInt32(cmd.ExecuteScalar());

                                if(rowCount > 0)
                                {
                                    string dodajDoNicku = random.Next(0, 10).ToString();
                                    userNick += dodajDoNicku;
                                }
                                else
                                {
                                    jestNick = false;
                                }
                            }
                            nick.Text = userNick;
                        }
                    });
                }
                else
                {
                    string insertNick = "INSERT INTO rekordy (Nick) VALUES (@userNick)";
                    MySqlCommand insertCmd = new MySqlCommand(insertNick, conn);
                    insertCmd.Parameters.AddWithValue("@userNick", userNick);

                    insertCmd.ExecuteNonQuery();

                    CustomMessageBox.MessageBoxOk("Dodano: " + userNick);
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
