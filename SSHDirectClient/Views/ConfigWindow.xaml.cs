﻿using SSHDirectClient.Database;
using SSHDirectClient.Database.Entities;
using SSHDirectClient.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace SSHDirectClient
{
    /// <summary>
    /// Interaction logic for ConfigAddWindow.xaml
    /// </summary>
    public partial class ConfigWindow : UWPHost.Window
    {
        DatabaseHandler DatabaseHandler = new DatabaseHandler();
        ObservableCollection<SSHConfigEntity> SSHConfigs = new ObservableCollection<SSHConfigEntity>();
        SSHConfigEntity? SelectedConfig = null;
        public ConfigWindow()
        {
            InitializeComponent();
            RefreshConfigList();
            ListViewConfigs.ItemsSource = SSHConfigs;
            ThemeManager.ApplyTheme();

        }

        private void RefreshConfigList()
        {
            SSHConfigs.Clear();
            var configs = DatabaseHandler.GetAll();
            foreach (SSHConfigEntity config in configs)
            {
                SSHConfigs.Add(config);
            }

        }

        public void CheckFields()
        {
            if (textBoxName.Text == "")
            {
                MessageBox.Show("Please enter a valid Host Address.");
                return;
            }
            if (textBoxHost.Text == "")
            {
                MessageBox.Show("Please enter a valid Host Address.");
                return;
            }

            if (textBoxHostPort.Text == "" || !(Int32.TryParse(textBoxHostPort.Text, out int res_port)))
            {
                MessageBox.Show("Please enter a valid Host Port.");
                return;
            }

            if (textBoxUsername.Text == "")
            {
                MessageBox.Show("Please enter a valid Username.");
                return;
            }

            if (passwordBoxPassword.Password == "")
            {
                MessageBox.Show("Please enter a valid Password.");
                return;
            }
        }
        private void ClearFields()
        {
            textBoxHost.Clear();
            textBoxHostPort.Clear();
            textBoxName.Clear();
            textBoxUsername.Clear();
            passwordBoxPassword.Clear();
        }

        #region events

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckFields();
                DatabaseHandler.Insert(new SSHConfigEntity { Name = textBoxName.Text, Password = passwordBoxPassword.Password, ServerAddress = textBoxHost.Text, ServerPort = Convert.ToUInt32(textBoxHostPort.Text), Username = textBoxUsername.Text });
                RefreshConfigList();
                ListViewConfigs.SelectedIndex = -1;
            }
            catch { }

        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedConfig == null)
                {
                    MessageBox.Show("No configuration is selected to edit!");
                    return;
                }
                else
                {
                    var id = SelectedConfig.Id;
                    CheckFields();
                    DatabaseHandler.Update(new SSHConfigEntity { Id = id, Name = textBoxName.Text, Password = passwordBoxPassword.Password, ServerAddress = textBoxHost.Text, ServerPort = Convert.ToUInt32(textBoxHostPort.Text), Username = textBoxUsername.Text });
                    RefreshConfigList();
                    ListViewConfigs.SelectedIndex = -1;
                }
            }
            catch { }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedConfig == null)
                {
                    MessageBox.Show("No configuration is selected to Remove!");
                    return;
                }
                else
                {
                    DatabaseHandler.Delete(SelectedConfig);
                    ButtonClear_Click(null, null);
                    RefreshConfigList();
                    ListViewConfigs.SelectedIndex = -1;
                }
            }
            catch { }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ListViewConfigs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedConfig = ListViewConfigs.SelectedItem as SSHConfigEntity;
            if(SelectedConfig != null)
            {
                textBoxUsername.Text = SelectedConfig.Username;
                textBoxName.Text = SelectedConfig.Name;
                textBoxHost.Text = SelectedConfig.ServerAddress;
                textBoxHostPort.Text = SelectedConfig.ServerPort.ToString();
                passwordBoxPassword.Password = SelectedConfig.Password;
            }
            else
            {
                ClearFields();
            }
        }
        #endregion

        #region Theme
        private void SwitchTheme(string theme)
        {


            if (theme == "Dark")
            {
                App.Current.Resources["BackElement"] = Brushes.Black;
                App.Current.Resources["Back"] = new SolidColorBrush(Color.FromRgb(25, 25, 25));
                App.Current.Resources["Fore"] = Brushes.White;
                App.Current.Resources["Accent"] = new SolidColorBrush(Color.FromRgb(16, 37, 100));
                foreach (System.Windows.Window wpfWindow in App.Current.Windows)
                {
                    var uwpWindow = wpfWindow as UWPHost.Window;
                    if (uwpWindow != null)
                    {
                        uwpWindow.Theme = "Light";
                        var grid = uwpWindow.Content as Grid;
                        grid.RowDefinitions[0].Height = new GridLength(0, GridUnitType.Pixel);
                        uwpWindow.Foreground = App.Current.Resources["Fore"] as SolidColorBrush;
                        uwpWindow.TitlebarBrush = App.Current.Resources["Back"] as SolidColorBrush;
                    }
                }

            }
            else if (theme == "Light")
            {
                App.Current.Resources["BackElement"] = Brushes.White;
                App.Current.Resources["Back"] = new SolidColorBrush(Color.FromRgb(223, 223, 223));
                App.Current.Resources["Fore"] = Brushes.Black;
                App.Current.Resources["Accent"] = new SolidColorBrush(Color.FromRgb(165, 185, 245));
                foreach (System.Windows.Window wpfWindow in App.Current.Windows)
                {
                    var uwpWindow = wpfWindow as UWPHost.Window;
                    if (uwpWindow != null)
                    {
                        uwpWindow.Theme = "Dark";
                        var grid = uwpWindow.Content as Grid;
                        grid.RowDefinitions[0].Height = new GridLength(33, GridUnitType.Pixel);
                        uwpWindow.Foreground = App.Current.Resources["Fore"] as SolidColorBrush;
                        uwpWindow.TitlebarBrush = App.Current.Resources["Back"] as SolidColorBrush;
                    }
                }
            }
        }
        #endregion

        #region TitleBarEvents
        /*
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void ButtonResize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                SystemCommands.RestoreWindow(this);
            }
            else
            {
                SystemCommands.MaximizeWindow(this);
            }

        }       
        */

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

 


        #endregion


    }
}
