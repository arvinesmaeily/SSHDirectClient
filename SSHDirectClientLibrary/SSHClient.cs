using Renci.SshNet;
using System;

namespace SSHDirectClientLibrary
{

    public class SSHClient
    {
        SshClient client;
        ForwardedPortDynamic port;

        public bool IsConnected = false;

        public void Initialize(string host, string username, string password, string ipAddress, uint port_no, long timeout, int retries)
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
            }
            else
            {
                client.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(timeout);
            }
            client.ConnectionInfo.RetryAttempts = retries;
            port = new ForwardedPortDynamic(ipAddress, port_no);

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
