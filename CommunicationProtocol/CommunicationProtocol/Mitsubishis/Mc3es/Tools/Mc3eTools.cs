using CommunicationProtocol.Mitsubishis.Mc3es.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Tools
{
    internal class Mc3eTools
    {
        private readonly static IReadOnlyDictionary<Mc3eRegionEnum, Mc3eDataTypeEnum> _bitTypeRegions = new Dictionary<Mc3eRegionEnum, Mc3eDataTypeEnum>()
        {
            [Mc3eRegionEnum.X] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.Y] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.M] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.L] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.S] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.B] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.F] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.TS] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.TC] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.CS] = Mc3eDataTypeEnum.BIT,
            [Mc3eRegionEnum.CC] = Mc3eDataTypeEnum.BIT,
        };

        internal static IReadOnlyDictionary<Mc3eRegionEnum, Mc3eDataTypeEnum> BitTypeRegions => _bitTypeRegions;

        private readonly static IReadOnlyDictionary<Mc3eRegionEnum, Mc3eDataTypeEnum> _wordTypeRegions = new Dictionary<Mc3eRegionEnum, Mc3eDataTypeEnum>()
        {
            [Mc3eRegionEnum.TN] = Mc3eDataTypeEnum.WORD,
            [Mc3eRegionEnum.CN] = Mc3eDataTypeEnum.WORD,
            [Mc3eRegionEnum.D] = Mc3eDataTypeEnum.WORD,
            [Mc3eRegionEnum.W] = Mc3eDataTypeEnum.WORD,
            [Mc3eRegionEnum.R] = Mc3eDataTypeEnum.WORD,

        };

        internal static IReadOnlyDictionary<Mc3eRegionEnum, Mc3eDataTypeEnum> WordTypeRegions => _wordTypeRegions;
    }
}
