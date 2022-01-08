## Full Featured C# Modbus Library

This library was created with the goal of clean architecture, especially with separate classes for the connection interface and the main class.
The slave (server site) contains a data server class that is intended to facilitate the access of the device application to the Modbus data.
An additional user control library supports the display of Modbus data in a desktop application.

Key Features:
* Master and Slave
* Interface TCP,UDP. RTU, ASCI
* Multimaster capable (TCP / UDP)
* Easy to use data server in slave mode
* Dataserver can manage multiple node-Ids
* additional gridview user control library 
* Demo Applications

Supported Modbus-Functions:
* READ_COILS = 0x01,
* READ_DISCRETE_INPUTS = 0x02,
* READ_HOLDING_REGISTERS = 0x03,
* READ_INPUT_REGISTERS = 0x04,
* WRITE_SINGLE_COIL = 0x05,
* WRITE_SINGLE_REGISTER = 0x06,
* WRITE_MULTIPLE_COILS = 0x0F,
* WRITE_MULTIPLE_REGISTERS = 0x10,
* READ_WRITE_MULTIPLE_REGISTERS = 0x17,     

## Library usage for slaves applications
```csharp
using System;
using System.Threading;
using csModbusLib;

namespace SlaveSimple
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Modbus-TCP slave demo");
            const int TCPport = 502;
            const int SlaveID = 1;
            const int NumModbusRegs = 8;
            const int ModbusRegsAddr = 10;

            // establish a modbus slave dataserver 
            StdDataServer MyDataServer = new StdDataServer(SlaveID);

            // add some data
            ushort[] SlaveRegs = new ushort[NumModbusRegs];
            MyDataServer.AddHoldingRegisters(ModbusRegsAddr, SlaveRegs);

            // create a Modbus slave server 
            MbSlaveServer modSlave = new MbSlaveServer();

            // start listening for your data server and your connection interface
            // the listener is running in his own thread and can be stopped with StopListen()
            modSlave.StartListen(new MbTCPSlave(TCPport), MyDataServer);

            Console.WriteLine("Listener started...");
            while (!Console.KeyAvailable) {

                // print some register values changed by a modbus master
                Console.Write(String.Format("{0} {1}\r", SlaveRegs[0], SlaveRegs[1]));

                // change a register readed by a modbus master
                SlaveRegs[7] += 1;
                // check for exit via master
                if (SlaveRegs[6] == 42)
                    break;
                Thread.Sleep(100);
            }

            modSlave.StopListen();
            Console.WriteLine("Listener stopped");
            Thread.Sleep(1000);
        }
    }
}
...
## Library usage for master applications
```csharp
using System;
using csModbusLib;

namespace MasterSimple
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Modbus-TCP master demo");

            const int TCPport = 502;
            const int SlaveID = 1;

            const int ModbusRegsAddr = 10;
            ushort[] SlaveRegs = new ushort[] {5,7,11,13,17,19};
            bool[] SlaveCoils = new bool[] {false,true,true,false };

            MbMaster modMaster = new MbMaster();
            modMaster.Connect(new MbTCPMaster("localhost",TCPport), SlaveID);

            // Registerfunctions
            modMaster.WriteSingleRegister(ModbusRegsAddr, SlaveRegs[4]);
            modMaster.WriteMultipleRegisters(ModbusRegsAddr, (ushort)SlaveRegs.Length, SlaveRegs);
            modMaster.ReadHoldingRegisters(ModbusRegsAddr, (ushort) SlaveRegs.Length, SlaveRegs);
            modMaster.ReadInputRegisters(ModbusRegsAddr, (ushort)SlaveRegs.Length, SlaveRegs);
            
            // coils functions
            modMaster.ReadCoils(ModbusRegsAddr, (ushort)SlaveCoils.Length,SlaveCoils);
            modMaster.ReadDiscreteInputs(ModbusRegsAddr, (ushort)SlaveCoils.Length, SlaveCoils);
            modMaster.WriteMultipleCoils(ModbusRegsAddr, (ushort)SlaveCoils.Length, SlaveCoils);
            modMaster.WriteSingleCoil(ModbusRegsAddr, SlaveCoils[1]);

            // read write register
            int RwSize = SlaveRegs.Length / 2;
            int WrAddr = ModbusRegsAddr + SlaveRegs.Length/2;

            modMaster.ReadWriteMultipleRegisters(ModbusRegsAddr, (ushort)RwSize, SlaveRegs,
                                                (ushort)WrAddr, (ushort)RwSize, SlaveRegs, 0, RwSize);

            modMaster.Close();

        }
    }
}
...
