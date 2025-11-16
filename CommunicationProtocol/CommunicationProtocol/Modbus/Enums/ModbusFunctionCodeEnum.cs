using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Modbus.Enums
{
    public enum ModbusReadFunctionCodeEnum
    {

        [Description("ReadInputCoilStatus")]
        ReadInputCoilStatus = 0x02,

        [Description("ReadInputRegister")]
        ReadInputRegister = 0x04,

        [Description("ReadCoilStatus")]
        ReadCoilStatus = 0x01,

        [Description("ReadHoldingRegister")]
        ReadHoldingRegister = 0x03,

    }
}
