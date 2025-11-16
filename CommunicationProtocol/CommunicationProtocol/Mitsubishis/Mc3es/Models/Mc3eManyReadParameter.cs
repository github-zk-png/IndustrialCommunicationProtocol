using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.Mitsubishis.Mc3es.Models
{
    public class Mc3eManyReadParameter
    {
        public IReadOnlyList<Mc3eReadParameter> BitParameters { get; set; } = default!;

        public IReadOnlyList<Mc3eReadParameter> WordParameters { get; set; } = default!;
    }
}

