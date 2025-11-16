using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Bases
{
    public interface ITcpProtocol:IAsyncDisposable
    {
        public TcpClient TcpClient { get; }

        public Task<bool> ConnectAsync();
    }
}
