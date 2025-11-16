using CommunicationProtocol.Siemens.S7Comms.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Siemens.S7Comms.Models
{
    public class S7CommTypeWriteParameter : S7CommOperateParameter
    {

        public S7CommDataTypeEnum DataType { get; set; }

        public object Value { get; set; } = default!;

    }
}
