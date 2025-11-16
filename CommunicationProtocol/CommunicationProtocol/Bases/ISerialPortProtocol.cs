using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace CommunicationProtocol.Bases
{
    public interface ISerialPortProtocol : IDisposable
    {
        public SerialPort SerialPort { get; }

        public bool Connect();
    }
}
