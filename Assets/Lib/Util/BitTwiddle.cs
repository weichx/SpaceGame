using System.Runtime.InteropServices;

namespace Weichx.Util {

     [StructLayout(LayoutKind.Explicit)]
        public struct BitTwiddle {

            [FieldOffset(0)] public int intVal;
            [FieldOffset(0)] public uint uintVal;

            [FieldOffset(0)] public byte byte0;
            [FieldOffset(1)] public byte byte1;
            [FieldOffset(2)] public byte byte2;
            [FieldOffset(3)] public byte byte3;

            [FieldOffset(0)] public sbyte sbyte0;
            [FieldOffset(1)] public sbyte sbyte1;
            [FieldOffset(2)] public sbyte sbyte2;
            [FieldOffset(3)] public sbyte sbyte3;
            
            [FieldOffset(0)] public short lowShort;
            [FieldOffset(1)] public short midShort;
            [FieldOffset(2)] public short highShort;

            [FieldOffset(0)] public ushort ulowShort;
            [FieldOffset(1)] public ushort umidShort;
            [FieldOffset(2)] public ushort uhighShort;
            
            public BitTwiddle(int value) : this() {
                intVal = value;
            }

            public BitTwiddle(short lowShort, short highShort) : this() {
                this.lowShort = lowShort;
                this.highShort = highShort;
            }

            public BitTwiddle(byte byte0, byte byte1, byte byte2, byte byte3) : this() {
                this.byte0 = byte0;
                this.byte1 = byte1;
                this.byte2 = byte2;
                this.byte3 = byte3;
            }

            public BitTwiddle(short lowShort, byte byte2, byte byte3) : this() {
                this.lowShort = lowShort;
                this.byte2 = byte2;
                this.byte3 = byte3;
            }

            public BitTwiddle(byte byte0, short midShort, byte byte3) : this() {
                this.byte0 = byte0;
                this.midShort = midShort;

                this.byte3 = byte3;
            }

            public BitTwiddle(byte byte0, byte byte1, short highShort) : this() {
                this.byte0 = byte0;
                this.byte1 = byte1;
                this.highShort = highShort;
            }

            public static implicit operator int(BitTwiddle bt) {
                return bt.intVal;
            }

            public byte this[int idx] {
                get {
                    switch (idx) {
                        case 0:  return byte0;
                        case 1:  return byte1;
                        case 2:  return byte2;
                        case 3:  return byte3;
                        default: return 0;
                    }
                }
                set {
                    switch (idx) {
                        case 0:
                            byte0 = value;
                            break;
                        case 1:
                            byte1 = value;
                            break;
                        case 2:
                            byte2 = value;
                            break;
                        case 3:
                            byte3 = value;
                            break;
                        default: return;
                    }
                }
            }

        }


}