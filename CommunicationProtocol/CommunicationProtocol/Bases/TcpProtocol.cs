using CommunicationProtocol.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Bases
{
    internal abstract class TcpProtocol
    {
        protected NetworkStream _networkStream = default!;

        protected TcpConnectParameter _tcoConnectParameter;

        public TcpClient TcpClient { get; protected set; }

        protected ushort _receiveTimeout => (ushort)TcpClient.ReceiveTimeout;

        protected TcpProtocol(TcpConnectParameter parameter)
        {
            TcpClient = new TcpClient
            {
                ReceiveTimeout = 5000,
            };
            _tcoConnectParameter = parameter;
        }

        protected abstract Task<ReadOnlyMemory<byte>> SendAndReceiveAsync(ReadOnlyMemory<byte> requestFrame);

        public virtual async Task<bool> ConnectAsync()
        {

            await TcpClient.ConnectAsync(_tcoConnectParameter.Host, _tcoConnectParameter.Port);

            _networkStream = TcpClient.GetStream();

            return true;

        }

        public async ValueTask DisposeAsync()
        {
            if (TcpClient != null)
            {
                try
                {
                    if (_networkStream != null)
                    {
                        await _networkStream.FlushAsync();
                        await _networkStream.DisposeAsync();
                    }

                    TcpClient.Client?.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException)
                {

                }
                finally
                {
                    TcpClient?.Close();
                    TcpClient?.Dispose();
                }
            }
        }
    }
}
