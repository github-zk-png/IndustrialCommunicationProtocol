using CommunicationProtocol.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Omrons.OmronCips.Models
{
    public class OmronCipConnectParameter : TcpConnectParameter
    {
        public override int Port { get; set; } = 44818;
    }
}
