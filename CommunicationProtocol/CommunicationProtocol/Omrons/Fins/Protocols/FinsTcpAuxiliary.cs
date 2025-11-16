using CommunicationProtocol.Bases;
using CommunicationProtocol.Extensions;
using CommunicationProtocol.Omrons.Fins.Enums;
using CommunicationProtocol.Omrons.Fins.Models;
using CommunicationProtocol.Omrons.Fins.Tools;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationProtocol.Omrons.Fins.Protocols
{
    internal abstract class FinsTcpAuxiliary : TcpProtocol
    {

        private int _transactionId = 0;

        private byte TransactionId => (byte)Interlocked.Increment(ref _transactionId);

        private readonly byte _pcipTailNode;

        protected byte _plcipTailNode;

        public FinsTcpAuxiliary(FinsTcpConnectParameter parameter) : base(parameter)
        {
            _pcipTailNode = Convert.ToByte(_tcoConnectParameter.Host.Split('.')[3]);
        }

        protected void ValidateConnectResponseFrame(ReadOnlyMemory<byte> responseFrame)
        {

            var statusCode = BinaryPrimitives.ReadUInt32BigEndian(responseFrame.Span[4..8]);

            if (statusCode != 0x00)
                throw new Exception("Connection failed");
        }

        protected ReadOnlyMemory<byte> GetConnectRequestFrame()
        {

            ReadOnlyMemory<byte> requestFrame = new byte[]
            {
                0x46,0x49,0x4E,0x53,
                0x00,0x00,0x00,0x0C,
                0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,0x00,
                0x00,0x00,0x00,_pcipTailNode
            };


            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(FinsReadParameter parameter)
        {

            ReadOnlySpan<byte> requestFrame = stackalloc byte[]
            {
                // ------    FINSTCP Header    --------
                0x46,0x49,0x4E,0x53,
                0x00,0x00,0x00,0x1A,
                0x00,0x00,0x00,0x02,
                0x00,0x00,0x00,0x00,

                // ------    FINS Header    --------
                0x80,
                0x00,
                0x02,
                0x00,
                _plcipTailNode,
                0x00,
                0x00,
                _pcipTailNode,
                0x00,
                TransactionId,

                // ------    FINS Command    --------
                0x01,0x01,
                (byte)parameter.regionDataType,
                (byte)(parameter.WordAddress>>8),
                (byte)parameter.WordAddress,
                parameter.BitAddress,
                (byte)(parameter.Count>>8),
                (byte)parameter.Count,
            };


            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(FinsWriteParameter parameter)
        {
            var length = 26 + parameter.Values.Length;
            Span<byte> requestFrame = stackalloc byte[8 + length];

            // ------    FINSTCP Header    --------
            requestFrame[0] = 0x46;
            requestFrame[1] = 0x49;
            requestFrame[2] = 0x4E;
            requestFrame[3] = 0x53;
            BinaryPrimitives.WriteInt32BigEndian(requestFrame[4..8], length);
            requestFrame[8..11].Clear();
            requestFrame[11] = 0x02;
            requestFrame[12..16].Clear();

            // ------    FINS Header    --------
            requestFrame[16] = 0x80;
            requestFrame[17] = 0x00;
            requestFrame[18] = 0x02;
            requestFrame[19] = 0x00;
            requestFrame[20] = _plcipTailNode;
            requestFrame[21..23].Clear();
            requestFrame[23] = _pcipTailNode;
            requestFrame[24] = 0x00;
            requestFrame[25] = TransactionId;

            // ------    FINS Command    --------
            requestFrame[26] = 0x01;
            requestFrame[27] = 0x02;
            requestFrame[28] = (byte)parameter.regionDataType;
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[29..31], parameter.WordAddress);
            requestFrame[31] = parameter.BitAddress;
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[32..34], parameter.Count);
            parameter.Values.Span.CopyTo(requestFrame[34..]);

            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(IReadOnlyCollection<FinsManyReadParameter> parameters)
        {
            var length = 20 + parameters.Count * 4;
            Span<byte> requestFrame = stackalloc byte[8 + length];
            // ------    FINSTCP Header    --------
            requestFrame[0] = 0x46;
            requestFrame[1] = 0x49;
            requestFrame[2] = 0x4E;
            requestFrame[3] = 0x53;
            BinaryPrimitives.WriteInt32BigEndian(requestFrame[4..8], length);
            requestFrame[8..11].Clear();
            requestFrame[11] = 0x02;
            requestFrame[12..16].Clear();

            // ------    FINS Header    --------
            requestFrame[16] = 0x80;
            requestFrame[17] = 0x00;
            requestFrame[18] = 0x02;
            requestFrame[19] = 0x00;
            requestFrame[20] = _plcipTailNode;
            requestFrame[21..23].Clear();
            requestFrame[23] = _pcipTailNode;
            requestFrame[24] = 0x00;
            requestFrame[25] = TransactionId;

            // ------    FINS Command    --------
            requestFrame[26] = 0x01;
            requestFrame[27] = 0x04;

            var index = 28;
            foreach (var parameter in parameters)
            {
                requestFrame[index] = (byte)parameter.regionDataType;
                BinaryPrimitives.WriteUInt16BigEndian(requestFrame[(index + 1)..(index + 3)], parameter.WordAddress);
                requestFrame[index + 3] = parameter.BitAddress;
                index += 4;
            }

            return requestFrame.ToArray();

        }

        protected override async Task<ReadOnlyMemory<byte>> SendAndReceiveAsync(ReadOnlyMemory<byte> requestFrame)
        {
            await _networkStream.WriteAsync(requestFrame);

            var responseFrameHead = await _networkStream.ReadExactAsync(8, _receiveTimeout);

            var responseFrameLength = BinaryPrimitives.ReadUInt32BigEndian(responseFrameHead.Span[4..]);

            var responseFrameBody = await _networkStream.ReadExactAsync(responseFrameLength, _receiveTimeout);

            return responseFrameBody;

        }

        protected ReadOnlyMemory<byte> GetDataAndValidate(ReadOnlyMemory<byte> responseFrame)
        {
            Memory<byte> responseFrameData = default;
            ValidateData(responseFrame, (responseFrameBody, _) =>
            {
                responseFrameData = responseFrameBody[22..].ToArray();
            });

            return responseFrameData;
        }

        protected IReadOnlyList<FinsReturnTypeData> GetTypeDataAndValidate(ReadOnlyMemory<byte> responseFrame, IReadOnlyCollection<FinsManyReadParameter> parameters)
        {

            var returnTypeDatas = new List<FinsReturnTypeData>(parameters.Count);
            ValidateData(responseFrame, (responseFrameBody, _) =>
            {
                var index = 23;
                foreach (var item in parameters)
                {
                    var dataLength = FinsTool.TypeByteCounts[item.DataType];

                    object value = item.DataType switch
                    {
                        FinsDataTypeEnum.BOOL => responseFrameBody[index..(index + dataLength)].ToBigEndianUnmanagedType<bool>(),
                        FinsDataTypeEnum.SHORT => responseFrameBody[index..(index + dataLength)].ToBigEndianUnmanagedType<short>(),
                        FinsDataTypeEnum.USHORT => responseFrameBody[index..(index + dataLength)].ToBigEndianUnmanagedType<ushort>(),
                        _ => throw new NotSupportedException($"Unsupported data type：{item.DataType}"),
                    };


                    returnTypeDatas.Add(new FinsReturnTypeData
                    {
                        TransactionId = item.TransactionId,
                        DataType = item.DataType,
                        Value = value
                    });

                    index += dataLength + 1;
                }
            });

            return returnTypeDatas;
        }

        protected void ValidateData(ReadOnlyMemory<byte> responseFrame, ReadOnlySpanAction<byte, byte>? GetDataBytes = default)
        {

            var responseFrameBody = responseFrame.Span;

            var statusCode = BinaryPrimitives.ReadUInt32BigEndian(responseFrameBody[4..8]);
            if (statusCode != 0)
            {
                throw new Exception($"FinsTcp Exception in response, status code：{responseFrameBody.ToHexString('-')}");
            }

            var endCode = BinaryPrimitives.ReadUInt16BigEndian(responseFrameBody[20..22]);
            if (endCode != 0)
            {
                throw new Exception($"Abnormal response with status code：{responseFrameBody.ToHexString('-')}");
            }

            GetDataBytes?.Invoke(responseFrameBody, default);
        }


    }
}
