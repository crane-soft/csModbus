using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace csModbusLib
{
    // TODO consider using template macro
    // https://stackoverflow.com/questions/32664/is-there-a-constraint-that-restricts-my-generic-method-to-numeric-types
    public class MbMasterEx : MbMaster
    {
        #region <Int16> Register Functions 
        public ErrorCodes ReadHoldingRegisters(ushort Address, Int16[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ReadHoldingRegisters<Int16>(Address, DestData, Length, DestOffs);
        }
        public ErrorCodes ReadInputRegisters(ushort Address, Int16[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ReadInputRegisters<Int16>(Address, DestData, Length, DestOffs);

        }
        public ErrorCodes WriteSingleRegister(ushort Address, Int16 Data)
        {
            return WriteSingleRegister<Int16>(Address, Data);
        }
        public ErrorCodes WriteMultipleRegisters(ushort Address, Int16[] SrcData, int Length = 0, int SrcOffs = 0)
        {
            return WriteMultipleRegisters<Int16>(Address, SrcData, Length, SrcOffs);
        }
        public ErrorCodes ReadWriteMultipleRegisters(ushort RdAddress, Int16[] DestData, int RdLength,
                                                     ushort WrAddress, Int16[] SrcData, int WrLength, int DestOffs = 0, int SrcOffs = 0)
        {
            return ReadWriteMultipleRegisters<Int16>(RdAddress, DestData, RdLength, WrAddress, SrcData, WrLength, DestOffs, SrcOffs);
        }
        #endregion

        #region <UInt32> Register Functions 
        public ErrorCodes ReadHoldingRegisters(ushort Address, UInt32[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ReadHoldingRegisters<UInt32>(Address, DestData, Length, DestOffs);
        }
        public ErrorCodes ReadInputRegisters(ushort Address, UInt32[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ReadInputRegisters<UInt32>(Address, DestData, Length, DestOffs);

        }
        public ErrorCodes WriteSingleRegister(ushort Address, UInt32 Data)
        {
            return WriteSingleRegister<UInt32>(Address, Data);
        }
        public ErrorCodes WriteMultipleRegisters(ushort Address, UInt32[] SrcData, int Length = 0, int SrcOffs = 0)
        {
            return WriteMultipleRegisters<UInt32>(Address, SrcData, Length, SrcOffs);
        }
        public ErrorCodes ReadWriteMultipleRegisters(ushort RdAddress, UInt32[] DestData, int RdLength,
                                                     ushort WrAddress, UInt32[] SrcData, int WrLength, int DestOffs = 0, int SrcOffs = 0)
        {
            return ReadWriteMultipleRegisters<UInt32>(RdAddress, DestData, RdLength, WrAddress, SrcData, WrLength, DestOffs, SrcOffs);
        }
        #endregion

        #region <Int32> Register Functions 
        public ErrorCodes ReadHoldingRegisters(ushort Address, Int32[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ReadHoldingRegisters<Int32>(Address, DestData, Length, DestOffs);
        }
        public ErrorCodes ReadInputRegisters(ushort Address, Int32[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ReadInputRegisters<Int32>(Address, DestData, Length, DestOffs);

        }
        public ErrorCodes WriteSingleRegister(ushort Address, Int32 Data)
        {
            return WriteSingleRegister<Int32>(Address, Data);
        }
        public ErrorCodes WriteMultipleRegisters(ushort Address, Int32[] SrcData, int Length = 0, int SrcOffs = 0)
        {
            return WriteMultipleRegisters<Int32>(Address, SrcData, Length, SrcOffs);
        }
        public ErrorCodes ReadWriteMultipleRegisters(ushort RdAddress, Int32[] DestData, int RdLength,
                                                     ushort WrAddress, Int32[] SrcData, int WrLength, int DestOffs = 0, int SrcOffs = 0)
        {
            return ReadWriteMultipleRegisters<Int32>(RdAddress, DestData, RdLength, WrAddress, SrcData, WrLength, DestOffs, SrcOffs);
        }
        #endregion

        #region <float> Register Functions 
        public ErrorCodes ReadHoldingRegisters(ushort Address, float[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ReadHoldingRegisters<float>(Address, DestData, Length, DestOffs);
        }
        public ErrorCodes ReadInputRegisters(ushort Address, float[] DestData, int Length = 0, int DestOffs = 0)
        {
            return ReadInputRegisters<float>(Address, DestData, Length, DestOffs);

        }
        public ErrorCodes WriteSingleRegister(ushort Address, float Data)
        {
            return WriteSingleRegister<float>(Address, Data);
        }
        public ErrorCodes WriteMultipleRegisters(ushort Address, float[] SrcData, int Length = 0, int SrcOffs = 0)
        {
            return WriteMultipleRegisters<float>(Address, SrcData, Length, SrcOffs);
        }
        public ErrorCodes ReadWriteMultipleRegisters(ushort RdAddress, float[] DestData, int RdLength,
                                                     ushort WrAddress, float[] SrcData, int WrLength, int DestOffs = 0, int SrcOffs = 0)
        {
            return ReadWriteMultipleRegisters<float>(RdAddress, DestData, RdLength, WrAddress, SrcData, WrLength, DestOffs, SrcOffs);
        }
        #endregion
    }
}
