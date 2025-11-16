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

        protected TcpClient _tcpClient;

        public TcpClient TcpClient => _tcpClient;

        protected ushort _receiveTimeout => (ushort)_tcpClient.ReceiveTimeout;

        protected TcpProtocol(TcpConnectParameter parameter)
        {
            _tcpClient = new TcpClient
            {
                ReceiveTimeout = 5000,
            };
            _tcoConnectParameter = parameter;
        }

        protected abstract Task<ReadOnlyMemory<byte>> SendAndReceiveAsync(ReadOnlyMemory<byte> requestFrame);

        public virtual async Task<bool> ConnectAsync()
        {

            await _tcpClient.ConnectAsync(_tcoConnectParameter.Host, _tcoConnectParameter.Port);

            _networkStream = _tcpClient.GetStream();

            return true;

        }

        public async ValueTask DisposeAsync()
        {
            if (_tcpClient != null)
            {
                try
                {
                    if (_networkStream != null)
                    {
                        await _networkStream.FlushAsync();
                        await _networkStream.DisposeAsync();
                    }

                    _tcpClient.Client?.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException)
                {

                }
                finally
                {
                    _tcpClient?.Close();
                    _tcpClient?.Dispose();
                }
            }
        }
    }
}
