using CommunicationProtocol.Omrons.Fins.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Models
{
    public class FinsReturnTypeData 
    {
        public byte TransactionId { get; set; }

        public FinsDataTypeEnum DataType { get; set; }

        public object Value { get; set; } = default!;
    }
}
