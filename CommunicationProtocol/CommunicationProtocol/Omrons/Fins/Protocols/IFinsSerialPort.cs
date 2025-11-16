using CommunicationProtocol.Bases;
using CommunicationProtocol.Omrons.Fins.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Protocols
{
    public interface IFinsSerialPort : ISerialPortProtocol
    {
        ReadOnlyMemory<byte> Read(byte unitNumber, FinsReadParameter parameter);
        IReadOnlyList<FinsReturnTypeData> Read(byte unitNumber, IReadOnlyList<FinsManyReadParameter> parameters);
        T Read<T>(byte unitNumber, string regionalAddress) where T : unmanaged;
        string ReadString(byte unitNumber, string regionalAddress, byte count);
        bool Write(byte unitNumber, FinsWriteParameter parameter);
        bool Write<T>(byte unitNumber, string regionalAddress, T value) where T : unmanaged;
        bool WriteString(byte unitNumber, string regionalAddress, string value);
    }
}
