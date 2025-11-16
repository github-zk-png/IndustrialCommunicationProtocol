using CommunicationProtocol.Omrons.Fins.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Models
{
    public class FinsOperateParameter 
    {
        public FinsRegionDataTypeEnum regionDataType { get; set; }

        public ushort WordAddress { get; set; }

        public byte BitAddress { get; set; }

    }
}
