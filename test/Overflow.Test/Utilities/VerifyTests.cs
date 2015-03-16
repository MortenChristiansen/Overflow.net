using System;
using Overflow.Utilities;
using Xunit;

namespace Overflow.Test.Utilities
{
    public class VerifyTests
    {
        private const string Message = "message";
        private const string Member = "member";

        [Fact]
        public void Non_null_values_are_correctly_verified()
        {
            Verify.NotNull(new Object(), Member);
        }

        [Fact]
        public void Null_values_are_correctly_verified()
        {
            Assert.Throws<ArgumentNullException>(() => Verify.NotNull(null, Member));
        }

        [Fact]
        public void Positive_values_are_correctly_verified_as_larger_than_zero()
        {
            Verify.LargerThanZero(1, Message);
        }

        [Fact]
        public void Zero_values_are_correctly_verified_as_not_larger_than_zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LargerThanZero(0, Message));
        }

        [Fact]
        public void Negative_values_are_correctly_verified_as_not_larger_than_zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LargerThanZero(-1, Message));
        }

        [Fact]
        public void Positive_values_are_correctly_verified_as_larger_than_or_equal_to_zero()
        {
            Verify.LargeThanOrEqualToZero(1, Message);
        }

        [Fact]
        public void Zero_values_are_correctly_verified_as_larger_than_or_equal_to_zero()
        {
            Verify.LargeThanOrEqualToZero(0, Message);
        }

        [Fact]
        public void Negative_values_are_correctly_verified_as_not_larger_than_or_equal_to_zero()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Verify.LargeThanOrEqualToZero(-1, Message));
        }

        [Fact]
        public void Arguments_are_correctly_verified_as_valid()
        {
            Verify.Argument(true, Message);
        }

        [Fact]
        public void Arguments_are_correctly_verified_as_invalid()
        {
            Assert.Throws<ArgumentException>(() => Verify.Argument(false, Message));
        }

        [Fact]
        public void Operations_are_correctly_verified_as_valid()
        {
            Verify.Operation(true, Message);
        }

        [Fact]
        public void Operations_are_correctly_verified_as_invalid()
        {
            Assert.Throws<InvalidOperationException>(() => Verify.Operation(false, Message));
        }
    }
}
