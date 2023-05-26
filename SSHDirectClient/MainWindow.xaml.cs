using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Renci.SshNet;


namespace SSHDirectClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public SshClient client = new SshClient("0", "0", "0");
        public ForwardedPortDynamic? port;

        public MainWindow()
        {
            InitializeComponent();
            InitializeControls();
            
        }

        private void InitializeControls()
        {
            try
            {
                textBoxHost.Text = SettingsMain.Default.Host;
                textBoxHostPort.Text = SettingsMain.Default.HostPort.ToString();
                textBoxUsername.Text = SettingsMain.Default.Username;
                passwordBoxPassword.Password = SettingsMain.Default.Password;

                textBoxIP.Text = SettingsMain.Default.IPAddress;
                textBoxPort.Text = SettingsMain.Default.Port.ToString();
                textBoxTimeout.Text = SettingsMain.Default.Timeout.ToString();
                textBoxKeepAlive.Text = SettingsMain.Default.KeepAlive.ToString();
                textBoxRetries.Text = SettingsMain.Default.Retries.ToString();

                expanderSocks.IsExpanded = SettingsMain.Default.ExpandSocks;
                expanderSSH.IsExpanded = SettingsMain.Default.ExpandSSH;
                expanderLogs.IsExpanded = SettingsMain.Default.ExpandLogs;
                expanderExtras.IsExpanded = SettingsMain.Default.ExpandExtras;
                SwitchTheme(SettingsMain.Default.Theme);

            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }

        public void InitializeClient(string host, int hostPort, string username, string password, string ipAddress, uint portNumber, long timeout, long keepAlive, int retries)
        {
            try
            {
                //Set up the SSH connection
                client = new SshClient(host, hostPort, username, password);

                if (timeout == 0)
                {
                    client.ConnectionInfo.Timeout = new TimeSpan(1, 0, 0, 0);
                    client.KeepAliveInterval = new TimeSpan(0, 0, 1, 0);
                }
                else
                {
                    client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(timeout);
                    client.KeepAliveInterval = TimeSpan.FromSeconds(keepAlive);
                }

                client.ConnectionInfo.RetryAttempts = retries;

                port = new ForwardedPortDynamic(ipAddress, portNumber);

            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }

        public void Connect()
        {
            try
            {
                if (!client.IsConnected)
                {
                    //Connect to the server
                    client.Connect();
                    client.AddForwardedPort(port);
                    port.Start();
                    buttonConnect.Content = "Disconnect";
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Connected!";
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "SOCKS5 proxy available on " + SettingsMain.Default.IPAddress + ":" + SettingsMain.Default.Port.ToString();
                }
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }

        public void Disconnect()
        {
            try
            {
                if (client.IsConnected)
                {
                    //Stop and remove the port forwarding
                    port.Stop();
                    client.RemoveForwardedPort(port);
                    //Disconnect from the server
                    client.Disconnect();
                    buttonConnect.Content = "Connect";
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Disconnected!";
                }
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;


            }

        }

        #region Events
        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (client.IsConnected == false)
                {

                    if (textBoxHost.Text == "")
                    {
                        textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Host Address.";
                        return;
                    }

                    if (textBoxHostPort.Text == "" || !(Int32.TryParse(textBoxHostPort.Text, out int res_port)))
                    {
                        textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Host Port.";
                        return;
                    }

                    if (textBoxUsername.Text == "")
                    {
                        textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Username.";
                        return;
                    }

                    if (passwordBoxPassword.Password == "")
                    {
                        textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Password.";
                        return;
                    }

                    if (textBoxIP.Text == "")
                    {
                        textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid IP Address.";
                        return;
                    }

                    if (textBoxPort.Text == "" || !(Int32.TryParse(textBoxPort.Text, out int res_port2)))
                    {
                        textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Port.";
                        return;
                    }

                    if (textBoxTimeout.Text == "" || !(Int64.TryParse(textBoxTimeout.Text, out long res_timeout)))
                    {
                        textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Timeout in seconds.";
                        return;
                    }

                    if (textBoxKeepAlive.Text == "" || !(Int64.TryParse(textBoxKeepAlive.Text, out long res_keepalive)))
                    {
                        textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Keep Alive interval in seconds.";
                        return;
                    }

                    if (textBoxRetries.Text == "" || !(Int32.TryParse(textBoxRetries.Text, out int res_retries)))
                    {
                        textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Number of Retries.";
                        return;
                    }



                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Connecting...";

                    InitializeClient(textBoxHost.Text, Convert.ToInt32(textBoxHostPort.Text), textBoxUsername.Text, passwordBoxPassword.Password, textBoxIP.Text, Convert.ToUInt32(textBoxPort.Text), Convert.ToInt64(textBoxTimeout.Text), Convert.ToInt64(textBoxKeepAlive.Text), Convert.ToInt32(textBoxRetries.Text));
                    Connect();

                }
                else
                {
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Disconnecting...";

                    Disconnect();
                }
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }

        }
        private void textBoxLogs_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var textbox = sender as TextBox;
                textbox.PageDown();
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }

        #endregion

        #region SavePreferences
        private void PasswordBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsMain.Default.Password = passwordBoxPassword.Password;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }
        private void TextBoxUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SettingsMain.Default.Username = textBoxUsername.Text;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }
        private void TextBoxHost_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SettingsMain.Default.Host = textBoxHost.Text;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }
        private void textBoxHostPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (textBoxPort.Text == "")
                    SettingsMain.Default.HostPort = 22;
                else
                    SettingsMain.Default.HostPort = Convert.ToUInt32(textBoxHostPort.Text);
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }
        private void textBoxIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SettingsMain.Default.IPAddress = textBoxIP.Text;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }
        private void expanderLogs_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsMain.Default.ExpandLogs = true;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }
        private void expanderLogs_Collapsed(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingsMain.Default.ExpandLogs = false;
                SettingsMain.Default.Save();
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
            }
        }
        private void ButtonThemeSwitch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(SettingsMain.Default.Theme == "Dark")
                {
                    SettingsMain.Default.Theme = "Light";
                }
                else if(SettingsMain.Default.Theme == "Light")
                {
                    SettingsMain.Default.Theme = "Dark";
                }
                SettingsMain.Default.Save();
                SwitchTheme(SettingsMain.Default.Theme);
            }
            catch (Exception ex)
            {
                textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;
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
            if(this.WindowState == WindowState.Maximized)
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
            var back = this.Resources["Back"];
            var fore = this.Resources["Fore"];

            if (theme == "Dark")
            {
                this.Resources["BackElement"] = Brushes.Black;
                this.Resources["Back"] = new SolidColorBrush(Color.FromRgb(32, 32, 32));
                this.Resources["Fore"] = Brushes.White;
                ButtonThemeSwitch.Content = "Current: Dark";
            }
            else if (theme == "Light")
            {
                this.Resources["BackElement"] = Brushes.White;
                this.Resources["Back"] = new SolidColorBrush(Color.FromRgb(223, 223, 223));
                this.Resources["Fore"] = Brushes.Black;
                ButtonThemeSwitch.Content = "Current: Light";
            }
        }
        #endregion
    }
}
