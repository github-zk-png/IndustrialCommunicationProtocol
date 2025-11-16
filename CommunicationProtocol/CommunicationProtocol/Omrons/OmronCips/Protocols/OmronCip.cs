using CommunicationProtocol.Extensions;
using CommunicationProtocol.Omrons.OmronCips.Enums;
using CommunicationProtocol.Omrons.OmronCips.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommunicationProtocol.Omrons.OmronCips.Protocols
{
    internal class OmronCip : OmronCipAuxiliary, IOmronCip
    {
        internal OmronCip(OmronCipConnectParameter parameter) : base(parameter)
        {

        }

        public override async Task<bool> ConnectAsync()
        {
            await base.ConnectAsync();

            var requestFrame = GetConnectRequestFrame();

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            ValidateConnectResponseFrame(responseFrame);

            responseFrame.Span[..4].CopyTo(_session.Span);

            return true;
        }

        public async Task<string> ReadStringAsync(string Name)
        {

            var parameter = new OmronCipReadParameter
            {
                Name = Name,
            };

            var values = await ReadAsync(parameter);

            var value = Encoding.UTF8.GetString(values.Span);

            return value;

        }

        public async Task<T> ReadAsync<T>(string Name) where T : unmanaged
        {
            var parameter = new OmronCipReadParameter
            {
                Name = Name,
            };

            var values = await ReadAsync(parameter);

            return values.ToLittleEndianUnmanagedType<T>();
        }

        public async Task<ReadOnlyMemory<byte>> ReadAsync(OmronCipReadParameter parameter)
        {

            var requestFrame = GetRequestFrame(parameter);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            return GetDataAndValidate(responseFrame);
        }

        public async Task<bool> WriteStringAsync(string Name, string value)
        {
            var zeroPaddingBytes = value.GetZeroPaddingBytes();

            var parameter = new OmronCipWriteParameter
            {
                Name = Name,
                dataType = OmronCipDataType.STRING,
                Values = zeroPaddingBytes,
            };

            return await WriteAsync(parameter);
        }

        public async Task<bool> WriteAsync<T>(string Name, OmronCipDataType dataType, T value) where T : unmanaged
        {
            var values = value.ToLittleEndianBytes();

            var parameter = new OmronCipWriteParameter
            {
                Name = Name,
                dataType = dataType,
                Values = values,
            };

            return await WriteAsync(parameter);
        }

        public async Task<bool> WriteAsync(OmronCipWriteParameter parameter)
        {
            var requestFrame = GetRequestFrame(parameter);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            ValidateData(responseFrame);

            return true;
        }
    }
}
