using CommunicationProtocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Models
{
    public class FinsTcpConnectParameter : TcpConnectParameter
    {
        public override int Port { get; set; } = 9600;
    }
}
