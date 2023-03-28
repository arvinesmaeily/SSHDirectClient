using Renci.SshNet;
using System;

namespace SSHDirectClientLibrary
{

    public class SSHClient
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public SshClient client;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        ForwardedPortDynamic port;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public bool IsConnected = false;

        public void Initialize(string host, string username, string password, string ipAddress, uint portNumber, long timeout, long keepAlive, int retries)
        {

            //Connection information
            string Username = username;
            string Password = password;
            string Host = host;

            //Set up the SSH connection
            client = new SshClient(Host, Username, Password);

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

            client.ErrorOccurred += Client_ErrorOccurred;
        }

        private void Client_ErrorOccurred(object? sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
            Disconnect();
            Connect();
        }

        public void Connect()
        {

            //Connect to the server
            client.Connect();
            client.AddForwardedPort(port);
            port.Start();

            IsConnected = true;
        }

        public void Disconnect()
        {

            //Stop and remove the port forwarding
            port.Stop();
            client.RemoveForwardedPort(port);

            //Disconnect from the server
            client.Disconnect();

            IsConnected = false;
        }
    }
} 
