using System;

namespace Overflow.Test.Fakes
{
    class FakeOperationBehaviorAttribute : OperationBehaviorAttribute
    {
        public FakeOperationBehaviorAttribute(Type operationType) : base(operationType) { }
    }
}
