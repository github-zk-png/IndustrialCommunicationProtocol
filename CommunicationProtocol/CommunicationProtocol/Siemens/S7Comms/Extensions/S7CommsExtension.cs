using CommunicationProtocol.Siemens.S7Comms.Enums;
using CommunicationProtocol.Siemens.S7Comms.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Extensions
{
    internal static class S7CommsExtension
    {

        private static S7CommBasicOperateParameter BitRegionalAddressParse(ReadOnlySpan<char> regionalAddressSpan, S7CommRegionEnum region)
        {
            var index = regionalAddressSpan.IndexOf('.');
            var regionalChar = regionalAddressSpan[0];
            var parameter = new S7CommBasicOperateParameter
            {
                Region = region,
                DbNumber = 0,
                ParameterItemType = S7CommParameterItemTypeEnum.BIT,
                ByteAddress = ushort.Parse(regionalAddressSpan[1..index]),
                BitAddress = byte.Parse(regionalAddressSpan[(index + 1)..]),
            };

            return parameter;
        }

        private static S7CommBasicOperateParameter VRegionalAddressParse(ReadOnlySpan<char> regionalAddressSpan)
        {
            var type = char.ToUpperInvariant(regionalAddressSpan[1]);

            var parameter = new S7CommBasicOperateParameter
            {
                Region = S7CommRegionEnum.V,
                DbNumber = 1,
                ParameterItemType = type == 'X' ? S7CommParameterItemTypeEnum.BIT : S7CommParameterItemTypeEnum.BYTE
            };

            var index = regionalAddressSpan.IndexOf('.');
            if (index > -1)
            {
                parameter.ByteAddress = ushort.Parse(regionalAddressSpan[2..index]);
                parameter.BitAddress = byte.Parse(regionalAddressSpan[(index + 1)..]);
            }
            else
            {
                parameter.ByteAddress = ushort.Parse(regionalAddressSpan[2..]);
            }

            return parameter;
        }

        private static S7CommBasicOperateParameter DBRegionalAddressParse(ReadOnlySpan<char> regionalAddressSpan)
        {

            var index = regionalAddressSpan.IndexOf('.');
            var numberSpan = regionalAddressSpan[2..index];

            var parameter = new S7CommBasicOperateParameter
            {
                Region = S7CommRegionEnum.DB,
                DbNumber = ushort.Parse(numberSpan),
            };
            //DB1.DBX100.2
            var addressSpan = regionalAddressSpan[(index + 4)..];

            index = addressSpan.IndexOf('.');

            if (index > -1)
            {
                parameter.ParameterItemType = S7CommParameterItemTypeEnum.BIT;
                parameter.ByteAddress = ushort.Parse(addressSpan[..index]);
                parameter.BitAddress = byte.Parse(addressSpan[(index + 1)..]);

            }
            else
            {
                parameter.ParameterItemType = S7CommParameterItemTypeEnum.BYTE;
                parameter.ByteAddress = ushort.Parse(addressSpan);
         
            }

            return parameter;
        }

        internal static S7CommBasicOperateParameter RegionalAddressParse(this string regionalAddress)
        {

            var regionalAddressSpan = regionalAddress.ToUpperInvariant().AsSpan();
            var regional = regionalAddressSpan[..2];

            if (regional.StartsWith("DB"))
            {
                return DBRegionalAddressParse(regionalAddressSpan);
            }

            var regionalChar = regionalAddressSpan[0];

            return regionalChar switch
            {
                'I' => BitRegionalAddressParse(regionalAddressSpan, S7CommRegionEnum.I),
                'Q' => BitRegionalAddressParse(regionalAddressSpan, S7CommRegionEnum.Q),
                'M' => BitRegionalAddressParse(regionalAddressSpan, S7CommRegionEnum.M),
                'V' => VRegionalAddressParse(regionalAddressSpan),
                _ => throw new ArgumentException($"Unsupported region type: {regionalChar}", nameof(regionalAddress))
            };

        }
    }
}
