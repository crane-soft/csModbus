using System;
using System.Collections.Generic;

namespace csModbusLib {

    public class ModbusData 
    {
        protected ModbusData NextData;
        protected int MyBaseAddr;
        protected int MySize;

        public class ModbusValueEventArgs : EventArgs 
        {
            public int BaseIdx { get; set; }
            public int Size { get; set; }
            public ModbusValueEventArgs (int aBaseIdx, int aSize)
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

        private void RaiseValueReadEvent(int Addr, int Size)  {
            ValueReadEvent?.Invoke(this, new ModbusValueEventArgs(Addr - MyBaseAddr, Size));
        }

        public ModbusData() {
            Init(0, 0);
        }

        public ModbusData(int aAddress, int aSize) {
            Init(aAddress, aSize);
        }

        protected void Init(int aAddress, int aSize) {
            MyBaseAddr = aAddress;
            MySize = aSize;
            NextData = null;
        }

        public int BaseAddr {
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

        public bool ScannAll4Reading(MBSFrame Frame) {
            return ScannAll(x => x.ReadMultipleEvent(Frame), Frame);
        }

        public bool ScannAll4Writing(MBSFrame Frame) {
            return ScannAll(x => x.WriteMultipleEvent(Frame), Frame);
        }

        public bool ScannAll4SingleWrite(MBSFrame Frame) {
            return ScannAll(x => x.WriteSingleEvent(Frame), Frame);
        }

        private bool ScannAll(Action<ModbusData> func, MBSFrame Frame)
        {
            if (Frame.MatchAddress(MyBaseAddr, MySize)) {
                func(this);
                RaiseValueReadEvent(Frame.DataAddress, Frame.DataCount);
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
            RaiseValueReadEvent(Frame.DataAddress, Frame.DataCount);
            ReadMultiple(Frame);
        }

        private void WriteMultipleEvent(MBSFrame Frame)
        {
            RaiseValueChangedEvent(Frame.DataAddress, Frame.DataCount);
            WriteMultiple(Frame);
        }

        private void WriteSingleEvent(MBSFrame Frame)
        {
            RaiseValueChangedEvent(Frame.DataAddress, Frame.DataCount);
            WriteSingle(Frame);
        }

        protected virtual void ReadMultiple(MBSFrame Frame) { }
        protected virtual void WriteMultiple(MBSFrame Frame) { }
        protected virtual void WriteSingle(MBSFrame Frame) { }
    }

    public class ModbusDataT<DataT> : ModbusData 
    {
        public DataT[] Data;

        public ModbusDataT()  {
            Data = null;
        }

        public ModbusDataT(int Address, int Length) : base(Address, Length)  {
            Data = new DataT[Length];
        }

        public ModbusDataT(int Address, DataT[] bData) : base(Address, bData.Length) {
            Data = bData;
        }

        public void AddData(int aAddress, int Length)
        {
            if (base.MySize == 0) {
                base.Init(aAddress, Length);
                Data = new DataT[Length];
            } else {
                AddModbusData(new ModbusDataT<DataT>(aAddress, Length));
            }
        }

        public void AddData(int aAddress, DataT[] bData)
        {
            if (base.MySize == 0) {
                base.Init(aAddress, bData.Length);
                Data = bData;
            } else {
                AddModbusData(new ModbusDataT<DataT>(aAddress, bData));
            }
        }

        protected override void ReadMultiple (MBSFrame Frame) 
        {
            if (typeof(DataT) == typeof(bool)) {
                Frame.PutResponseValues(MyBaseAddr, (bool[])(object)Data);
            } else {
                Frame.PutResponseValues(MyBaseAddr, (ushort[])(object)Data);
            }
        }

        protected override void WriteMultiple(MBSFrame Frame)
        {
            if (typeof(DataT) == typeof(bool)) {
                Frame.GetRequestValues(MyBaseAddr, (bool[])(object)Data);
            } else {
                Frame.GetRequestValues(MyBaseAddr, (ushort[])(object)Data);
            }
        }

        protected override void WriteSingle(MBSFrame Frame)
        {
            if (typeof(DataT) == typeof(bool)) {
                Data[Frame.DataAddress - MyBaseAddr] = (DataT)(object)Frame.GetRequestSingleBit();
            } else {
                Data[Frame.DataAddress - MyBaseAddr] = (DataT)(object)Frame.GetRequestSingleUInt16();
            }
        }
    }

    public class ModbusRegsData : ModbusDataT<ushort>
    {
        public ModbusRegsData() :base () {}
        public ModbusRegsData(int Address, int Length) : base(Address, Length) {}
        public ModbusRegsData(int Address, ushort[] Data) : base(Address, Data.Length) {}
    }

    public class ModbusCoilsData : ModbusDataT<bool>
    {
        public ModbusCoilsData() : base() { }
        public ModbusCoilsData(int Address, int Length) : base(Address, Length) { }
        public ModbusCoilsData(int Address, bool[] Data) : base(Address, Data.Length) { }
    }
}

