﻿using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SSHDirectClientLibrary;

namespace SSHDirectClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SSHClient SSHClient = new SSHClient();
        public MainWindow()
        {
            InitializeComponent();

            InitializeControls();

        }

        private void InitializeControls()
        {
            textBoxHost.Text = SettingsMain.Default.Host;
            textBoxUsername.Text = SettingsMain.Default.Username;
            passwordBoxPassword.Password = SettingsMain.Default.Password;

            textBoxIP.Text = SettingsMain.Default.IPAddress;
            textBoxPort.Text = SettingsMain.Default.Port.ToString();
            textBoxTimeout.Text = SettingsMain.Default.Timeout.ToString();
            textBoxRetries.Text = SettingsMain.Default.Retries.ToString();

            expanderSocks.IsExpanded = SettingsMain.Default.ExpandSocks;
            expanderSSH.IsExpanded = SettingsMain.Default.ExpandSSH;
            expanderLogs.IsExpanded = SettingsMain.Default.ExpandLogs;
        }


        private void PasswordBoxPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {

            SettingsMain.Default.Password = passwordBoxPassword.Password;
            SettingsMain.Default.Save();
        }

        private void TextBoxUsername_TextChanged(object sender, TextChangedEventArgs e)
        {

            SettingsMain.Default.Username = textBoxUsername.Text;
            SettingsMain.Default.Save();
        }

        private void TextBoxHost_TextChanged(object sender, TextChangedEventArgs e)
        {

            SettingsMain.Default.Host = textBoxHost.Text;
            SettingsMain.Default.Save();
        }

        private void textBoxIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            SettingsMain.Default.IPAddress = textBoxIP.Text;
            SettingsMain.Default.Save();
        }

        private void textBoxPort_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxPort.Text == "")
                SettingsMain.Default.Port = 1080;
            else
                SettingsMain.Default.Port = Convert.ToUInt32(textBoxPort.Text);
            SettingsMain.Default.Save();
        }

        private void textBoxTimeout_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxTimeout.Text == "")
                SettingsMain.Default.Timeout = 0;
            else
                SettingsMain.Default.Timeout = Convert.ToInt64(textBoxTimeout.Text);
            SettingsMain.Default.Save();
        }

        private void textBoxRetries_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxRetries.Text == "")
                SettingsMain.Default.Retries = 100;
            else
                SettingsMain.Default.Retries = Convert.ToInt32(textBoxRetries.Text);
            SettingsMain.Default.Save();
        }


        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (SSHClient.IsConnected == false)
            {
                //SettingsMain.Default.Save();

                if (textBoxHost.Text == "")
                {
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Host Address.";
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

                if (textBoxPort.Text == "" || !(Int32.TryParse(textBoxPort.Text, out int res_port)))
                {
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Port.";
                    return;
                }

                if (textBoxTimeout.Text == "" || !(Int64.TryParse(textBoxTimeout.Text, out long res_timeout)))
                {
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Timeout in milliseconds.";
                    return;
                }

                if (textBoxRetries.Text == "" || !(Int32.TryParse(textBoxRetries.Text, out int res_retries)))
                {
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Please enter a valid Number of Retries.";
                    return;
                }

                try
                {


                    SSHClient.Initialize(textBoxHost.Text, textBoxUsername.Text, passwordBoxPassword.Password, textBoxIP.Text, Convert.ToUInt32(textBoxPort.Text), Convert.ToInt64(textBoxTimeout.Text), Convert.ToInt32(textBoxRetries.Text));
                    SSHClient.Connect();

                    buttonConnect.Content = "Disconnect";
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Connected!";
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "SOCKS5 proxy available on " + SettingsMain.Default.IPAddress + ":" + SettingsMain.Default.Port.ToString();
                }
                catch (Exception ex)
                {
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;

                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Data;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.HelpLink;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.HResult;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.InnerException;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Source;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.StackTrace;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.TargetSite;

                }
            }
            else
            {
                try
                {
                    SSHClient.Disconnect();

                    buttonConnect.Content = "Connect";
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + "Disconnected!";
                }
                catch (Exception ex)
                {
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Message;

                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Data;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.HelpLink;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.HResult;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.InnerException;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.Source;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.StackTrace;
                    textBoxLogs.Text += "\n[" + DateTime.Now.ToString() + "] " + ex.TargetSite;

                }
            }

        }


        private void expanderSocks_Expanded(object sender, RoutedEventArgs e)
        {
            SettingsMain.Default.ExpandSocks = true;
            SettingsMain.Default.Save();
        }

        private void expanderSocks_Collapsed(object sender, RoutedEventArgs e)
        {
            SettingsMain.Default.ExpandSocks = false;
            SettingsMain.Default.Save();
        }

        private void expanderSSH_Expanded(object sender, RoutedEventArgs e)
        {
            SettingsMain.Default.ExpandSSH = true;
            SettingsMain.Default.Save();
        }

        private void expanderSSH_Collapsed(object sender, RoutedEventArgs e)
        {
            SettingsMain.Default.ExpandSSH = false;
            SettingsMain.Default.Save();
        }

        private void expanderLogs_Expanded(object sender, RoutedEventArgs e)
        {
            SettingsMain.Default.ExpandLogs = true;
            SettingsMain.Default.Save();
        }

        private void expanderLogs_Collapsed(object sender, RoutedEventArgs e)
        {
            SettingsMain.Default.ExpandLogs = false;
            SettingsMain.Default.Save();
        }
    }
}