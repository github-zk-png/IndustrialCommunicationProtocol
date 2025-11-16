using CommunicationProtocol.OPC.OpcUas.Models;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommunicationProtocol.OPC.OpcUas.Protocols
{
    public interface IOpcUa : IAsyncDisposable
    {
        ISession Session { get; }

        event Action<string, object> Notification;

        Task<bool> CertificateConnect(OpcUaConnectParameter parameter);
        Task<bool> AnonymousConnectAsync(string url);
        Task<bool> UserConnect(OpcUaConnectParameter parameter);
        IAsyncEnumerable<OpcUaReturnData> ReadAsync(IReadOnlyList<OpcUaReadParameter> parameters);
        Task<bool> SetMonitorNode(IReadOnlyList<string> parameters);
        Task<bool> WriteAsync(IReadOnlyList<OpcUaWriteParameter> parameters);
        Task<T> ReadAsync<T>(string name);
        Task<bool> WriteAsync(string name, object value);
    }
}
