using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.OPC.OpcUas.Protocols
{

	internal abstract class OpcUaAuxiliary
	{
		public ISession Session { get; protected set; } = default!;

	}
}
