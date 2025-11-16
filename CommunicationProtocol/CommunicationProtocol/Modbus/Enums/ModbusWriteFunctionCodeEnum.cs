using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommunicationProtocol.Modbus.Enums
{
    public enum ModbusWriteFunctionCodeEnum
    {

        //[Description("WriteCoilStatus")]
        //WriteCoilStatus = 0x05,

        [Description("WriteMultipleCoilStatus")]
        WriteMultipleCoilStatus = 0x0F,

        //[Description("WriteHoldingRegister")]
        //WriteHoldingRegister = 0x06,

        [Description("WriteMultipleHoldingRegister")]
        WriteMultipleHoldingRegister = 0x10,
    }
}
