using CommunicationProtocol.Modbus.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CommunicationProtocol.Modbus.Extensions
{
    internal static class ModbusExtension
    {

        private readonly static IReadOnlyDictionary<char, ModbusReadFunctionCodeEnum> _readFunctionCodes =new Dictionary<char, ModbusReadFunctionCodeEnum>
        {
             ['0'] = ModbusReadFunctionCodeEnum.ReadCoilStatus,
             ['1'] = ModbusReadFunctionCodeEnum.ReadInputCoilStatus,
             ['3'] = ModbusReadFunctionCodeEnum.ReadInputRegister,
             ['4'] = ModbusReadFunctionCodeEnum.ReadHoldingRegister,
        };

        private readonly static IReadOnlyDictionary<char, ModbusWriteFunctionCodeEnum> _writeFunctionCodes = new Dictionary<char, ModbusWriteFunctionCodeEnum>
        {
            ['0'] = ModbusWriteFunctionCodeEnum.WriteMultipleCoilStatus,
            ['4'] = ModbusWriteFunctionCodeEnum.WriteMultipleHoldingRegister,
        };

        internal static (ModbusReadFunctionCodeEnum, ushort) ReadRegionalAddressParse(this string regionalAddress)
        {
            var regionalAddressSpan = regionalAddress.AsSpan();

            var functionCode=_readFunctionCodes[regionalAddressSpan[0]];

            var startingAddress=ushort.Parse(regionalAddressSpan[1..]);

            return (functionCode, startingAddress);
        }

        internal static (ModbusWriteFunctionCodeEnum, ushort) WriteRegionalAddressParse(this string regionalAddress)
        {
            var regionalAddressSpan = regionalAddress.AsSpan();

            var functionCode = _writeFunctionCodes[regionalAddressSpan[0]];

            var startingAddress = ushort.Parse(regionalAddressSpan[1..]);

            return (functionCode, startingAddress);
        }

    }
}
