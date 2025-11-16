using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Enums
{
    public enum S7CommDataItemTypeEnum
    {
        //[Description("NULL")]
        //NULL = 0x00,

        [Description("BIT")]
        BIT = 0x03,

        [Description("BYTE")]
        BYTE = 0x04,

        //[Description("WORD")]    
        //WORD = 0x04,

        //[Description("DWORD")]
        //DWORD = 0x04,

        //[Description("INTEGER")]
        //INTEGER = 0x05,

        //[Description("REAL")]
        //REAL = 0x07,

        //[Description("OCTETSTRING")]
        //OCTETSTRING = 0x09
    }
}
