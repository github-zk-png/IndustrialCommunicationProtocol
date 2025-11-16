using CommunicationProtocol.Mitsubishis.Mc3es.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Models
{
    public class Mc3eOperateParameter
    {
        public Mc3eRegionEnum Region { get; set; }

        public Mc3eDataTypeEnum DataType { get; set; }

        public ushort Address { get; set; }

        public uint Count { get; set; } = 1;
    }
}
