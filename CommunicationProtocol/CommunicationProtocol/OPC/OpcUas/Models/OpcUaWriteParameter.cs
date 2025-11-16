using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.OPC.OpcUas.Models
{
    public class OpcUaWriteParameter
    {
        public string Name { get; set; } = default!;

        public object Value { get; set; } = default!;
    }
}
