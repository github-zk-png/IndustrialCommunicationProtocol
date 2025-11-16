using CommunicationProtocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Models
{
    public class Mc3eConnectParameter : TcpConnectParameter
    {
        public override int Port { get; set; } = 6000;
    }
}
