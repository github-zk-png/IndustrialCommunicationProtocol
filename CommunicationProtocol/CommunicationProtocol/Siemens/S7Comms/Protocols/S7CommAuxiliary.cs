using CommunicationProtocol.Bases;
using CommunicationProtocol.Extensions;
using CommunicationProtocol.Siemens.S7Comms.Enums;
using CommunicationProtocol.Siemens.S7Comms.Models;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationProtocol.Siemens.S7Comms.Protocols
{
    internal abstract class S7CommAuxiliary : TcpProtocol
    {
        private int _transactionId = 0;

        private ushort TransactionId => (ushort)Interlocked.Increment(ref _transactionId);

        protected ushort _plcPduLength;

        protected readonly S7CommConnectParameter _s7CommConnectParameter;

        protected S7CommAuxiliary(S7CommConnectParameter parameter) : base(parameter)
        {
            _s7CommConnectParameter = parameter;
        }

        private void FinalizeS7ParameterItems(IEnumerable<S7CommBasicOperateParameter> parameters, Span<byte> requestFrame)
        {
            var offset = 19;
            foreach (var parameter in parameters)
            {
                var dataAddress = (parameter.ByteAddress << 3) + parameter.BitAddress;

                // ==================== S7 - Parameter - Item====================
                requestFrame[offset] = 0x12; 
                requestFrame[offset + 1] = 0x0A; 
                requestFrame[offset + 2] = 0x10;
                requestFrame[offset + 3] = (byte)parameter.ParameterItemType; 
                BinaryPrimitives.WriteUInt16BigEndian(requestFrame[(offset + 4)..(offset + 6)], parameter.Count); 
                BinaryPrimitives.WriteUInt16BigEndian(requestFrame[(offset + 6)..(offset + 8)], parameter.DbNumber);
                requestFrame[offset + 8] = (byte)parameter.Region; 
                requestFrame[offset + 9] = (byte)(dataAddress >> 16);
                requestFrame[offset + 10] = (byte)(dataAddress >> 8);   
                requestFrame[offset + 11] = (byte)dataAddress;         
                offset += 12;
            }
        }

        private void FinalizeS7DataItems(IReadOnlyCollection<S7CommWriteParameter> parameters, Span<byte> requestFrame)
        {

            var offset = 4 + 3 + 10 + 2 + parameters.Count * 12;
            var lastIndex = parameters.Count - 1;
            var currentIndex = 0;

            foreach (var parameter in parameters)
            {
                var length = parameter.DataItemType == S7CommDataItemTypeEnum.BYTE ? (ushort)(parameter.Values.Length << 3) : (ushort)parameter.Values.Length;
                // ==================== S7 - DATA - Item====================
                requestFrame[offset] = 0x00; /
                requestFrame[offset + 1] = (byte)parameter.DataItemType;
                BinaryPrimitives.WriteUInt16BigEndian(requestFrame[(offset + 2)..(offset + 4)], length); 
                parameter.Values.Span.CopyTo(requestFrame[(offset + 4)..(offset + 4 + parameter.Values.Length)]); 

                offset += 4 + parameter.Values.Length;

                if ((parameter.Values.Length & 1) == 1 && (currentIndex != lastIndex))
                {
                    requestFrame[offset] = 0x00;

                    offset += 1;
                }

                currentIndex++;

            }

        }

        private ushort Gets7DataLength(IReadOnlyList<S7CommWriteParameter> parameters)
        {
            var s7DataLength = 0;
            var lastIndex = parameters.Count - 1;
            for (int i = 0; i < lastIndex; i++)
            {
                var item = parameters[i];
                s7DataLength += 4 + item.Values.Length + (item.Values.Length & 1);
            }

            var lastItem = parameters[lastIndex];
            s7DataLength += 4 + lastItem.Values.Length;
            return (ushort)s7DataLength;
        }

        protected ReadOnlyMemory<byte> GetCotpRequestFrame(byte rack, byte slot)
        {
            ReadOnlySpan<byte> requestFrame = stackalloc byte[22]
            {
                 // ==================== TPKT 层 ====================
                 0x03,         
                 0x00,          
                 0x00, 0x16,   
                   
                 // ==================== COTP 层 ====================
                 0x11,         
                 0xE0,          
                 0x00, 0x00,   
                 0x00, 0x01,   
                 0x00,          
                   
                 // ==================== COTP 参数：TPDU Size ====================
                 0xC0,          
                 0x01,         
                 0x0A,         
                   
                 // ==================== COTP 参数：Source TSAP ====================
                 0xC1,         
                 0x02,         
                 0x10,        
                 0x00,        
                   
                 // ==================== COTP 参数：Destination TSAP ====================
                 0xC2,         
                 0x02,         
                 0x03,          
                 (byte)((rack << 5 ) + slot),  
             };

            return requestFrame.ToArray();
        }

        protected ReadOnlyMemory<byte> GetSetupCommunicationRequestFrame()
        {

            ReadOnlySpan<byte> requestFrame = stackalloc byte[25]
            {
                // ==================== TPKT 层 ====================
                0x03,          
                0x00,          
                0x00, 0x19,    
                
                // ==================== COTP 层 ====================
                0x02,          
                0xF0,         
                0x80,          
                
                // ==================== S7 Header ====================
                0x32,          
                0x01,          
                0x00, 0x00,    
                0x00, 0x00,   
                0x00, 0x08,    
                0x00, 0x00,    
                                                                                   
                // ==================== S7 Parameter ====================          
                0xF0,          
                0x00,         
                0x01, 0x01,    
                0x00, 0x01,    
                0x03, 0xC0,    
            };

            return requestFrame.ToArray();

        }

        protected override async Task<ReadOnlyMemory<byte>> SendAndReceiveAsync(ReadOnlyMemory<byte> requestFrame)
        {
            await _networkStream.WriteAsync(requestFrame);

            var responseFrameHead = await _networkStream.ReadExactAsync(4, _receiveTimeout);

            var responseFrameLength = BinaryPrimitives.ReadUInt16BigEndian(responseFrameHead.Span[2..]);

            var responseFrameBody = await _networkStream.ReadExactAsync(responseFrameLength - 4, _receiveTimeout);

            return responseFrameBody;

        }

        protected void ValidateCotpResponseFrame(ReadOnlyMemory<byte> responseFrame)
        {
            if (responseFrame.Span[1] != 0xD0)
                throw new Exception("COTP Connection Failed");
        }

        protected void ValidateSetupCommunicationResponseFrame(ReadOnlyMemory<byte> responseFrame)
        {
            if (responseFrame.Span[14] != 0x00)
                throw new Exception("Connection Error: Setup Communication failed");
        }

        protected void ValidatePduLength(IReadOnlyList<S7CommBasicOperateParameter> parameters)
        {
            var s7DataLength = default(ushort);
            if (parameters is IReadOnlyList<S7CommWriteParameter> writeParameters)
            {
                s7DataLength = Gets7DataLength(writeParameters);
            }
            var length = 10 + 2 + parameters.Count * 12 + s7DataLength;
            if (_plcPduLength < length)
            {
                throw new ArgumentException($"Request data length{length}exceeds PLC PDU limit of {_plcPduLength}");
            }
        }

        protected void SavePlcPduLength(ReadOnlyMemory<byte> responseFrame)
        {
            _plcPduLength = BinaryPrimitives.ReadUInt16BigEndian(responseFrame.Span[21..]);
        }

        protected ReadOnlyMemory<byte> GetRequestFrame(IReadOnlyCollection<S7CommReadParameter> parameters)
        {
            var s7ParameterLength = (ushort)(2 + parameters.Count * 12);
            var requestFrameLength = (ushort)(4 + 3 + 10 + s7ParameterLength);
            Span<byte> requestFrame = stackalloc byte[requestFrameLength];

            // ==================== TPKT 层 ====================
            requestFrame[0] = 0x03;         
            requestFrame[1] = 0x00;          
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[2..4], requestFrameLength); 

            // ==================== COTP 层 ====================
            requestFrame[4] = 0x02;         
            requestFrame[5] = 0xF0;         
            requestFrame[6] = 0x80;         

            // ==================== S7 Header ====================
            requestFrame[7] = 0x32;         
            requestFrame[8] = 0x01;         
            requestFrame[9..11].Clear();    
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[11..13], TransactionId);       
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[13..15], s7ParameterLength);  
            requestFrame[15..17].Clear();   


            // ==================== S7 Parameter ====================
            requestFrame[17] = 0x04;        /
            requestFrame[18] = (byte)parameters.Count;      

            FinalizeS7ParameterItems(parameters, requestFrame);

            return requestFrame.ToArray();

        }

        protected ReadOnlyMemory<byte> GetRequestFrame(IReadOnlyList<S7CommWriteParameter> parameters)
        {
            var s7ParameterLength = (ushort)(2 + parameters.Count * 12);

            var s7DataLength = Gets7DataLength(parameters);

            var requestFrameLength = (ushort)(4 + 3 + 10 + s7ParameterLength + s7DataLength);
            Span<byte> requestFrame = stackalloc byte[requestFrameLength];

            // ==================== TPKT 层 ====================
            requestFrame[0] = 0x03;          
            requestFrame[1] = 0x00;          
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[2..4], requestFrameLength); 

            // ==================== COTP 层 ====================
            requestFrame[4] = 0x02;         
            requestFrame[5] = 0xF0;         
            requestFrame[6] = 0x80;         

            // ==================== S7 Header ====================
            requestFrame[7] = 0x32;         
            requestFrame[8] = 0x01;         
            requestFrame[9..11].Clear();   
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[11..13], TransactionId);      
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[13..15], s7ParameterLength);   
            BinaryPrimitives.WriteUInt16BigEndian(requestFrame[15..17], s7DataLength);   


            // ==================== S7 Parameter ====================
            requestFrame[17] = 0x05;        
            requestFrame[18] = (byte)parameters.Count;   

            FinalizeS7ParameterItems(parameters, requestFrame);

            FinalizeS7DataItems(parameters, requestFrame);

            return requestFrame.ToArray();

        }

        protected void ValidateData(ReadOnlyMemory<byte> responseFrame)
        {
            var responseFrameSpan = responseFrame.Span;
            var headerErrorCode = BinaryPrimitives.ReadInt16BigEndian(responseFrameSpan[13..15]);
            if (headerErrorCode != 0)
            {
                throw new Exception($"{nameof(headerErrorCode)}：{responseFrame.ToHexString('-')}");
            }

            var offset = 17;

            for (int i = 0; i < responseFrameSpan[16]; i++)
            {
                if (responseFrameSpan[offset] != 0xFF)
                {
                    throw new Exception($"item{i}：{responseFrame.ToHexString('-')}");
                }

                offset++;

            }
        }

        protected IReadOnlyList<S7CommReturnByteData> GetDataAndValidate(ReadOnlyMemory<byte> responseFrame)
        {
            var headerErrorCode = BinaryPrimitives.ReadInt16BigEndian(responseFrame.Span[13..15]);
            if (headerErrorCode != 0)
            {
                throw new Exception($"{nameof(headerErrorCode)}：{responseFrame.ToHexString('-')}");
            }

            var offset = 17;

            var results = new List<S7CommReturnByteData>(responseFrame.Span[16]);

            for (int i = 0; i < responseFrame.Span[16]; i++)
            {
                if (responseFrame.Span[offset] != 0xFF)
                {
                    throw new Exception($"item{i}：{responseFrame.ToHexString('-')}");
                }

 
                var dataType = responseFrame.Span[offset + 1];

                var dataLength = BinaryPrimitives.ReadUInt16BigEndian(responseFrame.Span[(offset + 2)..(offset + 4)]);

                var dataBytesLength = dataType == 0x04 ? dataLength / 8 : dataLength;

                var dataBytes = responseFrame.Slice(offset + 4, dataBytesLength);

                offset += 4 + dataBytesLength + (dataBytesLength & 1);

                results.Add(new S7CommReturnByteData
                {
                    Values = dataBytes
                });
            }

            return results;
        }
    }
}



