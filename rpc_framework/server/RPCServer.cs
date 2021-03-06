﻿using server.threadpool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using server.io;
using server.memory;

namespace server
{
    public enum eServerMode
    {
        kAsyncSocket,
        kRWThreadPool,
        kBlockingQueueThreadPool
    }

    public class RPCServer
    {
        #region Members
        private bool m_isRunning = false;
        private string m_endPoint = null;
        private ushort m_port = 0;
        private ushort m_maxConnections = 0;

        private Socket m_connectionSock = null;
        private eServerMode m_serverMode;

        private Thread m_serverThread = null; 
        #endregion

        #region Properties
        private AsyncSocket m_IOConnectionWrapper = null;
        private AsyncSocket IOConnectionWrapper
        {
            get
            {
                if (m_IOConnectionWrapper == null && m_connectionSock != null)
                    m_IOConnectionWrapper = new AsyncSocket(m_connectionSock);
                return m_IOConnectionWrapper;
            }
        }
        #endregion

        public RPCServer(eServerMode serverMode)
        {
            this.m_endPoint = server.Properties.Settings.Default.kEndPoint;
            this.m_port = server.Properties.Settings.Default.kPort;
            this.m_maxConnections = server.Properties.Settings.Default.kMaxConnections;

            this.m_connectionSock = this.InitConnectionSock();

            this.m_serverMode = serverMode;
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

        private void RunServer()
        {
            Console.WriteLine(string.Format("Starting RPC Server: PID - {0}. Listening on port: {1}", System.Diagnostics.Process.GetCurrentProcess().Id, this.m_port));

            while (this.m_isRunning)
            {
                lock (this)
                {
                    if (!this.m_isRunning)
                        break;
                }

                switch (this.m_serverMode)
                {
                    case eServerMode.kAsyncSocket:
                        this.IOConnectionWrapper.BeginAccept();
                        break;
                    case eServerMode.kRWThreadPool:
                        RWThreadPool.Instance.AddNewConnection(this.m_connectionSock.Accept());
                        break;
                    case eServerMode.kBlockingQueueThreadPool:
                        BlockingQueueThreadPool.Instance.AddNewConnection(this.m_connectionSock.Accept());
                        break;
                }
            }
        }

        public void Start(bool sendToBackground = true)
        {
            this.m_isRunning = true;

            if (sendToBackground)
            {
                this.m_serverThread = new Thread(new ThreadStart(() =>
                {
                    this.RunServer();
                }));
                this.m_serverThread.Start();
            }
            else
            {
                this.RunServer();
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
