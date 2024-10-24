//#define TCP_INTERFACE
#define RTU_INTERFACE

using System;
using csModbusLib;

namespace MasterSimple
{
    class Program
    {
        private static MbMasterEx modMaster = new MbMasterEx();

        static void PrintModbusError(ErrorCodes mbErr)
        {
            if (mbErr != ErrorCodes.NO_ERROR) {
                if (mbErr == ErrorCodes.MODBUS_EXCEPTION) {
                    ExceptionCodes exCode = modMaster.GetModusException();
                    Console.WriteLine("MODBUS_EXCEPTION " + exCode);
                } else {
                    Console.WriteLine("MODBUS_ERROR " + mbErr);
                }
            }
        }
       
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

            modMaster.Connect(MyInterface, SlaveID);


            // --- Test coils functions -----------------------------------
            ushort[] SlaveCoils = new ushort[8] ;
            const int ModbusCoilsAddr = 10;
            PrintModbusError ( modMaster.WriteMultipleCoils(ModbusCoilsAddr + 4, new UInt16[] { 1, 1,0,1 }));
            PrintModbusError (modMaster.ReadCoils(ModbusCoilsAddr, SlaveCoils));

            PrintModbusError(modMaster.WriteSingleCoil(ModbusCoilsAddr, 1));
            PrintModbusError(modMaster.WriteSingleCoil(ModbusCoilsAddr, 0));

            PrintModbusError(modMaster.ReadDiscreteInputs(ModbusCoilsAddr, SlaveCoils));

            // Registerfunctions
            const int ModbusRegsAddr = 10;
            short[] SlaveRegs = new short[] { 5, 7, 11, 13, 17, 19 };
            PrintModbusError(modMaster.WriteSingleRegister(ModbusRegsAddr, SlaveRegs[4]));
            PrintModbusError(modMaster.WriteMultipleRegisters(ModbusRegsAddr, SlaveRegs));
            PrintModbusError(modMaster.WriteMultipleRegisters(ModbusRegsAddr, SlaveRegs));

            PrintModbusError(modMaster.ReadInputRegisters(ModbusRegsAddr+10, SlaveRegs,5));


            // -- read write register -------------------------------- 

            int RwSize = SlaveRegs.Length / 2;
            int WrAddr = ModbusRegsAddr + SlaveRegs.Length / 2;

            PrintModbusError(modMaster.ReadWriteMultipleRegisters(ModbusRegsAddr, SlaveRegs, 6,
                                               ModbusRegsAddr, SlaveRegs, 6));

            for (int i = 0; i < SlaveRegs.Length; ++i) {
                Console.Write(String.Format("{0} ", SlaveRegs[i]));
            }
            Console.WriteLine("");

            // --- Test Float Data -------------------------------------------
            float[] FloatData = new float[] { 3.14f, 1.41f, 0f };
            PrintModbusError(modMaster.WriteMultipleRegisters(40, new float[]{ 0,0,0}));
            PrintModbusError(modMaster.WriteSingleRegister(40, 1.414f));
            PrintModbusError(modMaster.WriteSingleRegister(42, 3.141f));
            PrintModbusError(modMaster.WriteSingleRegister(44, 2.718f));

            PrintModbusError(modMaster.ReadInputRegisters(40, FloatData));
            PrintModbusError(modMaster.ReadInputRegisters(40,  FloatData,2, 1));

            PrintModbusError(modMaster.WriteMultipleRegisters(40, FloatData));
            PrintModbusError(modMaster.ReadHoldingRegisters(40, FloatData));
            PrintModbusError(modMaster.ReadHoldingRegisters(40,  FloatData,1 ,2));

            PrintModbusError(modMaster.ReadWriteMultipleRegisters(40, FloatData,3,
                                               40, new float[] {17.2f,37.8f,99.03f },3));

            modMaster.Close();

        }
    }
}
