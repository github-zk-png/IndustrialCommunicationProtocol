using CommunicationProtocol.Bases;
using CommunicationProtocol.Extensions;
using CommunicationProtocol.Mitsubishis.Mc3es.Models;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Protocols
{
    internal abstract class Mc3eAuxiliary : TcpProtocol
    {
        protected Mc3eAuxiliary(Mc3eConnectParameter parameter) : base(parameter)
        {

        }

        protected override async Task<ReadOnlyMemory<byte>> SendAndReceiveAsync(ReadOnlyMemory<byte> requestFrame)
        {

            await _networkStream.WriteAsync(requestFrame);

            var responseFrameHead = await _networkStream.ReadExactAsync(9, _receiveTimeout);

            var responseFrameLength = BinaryPrimitives.ReadUInt16LittleEndian(responseFrameHead.Span[^2..]);

            var responseFrameBody = await _networkStream.ReadExactAsync(responseFrameLength, _receiveTimeout);

            return responseFrameBody;

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(Mc3eReadParameter parameter)
        {
            ReadOnlySpan<byte> requestFrame = stackalloc byte[]
            {
                0x50,0x00,
                0x00,
                0xFF,
                0xFF,0x03,
                0x00,
                0x0C,0x00, 
                0x10,0x00,  
                0x01,0x04,
                (byte)parameter.DataType,0x00,
                (byte)parameter.Address,
                (byte)(parameter.Address >> 8),
                (byte)(parameter.Address >> 16),
                (byte)parameter.Region,
                (byte)parameter.Count,
                (byte)(parameter.Count >> 8)
            };

            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(Mc3eWriteParameter parameter)
        {
            var length = parameter.Values.Length;

            Span<byte> requestFrame = stackalloc byte[21 + length];
            requestFrame[0] = 0x50;
            requestFrame[1..3].Clear();
            requestFrame[3..5].Fill(0xff);
            requestFrame[5] = 0x03;
            requestFrame[6] = 0x00;
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[7..9], (ushort)(12 + length));
            requestFrame[9] = 0x10;
            requestFrame[10] = 0x00;
            requestFrame[11] = 0x01;
            requestFrame[12] = 0x14;
            requestFrame[13] = (byte)parameter.DataType;
            requestFrame[14] = 0x00;
            requestFrame[15] = (byte)parameter.Address;
            requestFrame[16] = (byte)(parameter.Address >> 8);
            requestFrame[17] = (byte)(parameter.Address >> 16);
            requestFrame[18] = (byte)parameter.Region;
            requestFrame[19] = (byte)parameter.Count;
            requestFrame[20] = (byte)(parameter.Count >> 8);

            parameter.Values.Span.CopyTo(requestFrame[21..]);

            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(Mc3eManyReadParameter parameter)
        {
            var length = parameter.WordParameters.Count + parameter.BitParameters.Count;

            Span<byte> requestFrame = stackalloc byte[17 + length * 6];
            requestFrame[0] = 0x50;
            requestFrame[1..3].Clear();
            requestFrame[3..5].Fill(0xff);
            requestFrame[5] = 0x03;
            requestFrame[6] = 0x00;
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[7..9], (ushort)(8 + length * 6));
            requestFrame[9] = 0x10;
            requestFrame[10] = 0x00;
            requestFrame[11] = 0x06;
            requestFrame[12] = 0x04;
            requestFrame[13..15].Clear();
            requestFrame[15] = (byte)parameter.WordParameters.Count;
            requestFrame[16] = (byte)parameter.BitParameters.Count;

            var index = 16;
            foreach (var item in parameter.WordParameters)
            {
                requestFrame[index++] = (byte)item.Address;
                requestFrame[index++] = (byte)(item.Address >> 8);
                requestFrame[index++] = (byte)(item.Address >> 16);
                requestFrame[index++] = (byte)item.Region;
                requestFrame[index++] = (byte)item.Count;
                requestFrame[index++] = (byte)(item.Count >> 8);
            }

            foreach (var item in parameter.BitParameters)
            {
                requestFrame[index++] = (byte)item.Address;
                requestFrame[index++] = (byte)(item.Address >> 8);
                requestFrame[index++] = (byte)(item.Address >> 16);
                requestFrame[index++] = (byte)item.Region;
                requestFrame[index++] = (byte)item.Count;
                requestFrame[index++] = (byte)(item.Count >> 8);
            }

            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetDataAndValidate(ReadOnlyMemory<byte> responseFrame)
        {
            Memory<byte> responseFrameData = default;
            ValidateData(responseFrame, (responseFrameBody, _) =>
            {
                responseFrameData = responseFrameBody[2..].ToArray();
            });

            return responseFrameData;
        }

        protected void ValidateData(ReadOnlyMemory<byte> responseFrame, ReadOnlySpanAction<byte, byte>? GetDataBytes = default)
        {

            var responseFrameBody = responseFrame.Span;

            var statusCode = BinaryPrimitives.ReadUInt16LittleEndian(responseFrameBody[..2]);
            if (statusCode != 0)
            {
                throw new Exception($"The status code response is abnormal. Status code：{responseFrameBody.ToHexString('-')}");
            }

            GetDataBytes?.Invoke(responseFrameBody, default);
        }

    }
}
