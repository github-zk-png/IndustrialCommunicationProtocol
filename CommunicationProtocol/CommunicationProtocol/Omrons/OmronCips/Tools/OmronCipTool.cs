
using CommunicationProtocol.Omrons.OmronCips.Enums;
using System.Collections.Generic;

namespace CommunicationProtocol.Omrons.OmronCips.Tools
{
    internal class OmronCipTool
    {
        private readonly static IReadOnlyDictionary<byte, byte> _typeByteConuts = new Dictionary<byte, byte>()
        {
            [0xC1] = 1,
            [0xC2] = 1,
            [0xC3] = 2,
            [0xC4] = 4,
            [0xC5] = 8,
            [0xC6] = 1,
            [0xC7] = 2,
            [0xC8] = 4,
            [0xC9] = 8,
            [0xCA] = 4,
            [0xCB] = 8,
            [0xD0] = 0,
            [0xD1] = 1,
            [0xD2] = 2,
            [0xD3] = 4,
            [0xD4] = 8,
        };

        internal static IReadOnlyDictionary<byte, byte> TypeByteConuts => _typeByteConuts;

        private readonly static IReadOnlyDictionary<OmronCipDataType, byte> _singleByteTypeConuts = new Dictionary<OmronCipDataType, byte>()
        {
            [OmronCipDataType.BOOL] = 1,
            [OmronCipDataType.SINT] = 1,
            [OmronCipDataType.UDINT] = 1,
            [OmronCipDataType.BYTE] = 1,
        };

        internal static IReadOnlyDictionary<OmronCipDataType, byte> SingleByteTypeConuts => _singleByteTypeConuts;

     
    }
}
