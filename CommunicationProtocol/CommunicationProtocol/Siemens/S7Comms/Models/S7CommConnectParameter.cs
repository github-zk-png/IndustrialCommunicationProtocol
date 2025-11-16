using CommunicationProtocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Models
{
    public class S7CommConnectParameter: TcpConnectParameter
    {
        public override int Port { get; set; } = 102;

        public byte Rack { get; set; }

        public byte Slot { get; set; }
    }
}
