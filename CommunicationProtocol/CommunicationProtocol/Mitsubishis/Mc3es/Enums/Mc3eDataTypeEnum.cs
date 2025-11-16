using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Enums
{
    public enum Mc3eDataTypeEnum
    {
        [Description("BIT")]
        BIT = 0x01,

        [Description("WORD")]
        WORD = 0x00
    }
}
