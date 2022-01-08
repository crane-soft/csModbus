using System;
using System.Threading;
using csModbusLib;

namespace SlaveEvents
{
    class Program
    {
        const int TCPport = 502;
        const int SlaveID = 1;
        const int NumModbusRegs = 8;
        const int ModbusRegsAddr = 10;

        static ushort[] SlaveRegs = new ushort[NumModbusRegs];
        static int ReadCnt = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Modbus-TCP slave demo with events");

            // establish a modbus slave dataserver 
            StdDataServer MyDataServer = new StdDataServer(SlaveID);
            ModbusDataT<ushort> HoldingRegisters = new ModbusDataT<ushort>(ModbusRegsAddr, SlaveRegs);
            MyDataServer.AddHoldingRegisters(HoldingRegisters);

            HoldingRegisters.ValueChangedEvent += HoldingRegisters_ValueChangedEvent;
            HoldingRegisters.ValueReadEvent += HoldingRegisters_ValueReadEvent;

            // create a Modbus slave server  with your connnection interface
            MbSlaveServer modSlave = new MbSlaveServer(new MbTCPSlave(TCPport));

            // start listening for your data server
            // the listener is running in his own thread and can be stopped with StopListen()
            modSlave.StartListen(MyDataServer);
            Console.WriteLine("Listener started...");

            // your device automation will normally run from here 
            while (!Console.KeyAvailable) {
                if (SlaveRegs[6] == 42)
                    break;
                Thread.Sleep(100);
            }
            modSlave.StopListen();
            Console.WriteLine("Listener stopped");
            Thread.Sleep(1000);
        }

        private static void HoldingRegisters_ValueReadEvent(object sender, ModbusData.ModbusValueEventArgs e)
        {
            ++ReadCnt;
            Console.Write(String.Format("RdRequest:{0}\r", ReadCnt));

        }

        private static void HoldingRegisters_ValueChangedEvent(object sender, ModbusData.ModbusValueEventArgs e)
        {
            ModbusData DataSrc = (ModbusData)sender;

            Console.WriteLine();
            Console.WriteLine("R{0} = {1} has changed", DataSrc.BaseAddr+e.BaseIdx, SlaveRegs[e.BaseIdx]);
        }
    }
}
