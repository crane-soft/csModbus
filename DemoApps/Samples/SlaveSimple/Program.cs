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
