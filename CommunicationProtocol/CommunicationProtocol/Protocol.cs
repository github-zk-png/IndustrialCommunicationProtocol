using CommunicationProtocol.Mitsubishis.Mc3es.Models;
using CommunicationProtocol.Mitsubishis.Mc3es.Protocols;
using CommunicationProtocol.Modbus.Models;
using CommunicationProtocol.Modbus.Protocols;
using CommunicationProtocol.Models;
using CommunicationProtocol.Omrons.Fins.Models;
using CommunicationProtocol.Omrons.Fins.Protocols;
using CommunicationProtocol.Omrons.OmronCips.Models;
using CommunicationProtocol.Omrons.OmronCips.Protocols;
using CommunicationProtocol.OPC.OpcUas.Models;
using CommunicationProtocol.OPC.OpcUas.Protocols;
using CommunicationProtocol.Siemens.S7Comms.Models;
using CommunicationProtocol.Siemens.S7Comms.Protocols;

namespace CommunicationProtocol
{
    public class Protocol
    {
        public static IModbusRtu CreateModbusRtu(ModbusSerialPortConnectParameter? parameter = default)
        {
            return new ModbusRtu(parameter ?? new ModbusSerialPortConnectParameter());
        }

        public static IModbusAscll CreateModbusAscll(ModbusSerialPortConnectParameter? parameter = default)
        {
            return new ModbusAscll(parameter ?? new ModbusSerialPortConnectParameter());
        }

        public static IModbusTcp CreateModbusTcp(ModbusTcpConnectParameter parameter)
        {
            return new ModbusTcp(parameter);
        }

        public static IS7Comm CreateS7Comm(S7CommConnectParameter parameter)
        {
            return new S7Comm(parameter);
        }

        public static IFinsSerialPort CreateFinsSerialPort(FinsSerialPortConnectParameter? parameter = default)
        {
            return new FinsSerialPort(parameter ?? new FinsSerialPortConnectParameter());
        }

        public static IFinsTcp CreateFinsTcp(FinsTcpConnectParameter parameter)
        {
            return new FinsTcp(parameter);
        }

        public static IOmronCip CreateOmronCip(OmronCipConnectParameter parameter)
        {
            return new OmronCip(parameter);
        }

        public static IMc3e CreateMc3e(Mc3eConnectParameter parameter)
        {
            return new Mc3e(parameter);
        }

        public static IOpcUa CreateOpcUa()
        {
            return new OpcUa();
        }
    }
}
