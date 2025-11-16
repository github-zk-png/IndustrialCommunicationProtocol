using CommunicationProtocol.Siemens.S7Comms.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Models
{
    public class S7CommReturnTypeData
    {
        public byte TransactionId { get; set; }

        public S7CommDataTypeEnum DataType { get; set; }

        public object Value { get; set; } = default!;
    }
}
