using System;
using System.Collections.Generic;
using System.Linq;
using Overflow.Extensibility;
using Overflow.Utilities;
using System.Reflection;

namespace Overflow
{
    /// <summary>
    /// One step in a larger workflow. Operations can be isolated or
    /// contain child operations.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// Get the child operations which have been executed during the execution of this
        /// operation.
        /// </summary>
        IEnumerable<ExecutionInfo> ExecutedChildOperations { get; }
 
        /// <summary>
        /// Initialize the operation. 
        /// </summary>
        /// <param name="configuration">The global configuration of the workflow</param>
        void Initialize(WorkflowConfiguration configuration);
        /// <summary>
        /// Run the logic of the operation followed by all the
        /// child operations.
        /// </summary>
        void Execute();
        /// <summary>
        /// Yield the child operations of the operation. This collection should always be
        /// evaluated one at a time, executing each task before retreiving the next. Otherwise,
        /// correct execution cannot be expected.
        /// </summary>
        /// <returns>Any child operations. Note that the execution of each child operation can
        /// influence the next sibling operations.</returns>
        IEnumerable<IOperation> GetChildOperations();
    }

    /// <summary>
    /// Extension methods for the IOperation interface.
    /// </summary>
    public static class IOperationExtensions
    {
        /// <summary>
        /// Retreive all the executed child operations in the entire hierarchy of child operations
        /// underneath the current operation.
        /// </summary>
        /// <param name="parentOperation">The root operation</param>
        /// <returns>All executed child operations</returns>
        public static IList<ExecutionInfo> GetExecutedChildOperationsForOperationHierarchy(this IOperation parentOperation)
        {
            Verify.NotNull(parentOperation, nameof(parentOperation));

            var result = new List<ExecutionInfo>();

            foreach (var execution in parentOperation.ExecutedChildOperations)
            {
                result.Add(execution);
                result.AddRange(execution.Operation.GetExecutedChildOperationsForOperationHierarchy());
            }

            return result;
        }

        /// <summary>
        /// Retrieve the original operation without beahviors.
        /// </summary>
        /// <param name="operation">An operation, possibly having behaviors attached to it</param>
        /// <returns>The original operation</returns>
        public static IOperation GetInnermostOperation(this IOperation operation)
        {
            Verify.NotNull(operation, nameof(operation));

            while (true)
            {
                var behavior = operation as OperationBehavior;
                if (behavior != null)
                {
                    operation = behavior.InnerOperation;
                    continue;
                }

                return operation;
            }
        }

        /// <summary>
        /// Provides input explicitly for an operation. Normally, inputs are supplied by
        /// the workflow infrastructure but if needed you can supply inputs directly 
        /// using this method - for example to provide inputs for the root operation.
        /// 
        /// This method expects there to be a property of type TInput that is annotated 
        /// with the Input attribute. This property will be assigned to the input value. 
        /// If the property is also annotated with the Pipe attribute, the value will
        /// be piped to child operations.
        /// </summary>
        /// <param name="operation">The operation to inspect for the input property.</param>
        /// <param name="input">The value of the input to assign to the input property.</param>
        public static void ProvideInput<TInput>(this IOperation operation, TInput input)
            where TInput : class
        {
            var innermostOperation = operation.GetInnermostOperation();

            if (innermostOperation is IInputOperation<TInput>)
            {
                ((IInputOperation<TInput>)innermostOperation).Input(input);
            }
            else
            {
                var inputData = innermostOperation.GetType().GetTypeInfo().GetProperties().Where(p => p.PropertyType == typeof(TInput) && p.GetCustomAttributes(typeof(InputAttribute), true).Any()).Select(p => Tuple.Create(p, p.GetCustomAttributes(typeof(PipeAttribute), true).Any())).FirstOrDefault();
                if (inputData?.Item1 != null)
                {
                    inputData.Item1.SetValue(innermostOperation, input, null);

                    if (inputData.Item2)
                        (innermostOperation as Operation)?.InternalPipeInputToChildOperations(input);
                }
                else
                {
                    throw new ArgumentException($"The operation type {innermostOperation.GetType().Name} does not have a public property of type {typeof(TInput).Name} with the {nameof(InputAttribute)}.");
                }
            }
        }
    }
}
