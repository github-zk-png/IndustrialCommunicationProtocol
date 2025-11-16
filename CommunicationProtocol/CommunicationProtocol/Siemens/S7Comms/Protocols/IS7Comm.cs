using CommunicationProtocol.Bases;
using CommunicationProtocol.Siemens.S7Comms.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Siemens.S7Comms.Protocols
{
    public interface IS7Comm : ITcpProtocol
    {
        Task<IReadOnlyList<S7CommReturnByteData>> ReadAsync(IReadOnlyList<S7CommReadParameter> parameters);
        Task<T> ReadAsync<T>(string regionalAddress) where T : unmanaged;
        IAsyncEnumerable<S7CommReturnTypeData> ReadAsync(IReadOnlyList<S7CommTypeReadParameter> parameters);
        Task<string> ReadStringAsync(string regionalAddress, byte count);
        Task<bool> WriteAsync(IReadOnlyList<S7CommWriteParameter> parameters);
        Task<bool> WriteAsync<T>(string regionalAddress, T data) where T : unmanaged;
        Task<bool> WriteAsync(IReadOnlyList<S7CommTypeWriteParameter> parameters);
        Task<bool> WriteStringAsync(string regionalAddress, string data);
    }
}
