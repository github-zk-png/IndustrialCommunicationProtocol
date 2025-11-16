using CommunicationProtocol.Extensions;
using CommunicationProtocol.Modbus.Extensions;
using CommunicationProtocol.Modbus.Models;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Modbus.Protocols
{
    internal class ModbusTcp : ModbusTcpAuxiliary, IModbusTcp
    {
        internal ModbusTcp(ModbusTcpConnectParameter parameter) : base(parameter)
        {

        }

        public async Task<string> ReadStringAsync(byte slave, string regionalAddress, byte length)
        {
            var parameter = new ModbusReadParameter
            {
                Slave = slave,
                Count = length.GetRemainderPaddingCount()
            };

            (parameter.FunctionCode, parameter.StartingAddress) = regionalAddress.ReadRegionalAddressParse();

            var values = await ReadAsync(parameter);

            var value = Encoding.UTF8.GetString(values.Span[..^1]);

            return value;

        }

        public async Task<T> ReadAsync<T>(byte slave, string regionalAddress) where T : unmanaged
        {
            var elementSize = Unsafe.SizeOf<T>();

            var parameter = new ModbusReadParameter
            {
                Slave = slave,
                Count = elementSize.GetDivideTwoCount()
            };

            (parameter.FunctionCode, parameter.StartingAddress) = regionalAddress.ReadRegionalAddressParse();

            var dataBytes = await ReadAsync(parameter);

            return dataBytes.ToUnmanagedType<T>();
        }

        public async Task<bool> WriteStringAsync(byte slave, string regionalAddress, string value)
        {

            var zeroPaddingBytes = value.GetZeroPaddingBytes();

            var parameter = new ModbusWriteParameter
            {
                Slave = slave,
                Values = zeroPaddingBytes,
                Count = (ushort)(zeroPaddingBytes.Length / 2)
            };
            (parameter.FunctionCode, parameter.StartingAddress) = regionalAddress.WriteRegionalAddressParse();

            return await WriteAsync(parameter);
        }

        public async Task<bool> WriteAsync<T>(byte slave, string regionalAddress, T value) where T : unmanaged
        {
            var values = value.ToBigEndiannBytes();

            var parameter = new ModbusWriteParameter
            {
                Slave = slave,
                Values = values,
                Count = values.Length.GetDivideTwoCount()
            };
            (parameter.FunctionCode, parameter.StartingAddress) = regionalAddress.WriteRegionalAddressParse();

            return await WriteAsync(parameter);
        }

        public async Task<ReadOnlyMemory<byte>> ReadAsync(ModbusReadParameter parameter)
        {
            var requestFrame = GetRequestFrame(parameter);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            return GetDataAndValidate(responseFrame);

        }

        public async Task<bool> WriteAsync(ModbusWriteParameter parameter)
        {
            var requestFrame = GetRequestFrame(parameter);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            ValidateData(responseFrame);

            return true;

        }

    }
}
