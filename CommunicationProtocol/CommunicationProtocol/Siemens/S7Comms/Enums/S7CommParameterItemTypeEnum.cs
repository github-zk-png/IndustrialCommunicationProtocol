using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Enums
{
    public enum S7CommParameterItemTypeEnum
    {
        [Description("BIT")]
        BIT = 0x01,

        [Description("BYTE")] 
        BYTE = 0x02,

        //[Description("CHAR")]
        //CHAR = 0x03,

        //[Description("WORD")]
        //WORD = 0x04,

        //[Description("INT")]
        //INT = 0x05,

        //[Description("双字节")]
        //DWORD = 0x06,

        //[Description("DINT")]
        //DINT = 0x07,

        //[Description("REAL")]
        //REAL = 0x08,

        //[Description("DATE")]
        //DATE = 0x09
    }
}
