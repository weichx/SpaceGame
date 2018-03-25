using System.Runtime.CompilerServices;

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

    }

}