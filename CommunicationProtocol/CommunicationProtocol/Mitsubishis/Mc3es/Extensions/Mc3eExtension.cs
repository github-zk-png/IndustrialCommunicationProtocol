using CommunicationProtocol.Mitsubishis.Mc3es.Enums;
using CommunicationProtocol.Mitsubishis.Mc3es.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Extensions
{
    internal static class Mc3eExtension
    {

        private readonly static IReadOnlyDictionary<Mc3eRegionEnum, Mc3eDataTypeEnum> _TypeRegions = new Dictionary<Mc3eRegionEnum, Mc3eDataTypeEnum>()
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
            [Mc3eRegionEnum.TN] = Mc3eDataTypeEnum.WORD,
            [Mc3eRegionEnum.CN] = Mc3eDataTypeEnum.WORD,
            [Mc3eRegionEnum.D] = Mc3eDataTypeEnum.WORD,
            [Mc3eRegionEnum.W] = Mc3eDataTypeEnum.WORD,
            [Mc3eRegionEnum.R] = Mc3eDataTypeEnum.WORD,
        };

        private readonly static IReadOnlyDictionary<string, Mc3eRegionEnum> _stringRegions = new Dictionary<string, Mc3eRegionEnum>()
        {
            ["X"] = Mc3eRegionEnum.X,
            ["Y"] = Mc3eRegionEnum.Y,
            ["M"] = Mc3eRegionEnum.M,
            ["L"] = Mc3eRegionEnum.L,
            ["S"] = Mc3eRegionEnum.S,
            ["B"] = Mc3eRegionEnum.B,
            ["F"] = Mc3eRegionEnum.F,
            ["TS"] = Mc3eRegionEnum.TS,
            ["TC"] = Mc3eRegionEnum.TC,
            ["CS"] = Mc3eRegionEnum.CS,
            ["CC"] = Mc3eRegionEnum.CC,
            ["TN"] = Mc3eRegionEnum.TN,
            ["CN"] = Mc3eRegionEnum.CN,
            ["D"] = Mc3eRegionEnum.D,
            ["W"] = Mc3eRegionEnum.W,
            ["R"] = Mc3eRegionEnum.R,
        };


        internal static Mc3eOperateParameter RegionalAddressParse(this string regionalAddress)
        {
            var regionalAddressSpan = regionalAddress.ToUpperInvariant().AsSpan();

            var regionalSpan = regionalAddressSpan[..2];

            var region = default(Mc3eRegionEnum);

            var index = default(byte);
            if (_stringRegions.TryGetValue(regionalSpan.ToString(), out region))
            {
                index = 2;
            }
            else
            {
                region = _stringRegions[regionalAddressSpan[0].ToString()];
                index = 1;
            }
            var parameter = new Mc3eOperateParameter
            {
                Region = region,
                DataType = _TypeRegions[region],
                Address = ushort.Parse(regionalAddressSpan[index..])
            };

            return parameter;

        }
    }
}
