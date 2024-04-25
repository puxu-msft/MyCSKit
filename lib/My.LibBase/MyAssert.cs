using System;

namespace My
{
    public static class Assert
    {
        public static void True(bool condition, string errorMessage = "")
        {
            if (!condition)
            {
                throw new Exception(errorMessage);
            }
        }

        public static void True(bool condition, Func<string> fnMessage)
        {
            if (!condition)
            {
                var message = fnMessage();
                throw new Exception(message);
            }
        }

        public static void Equals(object? a, object? b, string errorMessage = "")
        {
            if ((a == null) && (b == null))
                return;
            if ((a == null) != (b == null))
                throw new Exception(string.Format("{0}. Equals? a={1} b={2}", errorMessage, a, b));
            True(a.Equals(b), () => string.Format("{0}. Equals? a={1} b={2}", errorMessage, a, b));
        }

        public static void NotEquals(object? a, object? b, string errorMessage = "")
        {
            True(!object.Equals(a, b), () => string.Format("{0}. NotEquals? a={1} b={2}", errorMessage, a, b));
        }

        public static void bAndAllBits(UInt64 value, UInt64 bit, string errorMessage = "")
        {
            True((value & bit) == bit, () => string.Format("{0}. value={1} AndAllBits={2}", errorMessage, value, bit));
        }

        public static void bAndAnyBits(UInt64 value, UInt64 bit, string errorMessage = "")
        {
            True((value & bit) != 0, () => string.Format("{0}. value={1} AndAnyBits={2}", errorMessage, value, bit));
        }

        public static void bAndNoBits(UInt64 value, UInt64 bit, string errorMessage = "")
        {
            True((value & bit) == 0, () => string.Format("{0}. value={1} AndNoBits={2}", errorMessage, value, bit));
        }
    }
}
