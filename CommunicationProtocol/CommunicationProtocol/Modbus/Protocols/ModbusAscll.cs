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
    internal class ModbusAscll : ModbusSerialPort, IModbusAscll
    {
        internal ModbusAscll(ModbusSerialPortConnectParameter parameter) : base(parameter)
        {

        }

        private static byte CreateLrcCheckCode(ReadOnlySpan<byte> data)
        {
            if (data.Length == 0) return 0x00;

            int sum = 0;
            foreach (byte b in data)
            {
                sum += b;
            }

            return (byte)(-sum);
        }

        private ReadOnlyMemory<byte> ConvertToAsciiFrame(ReadOnlySpan<byte> binaryFrame)
        {
            var asciiLength = binaryFrame.Length * 2 + 3;
            Span<byte> asciiFrame = stackalloc byte[asciiLength];

            asciiFrame[0] = (byte)':';

            int pos = 1;
            foreach (byte b in binaryFrame)
            {
                asciiFrame[pos++] = ((byte)(b >> 4)).ToHexChar();
                asciiFrame[pos++] = ((byte)(b & 0x0F)).ToHexChar();
            }

            asciiFrame[asciiLength - 2] = (byte)'\r';
            asciiFrame[asciiLength - 1] = (byte)'\n';

            return asciiFrame.ToArray();
        }

        protected override ReadOnlyMemory<byte> GetRequestFrame(ModbusReadParameter parameter)
        {
            Span<byte> requestFrame = stackalloc byte[7];
            requestFrame[0] = parameter.Slave;
            requestFrame[1] = (byte)parameter.FunctionCode;
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[2..4], parameter.StartingAddress);
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[4..6], parameter.Count);
            var crcCheckCode = CreateLrcCheckCode(requestFrame[..^1]);
            requestFrame[^1] = crcCheckCode;

            return ConvertToAsciiFrame(requestFrame);

        }

        protected override ReadOnlyMemory<byte> GetRequestFrame(ModbusWriteParameter parameter)
        {
            var Length = 7 + parameter.Values.Length + 1;
            Span<byte> requestFrame = stackalloc byte[Length];
            requestFrame[0] = parameter.Slave;
            requestFrame[1] = (byte)parameter.FunctionCode;
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[2..4], parameter.StartingAddress);
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[4..6], parameter.Count);
            requestFrame[6] = (byte)parameter.Values.Length;
            parameter.Values.Span.CopyTo(requestFrame[7..]);
            var crcCheckCode = CreateLrcCheckCode(requestFrame[..^1]);
            requestFrame[^1] = crcCheckCode;

            return ConvertToAsciiFrame(requestFrame);
        }

        protected override int GetResponseFrameLength(ModbusReadParameter parameter)
        {

            return parameter.FunctionCode switch
            {
                ModbusReadFunctionCodeEnum.ReadCoilStatus => (((parameter.Count + 7) >> 3) + 4) * 2 + 3,
                ModbusReadFunctionCodeEnum.ReadInputCoilStatus => (((parameter.Count + 7) >> 3) + 4) * 2 + 3,
                ModbusReadFunctionCodeEnum.ReadHoldingRegister => (parameter.Count * 2 + 4) * 2 + 3,
                ModbusReadFunctionCodeEnum.ReadInputRegister => (parameter.Count * 2 + 4) * 2 + 3,
                _ => throw new Exception(" Invalid function code"),
            };

        }

        protected override int GetResponseFrameLength(ModbusWriteParameter parameter) => 17;

        protected override ReadOnlyMemory<byte> GetDataAndValidate(ReadOnlyMemory<byte> responseFrame)
        {
            Memory<byte> responseFrameData = default;
            ValidateData(responseFrame, (responseFrameBody, _) =>
            {
                responseFrameData = responseFrameBody[3..^1].ToArray();
            });

            return responseFrameData;
        }

        protected override void ValidateData(ReadOnlyMemory<byte> responseFrame, ReadOnlySpanAction<byte, byte>? GetDataBytes = default)
        {
            var hexLength = responseFrame.Length - 3;
            if (hexLength % 2 != 0)
            {
                throw new Exception("The length of the ASCII hexadecimal portion must be even");
            }

            Span<byte> responseFramBody = stackalloc byte[hexLength / 2];
            if (!responseFrame[1..^2].TryParseAsciiHex(responseFramBody))
            {
                throw new Exception("Invalid ASCII hexadecimal format");
            }

            var functionCode = responseFramBody[1];
            if (functionCode > 0x80)
            {
                byte exceptionCode = responseFramBody.Length > 2 ? responseFramBody[2] : (byte)0;
                throw new Exception($"Modbus Exception Response - Function Code: 0x{functionCode:X2}, Exception Code: 0x{exceptionCode:X2}");
            }

            var lrcCheckCode = CreateLrcCheckCode(responseFramBody[..^1]);
            var responseFrameLrcCheckCode = responseFramBody[^1];

            if (lrcCheckCode != responseFrameLrcCheckCode)
            {
                throw new Exception($"LRC Checksum failed - expected: 0x{lrcCheckCode:X2}, Actual: 0x{responseFrameLrcCheckCode:X2}");
            }

            GetDataBytes?.Invoke(responseFramBody, default);
        }


    }
}
