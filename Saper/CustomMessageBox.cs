using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;
using System.Windows.Controls.Primitives;
using Org.BouncyCastle.Tls;
using System.Diagnostics;
using MySql.Data.MySqlClient;
using System;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Saper
{
    public class CustomMessageBox
    {
        public static System.Windows.Media.Color jasnyRoz = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFBEBC");
        public static System.Windows.Media.Color ciemnyRoz = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF9999");
        public static System.Windows.Media.Color bardzoJasnyRoz = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFECE5");

        public string userNick { get; set; }
        public bool czyBezpiecznaStrefa;
        public Random random = new Random(); 

        public void MessageBoxOk(string message)
        {
            var customMessageBox = new Window
            {
                Width = 300,
                Height = 150,
                Background = new SolidColorBrush(bardzoJasnyRoz),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.None,
            };

            var textBlock = new TextBlock
            {
                Text = message,
                Margin = new Thickness(0, 15, 0, 0),
                FontSize = 20,
                Foreground = new SolidColorBrush(ciemnyRoz),
                Style = (Style)Application.Current.FindResource("textBlock"),
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                Height = 30,
                FontSize = 20,
                Style = (Style)Application.Current.FindResource("ButtonBaseStyle")

            };

            okButton.Click += (sender, args) => customMessageBox.Close();

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(okButton);

            customMessageBox.Content = stackPanel;

            customMessageBox.ShowDialog();
        }

        public MessageBoxResult MessageBoxYesNo(string message, Action<bool> actionAfterClick)
        {
            var customMessageBox = new Window
            {
                Width = 400,
                Height = 150,
                Background = new SolidColorBrush(bardzoJasnyRoz),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.None,
            };

            var textBlock = new TextBlock
            {
                Text = message,
                FontSize = 18,
                Margin = new Thickness(0, 10, 0, 5),
                Foreground = new SolidColorBrush(ciemnyRoz),
                Style = (Style)Application.Current.FindResource("textBlock"),
            };

            var yesButton = new Button
            {
                Content = "Tak",
                Width = 80,
                Height = 30,
                FontSize = 20,
                Style = (Style)Application.Current.FindResource("ButtonBaseStyle"),
                Tag = MessageBoxResult.Yes
            };

            var noButton = new Button
            {
                Content = "Nie",
                Width = 80,
                Height = 30,
                FontSize = 20,
                Margin = new Thickness(0, 0, 0, 4),
                Style = (Style)Application.Current.FindResource("ButtonBaseStyle"),
                Tag = MessageBoxResult.No
            };

            yesButton.Click += (sender, args) =>
            {
                actionAfterClick?.Invoke(true);
                customMessageBox.DialogResult = (bool?)true;
                customMessageBox.Close();
            };

            noButton.Click += (sender, args) =>
            {
                actionAfterClick?.Invoke(false);
                customMessageBox.DialogResult = (bool?)false;
                customMessageBox.Close();
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());

            Grid.SetColumn(yesButton, 0);
            Grid.SetColumn(noButton, 1);

            grid.Children.Add(yesButton);
            grid.Children.Add(noButton);

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(grid);

            customMessageBox.Content = stackPanel;

            customMessageBox.ShowDialog();

            return customMessageBox.DialogResult != null ?
                (bool)customMessageBox.DialogResult ? MessageBoxResult.Yes : MessageBoxResult.No
                : MessageBoxResult.None;
        }

        public string MessageBoxUstawienia(string userNick, bool czyBezpiecznaStrefa)
        {
            this.czyBezpiecznaStrefa = czyBezpiecznaStrefa;  
            this.userNick = userNick;

            var customMessageBox = new Window
            {
                Width = 250,
                Height = 350,
                Background = new SolidColorBrush(bardzoJasnyRoz),
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.None,
            };

            var title = new TextBlock
            {
                Text = "Ustawienia",
                FontSize = 30,
                Foreground = new SolidColorBrush(ciemnyRoz),
                Style = (Style)Application.Current.FindResource("textBlock"),
            };

            var bezpiecznaStrefa = new TextBlock
            {
                Text = "Bezpieczna strefa:",
                Foreground = new SolidColorBrush(ciemnyRoz),
                Style = (Style)Application.Current.FindResource("textBlock"),
            };

            var toggleButton = new ToggleButton
            {
                IsChecked = czyBezpiecznaStrefa,
                Style = (Style)Application.Current.FindResource("ToogleButtonSlider")
            };

            var zmienNick = new TextBlock
            {
                Text = "Zmien nick:",
                Foreground = new SolidColorBrush(ciemnyRoz),
                Style = (Style)Application.Current.FindResource("textBlock"),
            };

            var zmienNickTextBox = new TextBox
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Colors.Transparent),
                BorderBrush = new SolidColorBrush(Colors.Transparent),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = userNick,
            };

            var border = new Border
            {
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(5),
                Background = new SolidColorBrush(ciemnyRoz),
                Child = zmienNickTextBox,
            }; 

            var wylogujButton = new Button
            {
                Content = "WYLOGUJ",
                Width = 110,
                Height = 30,
                FontSize = 20,
                Margin = new Thickness(5, 2, 5, 2),
                Style = (Style)Application.Current.FindResource("ButtonBaseStyle"),

            };

            var zapiszButton = new Button
            {
                Content = "ZAPISZ",
                Width = 110,
                Height = 30,
                FontSize = 20,
                Margin = new Thickness(0, 2, 5, 2),
                Style = (Style)Application.Current.FindResource("ButtonBaseStyle"),

            };

            toggleButton.Checked += toggleButtonChecked;
            toggleButton.Unchecked += toggleButtonUnChecked;
            wylogujButton.Click += (sender, args) => wylogujButtonClick();
            zapiszButton.Click += (sender, args) => 
            {
                if(userNick != zmienNickTextBox.Text)
                {
                    sprawdzNick(zmienNickTextBox.Text, zmienNickTextBox);
                    userNick = this.userNick;
                }
                else
                {
                    customMessageBox.Close();
                }
                
            };

            var stackPanel = new StackPanel();

            var stackPanelHorizontal = new StackPanel();
            stackPanelHorizontal.Orientation = Orientation.Horizontal;
            stackPanelHorizontal.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanelHorizontal.Children.Add(wylogujButton);
            stackPanelHorizontal.Children.Add(zapiszButton);

            stackPanel.Children.Add(title);
            stackPanel.Children.Add(bezpiecznaStrefa);
            stackPanel.Children.Add(toggleButton);
            stackPanel.Children.Add(zmienNick);
            stackPanel.Children.Add(border);
            stackPanel.Children.Add(stackPanelHorizontal);    

            customMessageBox.Content = stackPanel;

            customMessageBox.ShowDialog();

            return this.userNick;
        }

        private void toggleButtonChecked(object sender, RoutedEventArgs e)
        {
            czyBezpiecznaStrefa = true;
        }

        private void toggleButtonUnChecked(object sender, RoutedEventArgs e)
        {
            czyBezpiecznaStrefa = false;
        }

        private void wylogujButtonClick()
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }

        private void sprawdzNick(string newUserNick, TextBox zmienNickTextBox)
        {
            string connectionString = "server=localhost;user id=root;password=;database=saper";
            MySqlConnection conn = new MySqlConnection(connectionString);

            try
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM rekordy WHERE Nick = @newUserNick";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@newUserNick", newUserNick);

                int rowCount = Convert.ToInt32(cmd.ExecuteScalar());

                if (rowCount > 0)
                {
                    MessageBoxOk("Użytownik o podanym nicku juz istnieje. Zaproponujemy ci inny!");
                    bool jestNick = true;
                    while (jestNick)
                    {
                        string query2 = "SELECT COUNT(*) FROM rekordy WHERE Nick = @newUserNick";
                        MySqlCommand cmd2 = new MySqlCommand(query2, conn);
                        cmd2.Parameters.AddWithValue("@newUserNick", newUserNick);
                        int rowCount2 = Convert.ToInt32(cmd2.ExecuteScalar());

                        if (rowCount2 > 0)
                        {
                            string dodajDoNicku = random.Next(0, 10).ToString();
                            newUserNick += dodajDoNicku;
                        }
                        else
                        {
                            jestNick = false;
                        }
                    }

                    MessageBoxYesNo("Czy odpowiada ci ten nick: " + newUserNick, (result) =>
                    {
                        if (result)
                        {
                            zmienNickTextBox.Text = newUserNick;
                            string updateNick = "UPDATE rekordy SET Nick = @newUserNick WHERE Nick = @userNick";
                            MySqlCommand cmd3 = new MySqlCommand(updateNick, conn);
                            cmd3.Parameters.AddWithValue("@userNick", userNick);
                            cmd3.Parameters.AddWithValue("@newUserNick", newUserNick);
                            cmd3.ExecuteNonQuery();
                            this.userNick = newUserNick;
                            MessageBoxOk("Zmieniono nick na: " + newUserNick);
                        }
                        else
                        {
                            newUserNick += random.Next(0, 10).ToString();
                            sprawdzNick(newUserNick, zmienNickTextBox);
                        }
                    });
                   
                }
                else
                {
                    zmienNickTextBox.Text = newUserNick;
                    string updateNick = "UPDATE rekordy SET Nick = @newUserNick WHERE Nick = @userNick";
                    MySqlCommand cmd3 = new MySqlCommand(updateNick, conn);
                    cmd3.Parameters.AddWithValue("@userNick", userNick);
                    cmd3.Parameters.AddWithValue("@newUserNick", newUserNick);
                    cmd3.ExecuteNonQuery();

                    this.userNick = newUserNick;
                    MessageBoxOk("Zmieniono nick na: " + newUserNick);
                }
            }
            catch (Exception ex)
            {
                MessageBoxOk("Błąd logowania: " + ex.Message);

            }
            finally
            {
                conn.Close();
            }
        }
    }
}
