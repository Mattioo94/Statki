using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Statki
{
    public partial class MainWindow : Window
    {
        private TcpClient klient;
        private TcpListener serwer;

        private Socket socket;
        private Stream stream;

        public MainWindow()
        {
            InitializeComponent();

            /* KONFIGURACJA KLIENTA */
            klient = new TcpClient
            {
                ReceiveTimeout = 200,
                SendTimeout = 200
            };

            /* KONFIGURACJA SERWERA */
            IPAddress ipAd = IPAddress.Parse(GetLocalIPAddress());
            serwer = new TcpListener(ipAd, 8001);

            info.Content = serwer.LocalEndpoint;

            /* START W TLE SEWERA */
            var s = new Thread(Sluchaj)
            {
                IsBackground = true
            };
            s.Start();

            /* DZIAŁANIA PRZED ZAMKNIĘCIEM APLIKACJI */
            Closing += OnClosing;
        }

        public void OnClosing(object o, EventArgs e)
        {
            if(socket?.Connected ?? false)
            {
                socket.Close();
            }

            serwer?.Stop();

            if(klient?.Connected ?? false)
            {
                klient.Close();
            }
        }

        public void Sluchaj()
        {
            try
            {
                serwer.Start();
                socket = serwer.AcceptSocket();
                MessageBox.Show($"Connection accepted from {socket.RemoteEndPoint}");

                Polaczenie.Dispatcher.Invoke(() =>
                {
                    Polaczenie.Visibility = Visibility.Hidden;
                });

                //byte[] b = new byte[100];
                //int k = s.Receive(b);

                //StringBuilder str = new StringBuilder();
                //for (int i = 0; i < k; i++)
                //    str.Append(Convert.ToChar(b[i]));

                //MessageBox.Show($"[K] Odebrano '{str.ToString()}'");

                //ASCIIEncoding asen = new ASCIIEncoding();
                //s.Send(asen.GetBytes("Odpowiedz"));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public void Polacz(string ip, out bool connected)
        {
            try
            {
                klient.Connect(ip, 8001);
                stream = klient.GetStream();

                serwer?.Stop();

                //ASCIIEncoding asen = new ASCIIEncoding();
                //byte[] ba = asen.GetBytes("Komunikat");

                //stm.Write(ba, 0, ba.Length);

                //byte[] bb = new byte[100];
                //int k = stm.Read(bb, 0, 100);

                //StringBuilder str = new StringBuilder();
                //for (int i = 0; i < k; i++)
                //    str.Append(Convert.ToChar(bb[i]));

                //MessageBox.Show($"[S] Odebrano '{str.ToString()}'");
                connected = true;
            }

            catch (Exception e)
            {
                connected = false;
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

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

        private void server_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox t = sender as TextBox;

                string serwer = string.Empty;
                t.Dispatcher.Invoke(() =>
                {
                    serwer = t.Text;
                });

                bool connected = false;
                Thread k = new Thread(() => Polacz(serwer, out connected));
                k.Start();
                k.Join();

                if(connected)
                {
                    Polaczenie.Dispatcher.Invoke(() =>
                    {
                        Polaczenie.Visibility = Visibility.Hidden;
                    });
                }
            }
        }
    }
}