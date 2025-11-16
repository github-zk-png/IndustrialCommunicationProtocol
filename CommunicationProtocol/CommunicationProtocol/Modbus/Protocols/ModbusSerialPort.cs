using CommunicationProtocol.Bases;
using CommunicationProtocol.Extensions;
using CommunicationProtocol.Modbus.Extensions;
using CommunicationProtocol.Modbus.Models;
using CommunicationProtocol.Models;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace CommunicationProtocol.Modbus.Protocols
{
    internal abstract class ModbusSerialPort : SerialPortProtocol
    {
        protected ModbusSerialPort(SerialPortConnectParameter parameter) : base(parameter)
        {

        }

        protected abstract ReadOnlyMemory<byte> GetRequestFrame(ModbusReadParameter parameter);

        protected abstract ReadOnlyMemory<byte> GetRequestFrame(ModbusWriteParameter parameter);

        protected abstract int GetResponseFrameLength(ModbusReadParameter parameter);

        protected abstract int GetResponseFrameLength(ModbusWriteParameter parameter);

        protected abstract ReadOnlyMemory<byte> GetDataAndValidate(ReadOnlyMemory<byte> responseFrame);

        protected abstract void ValidateData(ReadOnlyMemory<byte> responseFrame, ReadOnlySpanAction<byte, byte>? onValidated = default);

        public string ReadString(byte slave, string regionalAddress, byte length)
        {

            var parameter = new ModbusReadParameter
            {
                Slave = slave,
                Count = length.GetRemainderPaddingCount()
            };

            (parameter.FunctionCode, parameter.StartingAddress) = regionalAddress.ReadRegionalAddressParse();

            var values = Read(parameter);

            var value = Encoding.UTF8.GetString(values.Span[..^1]);

            return value;

        }

        public bool WriteString(byte slave, string regionalAddress, string value)
        {
            var zeroPaddingBytes = value.GetZeroPaddingBytes();

            var parameter = new ModbusWriteParameter
            {
                Slave = slave,
                Values = zeroPaddingBytes,
                Count = (ushort)(zeroPaddingBytes.Length / 2)
            };
            (parameter.FunctionCode, parameter.StartingAddress) = regionalAddress.WriteRegionalAddressParse();

            return Write(parameter);
        }

        public T Read<T>(byte slave, string regionalAddress) where T : unmanaged
        {
            var elementSize = Unsafe.SizeOf<T>();
            var parameter = new ModbusReadParameter
            {
                Slave = slave,
                Count = elementSize.GetDivideTwoCount()

            };

            (parameter.FunctionCode, parameter.StartingAddress) = regionalAddress.ReadRegionalAddressParse();

            return Read(parameter).ToBigEndianUnmanagedType<T>(elementSize);
        }

        public bool Write<T>(byte slave, string regionalAddress, T value) where T : unmanaged
        {
            var values = value.ToBigEndiannBytes();
            var parameter = new ModbusWriteParameter
            {
                Slave = slave,
                Values = values,
                Count = values.Length.GetDivideTwoCount()
            };

            (parameter.FunctionCode, parameter.StartingAddress) = regionalAddress.WriteRegionalAddressParse();

            return Write(parameter);
        }

        public ReadOnlyMemory<byte> Read(ModbusReadParameter parameter)
        {
            var requestFrame = GetRequestFrame(parameter);

            var responseFrameLength = GetResponseFrameLength(parameter);

            var responseFrame = SendAndReceive(requestFrame, responseFrameLength);

            return GetDataAndValidate(responseFrame);

        }

        public bool Write(ModbusWriteParameter parameter)
        {
            var requestFrame = GetRequestFrame(parameter);

            var responseFrameLength = GetResponseFrameLength(parameter);

            var responseFrame = SendAndReceive(requestFrame, responseFrameLength);

            ValidateData(responseFrame);

            return true;
        }

    }
}
