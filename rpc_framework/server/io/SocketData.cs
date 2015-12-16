using server.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.io.data
{
    public enum eSocketStatus
    {
        kSuccesfull,
        kNotReady,
        kUnconnected,
        kError
    }

    public class SocketData
    {
        protected int m_bytesReceived = 0;
        protected byte[] m_buffer = null;
        protected Socket m_socket = null;
        protected StringBuilder m_data = null;

        public Socket Connection { get { return m_socket; } }
        public StringBuilder Data { get { return m_data; } }

        public bool IsConnected
        {
            get
            {
                return this.IsSocketConnected() && this.m_socket.Connected;
            }
        }

        public SocketData(Socket socket)
        {
            this.m_socket = socket;
            this.m_buffer = new byte[Settings.Default.kBufferSize];
            this.m_data = new StringBuilder();
        }

        public void AppendData(byte[] byteData)
        {
            string data = System.Text.Encoding.UTF8.GetString(byteData).TrimEnd('\0');
            this.m_data.Append(data);
        }

        public void ClearData()
        {
            this.m_bytesReceived = 0;
            this.m_data.Clear(); 
        }

        public virtual void SendData(byte[] buffer)
        {
            this.m_socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, this.m_socket);
        }

        protected bool IsSocketConnected()
        {
            lock (this)
            {
                bool part1 = (this.m_socket.Poll(1000, SelectMode.SelectRead));
                bool part2 = (this.m_socket.Available == 0);

                if (part1 && part2)
                    return false;
                else
                    return true; 
            }
        }
    }
}
