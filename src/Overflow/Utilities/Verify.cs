using System;

namespace Overflow.Utilities
{
    static class Verify
    {
        public static void NotNull(object obj, string paramName)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }

        public static void LargerThanZero(long range, string message)
        {
            if (range < 1)
                throw new ArgumentOutOfRangeException(null, message);
        }

        public static void LargeThanOrEqualToZero(long range, string message)
        {
            if (range < 0)
                throw new ArgumentOutOfRangeException(null, message);
        }

        public static void Argument(bool condition, string message)
        {
            if(!condition)
                throw  new ArgumentException(message);
        }

        public static void Operation(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}
