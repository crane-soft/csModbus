using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csModbusView
{
    public class ModbusCoilGridViewCell : DataGridViewCheckBoxCell
    {
        public ModbusCoilGridViewCell(MbGridView GridView)
        {
            this.Tag = GridView;
            this.Value = false;
            Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
    }

    public abstract class ModbusRegGridViewCell : DataGridViewTextBoxCell
    {
        public ModbusRegGridViewCell(MbGridView GridView)
        {
            this.Tag = GridView;
            Value = "";
            this.ReadOnly = false;
            Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public abstract void SetValue(UInt16[] mValue, int idx = 0);
        public abstract UInt16[] GetValue();
    }

    public class UINT16_GridViewCell : ModbusRegGridViewCell
    {
        public UINT16_GridViewCell(MbGridView GridView)
            : base(GridView)
        {
        }
        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            this.Value = mValue[idx].ToString();
        }

        public override UInt16[] GetValue()
        {
            UInt16[] mValue = new UInt16[1];
            mValue[0] = Convert.ToUInt16(this.Value);
            return mValue;
        }
    }
    public class INT16_GridViewCell : ModbusRegGridViewCell
    {
        public INT16_GridViewCell(MbGridView GridView)
            : base(GridView)
        {
        }
        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            UInt16 iValue = Convert.ToUInt16(mValue[idx]);
            this.Value = unchecked((Int16)iValue);
        }

        public override UInt16[] GetValue()
        {
            UInt16[] mValue = new UInt16[1];
            Int16 iValue = Convert.ToInt16(this.Value);
            mValue[0] = unchecked((ushort)iValue);
            return mValue;
        }
    }

    public abstract class Regs32_GridViewCell : ModbusRegGridViewCell
    {
      
        private int LittleIdx;
        private int BigIdx;
        private MbGridView.Endianess _Int32Endianes;

        public Regs32_GridViewCell(MbGridView GridView, MbGridView.Endianess Int32Endianes)
            : base(GridView)
        {
            _Int32Endianes = Int32Endianes;
            if (_Int32Endianes == MbGridView.Endianess.LittleEndian) {
                LittleIdx = 0;
                BigIdx = 1;
            } else {
                LittleIdx = 1;
                BigIdx = 0;
            }
        }
        protected UInt32 GetLongValue(UInt16[] mValue, int idx)
        {
            UInt32 longValue;
            longValue = (UInt32)mValue[idx + LittleIdx];
            longValue |= (UInt32)mValue[idx + BigIdx] << 16;
            return longValue;
        }

        protected UInt16[] GetModbusData (UInt32 longValue)
        {
            UInt16[] modData = new UInt16[2];
            modData[LittleIdx] = (ushort)(longValue & 0xffff);
            modData[BigIdx] = (ushort)(longValue >> 16);
            return modData;
        }
    }

    public class UINT32_GridViewCell : Regs32_GridViewCell
    {
        public UINT32_GridViewCell(MbGridView GridView, MbGridView.Endianess Int32Endianes)
            : base(GridView, Int32Endianes)
        {
        }

        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            UInt32 longValue = GetLongValue(mValue, idx);
            this.Value = longValue.ToString();
        }

        public override UInt16[] GetValue()
        {
            UInt32 longValue = Convert.ToUInt32(this.Value);
            return GetModbusData(longValue);
        }
    }
    public class INT32_GridViewCell : Regs32_GridViewCell
    {
        public INT32_GridViewCell(MbGridView GridView, MbGridView.Endianess Int32Endianes)
            : base(GridView, Int32Endianes)
        {
        }

        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            UInt32 longValue = GetLongValue(mValue, idx);
            this.Value = unchecked((Int32)longValue);
        }

        public override UInt16[] GetValue()
        {
            Int32 iValue = Convert.ToInt32(this.Value);
            return GetModbusData(unchecked((UInt32)iValue));
        }
    }
    public class IEEE754_GridViewCell : Regs32_GridViewCell
    {
        public IEEE754_GridViewCell(MbGridView GridView, MbGridView.Endianess Int32Endianes)
            : base(GridView, Int32Endianes)
        {
        }

        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            UInt32[] bitValue = new UInt32[1];
            Single[] fValue = new Single[1];

            bitValue[0] = GetLongValue(mValue, idx);
            Buffer.BlockCopy(bitValue, 0, fValue, 0, 4);
            this.Value = fValue[0];
        }

        public override UInt16[] GetValue()
        {
            Single[] fValue = new Single[1];
            UInt32[] bitValue = new UInt32[1];

            fValue[0] = Convert.ToSingle(this.Value);
            Buffer.BlockCopy(fValue, 0, bitValue, 0, 4);
            return GetModbusData(bitValue[0]);
        }
    }

    public class PROSTUD_GridViewCell : ModbusRegGridViewCell
    {
        public PROSTUD_GridViewCell(MbGridView GridView)
            : base(GridView)
        {
        }

        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            int IntValue = mValue[idx+1];
            int sign = 1;
            if ((IntValue & 0x8000) != 0) {
                IntValue = IntValue & 0x7fff;
                sign = -1;
            }
            int FractValue = mValue[idx+0];
            double value = (double)IntValue;
            value += (double)FractValue / 100;
            if (sign < 0) {
                value = -value;
            }
            this.Value = value;
        }

        public override UInt16[] GetValue()
        {
            double dValue = Convert.ToDouble(this.Value);
            Int32 iValue;

            ushort sign = 0;
            if (dValue < 0) {
                dValue = -dValue;
                sign = 0x8000;
            }
            iValue = (ushort)Math.Truncate(dValue);
            if (iValue > Int16.MaxValue) {
                iValue = Int16.MaxValue;
            } else if (iValue < Int16.MinValue) {
                iValue = Int16.MinValue;
            }
            ushort[] modData = new ushort[2];
            modData[1] = (ushort)(iValue | sign);
            modData[0] = Convert.ToUInt16((dValue - iValue) * 100);
            return modData;
        }
    }
}
