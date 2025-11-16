using System.ComponentModel;


namespace CommunicationProtocol.Mitsubishis.Mc3es.Enums
{
    public enum Mc3eRegionEnum
    {
        [Description("X")]
        X = 0x9C,

        [Description("Y")]  
        Y = 0x9D,

        [Description("M")]
        M = 0x90,

        [Description("L")]
        L = 0x92,

        [Description("S")]
        S = 0x98,

        [Description("B")]
        B = 0xA0,

        [Description("F")]
        F = 0x93,

        [Description("TS")]
        TS = 0xC1,

        [Description("TC")]
        TC = 0xC0,

        [Description("TN")]
        TN = 0xC2,

        [Description("CS")]
        CS = 0xC4,

        [Description("CC")]
        CC = 0xC3,

        [Description("CN")]
        CN = 0xC5,

        [Description("D")]
        D = 0xA8,

        [Description("W")]
        W = 0xB4,

        [Description("R")]
        R = 0xAF,

        //[Description("ZR")]
        //ZR = 0xB0
    }
}
