//#define TCP_INTERFACE
#define RTU_INTERFACE

using System;
using System.Threading;
using csModbusLib;


namespace SlaveSimple
{
    class Program
    {
        static void Main(string[] args)
        {
#if TCP_INTERFACE
            Console.WriteLine("Modbus-TCP slave demo");
            const int TCPport = 502;
            MbInterface MyInterface = new MbTCPSlave(TCPport);
#endif
#if RTU_INTERFACE
            string ComPort = "COM2";
            int Baudrate = 19200;
            Console.WriteLine(string.Format("Modbus-RTU slave demo {0},{1}", ComPort, Baudrate));
            MbInterface MyInterface = new MbRTU (ComPort, Baudrate);
#endif

            const int SlaveID = 1;
            const int ModbusRegsAddr = 10;
            const int ModbusInputAddr = 20;
            const int ModbusCoilsAddr = 10;

            // establish a modbus slave dataserver 
            StdDataServer MyDataServer = new StdDataServer(SlaveID);

            // add some data
            ushort[] SlaveRegs = new ushort[] { 502, 703, 114, 137, 178, 199 ,0,0};
            ushort[] SlaveInputs = new ushort[5];
            ushort[] SlaveCoils = new ushort[4];

            MyDataServer.AddHoldingRegisters(ModbusRegsAddr, SlaveRegs);
            MyDataServer.AddInputRegisters(ModbusRegsAddr, SlaveRegs);
            MyDataServer.AddInputRegisters(ModbusInputAddr, SlaveInputs);
            MyDataServer.AddCoils(ModbusCoilsAddr, SlaveCoils);
            MyDataServer.AddDiscreteInputs(ModbusCoilsAddr, SlaveCoils);

            // create a Modbus slave server 
            MbSlaveServer modSlave = new MbSlaveServer();

            // start listening for your data server and your connection interface
            // the listener is running in his own thread and can be stopped with StopListen()

            modSlave.StartListen(MyInterface, MyDataServer);

            Console.WriteLine("Listener started...");
            while (!Console.KeyAvailable) {

                // print register values changed by a modbus master
                for (int i = 0; i < SlaveRegs.Length; ++i ) {
                    Console.Write(String.Format("{0} ", SlaveRegs[i]));
                }
                Console.Write("          \r");
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
