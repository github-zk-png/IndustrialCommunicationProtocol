using CommunicationProtocol.Siemens.S7Comms.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Models
{
    public class S7CommBasicOperateParameter: S7CommOperateParameter
    {
        public S7CommParameterItemTypeEnum ParameterItemType { get; set; }

        public ushort Count { get; set; } = 1;

    }
}
