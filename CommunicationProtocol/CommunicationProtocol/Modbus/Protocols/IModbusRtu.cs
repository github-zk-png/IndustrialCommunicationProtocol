using CommunicationProtocol.Bases;
using CommunicationProtocol.Extensions;
using CommunicationProtocol.Modbus.Extensions;
using CommunicationProtocol.Modbus.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Modbus.Protocols
{
    public interface IModbusRtu : ISerialPortProtocol
    {

        string ReadString(byte slave, string regionalAddress, byte length);

        bool WriteString(byte slave, string regionalAddress, string value);

        T Read<T>(byte slave, string regionalAddress) where T : unmanaged;

        bool Write<T>(byte slave, string regionalAddress, T value) where T : unmanaged;

        ReadOnlyMemory<byte> Read(ModbusReadParameter parameter);

        bool Write(ModbusWriteParameter parameter);

    }
}
