using System.Collections.Generic;

namespace Overflow.Test.Fakes
{
    class FakeOperation : Operation
    {
        private readonly IOperation[] _childOperations;

        public static readonly List<IOperation> ExecutedOperations = new List<IOperation>();

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
        }

        public override IEnumerable<IOperation> GetChildOperations()
        {
            return _childOperations;
        }
    }
}
