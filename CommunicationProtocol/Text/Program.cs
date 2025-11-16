using CommunicationProtocol;
using CommunicationProtocol.Mitsubishis.Mc3es.Enums;
using CommunicationProtocol.Mitsubishis.Mc3es.Models;
using CommunicationProtocol.Mitsubishis.Mc3es.Protocols;
using CommunicationProtocol.Modbus.Enums;
using CommunicationProtocol.Modbus.Models;
using CommunicationProtocol.Modbus.Protocols;
using CommunicationProtocol.Omrons.Fins.Enums;
using CommunicationProtocol.Omrons.Fins.Models;
using CommunicationProtocol.Omrons.Fins.Protocols;
using CommunicationProtocol.Omrons.OmronCips.Enums;
using CommunicationProtocol.Omrons.OmronCips.Models;
using CommunicationProtocol.OPC.OpcUas.Models;
using CommunicationProtocol.OPC.OpcUas.Protocols;
using CommunicationProtocol.Siemens.S7Comms.Enums;
using CommunicationProtocol.Siemens.S7Comms.Extensions;
using CommunicationProtocol.Siemens.S7Comms.Models;
using System.Buffers.Binary;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Text;



namespace Text
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");


            //var a = 5 / 2;
            //_ = Modbus();

            //_ = S7Comm();

            //_ = FINSSerialPort();

            //_ = Mc3EAsync();

            //_ = OPCUA();

            Console.ReadLine();
        }
        private static async Task OPCUA()
        {
            try
            {
                var protocol = Protocol.CreateOpcUa();
                await protocol.AnonymousConnectAsync("opc.tcp://192.168.70.129:49320");
                //var parameter = new OpcUaConnectParameter
                //{
                //    Url= "opc.tcp://192.168.70.129:49320",
                //    UserName= "zx-user",
                //    Password= "123456",
                //};
                //await protocol.UserConnect(parameter);

                //await protocol.CertificateConnect(parameter);

                //var a = new List<OpcUaReadParameter>
                //{
                //    new OpcUaReadParameter
                //    {
                //        TransactionId=1,
                //        Name="ns=2;s=数据类型示例.16 位设备.K 寄存器.Short1"
                //    },
                //    new OpcUaReadParameter
                //    {
                //        TransactionId=2,
                //        Name="ns=2;s=数据类型示例.16 位设备.K 寄存器.Short2"
                //    },
                //    new OpcUaReadParameter
                //    {
                //        TransactionId=3,
                //        Name="ns=2;s=数据类型示例.16 位设备.K 寄存器.ShortArray"
                //    },
                //};

                //var bv = protocol.ReadAsync(a);
                //await foreach (var result in bv)
                //{
                //    Console.WriteLine($"事务ID: {result.TransactionId}");
                //    Console.WriteLine($"节点名称: {result.Name}");
                //    Console.WriteLine($"值: {result.Value.ToString()}");
                //    Console.WriteLine($"类型: {result.Value?.GetType().Name}");
                //    Console.WriteLine("---");
                //}
                //var a = new List<string>
                //{
                //    "ns=2;s=数据类型示例.16 位设备.K 寄存器.Short1",
                //    "ns=2;s=数据类型示例.16 位设备.K 寄存器.Short2",
                //};

                //protocol.Notification += (name, value) =>
                //{
                //    Console.WriteLine($"节点名称: {name}");
                //    Console.WriteLine($"值: {value.ToString()}");
                //    Console.WriteLine($"类型: {value?.GetType().Name}");
                //    Console.WriteLine("---");
                //};

                //await protocol.SetMonitorNode(a);

                //var a = new List<OpcUaWriteParameter>
                //{
                //    new OpcUaWriteParameter
                //    {
                //        Name="ns=2;s=数据类型示例.16 位设备.K 寄存器.Short1",
                //        Value=(short)120
                //    },
                //    new OpcUaWriteParameter
                //    {
                //        Name="ns=2;s=数据类型示例.16 位设备.K 寄存器.Short2",
                //        Value=(short)1202
                //    },

                //};

                //var a=await protocol.ReadAsync<short>("ns=2;s=数据类型示例.16 位设备.K 寄存器.Short1");

                //await protocol.WriteAsync(a);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private static async Task Mc3EAsync()
        {
            try
            {
                //byte a= byte.Parse("123"[1..2]);
                //var c = Convert.ToInt32("8", 8);
                var protocol = Protocol.CreateMc3e(new Mc3eConnectParameter { Host = "192.168.70.129" });

                await protocol.ConnectAsync();

                await protocol.WriteAsync("X48", true);
                object a = await protocol.ReadAsync<bool>("X48");


                await protocol.WriteAsync("D8",12.5f);
                a = await protocol.ReadAsync<float>("D8");


                await protocol.WriteStringAsync("D15", "acbae");
                a = await protocol.ReadStringAsync("D15", 5);

                var parameter = new Mc3eReadParameter
                {
                    Region = Mc3eRegionEnum.D,
                    DataType = Mc3eDataTypeEnum.WORD,
                    Address = 0x10,
                    Count = 1,
                };

                //var parameter = new Mc3eReadParameter
                //{
                //    Region = Mc3eRegionEnum.X,
                //    DataType = Mc3eDataTypeEnum.BIT,
                //    Address = 0x30,
                //    Count = 1,
                //};

                //var parameter = new Mc3eReadParameter
                //{
                //    Region = Mc3eRegionEnum.M,
                //    DataType = Mc3eDataTypeEnum.BIT,
                //    Address = 10,
                //    Count = 1,
                //};

                //var a = await protocol.ReadAsync(parameter);


                //var parameter = new Mc3eWriteParameter
                //{
                //    Region = Mc3eRegionEnum.D,
                //    DataType = Mc3eDataTypeEnum.WORD,
                //    Address = 15,
                //    Count = 1,
                //    Values = new byte[] { 0x64, 0x00},
                //};

                //var parameter = new Mc3eWriteParameter
                //{
                //    Region = Mc3eRegionEnum.D,
                //    DataType = Mc3eDataTypeEnum.WORD,
                //    Address = 0x08,
                //    Count = 1,
                //    Values = new byte[] { 0x064, 0x00 },
                //};

                //var a = await protocol.WriteAsync(parameter);


                var parameters = new Mc3eManyReadParameter
                {
                    WordParameters = new List<Mc3eReadParameter>
                    {
                        new Mc3eReadParameter
                        {
                           Region = Mc3eRegionEnum.D,
                           DataType = Mc3eDataTypeEnum.WORD,
                           Address = 10,
                           Count = 3,
                        },
                        new Mc3eReadParameter
                        {
                           Region = Mc3eRegionEnum.D,
                           DataType = Mc3eDataTypeEnum.WORD,
                           Address = 20,
                           Count = 4,
                        }
                    },
                    BitParameters = new List<Mc3eReadParameter>
                    {
                        new Mc3eReadParameter
                        {
                            Region = Mc3eRegionEnum.X,
                            DataType = Mc3eDataTypeEnum.BIT,
                            Address = 10,
                            Count = 3,
                        },
                        new Mc3eReadParameter
                        {
                            Region = Mc3eRegionEnum.X,
                            DataType = Mc3eDataTypeEnum.BIT,
                            Address = 20,
                            Count = 2,
                        },
                        new Mc3eReadParameter
                        {
                            Region = Mc3eRegionEnum.M,
                            DataType = Mc3eDataTypeEnum.BIT,
                            Address = 10,
                            Count = 1,
                        }
                    }
                };

                //var a = await protocol.ReadAsync(parameters);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        static async Task Modbus()
        {
            try
            {
                //ReadOnlySpan<byte> bytes=stackalloc byte[2]
                //{
                //    0x01,0x01
                //};

                //var c = bytes.ToUnmanagedType<ushort>();

                var modbus = Protocol.CreateModbusAscll();
                modbus.Connect();
                //var modbus = Protocol.CreateModbusTcp(new ModbusTcpConnectParameter() { Host = "127.0.0.1" });
                //await modbus.ConnectAsync();
                //modbus.Connect();
                //modbus.Close();
                //var modbusReadParameter = new ModbusReadParameter
                //{
                //    FunctionCode = ModbusReadFunctionCodeEnum.ReadInputRegister,
                //    Slave = 1,
                //    StartingAddress = 0,
                //    Count = 3
                //};
                //var a = await modbus.ReadAsync(modbusReadParameter);
                //var afwa = modbus.Read(modbusReadParameter);
                //modbus.WriteString(1, "40001", "afeaaa1");
                //var a = modbus.ReadString(1, "40001", 7);

                //await modbus.WriteAsync(1, "00001", true);
                //var a = await modbus.ReadAsync<float>(1, "30001");

                //await modbus.WriteStringAsync(1, "40001", "afeaa");
                //var a = await modbus.ReadStringAsync(1, "40001",5);

                //var modbusWriteParameter = new ModbusWriteParameter
                //{
                //    FunctionCode = ModbusWriteFunctionCodeEnum.WriteMultipleHoldingRegister,
                //    Slave = 1,
                //    StartingAddress = 0,
                //    Count = 2,
                //    Values = new byte[] { 0x00, 0x03, 0x00, 0x03 }


                //};
                //var b = modbus.Write(1, "40000", 3.12f);
                //var b = modbus.Write(modbusWriteParameter);
                //var b = await modbus.WriteAsync(modbusWriteParameter);
            }
            catch (Exception ex)
            {

                throw;
            }




        }

        static async Task S7Comm()
        {
            try
            {
                var S7Comm = Protocol.CreateS7Comm(new S7CommConnectParameter { Host = "192.168.70.129" });
                await S7Comm.ConnectAsync();

                var listType = new List<S7CommTypeReadParameter>()
                {
                    new S7CommTypeReadParameter
                    {
                        Region = S7CommRegionEnum.DB,
                        DbNumber = 1,
                        ByteAddress = 100,
                        BitAddress=0,
                        TransactionId= 1,
                        DataType=S7CommDataTypeEnum.USHORT,
                    },
                    new S7CommTypeReadParameter
                    {
                        Region = S7CommRegionEnum.Q,
                        DbNumber = 0,
                        ByteAddress = 0,
                        BitAddress=6,
                        TransactionId= 2,
                        DataType=S7CommDataTypeEnum.BOOL,
                    },
                    new S7CommTypeReadParameter
                    {
                        Region = S7CommRegionEnum.DB,
                        DbNumber = 1,
                        ByteAddress =20,
                        BitAddress=0,
                        TransactionId= 3,
                        DataType=S7CommDataTypeEnum.BOOL,
                    },
                    new S7CommTypeReadParameter
                    {
                        Region = S7CommRegionEnum.DB,
                        DbNumber = 1,
                        ByteAddress =50,
                        BitAddress=0,
                        TransactionId= 4,
                        DataType=S7CommDataTypeEnum.FLOAT,
                    },
                };

                //var fawfawfe = S7Comm.ReadAsync(listType);

                //var afaw = new List<S7CommReturnTypeData>();
                //await foreach (var item in fawfawfe)
                //{
                //    afaw.Add(item);
                //}

                var list = new List<S7CommReadParameter>
                {
                    //new S7CommReadParameter
                    //{
                    //    Region = RegionEnum.DB,
                    //    ParameterItemType = ParameterItemTypeEnum.BYTE,
                    //    DbNumber = 1,
                    //    ByteAddress = 100,
                    //    Count = 2
                    //},
                    //new S7CommReadParameter
                    //{
                    //    Region = RegionEnum.DB,
                    //    ParameterItemType = ParameterItemTypeEnum.BYTE,
                    //    DbNumber = 1,
                    //    ByteAddress = 10,
                    //    Count = 1
                    //},
                    //new S7CommReadParameter
                    //{
                    //    Region = RegionEnum.Q,
                    //    ParameterItemType = ParameterItemTypeEnum.BIT,
                    //    BitAddress = 6,
                    //    Count = 1
                    //},
                    //new S7CommReadParameter
                    //{
                    //    Region = RegionEnum.DB,
                    //    ParameterItemType = ParameterItemTypeEnum.BIT,
                    //    DbNumber = 1,
                    //    ByteAddress = 20,
                    //    BitAddress = 2,
                    //    Count = 1
                    //},
                    new S7CommReadParameter
                    {
                        Region = S7CommRegionEnum.DB,
                        ParameterItemType = S7CommParameterItemTypeEnum.BYTE,
                        DbNumber = 1,
                        ByteAddress = 150,
                        BitAddress = 0,
                        Count = 10
                    },
                    //new S7CommReadParameter
                    //{
                    //    Region = RegionEnum.DB,
                    //    ParameterItemType = ParameterItemTypeEnum.BYTE,
                    //    DbNumber = 1,
                    //    ByteAddress = 50,
                    //    BitAddress = 0,
                    //    Count = 4
                    //},
                };

                //var a = await S7Comm.ReadAsync<bool>("DB1.DBX20.0");
                //var data_bytes = await S7Comm.ReadAsync<float>("DB1.Dbd50"); 
                //var data_bytesa = await S7Comm.ReadAsync<ushort>("DB1.Dbw100");
                //var data_bytesab = await S7Comm.ReadStringAsync("DB1.DBB150",10);

                //var fa = (sbyte)(a.ElementAt(1).Value.Span[0]);

                //var c = BinaryPrimitives.ReadSingleBigEndian(a.ElementAt(5).Value.Span);

                //var b = Encoding.UTF8.GetString(a.Span);

                //"adc".RegionalAddressParse();

                var af = Encoding.UTF8.GetBytes("avc");

                var fawe = new List<S7CommWriteParameter>
                {
                    //new S7CommWriteParameter
                    //{
                    //    Region = RegionEnum.DB,
                    //    ParameterItemType = ParameterItemTypeEnum.BYTE,
                    //    DbNumber = 1,
                    //    ByteAddress = 100,
                    //    DataItemType = DataItemTypeEnum.BYTE,
                    //    DataBytes=new byte[] { 0x00, 0x7B },
                    //    Count = 2
                    //},
                    //new S7CommWriteParameter
                    //{
                    //    Region = RegionEnum.DB,
                    //    ParameterItemType = ParameterItemTypeEnum.BYTE,
                    //    DbNumber = 1,
                    //    ByteAddress = 10,
                    //    DataItemType = DataItemTypeEnum.BYTE,
                    //    DataBytes=af,
                    //    Count = 3
                    //},
                    //new S7CommWriteParameter
                    //{
                    //    Region = RegionEnum.Q,
                    //    ParameterItemType = ParameterItemTypeEnum.BIT,
                    //    //DbNumber = 1,
                    //    //ByteAddress = 10,
                    //    BitAddress= 6,
                    //    DataItemType = DataItemTypeEnum.BIT,
                    //    DataBytes=new byte[] { 0x01 },
                    //    //Count = 3
                    //},
                    //new S7CommWriteParameter
                    //{
                    //    Region = RegionEnum.Q,
                    //    ParameterItemType = ParameterItemTypeEnum.BIT,
                    //    //DbNumber = 1,
                    //    //ByteAddress = 10,
                    //    BitAddress= 7,
                    //    DataItemType = DataItemTypeEnum.BIT,
                    //    DataBytes=new byte[] { 0x01 },
                    //    //Count = 3
                    //},
                    //new S7CommWriteParameter
                    //{
                    //    Region = RegionEnum.DB,
                    //    ParameterItemType = ParameterItemTypeEnum.BIT,
                    //    DbNumber = 1,
                    //    ByteAddress = 20,
                    //    BitAddress= 2,
                    //    DataItemType = DataItemTypeEnum.BIT,
                    //    DataBytes=new byte[] { 0x01 },
                    //    Count = 1
                    //},
                    //new S7CommWriteParameter
                    //{
                    //    Region = RegionEnum.DB,
                    //    ParameterItemType = ParameterItemTypeEnum.BYTE,
                    //    DbNumber = 1,
                    //    ByteAddress = 150,
                    //    BitAddress = 0,
                    //    DataItemType=DataItemTypeEnum.BYTE,
                    //    DataBytes=new byte[]{ 0x64, 0x03,0x61,0x76,0x63},
                    //    Count =5
                    //},
                };
                var afeafewwa = new List<S7CommTypeWriteParameter>
                {
                    new S7CommTypeWriteParameter
                    {
                        Region = S7CommRegionEnum.DB,
                        DbNumber = 1,
                        ByteAddress = 100,
                        BitAddress=0,
                        DataType= S7CommDataTypeEnum.SHORT,
                        Value= -456,
                    },
                    new S7CommTypeWriteParameter
                    {
                        Region = S7CommRegionEnum.Q,
                        DbNumber = 0,
                        ByteAddress = 0,
                        BitAddress=6,
                        DataType= S7CommDataTypeEnum.BOOL,
                        Value= true,
                    },
                    new S7CommTypeWriteParameter
                    {
                        Region = S7CommRegionEnum.DB,
                        DbNumber = 1,
                        ByteAddress = 20,
                        BitAddress=2,
                        DataType= S7CommDataTypeEnum.BOOL,
                        Value= true,
                    },
                    new S7CommTypeWriteParameter
                    {
                        Region = S7CommRegionEnum.DB,
                        DbNumber = 1,
                        ByteAddress = 50,
                        BitAddress=0,
                        DataType= S7CommDataTypeEnum.FLOAT,
                        Value= 20.5,
                    },
                };
                //var a = await S7Comm.WriteAsync(afeafewwa);
                //await S7Comm.WriteAsync("Q0.6", true);
                //await S7Comm.WriteAsync<ushort>("DB1.DBW100", 123);
                //await S7Comm.WriteAsync("DB1.DBX20.0", true);
                //await S7Comm.WriteAsync("DB1.DBD50", 23.5f);
                //await S7Comm.WriteStringAsync("DB1.DBB150", "avfa5w");
                //var ad = await S7Comm.WriteAsync(fawe);

            }
            catch (Exception ex)
            {

                throw;
            }




        }

        static async Task FINSSerialPort()
        {
            try
            {

                //ushort b = 0x1FF2;

                //var c = Convert.ToInt32("377", 8);

                //var a = BitConverter.GetBytes(b);

                //int a = int.MaxValue;

                //ushort b = (ushort)a;

                //Console.ReadLine();
                //var afaew= Encoding.ASCII.GetBytes("\0");


                //requestFrame[32] = '*';
                //requestFrame[33] = '\r';


                //var protocol = Protocol.CreateFinsSerialPort(new FinsSerialPortConnectParameter());
                //protocol.Connect();
                //var protocol = Protocol.CreateFinsTcp(new FinsTcpConnectParameter { Host = "127.0.0.1" });
                //await protocol.ConnectAsync();


                //var parameter = new FinsReadParameter
                //{
                //    regionDataType = FinsRegionDataTypeEnum.DMWord,
                //    WordAddress = 50,
                //    BitAddress = 0,
                //    Count = 3,
                //};

                //var parameter = new FinsReadParameter
                //{
                //    regionDataType = FinsRegionDataTypeEnum.CIOBit,
                //    WordAddress = 1,
                //    BitAddress = 13,
                //    Count = 3,
                //};

                //var a = protocol.Read(parameter);


                //await protocol.WriteAsync("DM10", -10.25f);
                //var a = await protocol.ReadAsync<float>("DM10");

                //await protocol.WriteAsync("CIO2.4", true);
                //var a = await protocol.ReadAsync<bool>("CIO2.4");


                //await protocol.WriteStringAsync("DM10", "ACSEDFG");
                //var a = await protocol.ReadStringAsync("DM10", 7);

                //Console.ReadLine();


                //var a = await protocol.ReadAsync(parameter);

                //await protocol.WriteAsync("DM10", -10.23f);
                //var a =await protocol.ReadAsync<float>("DM10");

                //await protocol.WriteStringAsync("DM10", "ACSEDG");
                //var a =await protocol.ReadStringAsync("DM10", 6);

                //var list = new List<FinsManyReadParameter>
                //{
                //    new FinsManyReadParameter
                //    {
                //       TransactionId=1,
                //       DataType= FinsDataTypeEnum.USHORT,
                //       regionDataType = FinsRegionDataTypeEnum.DMWord,
                //       WordAddress = 50,
                //       BitAddress = 0,
                //    },
                //    new FinsManyReadParameter
                //    {
                //       TransactionId=2,
                //       DataType= FinsDataTypeEnum.BOOL,
                //       regionDataType = FinsRegionDataTypeEnum.CIOBit,
                //       WordAddress = 5,
                //       BitAddress = 1,
                //    },
                //};

                //var a =await protocol.ReadAsync(list);

                //Console.ReadLine();
                //var parameter = new FinsWriteParameter
                //{
                //    regionDataType = FinsRegionDataTypeEnum.DMWord,
                //    WordAddress = 40,
                //    BitAddress = 0,
                //    Count = 2,
                //    Values = new byte[] { 0x01, 0x0A, 0x01, 0x14 },
                //};

                //var parameter = new FinsWriteParameter
                //{
                //    regionDataType = FinsRegionDataTypeEnum.CIOBit,
                //    WordAddress = 2,
                //    BitAddress = 13,
                //    Count = 3,
                //    Values = new byte[] { 0x01, 0x01, 0x01 },
                //};

                //var a = protocol.Write(parameter);

                //var a =await protocol.WriteAsync(parameter);


                var protocol = Protocol.CreateOmronCip(new OmronCipConnectParameter() { Host = "192.168.70.129" });

                await protocol.ConnectAsync();


                //var a = new OmronCipReadParameter
                //{
                //    NameFinalizeBytes = Encoding.UTF8.GetBytes("VarAA0"),
                //    NameLength = 5,
                //};

                //var values = await protocol.ReadAsync(a);

                //object values = await protocol.ReadAsync<ushort>("MemB");
                //values = await protocol.ReadAsync<bool>("VarAA");
                //values = await protocol.ReadAsync<float>("ValueFloat");
                //values = await protocol.ReadAsync<ushort>("ServerIn");
                //values = await protocol.ReadStringAsync("StrTest");

                //Console.ReadLine();
                //var a = new OmronCipWriteParameter
                //{
                //    Label = "MemB",
                //    dataType = OmronCipDataType.WORD,
                //    Values = new byte[] { 0x64, 0x00 }

                //};

                //var a = new OmronCipWriteParameter
                //{
                //    Label = "VarAA",
                //    dataType = OmronCipDataType.BOOL,
                //    Values = new byte[] { 0x00 }

                //};

                //var a = new OmronCipWriteParameter
                //{
                //    Label = "ServerIn",
                //    dataType = OmronCipDataType.INT,
                //    Values = new byte[] { 0x65, 0x00 }

                //};

                //var a = new OmronCipWriteParameter
                //{
                //    Label = "ValueFloat",
                //    dataType = OmronCipDataType.REAL,
                //    Values = new byte[] { 0x64, 0x00,0x00,0x00 }

                //};

                //var a = new OmronCipWriteParameter
                //{
                //    Name = "StrTest",
                //    dataType = OmronCipDataType.STRING,
                //    Values = new byte[] { 0x01,0x01 }
                //};

                //await protocol.WriteAsync(a);

                //await protocol.WriteAsync<ushort>("MemB", OmronCipDataType.WORD, 1234);
                //await protocol.WriteAsync("VarAA", OmronCipDataType.BOOL, false);
                //await protocol.WriteAsync("ValueFloat", OmronCipDataType.REAL, 1.50f);
                //await protocol.WriteAsync<ushort>("ServerIn", OmronCipDataType.INT, 456);
                //await protocol.WriteStringAsync("StrTest", "fdf");

                Console.ReadLine();
            }
            catch (Exception ex)
            {

                throw;
            }




        }
    }
}
