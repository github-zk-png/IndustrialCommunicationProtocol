using CommunicationProtocol.Bases;
using CommunicationProtocol.Extensions;
using CommunicationProtocol.Omrons.OmronCips.Enums;
using CommunicationProtocol.Omrons.OmronCips.Models;
using CommunicationProtocol.Omrons.OmronCips.Tools;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationProtocol.Omrons.OmronCips.Protocols
{
    internal abstract class OmronCipAuxiliary : TcpProtocol
    {
        private long _transactionId = 0;

        private long TransactionId => Interlocked.Increment(ref _transactionId);

        protected Memory<byte> _session;

        protected OmronCipAuxiliary(OmronCipConnectParameter parameter) : base(parameter)
        {
            _session = new byte[4];
        }


        protected override async Task<ReadOnlyMemory<byte>> SendAndReceiveAsync(ReadOnlyMemory<byte> requestFrame)
        {
            await _networkStream.WriteAsync(requestFrame);

            var responseFrameHead = await _networkStream.ReadExactAsync(4, _receiveTimeout);

            var responseFrameLength = BinaryPrimitives.ReadUInt16LittleEndian(responseFrameHead.Span[2..]);

            var responseFrameBody = await _networkStream.ReadExactAsync(20 + responseFrameLength, _receiveTimeout);

            return responseFrameBody;
        }

        protected ReadOnlyMemory<byte> GetConnectRequestFrame()
        {

            ReadOnlySpan<byte> requestFrame = stackalloc byte[]
            {
               0x65,0x00,
               0x04,0x00,
               0x00,0x00,0x00,0x00,
               0x00,0x00,0x00,0x00,
               0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
               0x00,0x00,0x00,0x00,
               0x01,0x00, 
               0x00,0x00  
            };

            return requestFrame.ToArray();

        }

        protected void ValidateConnectResponseFrame(ReadOnlyMemory<byte> responseFrame)
        {

            var statusCode = BinaryPrimitives.ReadUInt16LittleEndian(responseFrame.Span[4..8]);

            if (statusCode != 0x00)
                throw new Exception("Connection failed");
        }

        protected ReadOnlyMemory<byte> GetRequestFrame(OmronCipReadParameter parameter)
        {
            var nameBytesCount = Encoding.UTF8.GetByteCount(parameter.Name);

            var Length = nameBytesCount % 2;

            Span<byte> zeroPaddingBytes = new byte[nameBytesCount + Length];

            Encoding.UTF8.GetBytes(parameter.Name, zeroPaddingBytes);

            if (Length == 1)
            {
                zeroPaddingBytes[^1] = 0x00;
            }

            var zeroPaddingBytesLength = zeroPaddingBytes.Length;

            Span<byte> requestFrame = stackalloc byte[zeroPaddingBytesLength + 36 + 24];
            // ------    Encapsulation Header    --------
            requestFrame[0] = 0x6F;
            requestFrame[1] = 0x00;
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[2..4], (ushort)(zeroPaddingBytesLength + 36));
            _session.Span.CopyTo(requestFrame[4..8]);
            requestFrame[8..12].Clear();
            BinaryPrimitives.WriteInt64LittleEndian(requestFrame[12..20], TransactionId);
            requestFrame[20..24].Clear();
            // ------    Command-Specific Data    --------
            requestFrame[24..28].Clear();
            requestFrame[28] = 0x05;
            requestFrame[29] = 0x00;
            requestFrame[30] = 0x02;
            requestFrame[31] = 0x00;
            // ------    Address Item    --------
            requestFrame[32..36].Clear();
            // ------    Data item    --------
            requestFrame[36] = 0xB2;
            requestFrame[37] = 0x00;
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[38..40], (ushort)(zeroPaddingBytesLength + 20));
            requestFrame[40] = 0x52;
            requestFrame[41] = 0x02;
            requestFrame[42] = 0x20;
            requestFrame[43] = 0x06;
            requestFrame[44] = 0x24;
            requestFrame[45] = 0x01;
            requestFrame[46] = 0x05;
            requestFrame[47] = 0x00;
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[48..50], (ushort)(zeroPaddingBytesLength + 6));
            requestFrame[50] = 0x4C;
            requestFrame[51] = (byte)((zeroPaddingBytesLength + 2) / 2);
            requestFrame[52] = 0x91;
            requestFrame[53] = (byte)nameBytesCount;
            zeroPaddingBytes.CopyTo(requestFrame[54..^6]);
            requestFrame[^6] = 0x01;
            requestFrame[^5] = 0x00;
            requestFrame[^4] = 0x01;
            requestFrame[^3] = 0x00;
            requestFrame[^2] = 0x01;
            requestFrame[^1] = 0x00;


            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(OmronCipWriteParameter parameter)
        {
            var nameBytesCount = Encoding.UTF8.GetByteCount(parameter.Name);

            var Length = nameBytesCount % 2;

            Span<byte> zeroPaddingBytes = new byte[nameBytesCount + Length];

            Encoding.UTF8.GetBytes(parameter.Name, zeroPaddingBytes);

            if (Length == 1)
            {
                zeroPaddingBytes[^1] = 0x00;
            }

            var zeroPaddingBytesLength = zeroPaddingBytes.Length;

            var length = 54 + zeroPaddingBytesLength + parameter.Values.Length + 4;

            var singleByteTypeConut = OmronCipTool.SingleByteTypeConuts.GetValueOrDefault(parameter.dataType);

            if (parameter.dataType == OmronCipDataType.STRING)
            {
                length += 2;
            }

            length += singleByteTypeConut + 4;

            Span<byte> requestFrame = stackalloc byte[length];
            // ------    Encapsulation Header    --------
            requestFrame[0] = 0x6F;
            requestFrame[1] = 0x00;
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[2..4], (ushort)requestFrame[24..].Length);
            _session.Span.CopyTo(requestFrame[4..8]);
            requestFrame[8..12].Clear();
            BinaryPrimitives.WriteInt64LittleEndian(requestFrame[12..20], TransactionId);
            requestFrame[20..24].Clear();
            // ------   Command-Specific Data    --------
            requestFrame[24..28].Clear();
            requestFrame[28] = 0x05;
            requestFrame[29] = 0x00;
            requestFrame[30] = 0x02;
            requestFrame[31] = 0x00;
            // ------    Address Item    --------
            requestFrame[32..36].Clear();
            // ------    Data item    --------
            requestFrame[36] = 0xB2;
            requestFrame[37] = 0x00;
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[38..40], (ushort)requestFrame[40..].Length);
            requestFrame[40] = 0x52;
            requestFrame[41] = 0x02;
            requestFrame[42] = 0x20;
            requestFrame[43] = 0x06;
            requestFrame[44] = 0x24;
            requestFrame[45] = 0x01;
            requestFrame[46] = 0x05;
            requestFrame[47] = 0x00;
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[48..50], (ushort)requestFrame[50..^4].Length);
            requestFrame[50] = 0x4D;
            requestFrame[51] = (byte)((zeroPaddingBytesLength + 2) / 2);
            requestFrame[52] = 0x91;
            requestFrame[53] = (byte)nameBytesCount;

            var offset = 54 + zeroPaddingBytesLength;
            zeroPaddingBytes.CopyTo(requestFrame[54..offset]);
            requestFrame[offset++] = (byte)parameter.dataType;
            requestFrame[offset++] = 0x00;
            requestFrame[offset++] = 0x01;
            requestFrame[offset++] = 0x00;
            if (parameter.dataType == OmronCipDataType.STRING)
            {
                BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[offset++..(offset + 2)], (ushort)parameter.Values.Length);
                offset++;
            }

            parameter.Values.Span.CopyTo(requestFrame[offset++..(offset + parameter.Values.Length)]);

            if (singleByteTypeConut == 1)
            {
                requestFrame[offset + parameter.Values.Length] = 0x00;
            }

            requestFrame[^4] = 0x01;
            requestFrame[^3] = 0x00;
            requestFrame[^2] = 0x01;
            requestFrame[^1] = 0x00;


            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetDataAndValidate(ReadOnlyMemory<byte> responseFrame)
        {
            Memory<byte> responseFrameData = default;
            ValidateData(responseFrame, (responseFrameBody, _) =>
            {
                var typeBytesCount = OmronCipTool.TypeByteConuts[responseFrameBody[40]];

                if (typeBytesCount == 0)
                {
                    var length = BinaryPrimitives.ReadUInt16LittleEndian(responseFrameBody[42..44]);

                    responseFrameData = responseFrameBody[44..(44 + length)].ToArray();
                }
                else
                {

                    responseFrameData = responseFrameBody[42..(42 + typeBytesCount)].ToArray();

                }

            });

            return responseFrameData;
        }

        protected void ValidateData(ReadOnlyMemory<byte> responseFrame, ReadOnlySpanAction<byte, byte>? GetDataBytes = default)
        {

            var responseFrameBody = responseFrame.Span;

            var statusCode = BinaryPrimitives.ReadUInt32LittleEndian(responseFrameBody[4..8]);
            if (statusCode != 0)
            {
                throw new Exception($"Abnormal status code response. Status code：{responseFrameBody.ToHexString('-')}");
            }

            var endCode = BinaryPrimitives.ReadUInt16LittleEndian(responseFrameBody[38..40]);
            if (endCode != 0)
            {
                throw new Exception($"Abnormal response with status code：{responseFrameBody.ToHexString('-')}");
            }

            GetDataBytes?.Invoke(responseFrameBody, default);
        }

    }
}
