using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Enums
{
    public enum S7CommRegionEnum
    {
        [Description("I")]
        I = 0x81,

        [Description("Q")]
        Q = 0x82,

        [Description("M")]
        M = 0x83,

        [Description("DB")]
        DB = 0x84,

        [Description("V")]
        V = 0x84,


    }
}
