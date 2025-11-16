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
using System.Linq;
using System.Text;
using System.Threading;

namespace CommunicationProtocol.Omrons.Fins.Protocols
{
    internal abstract class FinsSerialPortAuxiliary : SerialPortProtocol
    {
        private int _transactionId = 0;

        private byte TransactionId => (byte)Interlocked.Increment(ref _transactionId);

        internal FinsSerialPortAuxiliary(FinsSerialPortConnectParameter parameter) : base(parameter)
        {

        }

        private byte CreateFcsCheckCode(ReadOnlySpan<byte> requestBody)
        {
            byte result = 0;
            for (int i = 0; i < requestBody.Length; i++)
            {
                result ^= requestBody[i];
            }

            return result;
        }

        protected int GetResponseFrameLength(FinsReadParameter parameter)
        {
            //@ 05 FA 00 40 00 00 91 0101 0000 0064 01C8 0000 36 *\r
            //@ 05 FA 00 40 00 00 D0 0101 0000 01 00 01 32 *\r
            var length = 23 + parameter.Count * FinsTool.DataTypeAsciiConuts[parameter.regionDataType] + 4;
            return length;
        }

        protected int GetResponseFrameLength(IReadOnlyCollection<FinsManyReadParameter> parameters)
        {
            var count = parameters.Sum(item => FinsTool.DataTypeAsciiConuts[item.regionDataType] + 2);

            return 23 + count + 4;
        }

        protected int GetResponseFrameLength(FinsWriteParameter parameter) => 27;

        protected ReadOnlyMemory<byte> GetRequestFrame(byte unitNumber, FinsReadParameter parameter)
        {

            Span<char> requestString = stackalloc char[32];
            requestString[0] = '@';
            unitNumber.TryFormat(requestString[1..3], out _, "00");
            requestString[3] = 'F';
            requestString[4] = 'A';
            requestString[5..12].Fill('0');
            TransactionId.TryFormat(requestString[12..14], out _, "X2");
            requestString[14] = '0';
            requestString[15] = '1';
            requestString[16] = '0';
            requestString[17] = '1';
            ((byte)parameter.regionDataType).TryFormat(requestString[18..20], out _, "X2");
            parameter.WordAddress.TryFormat(requestString[20..24], out _, "X4");
            parameter.BitAddress.TryFormat(requestString[24..26], out _, "X2");
            parameter.Count.TryFormat(requestString[26..30], out _, "X4");

            Span<byte> requestFrame = stackalloc byte[34];

            Encoding.ASCII.GetBytes(requestString, requestFrame);

            var fcsCheckCode = CreateFcsCheckCode(requestFrame[..^4]);

            requestFrame[30] = ((byte)(fcsCheckCode >> 4)).ToHexChar();
            requestFrame[31] = ((byte)(fcsCheckCode & 0x0F)).ToHexChar();
            requestFrame[32] = 0x2A;
            requestFrame[33] = 0x0D;

            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(byte unitNumber, IReadOnlyCollection<FinsManyReadParameter> parameters)
        {

            Span<char> requestString = stackalloc char[18 + parameters.Count * 8];
            requestString[0] = '@';
            unitNumber.TryFormat(requestString[1..3], out _, "00");
            requestString[3] = 'F';
            requestString[4] = 'A';
            requestString[5..12].Fill('0');
            TransactionId.TryFormat(requestString[12..14], out _, "X2");
            requestString[14] = '0';
            requestString[15] = '1';
            requestString[16] = '0';
            requestString[17] = '4';

            var index = 18;
            foreach (var parameter in parameters)
            {
                ((byte)parameter.regionDataType).TryFormat(requestString[index..(index + 2)], out _, "X2");
                parameter.WordAddress.TryFormat(requestString[(index + 2)..(index + 6)], out _, "X4");
                parameter.BitAddress.TryFormat(requestString[(index + 6)..(index + 8)], out _, "X2");
                index += 8;
            }

            Span<byte> requestFrame = stackalloc byte[requestString.Length + 4];

            Encoding.ASCII.GetBytes(requestString, requestFrame);

            var fcsCheckCode = CreateFcsCheckCode(requestFrame[..^4]);

            requestFrame[^4] = ((byte)(fcsCheckCode >> 4)).ToHexChar();
            requestFrame[^3] = ((byte)(fcsCheckCode & 0x0F)).ToHexChar();
            requestFrame[^2] = 0x2A;
            requestFrame[^1] = 0x0D;

            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(byte unitNumber, FinsWriteParameter parameter)
        {
            var vaulsesLength = parameter.Values.Length * 2;
            Span<char> requestString = stackalloc char[30 + vaulsesLength];
            requestString[0] = '@';
            unitNumber.TryFormat(requestString[1..3], out _, "00");
            requestString[3] = 'F';
            requestString[4] = 'A';
            requestString[5..12].Fill('0');
            TransactionId.TryFormat(requestString[12..14], out _, "X2");
            requestString[14] = '0';
            requestString[15] = '1';
            requestString[16] = '0';
            requestString[17] = '2';
            ((byte)parameter.regionDataType).TryFormat(requestString[18..20], out _, "X2");
            parameter.WordAddress.TryFormat(requestString[20..24], out _, "X4");
            parameter.BitAddress.TryFormat(requestString[24..26], out _, "X2");
            parameter.Count.TryFormat(requestString[26..30], out _, "X4");

            var values = parameter.Values.ToHexString().AsSpan();
            values.CopyTo(requestString.Slice(30, vaulsesLength));

            Span<byte> requestFrame = stackalloc byte[requestString.Length + 4];
            Encoding.ASCII.GetBytes(requestString, requestFrame);

            var fcsCheckCode = CreateFcsCheckCode(requestFrame[..^4]);

            requestFrame[^4] = ((byte)(fcsCheckCode >> 4)).ToHexChar();
            requestFrame[^3] = ((byte)(fcsCheckCode & 0x0F)).ToHexChar();
            requestFrame[^2] = 0x2A;
            requestFrame[^1] = 0x0D;

            Encoding.ASCII.GetBytes(requestString, requestFrame);

            return requestFrame.ToArray();


        }

        public ReadOnlyMemory<byte> GetDataAndValidate(ReadOnlyMemory<byte> responseFrame)
        {
            Memory<byte> responseFrameData = default;
            ValidateData(responseFrame, (responseFrameBody, _) =>
            {
                responseFrameData = responseFrameBody[11..^1].ToArray();
            });

            return responseFrameData;
        }

        public IReadOnlyList<FinsReturnTypeData> GetTypeDataAndValidate(ReadOnlyMemory<byte> responseFrame, IReadOnlyCollection<FinsManyReadParameter> parameters)
        {
            var returnTypeDatas = new List<FinsReturnTypeData>(parameters.Count);
            ValidateData(responseFrame, (responseFrameBody, _) =>
            {
                var index = 12;
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

        public void ValidateData(ReadOnlyMemory<byte> responseFrame, ReadOnlySpanAction<byte, byte>? GetDataBytes = default)
        {
           
            var responseFrameSpan = responseFrame.Span;

            var hexLength = responseFrameSpan.Length - 3;
            if (hexLength % 2 != 0)
            {
                throw new Exception("The length of the ASCII hexadecimal portion must be even");
            }

            Span<byte> responseFramBody = stackalloc byte[hexLength / 2];
            if (!responseFrameSpan[1..^2].TryParseAsciiHex(responseFramBody))
            {
                throw new Exception("Invalid ASCII hexadecimal format");
            }
            if (responseFramBody[9] != 0x00 || responseFramBody[10] != 0x00)
            {
                throw new Exception($"Status Code：{responseFrameSpan.ToHexString('-')}");
            }

            var fcsCheckCode = CreateFcsCheckCode(responseFrameSpan[..^4]);
            if (fcsCheckCode != responseFramBody[^1])
            {
                throw new Exception($"FCS Checksum Error ：{responseFrameSpan.ToHexString('-')}");
            }

            GetDataBytes?.Invoke(responseFramBody, default);
        }
    }
}
