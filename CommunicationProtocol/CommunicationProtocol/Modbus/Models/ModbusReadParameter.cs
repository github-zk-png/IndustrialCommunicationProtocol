using CommunicationProtocol.Modbus.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Modbus.Models
{
    public class ModbusReadParameter : ModbusOperateParameter
    {
        public ModbusReadFunctionCodeEnum  FunctionCode{ get; set; } = ModbusReadFunctionCodeEnum.ReadHoldingRegister;
    
    }
}
