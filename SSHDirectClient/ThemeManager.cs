using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SSHDirectClient.Models
{
    public static class ThemeManager
    {
        public static ThemeState CurrentTheme = new ThemeState();

        static ThemeManager()
        {
            CurrentTheme.PropertyChanged += ThemeChanged;
        }

        private static void ThemeChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentTheme.Theme))
            {
                ApplyTheme();
            }
        }

        public static void ApplyTheme()
        {
            if (Application.Current == null) return;

            if (CurrentTheme.Theme == Themes.Dark)
            {
                Application.Current.Resources["BackElement"] = Brushes.Black;
                Application.Current.Resources["Back"] = new SolidColorBrush(Color.FromRgb(25, 25, 25));
                Application.Current.Resources["Fore"] = Brushes.White;
                Application.Current.Resources["Accent"] = new SolidColorBrush(Color.FromRgb(16, 37, 100));
            }
            else
            {
                Application.Current.Resources["BackElement"] = Brushes.White;
                Application.Current.Resources["Back"] = new SolidColorBrush(Color.FromRgb(223, 223, 223));
                Application.Current.Resources["Fore"] = Brushes.Black;
                Application.Current.Resources["Accent"] = new SolidColorBrush(Color.FromRgb(165, 185, 245));
            }

            UpdateAllWindows();
        }

        private static void UpdateAllWindows()
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.Background = (SolidColorBrush)Application.Current.Resources["Back"];
                window.Foreground = (SolidColorBrush)Application.Current.Resources["Fore"];
                var uwpWindow = window as UWPHost.Window;
                if (uwpWindow != null && CurrentTheme.Theme == Themes.Dark)
                {
                    uwpWindow.Theme = "Light";
                    var grid = uwpWindow.Content as Grid;
                    grid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Pixel);
                    uwpWindow.Foreground = App.Current.Resources["Fore"] as SolidColorBrush;
                    uwpWindow.TitlebarBrush = App.Current.Resources["Back"] as SolidColorBrush;
                }
                else if(uwpWindow != null && CurrentTheme.Theme == Themes.Light)
                {
                    uwpWindow.Theme = "Dark";
                    var grid = uwpWindow.Content as Grid;
                    grid.RowDefinitions[0].Height = new GridLength(33, GridUnitType.Pixel);
                    uwpWindow.Foreground = App.Current.Resources["Fore"] as SolidColorBrush;
                    uwpWindow.TitlebarBrush = App.Current.Resources["Back"] as SolidColorBrush;
                }
                if (window is MainWindow)
                {
                    var mainWindow = window as MainWindow;

                    if (CurrentTheme.Theme == Themes.Light)
                    {
                        mainWindow.LabelCurrentTheme.Content = "Current: Light";
                        mainWindow.IconCurrentTheme.Kind = Material.Icons.MaterialIconKind.WeatherSunny;
                        mainWindow.IconCurrentTheme.Foreground = Brushes.Goldenrod;
                    }
                    else
                    {
                        mainWindow.LabelCurrentTheme.Content = "Current: Dark";
                        mainWindow.IconCurrentTheme.Kind = Material.Icons.MaterialIconKind.WeatherNight;
                        mainWindow.IconCurrentTheme.Foreground = Brushes.LightSteelBlue;
                    }
                }
            }
        }

        public static void ToggleTheme()
        {
            CurrentTheme.Theme = CurrentTheme.Theme == Themes.Dark ? Themes.Light : Themes.Dark;
        }
    }
}
