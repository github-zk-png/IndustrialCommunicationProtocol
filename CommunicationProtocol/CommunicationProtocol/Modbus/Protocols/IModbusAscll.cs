using CommunicationProtocol.Bases;
using CommunicationProtocol.Modbus.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Modbus.Protocols
{
    public interface IModbusAscll: ISerialPortProtocol
    {
        public string ReadString(byte slave, string regionalAddress, byte length);

        public bool WriteString(byte slave, string regionalAddress, string value);

        public T Read<T>(byte slave, string regionalAddress) where T : unmanaged;

        public bool Write<T>(byte slave, string regionalAddress, T value) where T : unmanaged;

        public ReadOnlyMemory<byte> Read(ModbusReadParameter parameter);

        public bool Write(ModbusWriteParameter parameter);
    }
}
