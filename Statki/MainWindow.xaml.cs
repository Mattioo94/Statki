using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Statki
{
    public partial class MainWindow : Window
    {
        private NetworkStream _streamOfClient;
        private NetworkStream _streamOfServer;

        private TcpClient client;

        public MainWindow()
        {
            InitializeComponent();
            client = new TcpClient();

            var tcpServerRunThread = new Thread(new ThreadStart(ServerRun))
            {
                IsBackground = true
            };
            tcpServerRunThread.Start();
        }

        #region Server
        public void ServerRun()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 8001);
            tcpListener.Start();

            info.Dispatcher.Invoke(() =>
            {
                info.Content = $"{GetLocalIPAddress()}";
            });

            while (true)
            {
                TcpClient client = tcpListener.AcceptTcpClient();
                var tcpHandlerThread = new Thread(new ParameterizedThreadStart(TcpHandler))
                {
                    IsBackground = true
                };
                tcpHandlerThread.Start(client);
            }
        }

        public void TcpHandler(object client)
        {
            TcpClient mClient = (TcpClient)client;
            _streamOfServer = mClient.GetStream();

            Polaczenie.Dispatcher.Invoke(() =>
            {
                Polaczenie.Visibility = Visibility.Hidden;
            });

            Menu.Dispatcher.Invoke(() =>
            {
                Menu.Visibility = Visibility.Visible;
            });

            while (true)
            {
                if (_streamOfServer.DataAvailable)
                {
                    byte[] bytes = new byte[1024];
                    int count = _streamOfServer.Read(bytes, 0, bytes.Length);

                    StringBuilder str = new StringBuilder();
                    for (int i = 0; i < count; i++)
                    {
                        str.Append(Convert.ToChar(bytes[i]));
                    }

                    if(!string.IsNullOrEmpty(str.ToString()))
                    {
                        MessageBox.Show(str.ToString());
                    }
                }
            }
        }
        #endregion

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void ClientRun(object ip)
        {
            if(client.Connected)
            {
                _streamOfClient.Close();
                client.Close();

                client = new TcpClient();
            }

            client.Connect(IPAddress.Parse((string)ip), 8001);
            _streamOfClient = client.GetStream();

            var clientListenThread = new Thread(new ThreadStart(ClientReceive))
            {
                IsBackground = true
            };
            clientListenThread.Start();

            Polaczenie.Dispatcher.Invoke(() =>
            {
                Polaczenie.Visibility = Visibility.Hidden;
            });

            Menu.Dispatcher.Invoke(() =>
            {
                Menu.Visibility = Visibility.Visible;
            });
        }

        public void ClientReceive()
        {
            while (true)
            {
                if (_streamOfClient.DataAvailable)
                {
                    byte[] bytes = new byte[1024];
                    int count = _streamOfClient.Read(bytes, 0, bytes.Length);

                    StringBuilder str = new StringBuilder();
                    for (int i = 0; i < count; i++)
                    {
                        str.Append(Convert.ToChar(bytes[i]));
                    }

                    if (!string.IsNullOrEmpty(str.ToString()))
                    {
                        MessageBox.Show(str.ToString());
                    }
                }
            }
        }

        private void Adres_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox t = sender as TextBox;

                string serwer = string.Empty;
                t.Dispatcher.Invoke(() =>
                {
                    serwer = t.Text;
                });

                var tcpClientRunThread = new Thread(new ParameterizedThreadStart(ClientRun))
                {
                    IsBackground = true
                };
                tcpClientRunThread.Start(serwer);
            }
        }

        private void komunikat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox t = sender as TextBox;

                string tekst = string.Empty;
                t.Dispatcher.Invoke(() =>
                {
                    tekst = t.Text;
                });

                byte[] bytes = Encoding.ASCII.GetBytes(tekst);

                if (client.Connected)
                {                 
                    _streamOfClient.Write(bytes, 0, bytes.Length);
                }
                else
                {
                    _streamOfServer.Write(bytes, 0, bytes.Length);
                }
            }
        }
    }
}