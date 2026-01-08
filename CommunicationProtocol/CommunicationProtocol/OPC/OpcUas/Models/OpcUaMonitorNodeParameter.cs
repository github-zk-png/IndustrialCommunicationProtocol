using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.OPC.OpcUas.Models
{
	public class OpcUaMonitorNodeParameter
	{
		public string Name { get; set; } = default!;

		public MonitoredItemNotificationEventHandler Notification { get; set; } = default!;
	}
}
