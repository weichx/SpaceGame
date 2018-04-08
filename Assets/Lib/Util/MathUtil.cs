using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Weichx.Util {

    public static class MathUtil {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max) {
            if (value < min) return min;
            return value > max ? max : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(float value) {
            if (value < 0) return 0;
            return value > 1 ? 1 : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PercentageOfRangeClamped(float input, float min, float max) {
            return Clamp01((input - min) / (max - min));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float PercentageOfRange(float input, float min, float max) {
            return (input - min) / (max - min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ValueFromRangePercentage(float percentage, float min, float max) {
            return percentage * (max - min) + min;
        }

        public static int CombineDigits(int a, int b) {
            int times = 10;
            while (times <= b) times *= 10;
            return a * times + b;
        }

        public static int SetHighLow16Bits(int high, int low) {
            return (high << 16) | (low & 0xffff);
        }

        public static int GetLow16Bits(int input) {
            return input & 0xffff;
        }

        public static int GetHigh16Bits(int input) {
            return (input >> 16) & (1 << 16) - 1;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct BitTwiddle {

            [FieldOffset(0)] public uint fullValue;

            [FieldOffset(0)] public ushort lowShort;
            [FieldOffset(1)] public ushort midShort;
            [FieldOffset(2)] public ushort highShort;
        
            [FieldOffset(0)] public byte byte0;
            [FieldOffset(1)] public byte byte1;
            [FieldOffset(2)] public byte byte2;
            [FieldOffset(3)] public byte byte3;

        }
    }

}