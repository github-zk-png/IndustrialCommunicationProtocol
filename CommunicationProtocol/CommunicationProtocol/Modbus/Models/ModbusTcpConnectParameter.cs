using CommunicationProtocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Modbus.Models
{
    public class ModbusTcpConnectParameter : TcpConnectParameter
    {
        public override int Port { get; set; } = 502;
    }
}
