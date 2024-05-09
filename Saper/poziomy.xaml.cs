using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Saper
{
    public partial class poziomy : Window
    {
        private CustomMessageBox CustomMessageBox = new CustomMessageBox();

        private bool czyRozmieszczoneBomby = false;
        private bool czyBezpiecznaStrefa = false;

        private string userNick;
        private string poziom;
        private Random random = new Random();
        private DispatcherTimer timer;

        private int seconds = 0;
        private int minutes = 0;
        private int hours = 0;

        private Button[,] tablicaPrzyciskow;

        private int iloscBomb;
        private int iloscKwiatkow;
        private int pozostalePola;
        private int iloscPrzyciskow;

        public static Image bombaImage = new Image { Source = new BitmapImage(new Uri("C:/Icons/bomba.png")), Stretch = Stretch.Fill };
        public static Image kwiatekImage = new Image { Source = new BitmapImage(new Uri("C:/Icons/kwiatek.png")), Stretch = Stretch.Fill };
        public static Image wybuchImage = new Image { Source = new BitmapImage(new Uri("C:/Icons/wybuch.png")), Stretch = Stretch.Fill, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

        public Window oknoPoziomy;
        public Window oknoGlowne;

        public poziomy(int iloscPrzyciskow, int iloscBomb, string poziom, string userNick, Window oknoGlowne, bool czyBezpiecznaStrefa)
        {
            this.oknoGlowne = oknoGlowne;
            oknoPoziomy = this;
            if (poziom == "latwy")
            {
                oknoPoziomy.Height = 450;
                oknoPoziomy.Width = 500;
            }
            if (poziom == "sredni")
            {
                oknoPoziomy.Height = 600;
                oknoPoziomy.Width = 650;
            }
            if (poziom == "trudny")
            {
                oknoPoziomy.Height = 800;
                oknoPoziomy.Width = 850;
            }
            this.iloscPrzyciskow = iloscPrzyciskow;
            this.tablicaPrzyciskow = new Button[iloscPrzyciskow, iloscPrzyciskow];
            this.iloscBomb = iloscBomb;
            this.pozostalePola = iloscPrzyciskow * iloscPrzyciskow - iloscBomb;
            this.iloscKwiatkow = iloscBomb;
            this.poziom = poziom;
            this.userNick = userNick;
            this.czyBezpiecznaStrefa = czyBezpiecznaStrefa;

            InitializeComponent();
            GenerujPlansze();

            bomby.Text = $"Bomby: {iloscKwiatkow}";
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            gameGrid.Rows = iloscPrzyciskow;
            gameGrid.Columns = iloscPrzyciskow;

            seconds = 0;
            minutes = 0;
            hours = 0;
            czas.Text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";

            if (!czyBezpiecznaStrefa)
            {
                RozmiescBomby(-1, -1);
                czyRozmieszczoneBomby = true;
            }
        }
        private void PierwszeKlikniecie(int row, int col)
        {
            RozmiescBomby(row, col);
            ObliczBombyDookola();
            OdkryjSasiedniePola(row, col);
        }
        private void GenerujPlansze()
        {
            for (int i = 0; i < iloscPrzyciskow; i++)
            {
                for (int j = 0; j < iloscPrzyciskow; j++)
                {
                    Button button = new Button();
                    button.Click += KlikniecieLewy;
                    button.MouseRightButtonDown += KliknieciePrawy;
                    button.Content = "";
                    gameGrid.Children.Add(button);
                    tablicaPrzyciskow[i, j] = button;
                }
            }
        }
        private void RozmiescBomby(int kliknieteRow, int kliknieteCol)
        {
            HashSet<int> tablicaBomb = new HashSet<int>();

            while (tablicaBomb.Count < iloscBomb)
            {
                int index = random.Next(0, iloscPrzyciskow * iloscPrzyciskow);
                int row = index / iloscPrzyciskow;
                int col = index % iloscPrzyciskow;

                if (Math.Abs(kliknieteRow - row) > 1 || Math.Abs(kliknieteCol - col) > 1)
                {
                    tablicaBomb.Add(index);
                }
            }

            foreach (int index in tablicaBomb)
            {
                int row = index / iloscPrzyciskow;
                int col = index % iloscPrzyciskow;
                tablicaPrzyciskow[row, col].Tag = "Bomb";
            }

            ObliczBombyDookola();
        }
        private void KlikniecieLewy(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;
            var buttonImage = button.Content as Image;

            int index = gameGrid.Children.IndexOf(button);
            int columns = gameGrid.Columns;
            int row = index / columns;
            int col = index % columns;


            if (buttonImage != null && buttonImage.Source == kwiatekImage.Source)
            {
                button.Content = "";
                iloscKwiatkow++;
                bomby.Text = $"Bomby: {iloscKwiatkow}";
            }

            if (!czyRozmieszczoneBomby)
            {
                timer.Start();
                PierwszeKlikniecie(row, col);
                czyRozmieszczoneBomby = true;
            }
            else
            {
                if (button.Tag != null && button.Tag.ToString() == "Bomb")
                {
                    timer.Stop();
                    button.Content = bombaImage;
                    AnimacjaWybuchu(button);

                    CustomMessageBox.MessageBoxYesNo("Koniec gry. Chcesz zresetować? Nie powoduje powrót do menu", (result) =>
                    {
                        if (result)
                        {
                            Restart();
                        }
                        else
                        {
                            this.Close();
                            oknoGlowne.Show();
                        }
                    });
                }
                else
                {
                    if (button.Tag == null)
                    {
                        OdkryjSasiedniePola(row, col);
                    }
                    else
                    {
                        button.Content = button.Tag;
                    }
                } 
            }
            pozostalePola -= 1;
            RezultatGry();
        }
        private void AnimacjaWybuchu(Button button)
        {
            button.Content = wybuchImage;

            DoubleAnimation scaleXAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };

            DoubleAnimation scaleYAnimation = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromSeconds(1)
            };

            scaleXAnimation.Completed += (s, e) =>
            {
                button.Content = bombaImage;
            };

            wybuchImage.RenderTransform = new ScaleTransform(0.5, 0.5);
            wybuchImage.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
            wybuchImage.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);

        }
        private void OdkryjSasiedniePola(int row, int col)
        {
            for (int i = Math.Max(0, row - 1); i <= Math.Min(iloscPrzyciskow - 1, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(iloscPrzyciskow - 1, col + 1); j++)
                {
                    Button button = tablicaPrzyciskow[i, j];

                    if (button.IsEnabled)
                    {
                        button.IsEnabled = false;
                        button.Content = button.Tag;
                        pozostalePola -= 1;

                        if (button.Tag == null)
                        {
                            OdkryjSasiedniePola(i, j);
                        }
                    }
                }
            }
        }
        private void ObliczBombyDookola()
        {
            for (int i = 0; i < iloscPrzyciskow; i++)
            {
                for (int j = 0; j < iloscPrzyciskow; j++)
                {
                    Button button = tablicaPrzyciskow[i, j];
                    if (button.Tag == null)
                    {
                        int iloscBomb = IleSasiednichBomb(i, j);
                        if (iloscBomb > 0)
                        {
                            button.Tag = iloscBomb.ToString();
                        }
                    }
                }
            }
        }
        private int IleSasiednichBomb(int row, int col)
        {
            int iloscBomb = 0;

            for (int i = Math.Max(0, row - 1); i <= Math.Min(iloscPrzyciskow - 1, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(iloscPrzyciskow - 1, col + 1); j++)
                {
                    if (tablicaPrzyciskow[i, j].Tag != null && tablicaPrzyciskow[i, j].Tag.ToString() == "Bomb")
                    {
                        iloscBomb++;
                    }
                }
            }
            return iloscBomb;
        }
        private void RezultatGry()
        {
            if (pozostalePola == 0)
            {
                timer.Stop();
                string connectionString = "server=localhost;user id=root;password=;database=saper";
                MySqlConnection conn = new MySqlConnection(connectionString);

                try
                {
                    conn.Open();
                    string checkRekord = "SELECT COUNT(*) FROM rekordy WHERE Nick = @userNick AND Poziom = @level";
                    MySqlCommand cmd = new MySqlCommand(checkRekord, conn);
                    cmd.Parameters.AddWithValue("@userNick", userNick);
                    cmd.Parameters.AddWithValue("@level", poziom);
                    int rowCount = Convert.ToInt32(cmd.ExecuteScalar());

                    TimeSpan gameTime = new TimeSpan(hours, minutes, seconds);
                    int gameSeconds = (int)gameTime.TotalSeconds;

                    if (rowCount > 0)
                    {
                        string selectId = "SELECT id FROM rekordy WHERE Nick = @userNick AND Poziom = @level";
                        MySqlCommand cmd2 = new MySqlCommand(checkRekord, conn);
                        cmd2.Parameters.AddWithValue("@userNick", userNick);
                        cmd2.Parameters.AddWithValue("@level", poziom);
                        cmd2.CommandText = selectId;
                        int id = Convert.ToInt32(cmd2.ExecuteScalar());

                        string selectScore = "SELECT Wynik FROM rekordy WHERE Id = @id";
                        MySqlCommand cmd3 = new MySqlCommand(selectScore, conn);
                        cmd3.Parameters.Clear();
                        cmd3.Parameters.AddWithValue("@id", id);
                        cmd3.CommandText = selectScore;

                        string score = Convert.ToString(cmd3.ExecuteScalar());

                        int bestScore = Convert.ToInt32(double.Parse(score));

                        if (gameSeconds < bestScore)
                        {
                            string updateScore = "UPDATE rekordy SET Wynik = @wynik WHERE Id = @id";
                            MySqlCommand cmd4 = new MySqlCommand(updateScore, conn);
                            cmd4.Parameters.Clear();
                            cmd4.Parameters.AddWithValue("@wynik", gameSeconds);
                            cmd4.Parameters.AddWithValue("@id", id);
                            cmd4.CommandText = updateScore;
                            cmd4.ExecuteNonQuery();

                            CustomMessageBox.MessageBoxOk("Gratulacje! Wygrałeś grę i ustanowiłeś nowy rekord!");
                            this.Close();
                            oknoGlowne.Show();
                        }
                        else
                        {
                            CustomMessageBox.MessageBoxOk("Gratulacje! Wygrałeś grę ale nie pobiłeś swojego rekordu!");
                            this.Close();
                            oknoGlowne.Show();
                        }
                    }
                    else
                    {
                        string insertUserScore = "INSERT INTO rekordy (Nick, Wynik, Poziom) VALUES (@n, @wynik, @p)";
                        MySqlCommand cmd2 = new MySqlCommand(insertUserScore, conn);
                        cmd2.CommandText = insertUserScore;
                        cmd2.Parameters.Clear();
                        cmd2.Parameters.AddWithValue("@n", userNick);
                        cmd2.Parameters.AddWithValue("@wynik", gameSeconds);
                        cmd2.Parameters.AddWithValue("@p", poziom);
                        cmd2.ExecuteNonQuery();

                        CustomMessageBox.MessageBoxOk("Gratulacje! Wygrałeś grę i ustanowiłeś nowy rekord!");
                    }
                }
                catch (Exception ex)
                {
                    CustomMessageBox.MessageBoxOk("Błąd dodawania wyniku: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        private void KliknieciePrawy(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            var buttonImage = button.Content as Image;

            if (buttonImage != null && buttonImage.Source == kwiatekImage.Source)
            {
                button.Content = "";
                iloscKwiatkow++;
                bomby.Text = $"Bomby: {iloscKwiatkow}";
            }
            else
            {
                if (iloscKwiatkow > 0)
                {
                    button.Content = new Image { Source = kwiatekImage.Source, Stretch = Stretch.Fill };
                    iloscKwiatkow--;
                    bomby.Text = $"Bomby: {iloscKwiatkow}";
                }
                else
                {
                    CustomMessageBox.MessageBoxOk("Nie masz już dostępnych kwiatków :(");
                }
            }
        }
        private void PrzyciskRestart(object sender, RoutedEventArgs e)
        {
            Restart();
        }
        public void Restart()
        {
            timer.Stop();
            gameGrid.Children.Clear();
            GenerujPlansze();
            if (czyBezpiecznaStrefa)
            {
                czyRozmieszczoneBomby = false;
            }
            else
            {
                RozmiescBomby(-1, -1);
            }           
            iloscKwiatkow = iloscBomb;
            bomby.Text = $"Bomby: {iloscKwiatkow}";
            pozostalePola = iloscPrzyciskow * iloscPrzyciskow - iloscBomb;
            seconds = 0;
            minutes = 0;
            hours = 0;
            czas.Text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
        private void TimerTick(object sender, EventArgs e)
        {
            seconds++;
            if (seconds == 60)
            {
                seconds = 0;
                minutes++;
                if (minutes == 60)
                {
                    minutes = 0;
                    hours++;
                }
            }
            czas.Text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
        private void PrzyciskPowrot(object sender, RoutedEventArgs e)
        {
            this.Close();
            oknoGlowne.Show();
        }
    }
}