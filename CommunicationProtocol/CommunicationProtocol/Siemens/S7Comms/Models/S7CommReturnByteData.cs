 using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Siemens.S7Comms.Models
{
    public class S7CommReturnByteData
    {
        public ReadOnlyMemory<byte> Values { get; set; }
    }
}
 