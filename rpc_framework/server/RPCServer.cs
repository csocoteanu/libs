﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    public class RPCServer
    {
        private bool m_isRunning = false;
        private string m_endPoint = null;
        private ushort m_port = 0;
        private ushort m_maxConnections = 0;

        private Socket m_connectionSock = null;
        private Thread m_serverThread = null;

        public RPCServer()
        {
            this.m_endPoint = server.Properties.Settings.Default.kEndPoint;
            this.m_port = server.Properties.Settings.Default.kPort;
            this.m_maxConnections = server.Properties.Settings.Default.kMaxConnections;

            this.m_connectionSock = this.InitConnectionSock();
        }

        private Socket InitConnectionSock()
        {
            IPAddress ipAddress = null;
            IPEndPoint configuredEndPoint = null;
            Socket sock = null;

            if (IPAddress.TryParse(this.m_endPoint, out ipAddress))
            {
                // Establish the local endpoint for the socket.
                configuredEndPoint = new IPEndPoint(ipAddress, this.m_port);

                // Create a TCP/IP socket.
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.
                sock.Bind(configuredEndPoint);
                sock.Listen(this.m_maxConnections);
            }

            return sock;
        }

        private void StartServer()
        {
            Thread.CurrentThread.Name = Utils.kDispatcherName;
            Console.WriteLine(string.Format("Starting RPC Server. Listening on port: {0}", this.m_port));

            while (this.m_isRunning)
            {
                lock (this)
                {
                    if (!this.m_isRunning)
                        break;
                }

                Socket newConnection = this.m_connectionSock.Accept();
                ThreadPool.Instance.AddTask(newConnection);
            }
        }

        public void Start(bool sendToBackground = true)
        {
            this.m_isRunning = true;
            if (sendToBackground)
            {
                this.m_serverThread = new Thread(new ThreadStart(() =>
                {
                    this.StartServer();
                }));
                this.m_serverThread.Start();
            }
            else
            {
                this.StartServer();
            }
        }

        public void Stop()
        {
            lock (this)
            {
                this.m_isRunning = false;
            }

            if (this.m_serverThread != null && this.m_serverThread.IsAlive)
            {
                this.m_serverThread.Abort();
            }
        }
    }
}