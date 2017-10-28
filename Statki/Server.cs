using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Statki
{
    class Server
    {
        private Socket _socket;
        private byte[] _buffer;

        public Server()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _buffer = new byte[1024];
        }

        public void Bind(int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public void Listen(int backlog)
        {
            _socket.Listen(backlog);
        }

        public void Accept()
        {
            _socket.BeginAccept(AcceptedCallback, null);
        }

        public void AcceptedCallback(IAsyncResult result)
        {
            Socket client = _socket.EndAccept(result);
            Accept();

            _buffer = new byte[1024];

            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceivedCallback, client);
        }

        public void ReceivedCallback(IAsyncResult result)
        {
            Socket client = result.AsyncState as Socket;

            int bufferSize = client.EndReceive(result);
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

            client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceivedCallback, client);
        }
    }
}
