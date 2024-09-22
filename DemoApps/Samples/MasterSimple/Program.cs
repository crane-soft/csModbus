//#define TCP_INTERFACE
#define RTU_INTERFACE

using System;
using csModbusLib;

namespace MasterSimple
{
    class Program
    {
        static void Main(string[] args)
        {
#if TCP_INTERFACE
            Console.WriteLine("Modbus-TCP master demo");
            const int TCPport = 502;
            MbInterface MyInterface = new MbTCPSlave(TCPport);
#endif
#if RTU_INTERFACE
            string ComPort = "COM1";
            int Baudrate = 19200;
            Console.WriteLine(string.Format("Modbus-RTU master demo {0},{1}", ComPort, Baudrate));
            MbInterface MyInterface = new MbRTU(ComPort, Baudrate);
#endif
            const int SlaveID = 1;
            const int ModbusRegsAddr = 10;
            ushort[] SlaveRegs = new ushort[] {5,7,11,13,17,19};
            ushort[] SlaveCoils = new ushort[] {0,1,1,0 };

            MbMaster modMaster = new MbMaster();
            modMaster.Connect(MyInterface,SlaveID);

            ErrorCodes mbErr;
            // Registerfunctions
            
            mbErr = modMaster.WriteSingleRegister(ModbusRegsAddr, SlaveRegs[4]);
            mbErr = modMaster.WriteMultipleRegisters(ModbusRegsAddr, (ushort)SlaveRegs.Length, SlaveRegs);
            mbErr = modMaster.ReadHoldingRegisters(ModbusRegsAddr, (ushort) SlaveRegs.Length, SlaveRegs);
            mbErr = modMaster.ReadInputRegisters(ModbusRegsAddr, (ushort)SlaveRegs.Length, SlaveRegs);

            // coils functions
            mbErr = modMaster.ReadCoils(ModbusRegsAddr, (ushort)SlaveCoils.Length,SlaveCoils);
            mbErr = modMaster.ReadDiscreteInputs(ModbusRegsAddr, (ushort)SlaveCoils.Length, SlaveCoils);
            mbErr = modMaster.WriteMultipleCoils(ModbusRegsAddr, (ushort)SlaveCoils.Length, SlaveCoils);
            mbErr = modMaster.WriteSingleCoil(ModbusRegsAddr, SlaveCoils[1]);
          

            // read write register
            int RwSize = SlaveRegs.Length / 2;
            int WrAddr = ModbusRegsAddr + SlaveRegs.Length/2;

            mbErr = modMaster.ReadWriteMultipleRegisters(ModbusRegsAddr, 6, SlaveRegs,
                                               ModbusRegsAddr, 6, SlaveRegs);
            for (int i = 0; i < SlaveRegs.Length; ++i) {
                Console.Write(String.Format("{0} ", SlaveRegs[i]));
            }
            Console.WriteLine("");

            modMaster.Close();

        }
    }
}
