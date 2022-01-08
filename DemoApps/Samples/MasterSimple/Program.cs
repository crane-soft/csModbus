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
