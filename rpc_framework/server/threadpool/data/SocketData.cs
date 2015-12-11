﻿using server.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server.threadpool.data
{
    public class SocketData
    {
        public enum eSocketStatus
        {
            kSuccesfull,
            kNotReady,
            kUnconnected,
            kError
        }

        private int m_bytesReceived = 0;
        private byte[] m_buffer = null;
        private Socket m_socket = null;
        private StringBuilder m_data = null;

        public Socket Connection { get { lock (this) { return m_socket; } } }
        public StringBuilder Data { get { lock (this) { return m_data; } } }

        public bool IsTransferComplete
        {
            get
            {
                lock (this)
                {
                    return (this.m_bytesReceived > 0) &&
                           (this.m_bytesReceived % Settings.Default.kBufferSize) != 0; 
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                lock (this)
                {
                    return this.IsSocketConnected() && this.m_socket.Connected; 
                }
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
            lock (this)
            {
                this.m_bytesReceived = 0;
                this.m_data.Clear(); 
            }
        }

        public eSocketStatus ReceiveData()
        {
            lock (this)
            {
                if (this.m_socket.Available > 0)
                {
                    try
                    {
                        this.m_bytesReceived += this.m_socket.Receive(this.m_buffer);
                        this.AppendData(this.m_buffer);

                        return this.IsTransferComplete ? eSocketStatus.kSuccesfull : eSocketStatus.kNotReady;
                    }
                    catch
                    {
                        return eSocketStatus.kError;
                    }
                }

                return eSocketStatus.kNotReady; 
            }
        }

        public void SendData(byte[] buffer)
        {
            lock (this)
            {
                this.m_socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, null, this.m_socket);
            }
        }

        private bool IsSocketConnected()
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