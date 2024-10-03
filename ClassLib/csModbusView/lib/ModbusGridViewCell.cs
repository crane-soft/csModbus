using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using csModbusLib;

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

        protected UInt32 GetHexValue()
        {
            string hexStr = this.Value.ToString().ToLower();
            if (hexStr.StartsWith("0x")) {
                hexStr = hexStr.Substring(2);
            } else {
                int hend = hexStr.IndexOf("h");
                if (hend >= 0) {
                    hexStr = hexStr.Substring(0, hend);
                }
            }
            UInt32 longValue = Convert.ToUInt32(hexStr, 16);
            return longValue;
        }
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

    public class HEX16_GridViewCell : ModbusRegGridViewCell
    {
        public HEX16_GridViewCell(MbGridView GridView)
            : base(GridView)
        {
        }
        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            this.Value = mValue[idx].ToString("X4")+ "h";
        }

        public override UInt16[] GetValue()
        {
            UInt32 longValue = GetHexValue();
            UInt16[] mValue = new UInt16[1];
            mValue[0] = (UInt16)(longValue & 0xffff);
            return mValue;
        }
    }

    public abstract class Regs32_GridViewCell : ModbusRegGridViewCell
    {
        protected B32Converter B32Converter;

        public Regs32_GridViewCell(MbGridView GridView, B32Endianess Int32Endianes = B32Endianess.LittleEndian)
            : base(GridView)
        {
            B32Converter = new B32Converter(Int32Endianes);
        }
    }

    public class UINT32_GridViewCell : Regs32_GridViewCell
    {
        public UINT32_GridViewCell(MbGridView GridView, B32Endianess Int32Endianes)
            : base(GridView, Int32Endianes)
        {
        }

        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            UInt32 longValue = B32Converter.getUInt32Value(mValue, idx);
            this.Value = longValue.ToString();
        }

        public override UInt16[] GetValue()
        {
            UInt32 longValue = Convert.ToUInt32(this.Value);
            return B32Converter.getModbusData(longValue);
        }
    }

    public class HEX32_GridViewCell : Regs32_GridViewCell
    {
        public HEX32_GridViewCell(MbGridView GridView, B32Endianess Int32Endianes)
            : base(GridView, Int32Endianes)
        {
        }

        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            UInt32 longValue = B32Converter.getUInt32Value(mValue, idx);
            this.Value = longValue.ToString("X8") + "h";
        }

        public override UInt16[] GetValue()
        {
            UInt32 longValue = GetHexValue();
            return B32Converter.getModbusData(longValue);
        }
    }

    public class INT32_GridViewCell : Regs32_GridViewCell
    {
        public INT32_GridViewCell(MbGridView GridView, B32Endianess Int32Endianes)
            : base(GridView, Int32Endianes)
        {
        }

        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            Int32 longValue = B32Converter.getInt32Value(mValue, idx);
            this.Value = longValue;
        }

        public override UInt16[] GetValue()
        {
            Int32 iValue = Convert.ToInt32(this.Value);
            return B32Converter.getModbusData(iValue);
        }
    }
    public class IEEE754_GridViewCell : Regs32_GridViewCell
    {
        public IEEE754_GridViewCell(MbGridView GridView, B32Endianess Int32Endianes)
            : base(GridView, Int32Endianes)
        {
        }

        public override void SetValue(UInt16[] mValue, int idx = 0)
        {
            float fValue = B32Converter.getFloatValue(mValue, idx);
            this.Value = fValue;
        }

        public override UInt16[] GetValue()
        {
            float fValue = Convert.ToSingle(this.Value);
            return B32Converter.getModbusData(fValue);
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
