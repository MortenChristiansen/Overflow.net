using System;
using System.Collections.Generic;

namespace Overflow.Test.Fakes
{
    class FakeOperation : Operation
    {
        private readonly IOperation[] _childOperations;

        public static readonly List<IOperation> ExecutedOperations = new List<IOperation>();

        public Exception ThrowOnExecute { get; set; }

        public FakeOperation(params IOperation[] childOperations)
        {
            _childOperations = childOperations;
            ExecutedOperations.Clear();
        }

        public bool HasExecuted { get; private set; }

        protected override void OnExecute()
        {
            HasExecuted = true;
            ExecutedOperations.Add(this);

            if (ThrowOnExecute != null)
                throw ThrowOnExecute;
        }

        public override IEnumerable<IOperation> GetChildOperations()
        {
            return _childOperations;
        }
    }
}
