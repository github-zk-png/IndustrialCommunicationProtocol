using CommunicationProtocol.Bases;
using CommunicationProtocol.Mitsubishis.Mc3es.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Protocols
{
    public interface IMc3e : ITcpProtocol
    {
        Task<ReadOnlyMemory<byte>> ReadAsync(Mc3eReadParameter parameter);
        Task<T> ReadAsync<T>(string regionalAddress) where T : unmanaged;
        Task<string> ReadStringAsync(string regionalAddress, byte length);
        Task<bool> WriteAsync(Mc3eWriteParameter parameter);
        Task<bool> WriteAsync<T>(string regionalAddress, T value) where T : unmanaged;
        Task<bool> WriteStringAsync(string regionalAddress, string value);
    }
}
