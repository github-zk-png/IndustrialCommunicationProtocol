using CommunicationProtocol.Extensions;
using CommunicationProtocol.Omrons.Fins.Extensions;
using CommunicationProtocol.Omrons.Fins.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Omrons.Fins.Protocols
{
    internal class FinsSerialPort : FinsSerialPortAuxiliary, IFinsSerialPort
    {
        public FinsSerialPort(FinsSerialPortConnectParameter parameter) : base(parameter)
        {
        }

        public string ReadString(byte unitNumber, string regionalAddress, byte length)
        {

            var parameter = regionalAddress.RegionalAddressParse();

            var readParameter = new FinsReadParameter
            {
                regionDataType = parameter.regionDataType,
                WordAddress = parameter.WordAddress,
                BitAddress = parameter.BitAddress,
                Count = length.GetRemainderPaddingCount()
            };

            var values = Read(unitNumber, readParameter);

            var value = Encoding.UTF8.GetString(values.Span[..^1]);

            return value;

        }

        public T Read<T>(byte unitNumber, string regionalAddress) where T : unmanaged
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


            return Read(unitNumber, readParameter).ToBigEndianUnmanagedType<T>(elementSize);
        }

        public ReadOnlyMemory<byte> Read(byte unitNumber, FinsReadParameter parameter)
        {

            var requestFrame = GetRequestFrame(unitNumber, parameter);

            var length = GetResponseFrameLength(parameter);

            var responseFrame = SendAndReceive(requestFrame, length);

            return GetDataAndValidate(responseFrame);
        }

        public IReadOnlyList<FinsReturnTypeData> Read(byte unitNumber, IReadOnlyList<FinsManyReadParameter> parameters)
        {

            var requestFrame = GetRequestFrame(unitNumber, parameters);

            var length = GetResponseFrameLength(parameters);

            var responseFrame = SendAndReceive(requestFrame, length);

            return GetTypeDataAndValidate(responseFrame, parameters);
        }

        public bool WriteString(byte unitNumber, string regionalAddress, string value)
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

            return Write(unitNumber, writeParameter);
        }

        public bool Write<T>(byte unitNumber, string regionalAddress, T value) where T : unmanaged
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

            return Write(unitNumber, writeParameter);
        }

        public bool Write(byte unitNumber, FinsWriteParameter parameter)
        {

            var requestFrame = GetRequestFrame(unitNumber, parameter);

            var length = GetResponseFrameLength(parameter);

            var responseFrame = SendAndReceive(requestFrame, length);

            ValidateData(responseFrame);

            return true;
        }

    }
}
