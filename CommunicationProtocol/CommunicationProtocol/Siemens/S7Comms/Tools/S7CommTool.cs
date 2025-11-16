using CommunicationProtocol.Siemens.S7Comms.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Tools
{
    internal class S7CommTool
    {
        private readonly static IReadOnlyDictionary<S7CommDataTypeEnum, byte> _typeByteCounts = new Dictionary<S7CommDataTypeEnum, byte>()
        {
            [S7CommDataTypeEnum.BOOL] = 1,
            [S7CommDataTypeEnum.SBYTE] = 1,
            [S7CommDataTypeEnum.BYTE] = 1,
            [S7CommDataTypeEnum.SBYTE] = 2,
            [S7CommDataTypeEnum.USHORT] = 2,
            [S7CommDataTypeEnum.INT] = 4,
            [S7CommDataTypeEnum.UINT] = 4,
            [S7CommDataTypeEnum.FLOAT] = 4,
        };
       
        public static IReadOnlyDictionary<S7CommDataTypeEnum, byte> TypeByteCounts => _typeByteCounts;

     
    }
}
