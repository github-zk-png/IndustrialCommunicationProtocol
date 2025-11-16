using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Models
{
    public class Mc3eReturnByteData
    {
        public ReadOnlyMemory<byte> Values { get; set; }
    }
}
