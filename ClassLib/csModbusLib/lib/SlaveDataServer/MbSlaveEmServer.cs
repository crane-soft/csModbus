using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csModbusLib 
{
    class MbSlaveEmServer : MbSlaveDataServer
    {
        public MbSlaveEmServer(int SlaveID) : base(SlaveID)
        {
        }

        protected override bool WriteSingleCoil()
        {
            return WriteSingleCoil(Frame.DataAddress, Frame.GetSingleBit());
        }

        protected override bool WriteSingleRegister() { 
            return WriteSingleRegister (Frame.DataAddress, Frame.GetSingleUInt16()); 
        }

        protected virtual bool WriteSingleCoil(ushort Address, ushort Bit) { return false; }
        protected virtual bool WriteSingleRegister(ushort Address, ushort Value) { return false; }
    }
}
