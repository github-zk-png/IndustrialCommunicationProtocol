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
        protected SerialPort _serialPort;

        public SerialPort SerialPort => _serialPort;

        protected SerialPortProtocol(SerialPortConnectParameter parameter)
        {
            _serialPort = new SerialPort
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
            _serialPort.Write(requestFrame.ToArray(), 0, requestFrame.Length);

            var responseFrame = new byte[length];
            var index = 0;
            while (index < responseFrame.Length)
            {

                try
                {
                    var readLength = _serialPort.Read(responseFrame, index, responseFrame.Length - index);

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
            _serialPort.Open();

            return _serialPort.IsOpen;
        }

        public void Dispose()
        {
            _serialPort?.Dispose();
            _serialPort?.Close();

        }

    }
}
