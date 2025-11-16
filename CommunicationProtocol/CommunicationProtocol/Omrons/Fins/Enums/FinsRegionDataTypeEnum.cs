using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Enums
{
    public enum FinsRegionDataTypeEnum
    {
        [Description("CIOBit")]
        CIOBit = 0x30,

        [Description("WRBit")]    
        WRBit = 0x31,

        [Description("HRBit")]
        HRBit = 0x32,

        [Description("ARBit")]
        ARBit = 0x33,

        [Description("DMBit")]
        DMBit = 0x02,

        [Description("TKBit")]
        TKBit = 0x06,

        [Description("IRBit")]
        IRBit = 0xDC,

        [Description("DRBit")]
        DRBit = 0xBC,

        [Description("CIOWord")]
        CIOWord = 0xB0,

        [Description("WRWord")]
        WRWord = 0xB1,

        [Description("HRWord")]
        HRWord = 0xB2,

        [Description("ARWord")]
        ARWord = 0xB3,

        [Description("DMWord")]
        DMWord = 0x82,

        [Description("TKWord")] 
        TKWord = 0x86,

        [Description("IRWord")]
        IRWord = 0x9C,

        [Description("DRWord")]
        DRWord = 0x9B,

    }
}
