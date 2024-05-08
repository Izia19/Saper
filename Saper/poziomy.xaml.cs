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
        public CustomMessageBox CustomMessageBox = new CustomMessageBox();

        private bool bombsPlaced = false;

        public string userNick;
        public string level;
        private Random random = new Random();
        private DispatcherTimer timer;

        private int seconds = 0;
        private int minutes = 0;
        private int hours = 0;

        private Button[,] gameButtons;

        private int bombCount;
        private int rightClicksCount;
        private int remaining_fields;

        public static Image bombaImage = new Image { Source = new BitmapImage(new Uri("C:/Icons/bomba.png")), Stretch = Stretch.Fill };
        public static Image kwiatekImage = new Image { Source = new BitmapImage(new Uri("C:/Icons/kwiatek.png")), Stretch = Stretch.Fill };

        public int numberOfButton;

        public Window window_poziomy;
        public Window window_okno_glowne;

        public poziomy(int numberOfButton, int bombCount, string level, string userNick, Window window_okno_glowne)
        {
            this.window_okno_glowne = window_okno_glowne;
            window_poziomy = this;
            if (level == "latwy")
            {
                window_poziomy.Height = 450;
                window_poziomy.Width = 500;
            }
            if (level == "sredni")
            {
                window_poziomy.Height = 600;
                window_poziomy.Width = 650;
            }
            if (level == "trudny")
            {
                window_poziomy.Height = 800;
                window_poziomy.Width = 850;
            }
            this.numberOfButton = numberOfButton;
            this.gameButtons = new Button[numberOfButton, numberOfButton];
            this.bombCount = bombCount;
            this.remaining_fields = numberOfButton * numberOfButton - bombCount;
            this.rightClicksCount = bombCount;
            this.level = level;
            this.userNick = userNick;

            InitializeComponent();
            GenerateGameBoard();

            bomby.Text = $"Bomby: {rightClicksCount}";
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            gameGrid.Rows = numberOfButton;
            gameGrid.Columns = numberOfButton;

            seconds = 0;
            minutes = 0;
            hours = 0;
            czas.Text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        private void FirstButtonClick(int row, int col)
        {
            PlaceBombs(row, col);
            CalculateNeighborBombCounts();
            OdkryjSasiedniePola(row, col);
        }
        private void GenerateGameBoard()
        {
            
            for (int i = 0; i < numberOfButton; i++)
            {
                for (int j = 0; j < numberOfButton; j++)
                {
                    Button button = new Button();
                    button.Click += Button_Click;
                    button.MouseRightButtonDown += Button_RightClick;
                    button.Content = "";
                    gameGrid.Children.Add(button);
                    gameButtons[i, j] = button;
                }
            }
        }
        private void PlaceBombs(int clickedRow, int clickedCol)
        {
            HashSet<int> bombIndices = new HashSet<int>();

            while (bombIndices.Count < bombCount)
            {
                int index = random.Next(0, numberOfButton * numberOfButton);
                int row = index / numberOfButton;
                int col = index % numberOfButton;

                if (Math.Abs(clickedRow - row) > 1 || Math.Abs(clickedCol - col) > 1)
                {
                    bombIndices.Add(index);
                }
            }

            foreach (int index in bombIndices)
            {
                int row = index / numberOfButton;
                int col = index % numberOfButton;
                gameButtons[row, col].Tag = "Bomb";
            }

            CalculateNeighborBombCounts();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
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
                rightClicksCount++;
                bomby.Text = $"Bomby: {rightClicksCount}";
            }

            if (!bombsPlaced)
            {
                timer.Start();
                FirstButtonClick(row, col);
                bombsPlaced = true;
            }
            else
            {
                if (button.Tag != null && button.Tag.ToString() == "Bomb")
                {
                    timer.Stop();
                    button.Content = bombaImage;
                    PlayExplosionAnimation(button);

                    CustomMessageBox.MessageBoxYesNo("Koniec gry. Chcesz zresetować? Nie powoduje powrót do menu", (result) =>
                    {
                        if (result)
                        {
                            GameRestart();
                        }
                        else
                        {
                            this.Close();
                            window_okno_glowne.Show();
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
            remaining_fields -= 1;
            CheckGameResult();
        }
        private void PlayExplosionAnimation(Button button)
        {
            Image explosionImage = new Image
            {
                Source = new BitmapImage(new Uri("C:/Icons/wybuch.png")),
                Stretch = Stretch.Fill,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            button.Content = explosionImage;

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

            explosionImage.RenderTransform = new ScaleTransform(0.5, 0.5);
            explosionImage.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleXAnimation);
            explosionImage.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleYAnimation);

        }
        private void OdkryjSasiedniePola(int row, int col)
        {
            for (int i = Math.Max(0, row - 1); i <= Math.Min(numberOfButton - 1, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(numberOfButton - 1, col + 1); j++)
                {
                    Button button = gameButtons[i, j];

                    if (button.IsEnabled)
                    {
                        button.IsEnabled = false;
                        button.Content = button.Tag;
                        remaining_fields -= 1;

                        if (button.Tag == null)
                        {
                            OdkryjSasiedniePola(i, j);
                        }
                    }
                }
            }
        }
        private void CalculateNeighborBombCounts()
        {
            for (int i = 0; i < numberOfButton; i++)
            {
                for (int j = 0; j < numberOfButton; j++)
                {
                    Button button = gameButtons[i, j];
                    if (button.Tag == null)
                    {
                        int bombCount = CountNeighborBombs(i, j);
                        if (bombCount > 0)
                        {
                            button.Tag = bombCount.ToString();
                        }
                    }
                }
            }
        }
        private int CountNeighborBombs(int row, int col)
        {
            int bombCount = 0;

            for (int i = Math.Max(0, row - 1); i <= Math.Min(numberOfButton - 1, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(numberOfButton - 1, col + 1); j++)
                {
                    if (gameButtons[i, j].Tag != null && gameButtons[i, j].Tag.ToString() == "Bomb")
                    {
                        bombCount++;
                    }
                }
            }
            return bombCount;
        }
        private void CheckGameResult()
        {
            if (remaining_fields == 0)
            {
                timer.Stop();
                string connectionString = "server=localhost;user id=root;password=;database=saper";
                MySqlConnection conn = new MySqlConnection(connectionString);

                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM rekordy WHERE Nick = @userNick AND Poziom = @level";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userNick", userNick);
                    cmd.Parameters.AddWithValue("@level", level);
                    int rowCount = Convert.ToInt32(cmd.ExecuteScalar());

                    TimeSpan gameTime = new TimeSpan(hours, minutes, seconds);
                    int gameSeconds = (int)gameTime.TotalSeconds;

                    if (rowCount > 0)
                    {
                        query = "SELECT id FROM rekordy WHERE Nick = @userNick AND Poziom = @level";
                        cmd.CommandText = query;
                        int id = Convert.ToInt32(cmd.ExecuteScalar());

                        query = "SELECT Wynik FROM rekordy WHERE Id = @id";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.CommandText = query;
                        string time = Convert.ToString(cmd.ExecuteScalar());

                        int bestTime = Convert.ToInt32(double.Parse(time));

                        if (gameSeconds < bestTime)
                        {
                            query = "UPDATE rekordy SET Wynik = @wynik WHERE Id = @id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@wynik", gameSeconds);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();

                            CustomMessageBox.MessageBoxOk("Gratulacje! Wygrałeś grę i ustanowiłeś nowy rekord!");
                            this.Close();
                            window_okno_glowne.Show();
                        }
                        else
                        {
                            CustomMessageBox.MessageBoxOk("Gratulacje! Wygrałeś grę ale nie pobiłeś swojego rekordu!");
                            this.Close();
                            window_okno_glowne.Show();
                        }
                    }
                    else
                    {
                        query = "INSERT INTO rekordy (Nick, Wynik, Poziom) VALUES (@n, @wynik, @p)";
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@n", userNick);
                        cmd.Parameters.AddWithValue("@wynik", gameSeconds);
                        cmd.Parameters.AddWithValue("@p", level);
                        cmd.ExecuteNonQuery();

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
        private void Button_RightClick(object sender, MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            var buttonImage = button.Content as Image;

            if (buttonImage != null && buttonImage.Source == kwiatekImage.Source)
            {
                button.Content = "";
                rightClicksCount++;
                bomby.Text = $"Bomby: {rightClicksCount}";
            }
            else
            {
                if (rightClicksCount > 0)
                {
                    button.Content = new Image { Source = kwiatekImage.Source, Stretch = Stretch.Fill };
                    rightClicksCount--;
                    bomby.Text = $"Bomby: {rightClicksCount}";
                }
                else
                {
                    CustomMessageBox.MessageBoxOk("Nie masz już dostępnych kwiatków :(");
                }
            }
        }
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            GameRestart();
        }
        public void GameRestart()
        {
            timer.Stop();
            gameGrid.Children.Clear();
            bombsPlaced = false;
            GenerateGameBoard();
            rightClicksCount = bombCount;
            bomby.Text = $"Bomby: {rightClicksCount}";
            remaining_fields = numberOfButton * numberOfButton - bombCount;
            seconds = 0;
            minutes = 0;
            hours = 0;
            czas.Text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
        private void Timer_Tick(object sender, EventArgs e)
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
        private void ButtonPowrot(object sender, RoutedEventArgs e)
        {
            this.Close();
            window_okno_glowne.Show();
        }
    }
}