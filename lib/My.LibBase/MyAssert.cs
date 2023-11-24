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
            True(object.Equals(a, b), () => string.Format("{0}. a={1} b={2}", errorMessage, a, b));
        }

        public static void NotEquals(object? a, object? b, string errorMessage = "")
        {
            True(!object.Equals(a, b), () => string.Format("{0}. a={1} b={2}", errorMessage, a, b));
        }
    }
}
