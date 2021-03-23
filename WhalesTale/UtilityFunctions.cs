using System;
using System.Linq;

namespace WhalesTale
{
    public static class UtilityFunctions
    {
        public static ushort SwapBytes(ushort val) => (ushort) ((ushort) (val >> 8) | (ushort) (val << 8));

        public static short SignExtend(byte[] bytes, bool swapBytes = false)
        {
            if (swapBytes) Array.Reverse(bytes);
            return BitConverter.ToInt16(bytes, 0);
        }

        public static byte ComputeCheckSum(byte[] data)
        {
            return unchecked((byte) data.Sum(x => (long) x));
        }
    }
}