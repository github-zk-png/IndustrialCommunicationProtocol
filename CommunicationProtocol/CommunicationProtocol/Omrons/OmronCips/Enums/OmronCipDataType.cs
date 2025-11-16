using System.ComponentModel;


namespace CommunicationProtocol.Omrons.OmronCips.Enums
{
    public enum OmronCipDataType
    {
        [Description("BOOL")]
        BOOL = 0xC1,

        [Description("SINT")]
        SINT = 0xC2,

        [Description("INT")]
        INT = 0xC3,

        [Description("DINT")]
        DINT = 0xC4,

        [Description("LINT")]
        LINT = 0xC5,

        [Description("USINT")]
        USINT = 0xC6,

        [Description("UINT")]
        UINT = 0xC7,

        [Description("UDINT")]
        UDINT = 0xC8,

        [Description("ULINT")]
        ULINT = 0xC9,

        [Description("REAL")]
        REAL = 0xCA,

        [Description("LREAL")]
        LREAL = 0xCB,

        [Description("STRING")]
        STRING = 0xD0,

        [Description("BYTE")]
        BYTE = 0xD1,

        [Description("WORD")]
        WORD = 0xD2,

        [Description("DWORD")]
        DWORD = 0xD3,

        [Description("LWORD")]
        LWORD = 0xD4
    }
}
