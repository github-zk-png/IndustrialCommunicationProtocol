using CommunicationProtocol.Bases;
using CommunicationProtocol.Extensions;
using CommunicationProtocol.Modbus.Enums;
using CommunicationProtocol.Modbus.Models;
using CommunicationProtocol.Models;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Modbus.Protocols
{
    internal class ModbusRtu : ModbusSerialPort, IModbusRtu
    {
        private readonly ushort[] _crcTable;

        internal ModbusRtu(ModbusSerialPortConnectParameter parameter) : base(parameter)
        {
            _crcTable = GenerateCrcTable();
        }

        private ushort[] GenerateCrcTable()
        {
            var table = new ushort[256];
            for (int i = 0; i < 256; i++)
            {
                ushort crc = (ushort)i;
                for (int j = 0; j < 8; j++)
                {
                    crc = (crc & 0x0001) != 0 ? (ushort)((crc >> 1) ^ 0xA001) : (ushort)(crc >> 1);
                }
                table[i] = crc;
            }
            return table;
        }

        private ushort CreateCrc16CheckCode(ReadOnlySpan<byte> data)
        {
            ushort crc = 0xFFFF;
            foreach (byte b in data)
            {
                crc = (ushort)((crc >> 8) ^ _crcTable[(crc ^ b) & 0xFF]);
            }
            return crc;
        }

        protected override ReadOnlyMemory<byte> GetRequestFrame(ModbusReadParameter parameter)
        {
            Span<byte> requestFrame = stackalloc byte[8];
            requestFrame[0] = parameter.Slave;
            requestFrame[1] = (byte)parameter.FunctionCode;
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[2..4], parameter.StartingAddress);
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[4..6], parameter.Count);
            var crcCheckCode = CreateCrc16CheckCode(requestFrame[..^2]);
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[^2..], crcCheckCode);
            return requestFrame.ToArray();
        }

        protected override ReadOnlyMemory<byte> GetRequestFrame(ModbusWriteParameter parameter)
        {
            var Length = 7 + parameter.Values.Length + 2;
            Span<byte> requestFrame = stackalloc byte[Length];
            requestFrame[0] = parameter.Slave;
            requestFrame[1] = (byte)parameter.FunctionCode;
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[2..4], parameter.StartingAddress);
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[4..6], parameter.Count);
            requestFrame[6] = (byte)parameter.Values.Length;
            parameter.Values.Span.CopyTo(requestFrame[7..]);
            var crcCheckCode = CreateCrc16CheckCode(requestFrame[..^2]);
            BinaryPrimitives.WriteUInt16LittleEndian(requestFrame[^2..], crcCheckCode);

            return requestFrame.ToArray();
        }

        protected override int GetResponseFrameLength(ModbusReadParameter parameter)
        {
            return parameter.FunctionCode switch
            {
                ModbusReadFunctionCodeEnum.ReadCoilStatus => ((parameter.Count + 7) >> 3) + 5,
                ModbusReadFunctionCodeEnum.ReadInputCoilStatus => ((parameter.Count + 7) >> 3) + 5,
                ModbusReadFunctionCodeEnum.ReadHoldingRegister => parameter.Count * 2 + 5,
                ModbusReadFunctionCodeEnum.ReadInputRegister => parameter.Count * 2 + 5,
                _ => throw new Exception("Invalid function code"),
            };
        }

        protected override int GetResponseFrameLength(ModbusWriteParameter parameter) => 8;

        protected override ReadOnlyMemory<byte> GetDataAndValidate(ReadOnlyMemory<byte> responseFrame)
        {
            ValidateData(responseFrame);

            return responseFrame[3..^2];
        }

        protected override void ValidateData(ReadOnlyMemory<byte> responseFrame, ReadOnlySpanAction<byte, byte>? onValidated = default)
        {
            var responseFrameSpan = responseFrame.Span;
            var crcCheckCode = CreateCrc16CheckCode(responseFrameSpan[..^2]);
            var responseFrameCrcCheckCode = BinaryPrimitives.ReadUInt16LittleEndian(responseFrameSpan[^2..]);

            if (crcCheckCode != responseFrameCrcCheckCode)
            {
                throw new Exception("Data transmission error: checksum mismatch");
            }

            if (responseFrameSpan[1] > 0x80)
            {

                throw new Exception(responseFrameSpan.ToHexString('-'));
            }

        }



    }
}
