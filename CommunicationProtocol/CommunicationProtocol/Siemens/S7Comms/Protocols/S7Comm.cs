using CommunicationProtocol.Extensions;
using CommunicationProtocol.Models;
using CommunicationProtocol.Siemens.S7Comms.Enums;
using CommunicationProtocol.Siemens.S7Comms.Extensions;
using CommunicationProtocol.Siemens.S7Comms.Models;
using CommunicationProtocol.Siemens.S7Comms.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Siemens.S7Comms.Protocols
{
    internal class S7Comm : S7CommAuxiliary, IS7Comm
    {
        internal S7Comm(S7CommConnectParameter parameter) : base(parameter)
        {
        }

        public override async Task<bool> ConnectAsync()
        {
            await base.ConnectAsync();

            var connectRequestFrame = GetCotpRequestFrame(_s7CommConnectParameter.Rack, _s7CommConnectParameter.Slot);

            var cotpreResponseFrame = await SendAndReceiveAsync(connectRequestFrame);

            ValidateCotpResponseFrame(cotpreResponseFrame);

            var setupCommunicationRequestFrame = GetSetupCommunicationRequestFrame();

            var setupCommunicationResponseFrame = await SendAndReceiveAsync(setupCommunicationRequestFrame);

            ValidateSetupCommunicationResponseFrame(setupCommunicationResponseFrame);

            SavePlcPduLength(setupCommunicationResponseFrame);

            return true;
        }

        public async Task<string> ReadStringAsync(string regionalAddress, byte length)
        {
            var parameter = regionalAddress.RegionalAddressParse();

            var S7CommReadParameters = new List<S7CommReadParameter>()
            {
               new S7CommReadParameter
               {
                 Region=parameter.Region,
                 ParameterItemType=parameter.ParameterItemType,
                 DbNumber=parameter.DbNumber,
                 ByteAddress=parameter.ByteAddress,
                 BitAddress=parameter.BitAddress,
                 Count=length
               }
            };

            var returnByteDatas = await ReadAsync(S7CommReadParameters);

            var count = returnByteDatas[0].Values.Span[1];

            var stringValue = Encoding.UTF8.GetString(returnByteDatas[0].Values.Span.Slice(2, count));

            return stringValue;

        }

        public async Task<T> ReadAsync<T>(string regionalAddress) where T : unmanaged
        {
            var elementSize = (ushort)Unsafe.SizeOf<T>();

            var parameter = regionalAddress.RegionalAddressParse();

            var S7CommReadParameters = new List<S7CommReadParameter>()
            {
               new S7CommReadParameter
               {
                 Region=parameter.Region,
                 ParameterItemType=parameter.ParameterItemType,
                 DbNumber=parameter.DbNumber,
                 ByteAddress=parameter.ByteAddress,
                 BitAddress=parameter.BitAddress,
                 Count=elementSize
               }
            };

            var returnByteDatas = await ReadAsync(S7CommReadParameters);

            return returnByteDatas[0].Values.ToBigEndianUnmanagedType<T>(elementSize);

        }

        public async IAsyncEnumerable<S7CommReturnTypeData> ReadAsync(IReadOnlyList<S7CommTypeReadParameter> parameters)
        {
            var s7CommReadParameters = new List<S7CommReadParameter>(parameters.Count);

            foreach (var item in parameters)
            {
                s7CommReadParameters.Add(new S7CommReadParameter()
                {
                    Region = item.Region,
                    ParameterItemType = item.DataType == S7CommDataTypeEnum.BOOL ? S7CommParameterItemTypeEnum.BIT : S7CommParameterItemTypeEnum.BYTE,
                    DbNumber = item.DbNumber,
                    ByteAddress = item.ByteAddress,
                    BitAddress = item.BitAddress,
                    Count = S7CommTool.TypeByteCounts[item.DataType],
                });
            }

            var returnByteDatas = await ReadAsync(s7CommReadParameters);

            for (int i = 0; i < returnByteDatas.Count; i++)
            {
                var returnByteData = returnByteDatas[i];
                var parameter = parameters[i];

                object value = parameter.DataType switch
                {
                    S7CommDataTypeEnum.BOOL => returnByteData.Values.ToUnmanagedType<bool>(),
                    S7CommDataTypeEnum.SBYTE => returnByteData.Values.ToUnmanagedType<sbyte>(),
                    S7CommDataTypeEnum.BYTE => returnByteData.Values.ToUnmanagedType<byte>(),
                    S7CommDataTypeEnum.SHORT => returnByteData.Values.ToUnmanagedType<short>(),
                    S7CommDataTypeEnum.USHORT => returnByteData.Values.ToUnmanagedType<ushort>(),
                    S7CommDataTypeEnum.INT => returnByteData.Values.ToUnmanagedType<int>(),
                    S7CommDataTypeEnum.UINT => returnByteData.Values.ToUnmanagedType<uint>(),
                    S7CommDataTypeEnum.FLOAT => returnByteData.Values.ToUnmanagedType<float>(),
                    _ => throw new Exception("Incorrect return data type ")
                };

                yield return new S7CommReturnTypeData()
                {
                    TransactionId = parameter.TransactionId,
                    DataType = parameter.DataType,
                    Value = value,
                };
            }

        }

        public async Task<IReadOnlyList<S7CommReturnByteData>> ReadAsync(IReadOnlyList<S7CommReadParameter> parameters)
        {
            ValidatePduLength(parameters);

            var requestFrame = GetRequestFrame(parameters);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            return GetDataAndValidate(responseFrame);

        }

        public async Task<bool> WriteStringAsync(string regionalAddress, string value)
        {
            var valueBytesCount = Encoding.UTF8.GetByteCount(value);

            Memory<byte> valueBytes = new byte[valueBytesCount + 1];

            valueBytes.Span[0] = (byte)valueBytesCount;

            Encoding.UTF8.GetBytes(value, valueBytes.Span[1..]);

            var parameter = regionalAddress.RegionalAddressParse();
            var S7CommWriteParameters = new List<S7CommWriteParameter>()
            {
               new S7CommWriteParameter
               {
                 Region=parameter.Region,
                 ParameterItemType=parameter.ParameterItemType,
                 DbNumber=parameter.DbNumber,
                 ByteAddress=parameter.ByteAddress+1,
                 BitAddress=parameter.BitAddress,
                 DataItemType=parameter.ParameterItemType==S7CommParameterItemTypeEnum.BYTE?S7CommDataItemTypeEnum.BYTE:S7CommDataItemTypeEnum.BIT,
                 Values=valueBytes,
                 Count=(ushort)(valueBytes.Length),
               }
            };

            return await WriteAsync(S7CommWriteParameters);
        }

        public async Task<bool> WriteAsync<T>(string regionalAddress, T value) where T : unmanaged
        {

            var values = value.ToBigEndiannBytes();
            var parameter = regionalAddress.RegionalAddressParse();
            var S7CommWriteParameters = new List<S7CommWriteParameter>()
            {
               new S7CommWriteParameter
               {
                 Region=parameter.Region,
                 ParameterItemType=parameter.ParameterItemType,
                 DbNumber=parameter.DbNumber,
                 ByteAddress=parameter.ByteAddress,
                 BitAddress=parameter.BitAddress,
                 DataItemType=parameter.ParameterItemType==S7CommParameterItemTypeEnum.BYTE?S7CommDataItemTypeEnum.BYTE:S7CommDataItemTypeEnum.BIT,
                 Values=values,
                 Count=(ushort)values.Length,
               }
            };

            return await WriteAsync(S7CommWriteParameters);
        }

        public async Task<bool> WriteAsync(IReadOnlyList<S7CommTypeWriteParameter> parameters)
        {

            var s7CommWriteParameters = new List<S7CommWriteParameter>(parameters.Count);

            foreach (var item in parameters)
            {
                var values = item.DataType switch
                {
                    S7CommDataTypeEnum.BOOL => Convert.ToBoolean(item.Value).ToBytes(),
                    S7CommDataTypeEnum.SBYTE => Convert.ToSByte(item.Value).ToBytes(),
                    S7CommDataTypeEnum.BYTE => Convert.ToByte(item.Value).ToBytes(),
                    S7CommDataTypeEnum.SHORT => Convert.ToInt16(item.Value).ToBytes(),
                    S7CommDataTypeEnum.USHORT => Convert.ToUInt16(item.Value).ToBytes(),
                    S7CommDataTypeEnum.INT => Convert.ToInt32(item.Value).ToBytes(),
                    S7CommDataTypeEnum.UINT => Convert.ToUInt32(item.Value).ToBytes(),
                    S7CommDataTypeEnum.FLOAT => Convert.ToSingle(item.Value).ToBytes(),
                    _ => throw new Exception("Incorrect return data type ")
                };

                s7CommWriteParameters.Add(new S7CommWriteParameter
                {
                    Region = item.Region,
                    ParameterItemType = item.DataType == S7CommDataTypeEnum.BOOL ? S7CommParameterItemTypeEnum.BIT : S7CommParameterItemTypeEnum.BYTE,
                    DbNumber = item.DbNumber,
                    ByteAddress = item.ByteAddress,
                    BitAddress = item.BitAddress,
                    DataItemType = item.DataType == S7CommDataTypeEnum.BOOL ? S7CommDataItemTypeEnum.BIT : S7CommDataItemTypeEnum.BYTE,
                    Values = values,
                    Count = (ushort)values.Length,
                });
            }

            return await WriteAsync(s7CommWriteParameters);
        }

        public async Task<bool> WriteAsync(IReadOnlyList<S7CommWriteParameter> parameters)
        {
            ValidatePduLength(parameters);

            var requestFrame = GetRequestFrame(parameters);

            var responseFrame = await SendAndReceiveAsync(requestFrame);

            ValidateData(responseFrame);

            return true;

        }
    }
}
