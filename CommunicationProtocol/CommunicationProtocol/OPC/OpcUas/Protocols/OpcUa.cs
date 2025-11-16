using CommunicationProtocol.Extensions;
using CommunicationProtocol.Mitsubishis.Mc3es.Models;
using CommunicationProtocol.Models;
using CommunicationProtocol.OPC.OpcUas.Models;
using Newtonsoft.Json.Linq;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationProtocol.OPC.OpcUas.Protocols
{
    internal class OpcUa : OpcUaAuxiliary, IOpcUa
    {

        public async Task<bool> AnonymousConnectAsync(string url)
        {
            var SessionFactory = DefaultSessionFactory.Instance;

            var applicationConfiguration = new ApplicationConfiguration
            {
                ClientConfiguration = new ClientConfiguration()
            };

            var configuredEndpoint = new ConfiguredEndpoint(null, new EndpointDescription(url));

            var userIdentity = new UserIdentity();

            var preferredLocales = new List<string>();

            _session = await SessionFactory.CreateAsync(applicationConfiguration, configuredEndpoint, true, "opc", 5000, userIdentity, preferredLocales);

            return true;
        }

        public async Task<bool> UserConnect(OpcUaConnectParameter parameter)
        {
            var applicationConfiguration = new ApplicationConfiguration
            {
                ClientConfiguration = new ClientConfiguration()
            };

            var validator = applicationConfiguration.CertificateValidator;

            validator.CertificateValidation += (se, ev) =>
            {
                if (ev.Error.StatusCode.Code == StatusCodes.BadCertificateUntrusted)
                    ev.Accept = true;
            };

            applicationConfiguration.SecurityConfiguration = new SecurityConfiguration
            {
                RejectSHA1SignedCertificates = false,
                AutoAcceptUntrustedCertificates = true
            };

            await validator.UpdateAsync(applicationConfiguration);

            applicationConfiguration.CertificateValidator = validator;

            var configuredEndpoint = new ConfiguredEndpoint(null, new EndpointDescription(parameter.Url));

            var userIdentity = new UserIdentity(parameter.UserName, parameter.Password);

            var preferredLocales = new List<string>();

            var SessionFactory = DefaultSessionFactory.Instance;

            _session = await SessionFactory.CreateAsync(applicationConfiguration, configuredEndpoint, true, "opc", 5000, userIdentity, preferredLocales);

            return true;
        }

        public async Task<bool> CertificateConnect(OpcUaConnectParameter parameter)
        {
            var applicationConfiguration = new ApplicationConfiguration
            {
                ApplicationName = "MyOpc",
                ClientConfiguration = new ClientConfiguration()
            };

            var validator = new CertificateValidator();

            validator.CertificateValidation += (se, ev) =>
            {
                if (ev.Error.StatusCode.Code == StatusCodes.BadCertificateUntrusted)
                    ev.Accept = true;
            };
            applicationConfiguration.CertificateValidator = validator;

            applicationConfiguration.SecurityConfiguration = new SecurityConfiguration()
            {
                RejectSHA1SignedCertificates = false,

                ApplicationCertificate = new CertificateIdentifier
                {
                    StoreType = "Directory",
                    // CommonApplicationData : C:\\ProgramData
                    StorePath = @"%CommonApplicationData%/OPC/CertificateStores/MachineDefault",
                    SubjectName = $"CN=MyOpc,DC={Utils.GetHostName()}"
                },
                TrustedPeerCertificates = new CertificateTrustList()
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%/OPC/CertificateStores/UAApplications"
                },
                TrustedIssuerCertificates = new CertificateTrustList()
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%/OPC/CertificateStores/UACertificate Authorities"
                },
                RejectedCertificateStore = new CertificateTrustList()
                {
                    StoreType = "Directory",
                    StorePath = @"%CommonApplicationData%/OPC/CertificateStores/RejectedCertificates"
                }
            };

            applicationConfiguration.ValidateAsync(ApplicationType.Client).Wait();

            var instance = new ApplicationInstance()
            {
                ApplicationConfiguration = applicationConfiguration,
            };
            await instance.CheckApplicationInstanceCertificatesAsync(false, 1024).ConfigureAwait(false);

            var endpointDescription = new EndpointDescription(parameter.Url)
            {
                SecurityMode = MessageSecurityMode.SignAndEncrypt,
                SecurityPolicyUri = SecurityPolicies.Basic256,
            };

            var configuredEndpoint = new ConfiguredEndpoint(null, endpointDescription);

            var preferredLocales = new List<string>();

            var SessionFactory = DefaultSessionFactory.Instance;

            var userIdentity = new UserIdentity(parameter.UserName, parameter.Password);

            _session = await SessionFactory.CreateAsync(applicationConfiguration, configuredEndpoint, true, "opc", 5000, userIdentity, preferredLocales);

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_session != null && _session.Connected)
            {
                await _session.CloseAsync();
            }
        }

        public async Task<T> ReadAsync<T>(string name)
        {
            var parameters = new List<OpcUaReadParameter>
            {
                new OpcUaReadParameter
                {
                    Name = name,
                }
            };

            await foreach (var item in ReadAsync(parameters))
            {
                return (T)item.Value;
            }

            return default!;
        }

        public async Task<bool> WriteAsync(string name, object value)
        {
            var parameters = new List<OpcUaWriteParameter>
            {
                new OpcUaWriteParameter
                {
                    Name = name,
                    Value= value
                }
            };
            return await WriteAsync(parameters);
        }

        public async IAsyncEnumerable<OpcUaReturnData> ReadAsync(IReadOnlyList<OpcUaReadParameter> parameters)
        {
            var readValueIds = new ReadValueIdCollection();

            foreach (var item in parameters)
            {
                readValueIds.Add(new ReadValueId
                {
                    NodeId = item.Name,
                    AttributeId = Attributes.Value
                });
            }

            var readResponse = await _session.ReadAsync(new RequestHeader(), 0, TimestampsToReturn.Both, readValueIds, CancellationToken.None);

            var index = default(ushort);
            foreach (DataValue item in readResponse.Results)
            {
                yield return new OpcUaReturnData
                {
                    TransactionId = parameters[index].TransactionId,
                    Name = parameters[index].Name,
                    Value = item.Value
                };
            }
        }

        public async Task<bool> SetMonitorNode(IReadOnlyList<string> parameters)
        {
            var subscription = _session.DefaultSubscription;

            foreach (var item in parameters)
            {
                var monitoredItem = new MonitoredItem()
                {
                    StartNodeId = item,
                };
                monitoredItem.Notification += MonitoredItem_Notification;
                subscription.AddItem(monitoredItem);
            }
            _session.AddSubscription(subscription);

            await subscription.CreateAsync();

            return true;
        }

        public async Task<bool> WriteAsync(IReadOnlyList<OpcUaWriteParameter> parameters)
        {
            var writeValues = new WriteValueCollection();

            foreach (var item in parameters)
            {
                var writeValue = new WriteValue
                {
                    NodeId = item.Name,
                    AttributeId = Attributes.Value,
                    Value = new DataValue()
                    {
                        Value = item.Value,
                    }
                };
                writeValues.Add(writeValue);
            }

            var response = await _session.WriteAsync(new RequestHeader(), writeValues, CancellationToken.None);

            return true;
        }
    }
}
