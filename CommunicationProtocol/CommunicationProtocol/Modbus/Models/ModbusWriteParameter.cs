using CommunicationProtocol.Modbus.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Modbus.Models
{
    public class ModbusWriteParameter : ModbusOperateParameter
    {
        public ModbusWriteFunctionCodeEnum FunctionCode { get; set; } = ModbusWriteFunctionCodeEnum.WriteMultipleHoldingRegister;

        public ReadOnlyMemory<byte> Values { get; set; }
    }
}
