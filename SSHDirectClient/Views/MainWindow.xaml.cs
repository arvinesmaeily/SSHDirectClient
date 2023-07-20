using Renci.SshNet;
using SSHDirectClient.Database;
using SSHDirectClient.Database.Entities;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SSHDirectClient
{
    public enum StateColor { Gray, Green, Red, Yellow };
    public enum StateProgressBar { Static, Moving };

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DatabaseHandler DatabaseHandler = new DatabaseHandler();
        public SshClient client = new SshClient("0", "0", "0");
        public ForwardedPortDynamic port = new ForwardedPortDynamic(1080);
        public System.Timers.Timer ReconnectTimer = new System.Timers.Timer(5000);
        public ConnectionState connectionState = new ConnectionState(); 
        ObservableCollection<SSHConfigEntity> SSHConfigs = new ObservableCollection<SSHConfigEntity>();
        SSHConfigEntity? SelectedConfig = null;

        string host = "";
        int hostPort;
        string username = "";
        string password = "";
        string ipAddress = "";
        uint portNumber;
        long timeout;
        long keepAlive;
        int retries;

        int retriesDone = 0;
        int maxRetries = 0;


        public MainWindow()
        {
            InitializeComponent();

            ListViewConfigs.ItemsSource = SSHConfigs;
            RefreshConfigList();

            InitializeControls();

            ReconnectTimer.Elapsed += ReconnectTimer_Elapsed;
            connectionState.PropertyChanged += ConnectionState_PropertyChanged;
            
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

        private void InitializeControls()
        {
            try
            {


                textBoxIP.Text = SettingsMain.Default.IPAddress;
                textBoxPort.Text = SettingsMain.Default.Port.ToString();
                textBoxTimeout.Text = SettingsMain.Default.Timeout.ToString();
                textBoxKeepAlive.Text = SettingsMain.Default.KeepAlive.ToString();
                textBoxRetries.Text = SettingsMain.Default.Retries.ToString();
                ListViewConfigs.SelectedIndex = SettingsMain.Default.LastSelectedConfigIndex;
                
                SwitchTheme(SettingsMain.Default.Theme);
                SwitchLogsVisibility(SettingsMain.Default.ExpandGridLogs);

            }
            catch (Exception ex)
            {
                LogError(ex.Message);

            }
        }

        public void InitializeClient()
        {
            try
            {
                //Set up the SSH connection
                client = new SshClient(host, hostPort, username, password);

                client.ErrorOccurred += Client_ErrorOccurred;

                if (timeout == 0)
                {
                    client.ConnectionInfo.Timeout = new TimeSpan(1, 0, 0, 0);

                }
                else
                {
                    client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(timeout);

                }

                if (keepAlive == 0)
                {
                    client.KeepAliveInterval = new TimeSpan(0, 0, 1, 0);
                }
                else
                {
                    client.KeepAliveInterval = TimeSpan.FromSeconds(keepAlive);
                }
                
                if (retries == 0)
                {
                    maxRetries = 10;
                }
                else
                {
                    maxRetries = retries;
                }


                client.ConnectionInfo.RetryAttempts = maxRetries;

                port = new ForwardedPortDynamic(ipAddress, portNumber);

            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ChangeState(StateColor.Red, StateProgressBar.Static);
            }
        }

        public void LogError(string message)
        {
            Dispatcher.BeginInvoke(() => { textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + message; });


        }

        public void ChangeState(StateColor stateColor, StateProgressBar progressBarState)
        {
            Dispatcher.BeginInvoke(() =>
            {
                this.TaskbarItemInfo = new System.Windows.Shell.TaskbarItemInfo() { ProgressValue = 100 };
                string StateString = stateColor.ToString();
                this.Icon = new BitmapImage(new Uri("pack://application:,,,/Resources/icon-" + StateString + ".png"));
                //this.TaskbarItemInfo.Overlay = new BitmapImage(new Uri("pack://application:,,,/Resources/icon-" + StateString + ".png"));

                if (progressBarState == StateProgressBar.Static)
                {
                    ProgressBarState.IsIndeterminate = false;
                    TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                }
                else if (progressBarState == StateProgressBar.Moving)
                {
                    ProgressBarState.IsIndeterminate = true;
                    this.TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Indeterminate;
                }
                switch (stateColor)
                {
                    case StateColor.Red:
                        ProgressBarState.Foreground = Brushes.Red;
                        TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Error;
                        break;
                    case StateColor.Yellow:
                        ProgressBarState.Foreground = Brushes.Goldenrod;
                        TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Paused;
                        break;
                    case StateColor.Green:
                        ProgressBarState.Foreground = Brushes.Green;
                        TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.Normal;
                        break;
                    case StateColor.Gray:
                        ProgressBarState.Foreground = Brushes.Transparent;
                        TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                        break;
                    default:
                        ProgressBarState.Foreground = Brushes.White;
                        TaskbarItemInfo.ProgressState = System.Windows.Shell.TaskbarItemProgressState.None;
                        break;
                }
            });
        }

        public void ChangeButtonState(bool IsEnabled)
        {
            Dispatcher.BeginInvoke(() =>
            {
                buttonConnect.IsEnabled = IsEnabled;
            });




        }

        public void ChangeUserConnectionFlag(bool flag)
        {
            Dispatcher.BeginInvoke(() =>
            {
                connectionState.UserConnectionFlag = flag;
            });
        }
        public async void ConnectUser()
        {
            await Task.Run(async () =>
            {
                try
                {
                    ChangeButtonState(false);
                    //Connect to the server
                    await client.ConnectAsync(CancellationToken.None);
                    client.AddForwardedPort(port);
                    port.Start();

                    LogError("Connected!");
                    LogError("SOCKS5 proxy available on " + SettingsMain.Default.IPAddress + ":" + SettingsMain.Default.Port.ToString());
                    ChangeState(StateColor.Green, StateProgressBar.Static);

                    ChangeUserConnectionFlag(true);
                    ChangeButtonState(true);
                }
                catch (Exception ex)
                {
                    ChangeState(StateColor.Red, StateProgressBar.Static);
                    LogError(ex.Message);
                    ChangeButtonState(true);
                }
            });
        }

        public async void DisconnectUser()
        {
            await Task.Run(() =>
            {

                try
                {
                    ChangeButtonState(false);

                    if (ReconnectTimer.Enabled)
                    {
                        ReconnectTimer.Stop();
                    }
                    //Stop and remove the port forwarding
                    port.Stop();
                    client.RemoveForwardedPort(port);
                    //Disconnect from the server
                    client.Disconnect();

                    LogError("Disconnected!");
                    ChangeState(StateColor.Gray, StateProgressBar.Static);

                    ChangeUserConnectionFlag(false);
                    ChangeButtonState(true);




                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                    ChangeState(StateColor.Red, StateProgressBar.Static);
                    ChangeButtonState(true);
                }
            });
        }

        public async void ConnectClient()
        {
            await Task.Run(async () =>
            {
                try
                {
                    //Connect to the server
                    await client.ConnectAsync(CancellationToken.None);
                    client.AddForwardedPort(port);
                    port.Start();

                    LogError("Connected!");
                    LogError("SOCKS5 proxy available on " + SettingsMain.Default.IPAddress + ":" + SettingsMain.Default.Port.ToString());
                    ChangeState(StateColor.Green, StateProgressBar.Static);
                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                }
            });
        }

        public async void DisconnectClient()
        {
            await Task.Run(() =>
            {
                try
                {

                    //Stop and remove the port forwarding
                    port.Stop();
                    client.RemoveForwardedPort(port);
                    //Disconnect from the server
                    client.Disconnect();

                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                }
            });
        }

        #region Events

        private void buttonEditConfigs_Click(object sender, RoutedEventArgs e)
        {
            this.Focusable = false;
            RectangleOverlay.Visibility = Visibility.Visible;
            ConfigWindow configAddWindow = new ConfigWindow();
            configAddWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            configAddWindow.Owner = this;
            configAddWindow.ShowDialog();
            RefreshConfigList();
            RectangleOverlay.Visibility = Visibility.Collapsed;
            this.Focusable = true;

        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!connectionState.UserConnectionFlag)
                {
                    

                    if (textBoxIP.Text == "")
                    {
                        LogError("Please enter a valid IP Address.");
                        return;
                    }

                    if (textBoxPort.Text == "" || !(Int32.TryParse(textBoxPort.Text, out int res_port2)))
                    {
                        LogError("Please enter a valid Port.");
                        return;
                    }

                    if (textBoxTimeout.Text == "" || !(Int64.TryParse(textBoxTimeout.Text, out long res_timeout)))
                    {
                        LogError("Please enter a valid Timeout in seconds.");
                        return;
                    }

                    if (textBoxKeepAlive.Text == "" || !(Int64.TryParse(textBoxKeepAlive.Text, out long res_keepalive)))
                    {
                        LogError("Please enter a valid Keep Alive interval in seconds.");
                        return;
                    }

                    if (textBoxRetries.Text == "" || !(Int32.TryParse(textBoxRetries.Text, out int res_retries)))
                    {
                        LogError("Please enter a valid Number of Retries.");
                        return;
                    }



                    LogError("Connecting...");
                    ChangeState(StateColor.Green, StateProgressBar.Moving);

/*                    host = textBoxHost.Text;
                    hostPort = Convert.ToInt32(textBoxHostPort.Text);
                    username = textBoxUsername.Text;
                    password = passwordBoxPassword.Password;*/
                    ipAddress = textBoxIP.Text;
                    portNumber = Convert.ToUInt32(textBoxPort.Text);
                    timeout = Convert.ToInt64(textBoxTimeout.Text);
                    keepAlive = Convert.ToInt64(textBoxKeepAlive.Text);
                    retries = Convert.ToInt32(textBoxRetries.Text);


                    retriesDone = 0;
                    InitializeClient();

                    ConnectUser();

                }
                else
                {

                    LogError("Disconnecting...");
                    ChangeState(StateColor.Gray, StateProgressBar.Static);

                    DisconnectUser();

                }



            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ChangeState(StateColor.Red, StateProgressBar.Static);
            }

        }

        private void textBoxLogs_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox? textbox = sender as TextBox;
                textbox?.PageDown();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
                ChangeState(StateColor.Red, StateProgressBar.Static);
            }
        }


        private void Client_ErrorOccurred(object? sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {


            LogError(e.Exception.Message);
            ChangeState(StateColor.Yellow, StateProgressBar.Moving);

            LogError("Attempting to reconnect...");
            ReconnectTimer.Start();

        }

        private async void ReconnectTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (connectionState.UserConnectionFlag)
                    {


                        if (retriesDone < maxRetries)
                        {
                            retriesDone++;


                            if (!client.IsConnected)
                            {
                                LogError("Retrying (" + (retriesDone).ToString() + "/" + maxRetries.ToString() + ")");

                                DisconnectClient();

                                ConnectClient();

                            }
                            else
                            {
                                ReconnectTimer.Stop();
                                return;
                            }
                        }
                        else
                        {
                            ReconnectTimer.Stop();
                            DisconnectClient();
                            ChangeUserConnectionFlag(false);
                            ChangeState(StateColor.Red, StateProgressBar.Static);
                            LogError("Maximum number of retries reached!");
                        }
                    }
                    else
                    {
                        ReconnectTimer.Stop();
                        DisconnectClient();
                        ChangeState(StateColor.Gray, StateProgressBar.Static);
                        LogError("Aborted by User!");
                    }

                }
                catch (Exception ex)
                {
                    LogError(ex.Message);
                }
            });
        }

        private void ConnectionState_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (connectionState.UserConnectionFlag)
            {
                buttonConnect.Content = "Disconnect";
            }
            else
            {
                buttonConnect.Content = "Connect";
            }
        }

        #endregion

        #region SavePreferences
        
        private void textBoxIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SettingsMain.Default.IPAddress = textBoxIP.Text;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void textBoxPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBoxPort.Text == "")
                    SettingsMain.Default.Port = 1080;
                else
                    SettingsMain.Default.Port = Convert.ToUInt32(textBoxPort.Text);
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void textBoxKeepAlive_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBoxKeepAlive.Text == "")
                    SettingsMain.Default.KeepAlive = 0;
                else
                    SettingsMain.Default.KeepAlive = Convert.ToInt64(textBoxKeepAlive.Text);
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void textBoxTimeout_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBoxTimeout.Text == "")
                    SettingsMain.Default.Timeout = 0;
                else
                    SettingsMain.Default.Timeout = Convert.ToInt64(textBoxTimeout.Text);
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void textBoxRetries_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBoxRetries.Text == "")
                    SettingsMain.Default.Retries = 100;
                else
                    SettingsMain.Default.Retries = Convert.ToInt32(textBoxRetries.Text);
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void expanderSocks_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsMain.Default.ExpandSocks = true;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void expanderSocks_Collapsed(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsMain.Default.ExpandSocks = false;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void expanderSSH_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsMain.Default.ExpandSSH = true;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void expanderSSH_Collapsed(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsMain.Default.ExpandSSH = false;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void expanderExtras_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsMain.Default.ExpandExtras = true;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void expanderExtras_Collapsed(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsMain.Default.ExpandExtras = false;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void ButtonThemeSwitch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SettingsMain.Default.Theme == "Dark")
                {
                    SettingsMain.Default.Theme = "Light";
                }
                else if (SettingsMain.Default.Theme == "Light")
                {
                    SettingsMain.Default.Theme = "Dark";
                }
                SettingsMain.Default.Save();
                SwitchTheme(SettingsMain.Default.Theme);
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }
        private void ButtonLogSwitch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SettingsMain.Default.ExpandGridLogs)
                {

                    SettingsMain.Default.ExpandGridLogs = false;
                }
                else
                {

                    SettingsMain.Default.ExpandGridLogs = true;
                }

                SettingsMain.Default.Save();

                SwitchLogsVisibility(SettingsMain.Default.ExpandGridLogs);

            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }

        #endregion

        #region TitleBarEvents
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

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Theme
        private void SwitchTheme(string theme)
        {


            if (theme == "Dark")
            {
                App.Current.Resources["BackElement"] = Brushes.Black;
                App.Current.Resources["Back"] = new SolidColorBrush(Color.FromRgb(32, 32, 32));
                App.Current.Resources["Fore"] = Brushes.White;
                App.Current.Resources["Accent"] = new SolidColorBrush(Color.FromRgb(16, 37, 100));
                ButtonThemeSwitch.Content = "Current: Dark";
            }
            else if (theme == "Light")
            {
                App.Current.Resources["BackElement"] = Brushes.White;
                App.Current.Resources["Back"] = new SolidColorBrush(Color.FromRgb(223, 223, 223));
                App.Current.Resources["Fore"] = Brushes.Black;
                App.Current.Resources["Accent"] = new SolidColorBrush(Color.FromRgb(165, 185, 245));
                ButtonThemeSwitch.Content = "Current: Light";
            }
        }

        private void SwitchLogsVisibility(bool visible)
        {
            try
            {
                if (visible)
                {
                    GridBody.ColumnDefinitions[1].Width = new GridLength(400, GridUnitType.Pixel);
                    GridLogs.Visibility = Visibility.Visible;
                    this.Width = this.Width + 400;
                    ButtonLogSwitch.Content = "Current: Visible";
                }
                else
                {
                    GridBody.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Star);
                    GridLogs.Visibility = Visibility.Collapsed;
                    this.Width = this.Width - 400;
                    ButtonLogSwitch.Content = "Current: Collapsed";
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message);
            }
        }


        #endregion

        private void ListViewConfigs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SettingsMain.Default.LastSelectedConfigIndex = ListViewConfigs.SelectedIndex;
            SettingsMain.Default.Save();
            SelectedConfig = ListViewConfigs.SelectedItem as SSHConfigEntity;
            if (SelectedConfig != null)
            {
                username = SelectedConfig.Username;
                host = SelectedConfig.ServerAddress;
                hostPort = Convert.ToInt32(SelectedConfig.ServerPort);
                password = SelectedConfig.Password;
            }
        }
    }
}
