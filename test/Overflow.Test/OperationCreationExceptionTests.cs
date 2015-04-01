using System;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Xunit;
using Xunit.Extensions;

namespace Overflow.Test
{
    public class OperationCreationExceptionTests
    {
        [Theory, AutoMoqData]
        public void Creating_an_operation_creation_exception_sets_the_inner_operation(Exception error)
        {
            var sut = new OperationCreationException<Operation>(error);

            Assert.Equal(error, sut.InnerException);
        }

        [Theory, AutoMoqData]
        public void The_error_description_describes_the_overall_problem(Exception error)
        {
            var sut = new OperationCreationException<FakeOperation>(error);

            Assert.Equal("An error occurred while attempting to create an instance of the FakeOperation type. See the inner operation for details.", sut.Message);
        }
    }
}
