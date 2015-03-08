using System;
using System.Collections.Generic;
using System.Linq;

namespace Overflow
{
    public class WorkflowConfiguration<TOperation> : WorkflowConfiguration
        where TOperation : IOperation
    {
        public override IOperation CreateOperation()
        {
            return (TOperation)Operation.Create<TOperation>(this);
        }
    }

    public abstract class WorkflowConfiguration
    {
        public IOperationResolver Resolver { get; set; }
        public IWorkflowLogger Logger { get; set; }
        public IList<IOperationBehaviorFactory> BehaviorFactories { get; private set; }
        public IList<Type> RetryExceptionTypes { get; private set; }
        public int TimesToRetry { get; set; }
        public TimeSpan RetryDelay { get; set; }

        public abstract IOperation CreateOperation();

        protected WorkflowConfiguration()
        {
            BehaviorFactories = new List<IOperationBehaviorFactory>();
            RetryExceptionTypes = new List<Type>();
        }

        public WorkflowConfiguration WithResolver(IOperationResolver resolver)
        {
            Resolver = resolver;

            return this;
        }

        public WorkflowConfiguration WithLogger(IWorkflowLogger logger)
        {
            Logger = logger;

            return this;
        }

        public WorkflowConfiguration WithBehaviorFactory(IOperationBehaviorFactory factory)
        {
            BehaviorFactories.Add(factory);

            return this;
        }

        public WorkflowConfiguration WithGlobalRetryBehavior(int timesToRetry, TimeSpan retryDelay, params Type[] retryExceptionTypes)
        {
            if (retryExceptionTypes.Any(t => !typeof(Exception).IsAssignableFrom(t)))
                throw new ArgumentException("Only exception types are valid.", "retryExceptionTypes");

            TimesToRetry = timesToRetry;
            RetryDelay = retryDelay;

            foreach(var retryExceptionType in retryExceptionTypes)
                RetryExceptionTypes.Add(retryExceptionType);

            return this;
        }
    }
}
