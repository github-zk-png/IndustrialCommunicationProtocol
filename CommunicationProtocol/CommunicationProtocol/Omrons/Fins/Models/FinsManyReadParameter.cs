using CommunicationProtocol.Omrons.Fins.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Models
{
    public class FinsManyReadParameter : FinsOperateParameter
    {
        public byte TransactionId { get; set; }

        public FinsDataTypeEnum DataType { get; set; }
    }
}
