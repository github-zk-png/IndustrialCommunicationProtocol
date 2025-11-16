using CommunicationProtocol.Bases;
using CommunicationProtocol.Extensions;
using CommunicationProtocol.Modbus.Enums;
using CommunicationProtocol.Modbus.Models;
using CommunicationProtocol.Models;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationProtocol.Modbus.Protocols
{
    internal abstract class ModbusTcpAuxiliary : TcpProtocol
    {
        private int _transactionId = 0;

        private ushort TransactionId => (ushort)Interlocked.Increment(ref _transactionId);

        protected ModbusTcpAuxiliary(ModbusTcpConnectParameter parameter) : base(parameter)
        {

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(ModbusReadParameter parameter)
        {

            ReadOnlySpan<byte> requestFrame = stackalloc byte[12]
            {
                (byte)(TransactionId >> 8),
                (byte)TransactionId,
                0x00,0x00,
                0x00,0x06,
                parameter.Slave,
                (byte)parameter.FunctionCode,
                (byte)(parameter.StartingAddress >> 8),
                (byte)parameter.StartingAddress ,
                (byte)(parameter.Count >> 8),
                (byte)parameter.Count,
            };

            return requestFrame.ToArray();
        }

        protected ReadOnlyMemory<byte> GetRequestFrame(ModbusWriteParameter parameter)
        {
            var length = parameter.Values.Length;
  

            Span<byte> requestFrame = stackalloc byte[6 + 7 + length];
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[0..2], TransactionId);
            requestFrame[2..4].Clear();
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[4..6], (ushort)(length + 7));
            requestFrame[6] = parameter.Slave;
            requestFrame[7] = (byte)parameter.FunctionCode;
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[8..10], parameter.StartingAddress);
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[10..12], parameter.Count);
            requestFrame[12] = (byte)parameter.Values.Length;
            parameter.Values.Span.CopyTo(requestFrame[13..]);

            return requestFrame.ToArray();
        }

        protected void ValidateData(ReadOnlyMemory<byte> responseFrame)
        {
            if (responseFrame.Span[1] > 0x80)
            {
                throw new Exception(responseFrame.ToHexString('-'));
            }
        }

        protected ReadOnlyMemory<byte> GetDataAndValidate(ReadOnlyMemory<byte> responseFrame)
        {
            ValidateData(responseFrame);

            return responseFrame[3..];
        }

        protected override async Task<ReadOnlyMemory<byte>> SendAndReceiveAsync(ReadOnlyMemory<byte> requestFrame)
        {
            await _networkStream.WriteAsync(requestFrame);

            var responseFrameHead = await _networkStream.ReadExactAsync(6, _receiveTimeout);

            var responseFrameLength = BinaryPrimitives.ReadUInt16BigEndian(responseFrameHead.Span[^2..]);

            var responseFrameBody = await _networkStream.ReadExactAsync(responseFrameLength, _receiveTimeout);

            return responseFrameBody;

        }


    }
}
