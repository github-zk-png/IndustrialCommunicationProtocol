using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommunicationProtocol.OPC.OpcUas.Protocols
{

    internal abstract class OpcUaAuxiliary
    {
        protected ISession _session = default!;

        public ISession Session => _session;

        public event Action<string, object> Notification = default!;

        protected void MonitoredItem_Notification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            MonitoredItemNotification? monitoredItemNotification = (MonitoredItemNotification)e.NotificationValue;

            Notification?.Invoke(monitoredItem.StartNodeId.ToString(), monitoredItemNotification.Value.Value);
        }

    }
}
