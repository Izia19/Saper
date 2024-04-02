using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Saper
{
    public partial class Poziom_latwy : Window
    {
        public string n;
        public string p;
        public Window window;
        public static Image bombaImage = new Image { Source = new BitmapImage(new Uri("C:/Icons/bomba.png")), Stretch = Stretch.Fill };
        private Random random = new Random();
        private DispatcherTimer timer;
        private int seconds = 0;
        private int minutes = 0;
        private int hours = 0;
        private Button[,] gameButtons = new Button[10, 10];
        private int bombCount = 10;
        private int rightClicksLeft = 10;
        private int remaining_fields = 90;

        public Poziom_latwy()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            GenerateGameBoard();
            PlaceBombs();
            CalculateNeighborBombCounts();
            bomby.Text = $"Bomby: {rightClicksLeft}";
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            timer.Start();

        }
        private void GenerateGameBoard()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
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
        private void PlaceBombs()
        {
            HashSet<int> bombIndices = new HashSet<int>();

            while (bombIndices.Count < bombCount)
            {
                int index = random.Next(0, 100);
                bombIndices.Add(index);
            }

            foreach (int index in bombIndices)
            {
                int row = index / 10;
                int col = index % 10;
                gameButtons[row, col].Tag = "Bomb";
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;

            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

            if (button.Tag != null && button.Tag.ToString() == "Bomb")
            {
                timer.Stop();
                button.Content = bombaImage;
                MessageBoxResult result = MessageBox.Show("Koniec gry. Chcesz zresetować? Nie powoduje powrót do menu", "Boom! Trafiłeś na bombę!", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    gameGrid.Children.Clear();
                    InitializeComponent();
                    GenerateGameBoard();
                    PlaceBombs();
                    CalculateNeighborBombCounts();
                    remaining_fields = 90;
                    rightClicksLeft = 10;
                    seconds = 0;
                    minutes = 0;
                    hours = 0;
                    czas.Text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
                    timer.Start();
                }
                else
                {
                    this.Close();
                    window.Show();
                }
            }
            else
            {
                button.Content = button.Tag;
            }
            remaining_fields -= 1;
            CheckGameResult();
        }
        private void CalculateNeighborBombCounts()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
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

            for (int i = Math.Max(0, row - 1); i <= Math.Min(9, row + 1); i++)
            {
                for (int j = Math.Max(0, col - 1); j <= Math.Min(9, col + 1); j++)
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
                    string query = "SELECT COUNT(*) FROM rekordy WHERE Nick = @n AND Poziom = @p";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@n", n);
                    cmd.Parameters.AddWithValue("@p", p);
                    int rowCount = Convert.ToInt32(cmd.ExecuteScalar());

                    TimeSpan gameTime = new TimeSpan(hours, minutes, seconds);
                    int gameSeconds = (int)gameTime.TotalSeconds; // Konwersja na sekundy

                    if (rowCount > 0)
                    {
                        query = "SELECT id FROM rekordy WHERE Nick = @n AND Poziom = @p";
                        cmd.CommandText = query;
                        int id = Convert.ToInt32(cmd.ExecuteScalar());

                        query = "SELECT Wynik FROM rekordy WHERE Id = @id";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.CommandText = query;
                        string time = Convert.ToString(cmd.ExecuteScalar());

                        int bestTime = Convert.ToInt32(double.Parse(time)); // Konwersja najlepszego czasu na sekundy

                        if (gameSeconds < bestTime)
                        {
                            // Aktualizacja najlepszego czasu
                            query = "UPDATE rekordy SET Wynik = @wynik WHERE Id = @id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@wynik", gameSeconds);
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.CommandText = query;
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Gratulacje! Wygrałeś grę i ustanowiłeś nowy rekord!", "Koniec gry", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Gratulacje! Wygrałeś grę ale niestety nie pokonałeś swojego rekordu!", "Koniec gry", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        // Wstawienie nowego rekordu
                        query = "INSERT INTO rekordy (Nick, Wynik, Poziom) VALUES (@n, @wynik, @p)";
                        cmd.CommandText = query;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@n", n);
                        cmd.Parameters.AddWithValue("@wynik", gameSeconds);
                        cmd.Parameters.AddWithValue("@p", p);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Gratulacje! Wygrałeś grę i ustanowiłeś nowy rekord!", "Koniec gry", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Błąd dodawania wyniku: " + ex.Message);
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
            if (button.Content.ToString() == "o")
            {
                button.Content = "";
                rightClicksLeft++;
                bomby.Text = $"Bomby: {rightClicksLeft}";
            }
            else
            {
                if (rightClicksLeft > 0)
                {
                    button.Content = "o";
                    rightClicksLeft--;
                    bomby.Text = $"Bomby: {rightClicksLeft}";
                }
                else
                {
                    MessageBox.Show("Nie masz już dostępnych kliknięć prawym przyciskiem myszy.", "Komunikat", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            gameGrid.Children.Clear();
            GenerateGameBoard();
            PlaceBombs();
            CalculateNeighborBombCounts();
            rightClicksLeft = 10;
            remaining_fields = 90;
            seconds = 0;
            minutes = 0;
            hours = 0;
            czas.Text = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
            timer.Start();
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
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (window != null && !window.IsVisible)
            {
                window.Close();
            }
        }

    }
}
