using CommunicationProtocol.Siemens.S7Comms.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Siemens.S7Comms.Models
{
    public class S7CommWriteParameter : S7CommBasicOperateParameter
    {

        public S7CommDataItemTypeEnum DataItemType { get; set; }

        public ReadOnlyMemory<byte> Values { get; set; }

    }
}
