using System;
using Overflow.Utilities;
using Xunit;

namespace Overflow.Test.Utilities
{
    public class VerifyTests
    {
        [Fact]
        public void Non_null_values_are_correctly_verified()
        {
            Verify.NotNull(new Object(), "member");
        }

        [Fact]
        public void Null_values_are_correctly_verified()
        {
            Assert.Throws<ArgumentNullException>(() => Verify.NotNull(null, "member"));
        }

        [Fact]
        public void Positive_values_are_correctly_verified_as_larger_than_zero()
        {
            Verify.LargerThanZero(1, "message");
        }

        [Fact]
        public void Zero_values_are_correctly_verified_as_not_larger_than_zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LargerThanZero(0, "message"));
        }

        [Fact]
        public void Negative_values_are_correctly_verified_as_not_larger_than_zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LargerThanZero(-1, "message"));
        }

        [Fact]
        public void Positive_values_are_correctly_verified_as_larger_than_or_equal_to_zero()
        {
            Verify.LargeThanOrEqualToZero(1, "message");
        }

        [Fact]
        public void Zero_values_are_correctly_verified_as_larger_than_or_equal_to_zero()
        {
            Verify.LargeThanOrEqualToZero(0, "message");
        }

        [Fact]
        public void Negative_values_are_correctly_verified_as_not_larger_than_or_equal_to_zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LargeThanOrEqualToZero(-1, "message"));
        }

        [Fact]
        public void Arguments_are_correctly_verified_as_valid()
        {
            Verify.Argument(true, "message");
        }

        [Fact]
        public void Arguments_are_correctly_verified_as_invalid()
        {
            Assert.Throws<ArgumentException>(() => Verify.Argument(false, "message"));
        }

        [Fact]
        public void Operations_are_correctly_verified_as_valid()
        {
            Verify.Operation(true, "message");
        }

        [Fact]
        public void Operations_are_correctly_verified_as_invalid()
        {
            Assert.Throws<InvalidOperationException>(() => Verify.Operation(false, "message"));
        }
    }
}
