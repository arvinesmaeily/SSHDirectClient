using System;
using System.ComponentModel;

namespace SSHDirectClient.Models
{
    public enum Themes
    {
        Dark,
        Light
    }

    public class ThemeState : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static ThemeState Current { get; } = new ThemeState();

        private Themes _Theme = Themes.Dark;

        public Themes Theme
        {
            get => _Theme;
            set
            {
                if (_Theme != value)
                {
                    _Theme = value;
                    OnPropertyChanged(nameof(Theme));
                    SaveTheme(); // Save when changed
                }
            }
        }

        public ThemeState()
        {
            LoadTheme(); // Load saved theme on startup
        }

        private void LoadTheme()
        {
            if (Enum.TryParse(SettingsMain.Default.Theme, out Themes savedTheme))
            {
                _Theme = savedTheme;
            }
            else
            {
                _Theme = Themes.Dark; // Default fallback
            }
        }

        private void SaveTheme()
        {
            SettingsMain.Default.Theme = Theme.ToString();
            SettingsMain.Default.Save();
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
