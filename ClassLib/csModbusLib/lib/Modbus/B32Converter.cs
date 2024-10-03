using System;

namespace csModbusLib
{
    /// <summary>
    /// Endianness used only for long or float values
    /// </summary>
    public enum B32Endianess
    {
        LittleEndian,
        BigEndian,
    }

    public class B32Converter
    {
        private int LittleIdx = 0;
        private int BigIdx = 1;

        public B32Converter()
        {
            setEndianess(B32Endianess.BigEndian);
        }

        public B32Converter(B32Endianess Endianess)
        {
            setEndianess(Endianess);
        }

        public void setEndianess(B32Endianess Endianess)
        {
            if (Endianess == B32Endianess.LittleEndian) {
                LittleIdx = 0;
                BigIdx = 1;
            } else {
                LittleIdx = 1;
                BigIdx = 0;
            }
        }

        public static bool Is16BitType<DataT>()
        {
            return ((typeof(DataT) == typeof(UInt16)) 
                || (typeof(DataT) == typeof(Int16)));
        }

        public static bool Is32BitType<DataT>()
        {
            return ((typeof(DataT) == typeof(UInt32))
                || (typeof(DataT) == typeof(Int32))
                || (typeof(DataT) == typeof(Single)));
        }


        public UInt16[] getModbusData(UInt32 longValue)
        {
            UInt16[] modData = new UInt16[2];
            copyUint32Value(longValue, modData, 0);
            return modData;
        }
        public UInt16[] getModbusData(Int32 int32Value)
        {
            return getModbusData(unchecked((UInt32)int32Value));
        }
        public UInt16[] getModbusData(float floatValue)
        {
            float[] fValue = new float[] { floatValue };
            UInt32[] bitValue = new UInt32[1];
            Buffer.BlockCopy(fValue, 0, bitValue, 0, 4);
            return getModbusData(bitValue[0]);
        }
        public UInt16[] getModbusData<DataT>(DataT[] srcData, int srcOffset = 0, int count = 0)
        {
            if (count == 0)
                count = srcData.Length;

            ushort[] modData = new ushort[count * 2];
            for (int i = 0; i < count; ++i) {
                DataT b32Value = srcData[srcOffset + i];
                if (copyValue(b32Value, modData, i * 2) == false)
                    return null;
            }
            return modData;
        }
        public UInt32 getUInt32Value(UInt16[] mValue, int idx)
        {
            UInt32 longValue;
            longValue = (UInt32)mValue[idx + LittleIdx];
            longValue |= (UInt32)mValue[idx + BigIdx] << 16;
            return longValue;
        }
        public Int32 getInt32Value(UInt16[] mValue, int idx)
        {
            Int32 int32Value = unchecked((Int32)getUInt32Value(mValue, idx));
            return int32Value;
        }
        public float getFloatValue(UInt16[] mValue, int idx)
        {
            UInt32[] bitValue = new UInt32[1];
            float[] fValue = new float[1];

            bitValue[0] = getUInt32Value(mValue, idx);
            Buffer.BlockCopy(bitValue, 0, fValue, 0, 4);
            return fValue[0];
        }
        public void getValues<DataT>(UInt16[] srcData, DataT[] dstData, int dstOffs = 0, int count = 0)
        {
            if (count == 0)
                count = dstData.Length;
            for (int i = 0; i < count; ++i) {
                dstData[dstOffs + i] = getValue<DataT>(srcData, i * 2);
            }
        }

        #region Private Functions
        private DataT getValue<DataT>(UInt16[] mValue, int idx)
        {
            object dstValue = (UInt32)0;
            if (typeof(DataT) == typeof(UInt32)) {
                dstValue = getUInt32Value(mValue, idx);
            } else if (typeof(DataT) == typeof(Int32)) {
                dstValue = getInt32Value(mValue, idx);
            } else if (typeof(DataT) == typeof(float)) {
                dstValue = getFloatValue(mValue, idx);
            } else {
                throw new ModbusException(ErrorCodes.ILLEGAL_DATA_TYPE);
            }
            return (DataT)dstValue;
        }

        private bool copyValue<DataT>(DataT srcValue, UInt16[] dstData, int idx = 0)
        {
            switch (srcValue) {
                case UInt32 uint32value:
                    copyUint32Value(uint32value, dstData, idx);
                    break;
                case Int32 int32value:
                    copyUint32Value((UInt32)int32value, dstData, idx);
                    break;
                case Single floatValue:
                    copyFloatValue(floatValue, dstData, idx);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void copyUint32Value(UInt32 uint32value, UInt16[] dstData, int idx)
        {
            dstData[idx + LittleIdx] = (UInt16)(uint32value & 0xffff);
            dstData[idx + BigIdx] = (UInt16)(uint32value >> 16);
        }

        private void copyFloatValue(float fValue, UInt16[] dstData, int idx)
        {
            UInt32 uivalue = FloatToUint32(fValue);
            copyUint32Value(uivalue, dstData, idx);
        }

        private UInt32 FloatToUint32(float fValue)
        {
            byte[] bytes = BitConverter.GetBytes(fValue);
            return BitConverter.ToUInt32(bytes, 0);
        }
        #endregion
    }
}