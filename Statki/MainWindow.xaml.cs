using System;
using System.IO;
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
        public MainWindow()
        {
            InitializeComponent();
            client.Content = $"{GetLocalIPAddress()}:8001";

            var s = new Thread(Sluchaj)
            {
                IsBackground = true
            };
            s.Start();
        }

        public void Sluchaj()
        {
            try
            {
                IPAddress ipAd = IPAddress.Parse(GetLocalIPAddress());
                TcpListener myList = new TcpListener(ipAd, 8001);
               
                myList.Start();

                MessageBox.Show($"The local End point is  : {myList.LocalEndpoint}{Environment.NewLine}Oczekiwanie na polaczenie..");

                Socket s = myList.AcceptSocket();
                MessageBox.Show($"Connection accepted from {s.RemoteEndPoint}");

                byte[] b = new byte[100];
                int k = s.Receive(b);

                StringBuilder str = new StringBuilder();
                for (int i = 0; i < k; i++)
                    str.Append(Convert.ToChar(b[i]));

                MessageBox.Show($"[K] Odebrano '{str.ToString()}'");

                ASCIIEncoding asen = new ASCIIEncoding();
                s.Send(asen.GetBytes("Odpowiedz"));

                s.Close();
                myList.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error..... " + e.StackTrace);
            }
        }

        public void Polacz(string serwer)
        {
            try
            {
                TcpClient tcpclnt = new TcpClient();
                tcpclnt.Connect(serwer, 8001);

                Stream stm = tcpclnt.GetStream();

                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes("Komunikat");

                stm.Write(ba, 0, ba.Length);

                byte[] bb = new byte[100];
                int k = stm.Read(bb, 0, 100);

                StringBuilder str = new StringBuilder();
                for (int i = 0; i < k; i++)
                    str.Append(Convert.ToChar(bb[i]));

                MessageBox.Show($"[S] Odebrano '{str.ToString()}'");

                tcpclnt.Close();
            }

            catch (Exception e)
            {
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
            TextBox t = sender as TextBox;

            if (e.Key == Key.Enter)
            {
                string serwer = string.Empty;
                t.Dispatcher.Invoke(() =>
                {
                    serwer = t.Text;
                });

                Thread k = new Thread(() => Polacz(serwer));
                k.Start();
                k.Join();
            }
        }
    }
}