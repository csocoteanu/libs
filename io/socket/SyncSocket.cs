using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using server.io.data;
using System.Net.Sockets;

namespace server.io
{
    public class SyncSocket : SocketData
    {
        public SyncSocket(int bufferSize, Socket sock) : base(bufferSize, sock) { }

        public bool IsTransferComplete
        {
            get
            {
                lock (this)
                {
                    return (this.m_bytesReceived > 0) &&
                           (this.m_bytesReceived % m_bufferSize) != 0;
                }
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
    }
}
