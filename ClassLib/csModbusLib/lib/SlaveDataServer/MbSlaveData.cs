using System;
using System.Collections.Generic;

namespace csModbusLib {

    public class ModbusData
    {
        protected ModbusData NextData;
        protected int MyBaseAddr;
        protected int MySize;
        public ushort[] Data;

        public class ModbusValueEventArgs : EventArgs
        {
            public int BaseIdx { get; set; }
            public int Size { get; set; }
            public ModbusValueEventArgs(int aBaseIdx, int aSize)
            {
                BaseIdx = aBaseIdx;
                Size = aSize;
            }
        }

        public delegate void ValueChangedHandler(object sender, ModbusValueEventArgs e);
        public delegate void ValueReadHandler(object sender, ModbusValueEventArgs e);

        public event ValueChangedHandler ValueChangedEvent;
        public event ValueReadHandler ValueReadEvent;

        private void RaiseValueChangedEvent(int Addr, int Size) {
            ValueChangedEvent?.Invoke(this, new ModbusValueEventArgs(Addr - MyBaseAddr, Size));
        }

        private void RaiseValueReadEvent(int Addr, int Size) {
            ValueReadEvent?.Invoke(this, new ModbusValueEventArgs(Addr - MyBaseAddr, Size));
        }

        public ModbusData() {
            Data = null;
            Init(0, 0);
        }

        public ModbusData(int aAddress, int aSize) {
            Data = new ushort[aSize];
            Init(aAddress, aSize);
        }

        public ModbusData(int Address, ushort[] bData)
        {
            Data = bData;
            Init(Address, bData.Length);
        }

        protected void Init(int aAddress, int aSize)
        {
            MyBaseAddr = aAddress;
            MySize = aSize;
            NextData = null;
        }

        public int BaseAddr
        {
            get {
                return MyBaseAddr;
            }
        }

        public void AddModbusData(ModbusData newData)
        {
            if (NextData == null) {
                NextData = newData;
            } else {
                NextData.AddModbusData(newData);
            }
        }

        public bool ScannAll4Reading(MBSFrame Frame)
        {
            return ScannAll(x => x.ReadMultipleEvent(Frame), Frame);
        }

        public bool ScannAll4Writing(MBSFrame Frame)
        {
            return ScannAll(x => x.WriteMultipleEvent(Frame), Frame);
        }

        public bool ScannAll4SingleWrite(MBSFrame Frame)
        {
            return ScannAll(x => x.WriteSingleEvent(Frame), Frame);
        }

        private bool ScannAll(Action<ModbusData> func, MBSFrame Frame)
        {
            if (Frame.MatchAddress(MyBaseAddr, MySize)) {
                func(this);
                return true;
            } else {
                if (NextData != null) {
                    return NextData.ScannAll(func, Frame);
                }
                return false;
            }
        }

        private void ReadMultipleEvent(MBSFrame Frame)
        {
            ReadMultiple(Frame);
            RaiseValueReadEvent(Frame.DataAddress, Frame.DataCount);
        }

        private void WriteMultipleEvent(MBSFrame Frame)
        {
            WriteMultiple(Frame);
            RaiseValueChangedEvent(Frame.DataAddress, Frame.DataCount);
        }

        private void WriteSingleEvent(MBSFrame Frame)
        {
            WriteSingle(Frame);
            RaiseValueChangedEvent(Frame.DataAddress, Frame.DataCount);
        }

        protected virtual void ReadMultiple(MBSFrame Frame) { }
        protected virtual void WriteMultiple(MBSFrame Frame) { }
        protected virtual void WriteSingle(MBSFrame Frame) { }


        public void AddData(int aAddress, int Length)
        {
            if (MySize == 0) {
                Init(aAddress, Length);
                Data = new ushort[Length];
            } else {
                AddModbusData(new ModbusData(aAddress, Length));
            }
        }

        public void AddData(int aAddress, ushort[] bData)
        {
            if (MySize == 0) {
                Init(aAddress, bData.Length);
                Data = bData;
            } else {
                AddModbusData(new ModbusData(aAddress, bData));
            }
        }
    }

    public class ModbusRegsData : ModbusData
    {
        public ModbusRegsData() :base () {}
        public ModbusRegsData(int Address, int Length) : base(Address, Length) {}
        public ModbusRegsData(int Address, ushort[] Data) : base(Address, Data) { }

        protected override void ReadMultiple(MBSFrame Frame) {
            Frame.PutResponseValues(MyBaseAddr, Data);
        }
        protected override void WriteMultiple(MBSFrame Frame) {
            Frame.GetRequestValues(MyBaseAddr, Data);
        }
        protected override void WriteSingle(MBSFrame Frame) {
            Data[Frame.DataAddress - MyBaseAddr] = Frame.GetRequestSingleUInt16();
        }
    }

    public class ModbusCoilsData : ModbusData
    {
        public ModbusCoilsData() : base() { }
        public ModbusCoilsData(int Address, int Length) : base(Address, Length) { }
        public ModbusCoilsData(int Address, ushort[] Data) : base(Address, Data) { }
        protected override void ReadMultiple(MBSFrame Frame) {
            Frame.PutResponseBitValues(MyBaseAddr, Data);
        }
        protected override void WriteMultiple(MBSFrame Frame) {
            Frame.GetRequestBitValues(MyBaseAddr, Data);
        }
        protected override void WriteSingle(MBSFrame Frame) {
            Data[Frame.DataAddress - MyBaseAddr] = Frame.GetRequestSingleBit();
        }
    }
}

