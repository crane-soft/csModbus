using System;

namespace csModbusLib {

    public class ModbusData 
    {
        protected ModbusData NextData;
        protected int MyBaseAddr;
        protected int MySize;

        public class ValueChangedEventArgs : EventArgs 
        {
            public int BaseIdx { get; set; }
            public int Size { get; set; }
            public ValueChangedEventArgs (int aBaseIdx, int aSize)
            {
                BaseIdx = aBaseIdx;
                Size = aSize;
            }
        }

        public delegate void ValueChangedHandler(object sender, ValueChangedEventArgs e);
        public event ValueChangedHandler ValueChangedEvent;

        private void RaiseValueChangedEvent(int Addr, int Size)
        {
            if (ValueChangedEvent != null) {
                ValueChangedEvent(this, new ValueChangedEventArgs(Addr - MyBaseAddr, Size));
            }

        }

        public ModbusData()
        {
            Init(0, 0);
        }

        public ModbusData(int aAddress, int aSize)
        {
            Init(aAddress, aSize);
        }
  
        protected void Init(int aAddress, int aSize)
        {
            MyBaseAddr = aAddress;
            MySize = aSize;
            NextData = null;
        }


        public virtual void AddModbusData(ModbusData newData)
        {
            if (NextData == null) {
                NextData = newData;
            } else {
                NextData.AddModbusData(newData);
            }
        }

        public void ScannAll4Reading(MBSFrame Frame)
        {
            if (Frame.MatchAddress(MyBaseAddr, MySize)) {
                ReadMultiple(Frame);
            } else {
                if (NextData != null) {
                    NextData.ScannAll4Reading(Frame);
                }
            }
        }

        public void ScannAll4Writing(MBSFrame Frame)
        {
            if (Frame.MatchAddress(MyBaseAddr, MySize)) {
                WriteMultiple(Frame);
                RaiseValueChangedEvent(Frame.DataAddress, Frame.DataCount);
            } else {
                if (NextData != null) {
                    NextData.ScannAll4Writing(Frame);
                }
            }
        }

        public void ScannAll4SingleWrite(MBSFrame Frame)
        {
            if (Frame.MatchAddress(MyBaseAddr, MySize)) {
                WriteSingle(Frame);
                RaiseValueChangedEvent(Frame.DataAddress, Frame.DataCount);
            } else {
                if (NextData != null) {
                    NextData.ScannAll4SingleWrite(Frame);
                }
            }
        }

        protected virtual void ReadMultiple(MBSFrame Frame) { }
        protected virtual void WriteMultiple(MBSFrame Frame) { }
        protected virtual void WriteSingle(MBSFrame Frame) { }

    }

    public class ModbusBoolData : ModbusData {
        public bool[] Data;

        public ModbusBoolData()
        {   Data = null;
        }

        public ModbusBoolData (int Address, int Length)
            : base(Address, Length)
        {
            Data = new bool[Length];
        }

        public ModbusBoolData(int Address, bool[] bData)
            : base(Address, bData.Length)
        {
            Data = bData;
        }

        public void AddBools (int aAddress, int Length)
        {
            if (base.MySize == 0) {
                base.Init(aAddress, Length);
                Data = new bool[Length];
            } else {
                AddModbusData(new ModbusBoolData(aAddress, Length));
            }
        }

        public void AddBools(int aAddress, bool[] bData)
        {
            if (base.MySize == 0) {
                base.Init(aAddress, bData.Length);
                Data = bData;
            } else {
                AddModbusData(new ModbusBoolData(aAddress, bData));
            }
        }

        protected override void ReadMultiple(MBSFrame Frame)
        {   Frame.PutResponseBits(MyBaseAddr,Data);
        }

        protected override void WriteMultiple(MBSFrame Frame)
        {    Frame.GetRequestBits(MyBaseAddr,Data);
        }

        protected override void WriteSingle(MBSFrame Frame)
        {   Data[Frame.DataAddress-MyBaseAddr] = Frame.GetRequestSingleBit();
        }
    }

    public class ModbusRegsData : ModbusData {
        public ushort[] Data;

        public ModbusRegsData()
        {   Data = null;
        }

        public ModbusRegsData(int Address, ushort[] uData)
            : base(Address, uData.Length)
        {
            Data = uData;
        }

        public ModbusRegsData (int Address, int Length)
            : base(Address, Length)
        {
            Data = new UInt16[Length];
        }

        public void AddRegister (int aAddress, int Length)
        {
            if (base.MySize == 0) {
                base.Init(aAddress, Length);
                Data = new UInt16[Length];
            } else {
                AddModbusData(new ModbusRegsData(aAddress, Length));
            }
        }

        public void AddRegister(int aAddress, ushort[] uData)
        {
            if (base.MySize == 0) {
                base.Init(aAddress, uData.Length);
                Data = uData;
            } else {
                AddModbusData(new ModbusRegsData(aAddress, uData));
            }
        }

        protected override void ReadMultiple(MBSFrame Frame)
        {    Frame.PutResponseValues(MyBaseAddr, Data);
        }

        protected override void WriteMultiple(MBSFrame Frame)
        {    Frame.GetRequestValues(MyBaseAddr,Data);
        }
        protected override void WriteSingle(MBSFrame Frame)
        {    Data[Frame.DataAddress-MyBaseAddr] = Frame.GetRequestSingleUInt16();
        }
    }
}

