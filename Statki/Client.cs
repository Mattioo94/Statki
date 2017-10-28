using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Statki
{
    class Client
    {
        private Socket _socket;
        private byte[] _buffer;

        public Client()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ip, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), ConnectCallback, null);
        }

        public void ConnectCallback(IAsyncResult result)
        {
            if(_socket.Connected)
            {
                MessageBox.Show("Connected to server");

                _buffer = new byte[1024];
                _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceivedCallback, null);
            }
            else MessageBox.Show("Could not connect");
        }

        public void ReceivedCallback(IAsyncResult result)
        {
            int bufferSize = _socket.EndReceive(result);
            byte[] packet = new byte[bufferSize];

            Array.Copy(_buffer, packet, packet.Length);

            ///////////////////////////////////////////
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < packet.Length; i++)
            {
                str.Append(Convert.ToChar(packet[i]));
            }
            MessageBox.Show(str.ToString());
            ///////////////////////////////////////////

            _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceivedCallback, null);
        }
    }
}
