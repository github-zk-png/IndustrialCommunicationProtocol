using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.OPC.OpcUas.Models
{
    public class OpcUaReturnData
    {
        public byte TransactionId { get; set; }

        public string Name { get; set; } = default!;

        public object Value { get; set; } = default!;
    }
}
