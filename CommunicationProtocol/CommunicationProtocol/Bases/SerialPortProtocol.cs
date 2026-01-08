using CommunicationProtocol.Extensions;
using CommunicationProtocol.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationProtocol.Bases
{
    internal abstract class SerialPortProtocol
    {
        public SerialPort SerialPort { get; protected set; }

        protected SerialPortProtocol(SerialPortConnectParameter parameter)
        {
            SerialPort = new SerialPort
            {
                PortName = parameter.PortName,
                BaudRate = parameter.BaudRate,
                DataBits = parameter.DataBits,
                Parity = parameter.Parity,
                StopBits = parameter.StopBits,
                ReadTimeout = 2000,
            };

        }

        protected virtual ReadOnlyMemory<byte> SendAndReceive(ReadOnlyMemory<byte> requestFrame, int length)
        {
            SerialPort.Write(requestFrame.ToArray(), 0, requestFrame.Length);

            var responseFrame = new byte[length];
            var index = 0;
            while (index < responseFrame.Length)
            {

                try
                {
                    var readLength = SerialPort.Read(responseFrame, index, responseFrame.Length - index);

                    index += readLength;
                }
                catch (Exception)
                {
                    throw new Exception("超时：" + responseFrame.ToHexString('-'));
                }
            }

            return responseFrame;
        }

        public bool Connect()
        {
            SerialPort.Open();

            return SerialPort.IsOpen;
        }

        public void Dispose()
        {
            SerialPort?.Dispose();
            SerialPort?.Close();

        }

    }
}
