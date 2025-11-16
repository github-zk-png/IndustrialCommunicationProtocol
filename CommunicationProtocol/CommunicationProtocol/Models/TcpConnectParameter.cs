using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Models
{
    public class TcpConnectParameter
    {
        public string Host { get; set; } = string.Empty;

        public virtual int Port { get; set; }
    }
}
