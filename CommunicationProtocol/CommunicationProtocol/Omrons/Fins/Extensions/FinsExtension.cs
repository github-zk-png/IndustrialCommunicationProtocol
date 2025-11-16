using CommunicationProtocol.Omrons.Fins.Enums;
using CommunicationProtocol.Omrons.Fins.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net;
using System.Text;

namespace CommunicationProtocol.Omrons.Fins.Extensions
{
    internal static class FinsExtension
    {

        private static IReadOnlyDictionary<string, FinsRegionDataTypeEnum> _bitRegionDataTypes = new Dictionary<string, FinsRegionDataTypeEnum>()
        {
            ["DM"] = FinsRegionDataTypeEnum.DMBit,
            ["CIO"] = FinsRegionDataTypeEnum.CIOBit,
            ["WR"] = FinsRegionDataTypeEnum.WRBit,
            ["HR"] = FinsRegionDataTypeEnum.HRBit,
            ["AR"] = FinsRegionDataTypeEnum.ARBit,
            ["TK"] = FinsRegionDataTypeEnum.TKBit,
            ["DR"] = FinsRegionDataTypeEnum.DRBit,
            ["IR"] = FinsRegionDataTypeEnum.IRBit,
        };


        private static IReadOnlyDictionary<string, FinsRegionDataTypeEnum> _wordRegionDataTypes = new Dictionary<string, FinsRegionDataTypeEnum>()
        {
            ["DM"] = FinsRegionDataTypeEnum.DMWord,
            ["CIO"] = FinsRegionDataTypeEnum.CIOWord,
            ["WR"] = FinsRegionDataTypeEnum.WRWord,
            ["HR"] = FinsRegionDataTypeEnum.HRWord,
            ["AR"] = FinsRegionDataTypeEnum.ARWord,
            ["TK"] = FinsRegionDataTypeEnum.TKWord,
            ["DR"] = FinsRegionDataTypeEnum.DRWord,
            ["IR"] = FinsRegionDataTypeEnum.IRWord,
        };

        internal static FinsOperateParameter RegionalAddressParse(this string regionalAddress)
        {
            var parameter = new FinsOperateParameter();

            var index = regionalAddress.IndexOf('.');

            var regionalAddressSpan = regionalAddress.ToUpperInvariant().AsSpan();
            var regional = regionalAddressSpan[..3];
            if (regional.StartsWith("CIO"))
            {
                if (index > 0)
                {
                    parameter.regionDataType = FinsRegionDataTypeEnum.CIOBit;
                    parameter.WordAddress = ushort.Parse(regionalAddressSpan[3..index]);
                    parameter.BitAddress = byte.Parse(regionalAddressSpan[(index + 1)..]);
                }
                else
                {
                    parameter.regionDataType = FinsRegionDataTypeEnum.CIOWord;
                    parameter.WordAddress = ushort.Parse(regionalAddressSpan[3..]);

                }
            }
            else
            {
                var regionalSting = regionalAddressSpan[..2].ToString();
                if (index > 0)
                {
                    parameter.regionDataType = _bitRegionDataTypes[regionalSting];
                    parameter.WordAddress = ushort.Parse(regionalAddressSpan[2..index]);
                    parameter.BitAddress = byte.Parse(regionalAddressSpan[(index + 1)..]);
                }
                else
                {
                    parameter.regionDataType = _wordRegionDataTypes[regionalSting];
                    parameter.WordAddress = ushort.Parse(regionalAddressSpan[2..]);
                }
            }

            return parameter;
        }
    }
}
