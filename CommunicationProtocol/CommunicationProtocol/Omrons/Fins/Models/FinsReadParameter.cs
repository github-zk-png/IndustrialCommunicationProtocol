using CommunicationProtocol.Omrons.Fins.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Models
{
    public class FinsReadParameter : FinsOperateParameter
    {
        public ushort Count { get; set; } = 1;
    }
}
