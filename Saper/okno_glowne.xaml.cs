﻿using System;
using System.Collections.ObjectModel;
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
        public string userNick;
        public string level;
        public Window window_okno_glowne;
        private ToggleButton lastClickedButton = null;

        public okno_glowne()
        {
            User user = new User("1","2","3");
            List<User> lista = new List<User>();
            lista.Add(user);


            user_wyniki.ItemsSource = lista;

            OpenWindowsAsync();
            InitializeComponent();
            window_okno_glowne = this;
            nick_u.Content += " " + userNick;
        }

        private async void OpenWindowsAsync()
        {
            ladowanie ladowanie = new ladowanie();
            ladowanie.ShowDialog();
            this.userNick = ladowanie.userNick;
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
                    clickedButton.Background = (Brush)new BrushConverter().ConvertFrom("#FFABAB"); ;
                }
            }
        }

        public void Graj(object sender, RoutedEventArgs e)
        {
            int bombCount = 0;
            int numberOfButton = 0;
            if (level == "latwy")
            {
                bombCount = 10;
                numberOfButton = 10;
            }
            else if (level == "sredni")
            {
                numberOfButton = 15;
                bombCount = 20;
            }
            else if (level == "trudny")
            {
                numberOfButton = 20;
                bombCount = 100;
            }
   
            poziomy poziom = new poziomy(numberOfButton, bombCount, level, userNick, window_okno_glowne);
            this.Hide();
            poziom.ShowDialog();
        }

    }
}