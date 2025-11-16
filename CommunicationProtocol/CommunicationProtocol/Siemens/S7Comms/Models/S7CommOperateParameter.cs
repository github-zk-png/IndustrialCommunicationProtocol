using CommunicationProtocol.Siemens.S7Comms.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Models
{
    public abstract class S7CommOperateParameter 
    {
        public S7CommRegionEnum Region { get; set; }

        public ushort DbNumber { get; set; }

        public int ByteAddress { get; set; }

        public byte BitAddress { get; set; }
    }
}
