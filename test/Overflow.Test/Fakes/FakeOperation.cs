using System;
using System.Collections.Generic;

namespace Overflow.Test.Fakes
{
    public class FakeOperation : Operation
    {
        private readonly IOperation[] _childOperations;

        public static readonly List<IOperation> ExecutedOperations = new List<IOperation>();

        public Exception ThrowOnExecute { get; set; }
        public bool HasExecuted { get; private set; }
        public WorkflowConfiguration InitializedConfiguration { get; private set; }
        public int ErrorCount { get; set; }
        public Action ExecuteAction { get; set; }

        public FakeOperation(params IOperation[] childOperations)
        {
            ErrorCount = -1;
            _childOperations = childOperations;
            ExecutedOperations.Clear();
        }

        protected override void OnExecute()
        {
            HasExecuted = true;
            ExecutedOperations.Add(this);

            ExecuteAction?.Invoke();

            if (ThrowOnExecute != null && ErrorCount-- != 0)
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

        public IOperation PublicCreate<TOperation, TInput>(TInput input)
            where TInput : class
            where TOperation : IOperation
        {
            return Create<TOperation, TInput>(input);
        }

        public IOperation PublicCreate<TOperation, TInput1, TInput2>(TInput1 input1, TInput2 input2)
            where TInput1 : class
            where TInput2 : class
            where TOperation : IOperation
        {
            return Create<TOperation, TInput1, TInput2>(input1, input2);
        }

        public IOperation PublicCreate<TOperation, TInput1, TInput2, TInput3>(TInput1 input1, TInput2 input2, TInput3 input3)
            where TInput1 : class
            where TInput2 : class
            where TInput3 : class
            where TOperation : IOperation
        {
            return Create<TOperation, TInput1, TInput2, TInput3>(input1, input2, input3);
        }

        public void PublicPipeInputToChildOperations<TInput>(TInput input)
        {
            PipeInputToChildOperations(input);
        }

        public object PublicGetChildOutputValue<TOutput>()
            where TOutput : class
        {
            return GetChildOutputValue<TOutput>();
        }
    }
}
