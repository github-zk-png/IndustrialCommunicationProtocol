using CommunicationProtocol.Bases;
using CommunicationProtocol.Omrons.OmronCips.Enums;
using CommunicationProtocol.Omrons.OmronCips.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Omrons.OmronCips.Protocols
{
    public interface IOmronCip : ITcpProtocol
    {
        Task<ReadOnlyMemory<byte>> ReadAsync(OmronCipReadParameter parameter);
        Task<T> ReadAsync<T>(string Name) where T : unmanaged;
        Task<string> ReadStringAsync(string Name);
        Task<bool> WriteAsync(OmronCipWriteParameter parameter);
        Task<bool> WriteAsync<T>(string Name, OmronCipDataType dataType, T value) where T : unmanaged;
        Task<bool> WriteStringAsync(string Name, string value);
    }
}
