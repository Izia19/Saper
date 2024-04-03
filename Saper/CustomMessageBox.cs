using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Security.Cryptography.X509Certificates;
using System.Drawing;

namespace Saper
{
    public class CustomMessageBox
    {
        public System.Windows.Media.Color jasnyRoz = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFBEBC");
        public System.Windows.Media.Color ciemnyRoz = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF9999");
        public System.Windows.Media.Color bardzoJasnyRoz = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFECE5");
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
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 20,
                FontFamily = new FontFamily("Calibri"),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(ciemnyRoz),
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
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 18,
                FontFamily = new FontFamily("Calibri"),
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(ciemnyRoz),
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
    }
}
