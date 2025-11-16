using CommunicationProtocol.Bases;
using CommunicationProtocol.Modbus.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Modbus.Protocols
{
    public interface IModbusTcp : ITcpProtocol
    {
        Task<T> ReadAsync<T>(byte slave, string regionalAddress) where T : unmanaged;
        Task<ReadOnlyMemory<byte>> ReadAsync(ModbusReadParameter parameter);
        Task<string> ReadStringAsync(byte slave, string regionalAddress, byte length);
        Task<bool> WriteAsync<T>(byte slave, string regionalAddress, T value) where T : unmanaged;
        Task<bool> WriteAsync(ModbusWriteParameter parameter);
        Task<bool> WriteStringAsync(byte slave, string regionalAddress, string value);
    }
}
