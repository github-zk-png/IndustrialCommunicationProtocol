using CommunicationProtocol.Extensions;
using CommunicationProtocol.Omrons.Fins.Extensions;
using CommunicationProtocol.Omrons.Fins.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Omrons.Fins.Protocols
{
    internal class FinsTcp : FinsTcpAuxiliary, IFinsTcp
    {

        internal FinsTcp(FinsTcpConnectParameter parameter) : base(parameter)
        {
        }

        public override async Task<bool> ConnectAsync()
        {
            await base.ConnectAsync();

            var requestFrame = GetConnectRequestFrame();

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            ValidateConnectResponseFrame(responseFrame);

            _plcipTailNode = responseFrame.Span[15];

            return true;
        }

        public async Task<string> ReadStringAsync(string regionalAddress, byte length)
        {

            var parameter = regionalAddress.RegionalAddressParse();

            var readParameter = new FinsReadParameter
            {
                regionDataType = parameter.regionDataType,
                WordAddress = parameter.WordAddress,
                BitAddress = parameter.BitAddress,
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

            var readParameter = new FinsReadParameter
            {
                regionDataType = parameter.regionDataType,
                WordAddress = parameter.WordAddress,
                BitAddress = parameter.BitAddress,
                Count = elementSize.GetDivideTwoCount()
            };

            var values = await ReadAsync(readParameter);

            return values.ToBigEndianUnmanagedType<T>(elementSize);
        }

        public async Task<ReadOnlyMemory<byte>> ReadAsync(FinsReadParameter parameter)
        {

            var requestFrame = GetRequestFrame(parameter);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            return GetDataAndValidate(responseFrame);
        }

        public async Task<IReadOnlyList<FinsReturnTypeData>> ReadAsync(IReadOnlyList<FinsManyReadParameter> parameters)
        {

            var requestFrame = GetRequestFrame(parameters);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            return GetTypeDataAndValidate(responseFrame, parameters);
        }

        public async Task<bool> WriteStringAsync(string regionalAddress, string value)
        {
            var zeroPaddingBytes = value.GetZeroPaddingBytes();

            var parameter = regionalAddress.RegionalAddressParse();

            var writeParameter = new FinsWriteParameter
            {
                regionDataType = parameter.regionDataType,
                WordAddress = parameter.WordAddress,
                BitAddress = parameter.BitAddress,
                Values = zeroPaddingBytes,
                Count = (ushort)(zeroPaddingBytes.Length / 2)
            };

            return await WriteAsync(writeParameter);
        }

        public async Task<bool> WriteAsync<T>(string regionalAddress, T value) where T : unmanaged
        {
            var values = value.ToBigEndiannBytes();

            var parameter = regionalAddress.RegionalAddressParse();

            var writeParameter = new FinsWriteParameter
            {
                regionDataType = parameter.regionDataType,
                WordAddress = parameter.WordAddress,
                BitAddress = parameter.BitAddress,
                Values = values,
                Count = values.Length.GetDivideTwoCount()
            };

            return await WriteAsync(writeParameter);
        }

        public async Task<bool> WriteAsync(FinsWriteParameter parameter)
        {

            var requestFrame = GetRequestFrame(parameter);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            ValidateData(responseFrame);

            return true;
        }

    }
}
