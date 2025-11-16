using CommunicationProtocol.Modbus.Enums;

namespace CommunicationProtocol.Modbus.Models
{
    public abstract class ModbusOperateParameter
    {
        public byte Slave { get; set; } = 1;

        public ushort StartingAddress { get; set; } = 0;

        public ushort Count { get; set; } = 1;

    }
}
