using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Enums
{
    public enum S7CommDataTypeEnum
    {
        [Description("BOOL")]
        BOOL,

        [Description("SBYTE")]
        SBYTE,

        [Description("BYTE")]
        BYTE,

        [Description("SHORT")]
        SHORT,

        [Description("USHORT")]
        USHORT,

        [Description("INT")]
        INT,

        [Description("UINT")]
        UINT,

        [Description("FLOAT")]
        FLOAT,

        //[Description("STRING")]
        //STRING
    }
}
