using CommunicationProtocol.Extensions;
using CommunicationProtocol.Mitsubishis.Mc3es.Extensions;
using CommunicationProtocol.Mitsubishis.Mc3es.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Protocols
{
    internal class Mc3e : Mc3eAuxiliary, IMc3e
    {
        internal Mc3e(Mc3eConnectParameter parameter) : base(parameter)
        {

        }

        public async Task<string> ReadStringAsync(string regionalAddress, byte length)
        {

            var parameter = regionalAddress.RegionalAddressParse();

            var readParameter = new Mc3eReadParameter
            {
                Region = parameter.Region,
                DataType = parameter.DataType,
                Address = parameter.Address,
                Count = length.GetRemainderPaddingCount()
            };

            var values = await ReadAsync(readParameter);

            var value = Encoding.UTF8.GetString(values.Span[..^1]);

            return value;

        }

        public async Task<T> ReadAsync<T>(string regionalAddress) where T : unmanaged
        {
            var elementSize = Unsafe.SizeOf<T>();
            var parameter = regionalAddress.RegionalAddressParse();

            var readParameter = new Mc3eReadParameter
            {
                Region = parameter.Region,
                DataType = parameter.DataType,
                Address = parameter.Address,
                Count = elementSize.GetDivideTwoCount()
            };

            var values = await ReadAsync(readParameter);

            return values.ToLittleEndianUnmanagedType<T>(elementSize);
        }

        public async Task<ReadOnlyMemory<byte>> ReadAsync(Mc3eReadParameter parameter)
        {

            var requestFrame = GetRequestFrame(parameter);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            return GetDataAndValidate(responseFrame);
        }

        public async Task<bool> WriteStringAsync(string regionalAddress, string value)
        {
            var zeroPaddingBytes = value.GetZeroPaddingBytes();

            var parameter = regionalAddress.RegionalAddressParse();

            var writeParameter = new Mc3eWriteParameter
            {
                Region = parameter.Region,
                DataType = parameter.DataType,
                Address = parameter.Address,
                Values = zeroPaddingBytes,
                Count = (ushort)(zeroPaddingBytes.Length / 2)
            };

            return await WriteAsync(writeParameter);
        }

        public async Task<bool> WriteAsync<T>(string regionalAddress, T value) where T : unmanaged
        {
            var values = default(ReadOnlyMemory<byte>);

            if (typeof(T) == typeof(bool))
            {
                values = (bool)(object)value == true ? new byte[1] { 0x10 } : new byte[1] { 0x00 };
            }
            else
            {
                values = value.ToLittleEndianBytes();
            };

            var parameter = regionalAddress.RegionalAddressParse();

            var writeParameter = new Mc3eWriteParameter
            {
                Region = parameter.Region,
                DataType = parameter.DataType,
                Address = parameter.Address,
                Values = values,
                Count = values.Length.GetDivideTwoCount()
            };

            return await WriteAsync(writeParameter);
        }

        public async Task<bool> WriteAsync(Mc3eWriteParameter parameter)
        {
            var requestFrame = GetRequestFrame(parameter);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            ValidateData(responseFrame);

            return true;
        }


    }
}
