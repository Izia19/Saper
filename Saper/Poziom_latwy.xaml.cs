﻿using MySql.Data.MySqlClient;
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
        public CustomMessageBox CustomMessageBox = new CustomMessageBox();

        public string userNick;
        public string level;
        public Window window;
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

        public Window window2;

        public Poziom_latwy(int numberOfButton, int bombCount, string level, string userNick)
        {
            window2 = this;
            if(level == "latwy")
            {
                window2.Height = 450;
                window2.Width = 500;
            }
            if(level == "sredni")
            {
                window2.Height = 600;
                window2.Width = 650;
            }
            if (level == "trudny")
            {
                window2.Height = 800;
                window2.Width = 850;
            }
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            this.numberOfButton = numberOfButton;
            this.gameButtons = new Button[numberOfButton, numberOfButton];
            this.bombCount = bombCount;
            this.remaining_fields = numberOfButton * numberOfButton - bombCount;
            this.rightClicksCount = bombCount;
            this.level = level;
            this.userNick = userNick;

            InitializeComponent();
            GenerateGameBoard();
            PlaceBombs();
            CalculateNeighborBombCounts();

            bomby.Text = $"Bomby: {rightClicksCount}";
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            gameGrid.Rows = numberOfButton;
            gameGrid.Columns = numberOfButton; 

            timer.Start();   
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
        private void PlaceBombs()
        {
            HashSet<int> bombIndices = new HashSet<int>();

            while (bombIndices.Count < bombCount)
            {
                int index = random.Next(0, numberOfButton * numberOfButton);
                bombIndices.Add(index);
            }

            foreach (int index in bombIndices)
            {
                int row = index / numberOfButton;
                int col = index % numberOfButton;
                gameButtons[row, col].Tag = "Bomb";
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.IsEnabled = false;
            var buttonImage = button.Content as Image;

            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

            if (buttonImage != null && buttonImage.Source == kwiatekImage.Source)
            {
                button.Content = "";
                rightClicksCount++;
                bomby.Text = $"Bomby: {rightClicksCount}";
            }
            if (button.Tag != null && button.Tag.ToString() == "Bomb")
            {
                timer.Stop();
                button.Content = bombaImage;

                CustomMessageBox.MessageBoxYesNo("Koniec gry. Chcesz zresetować? Nie powoduje powrót do menu", (result) =>
                {
                    if (result)
                    {
                        gameGrid.Children.Clear();
                        InitializeComponent();
                        GenerateGameBoard();
                        PlaceBombs();
                        CalculateNeighborBombCounts();
                        remaining_fields = 90;
                        rightClicksCount = 10;
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
                });
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
                    int gameSeconds = (int)gameTime.TotalSeconds; // Konwersja na sekundy

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

                            CustomMessageBox.MessageBoxOk("Gratulacje! Wygrałeś grę i ustanowiłeś nowy rekord!");
                        }
                        else
                        {
                            CustomMessageBox.MessageBoxOk("Gratulacje! Wygrałeś grę ale nie pobiłeś swojego rekordu!");
                        }
                    }
                    else
                    {
                        // Wstawienie nowego rekordu
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
            gameGrid.Children.Clear();
            GenerateGameBoard();
            PlaceBombs();
            CalculateNeighborBombCounts();
            rightClicksCount = 10;
            bomby.Text = $"Bomby: {rightClicksCount}";
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
                window.Show();
            }
        }

    }
}
