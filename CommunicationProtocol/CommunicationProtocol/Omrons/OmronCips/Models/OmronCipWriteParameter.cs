using CommunicationProtocol.Omrons.OmronCips.Enums;
using System;

namespace CommunicationProtocol.Omrons.OmronCips.Models
{
    public class OmronCipWriteParameter
    {
        public string Name { get; set; } = default!;

        public OmronCipDataType dataType { get; set; }

        public ReadOnlyMemory<byte> Values { get; set; }
    }
}
