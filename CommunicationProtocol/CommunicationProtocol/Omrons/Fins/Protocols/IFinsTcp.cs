using CommunicationProtocol.Bases;
using CommunicationProtocol.Omrons.Fins.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Omrons.Fins.Protocols
{
    public interface IFinsTcp : ITcpProtocol
    {
        Task<ReadOnlyMemory<byte>> ReadAsync(FinsReadParameter parameter);
        Task<IReadOnlyList<FinsReturnTypeData>> ReadAsync(IReadOnlyList<FinsManyReadParameter> parameters);
        Task<T> ReadAsync<T>(string regionalAddress) where T : unmanaged;
        Task<string> ReadStringAsync(string regionalAddress, byte length);
        Task<bool> WriteAsync(FinsWriteParameter parameter);
        Task<bool> WriteAsync<T>(string regionalAddress, T value) where T : unmanaged;
        Task<bool> WriteStringAsync(string regionalAddress, string value);
    }
}
