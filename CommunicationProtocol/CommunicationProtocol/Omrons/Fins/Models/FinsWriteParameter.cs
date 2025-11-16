using CommunicationProtocol.Omrons.Fins.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Models
{
    public class FinsWriteParameter : FinsOperateParameter
    {
        public ReadOnlyMemory<byte> Values { get; set; }

        public ushort Count { get; set; } = 1;
    }
}
