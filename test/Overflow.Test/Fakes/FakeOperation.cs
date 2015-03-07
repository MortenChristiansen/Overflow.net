using System;
using System.Collections.Generic;

namespace Overflow.Test.Fakes
{
    class FakeOperation : Operation
    {
        private readonly IOperation[] _childOperations;

        public static readonly List<IOperation> ExecutedOperations = new List<IOperation>();

        public Exception ThrowOnExecute { get; set; }
        public bool HasExecuted { get; private set; }
        public WorkflowConfiguration InitializedConfiguration { get; private set; }

        public FakeOperation(params IOperation[] childOperations)
        {
            _childOperations = childOperations;
            ExecutedOperations.Clear();
        }

        protected override void OnExecute()
        {
            HasExecuted = true;
            ExecutedOperations.Add(this);

            if (ThrowOnExecute != null)
                throw ThrowOnExecute;
        }

        public override void Initialize(WorkflowConfiguration configuration)
        {
            InitializedConfiguration = configuration;

            base.Initialize(configuration);
        }

        public override IEnumerable<IOperation> GetChildOperations()
        {
            return _childOperations;
        }

        public IOperation PublicCreate<T>() 
            where T : IOperation
        {
            return Create<T>();
        }
    }
}
