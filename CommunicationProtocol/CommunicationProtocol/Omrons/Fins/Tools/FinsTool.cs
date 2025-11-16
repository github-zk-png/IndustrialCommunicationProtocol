
using CommunicationProtocol.Omrons.Fins.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Tools
{
    internal class FinsTool
    {
        private readonly static IReadOnlyDictionary<FinsRegionDataTypeEnum, byte> _dataTypeAsciiConuts = new Dictionary<FinsRegionDataTypeEnum, byte>()
        {
            [FinsRegionDataTypeEnum.CIOBit] = 2,
            [FinsRegionDataTypeEnum.HRBit] = 2,
            [FinsRegionDataTypeEnum.ARBit] = 2,
            [FinsRegionDataTypeEnum.IRBit] = 2,
            [FinsRegionDataTypeEnum.WRBit] = 2,
            [FinsRegionDataTypeEnum.DMBit] = 2,
            [FinsRegionDataTypeEnum.TKBit] = 2,
            [FinsRegionDataTypeEnum.DRBit] = 2,


            [FinsRegionDataTypeEnum.CIOWord] = 4,
            [FinsRegionDataTypeEnum.DMWord] = 4,
            [FinsRegionDataTypeEnum.WRWord] = 4,
            [FinsRegionDataTypeEnum.IRWord] = 4,
            [FinsRegionDataTypeEnum.ARWord] = 4,
            [FinsRegionDataTypeEnum.HRWord] = 4,
            [FinsRegionDataTypeEnum.TKWord] = 4,
            [FinsRegionDataTypeEnum.DRWord] = 4,

        };

        internal static IReadOnlyDictionary<FinsRegionDataTypeEnum, byte> DataTypeAsciiConuts => _dataTypeAsciiConuts;

        private readonly static IReadOnlyDictionary<FinsDataTypeEnum, byte> _typeByteCounts = new Dictionary<FinsDataTypeEnum, byte>()
        {
            [FinsDataTypeEnum.BOOL] = 1,
            [FinsDataTypeEnum.SHORT] = 2,
            [FinsDataTypeEnum.USHORT] = 2,

        };

        internal static IReadOnlyDictionary<FinsDataTypeEnum, byte> TypeByteCounts => _typeByteCounts;


    }
}
