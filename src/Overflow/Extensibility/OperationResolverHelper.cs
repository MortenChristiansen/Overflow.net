using System.Linq;

namespace Overflow.Extensibility
{
    /// <summary>
    /// Provides functionality which makes it easier to write your own
    /// IOperationResolver implementations.
    /// </summary>
    public static class OperationResolverHelper
    {
        /// <summary>
        /// Creates all the applicable beahviors and applies them to
        /// the orperation in the correct order.
        /// </summary>
        /// <param name="operation">The original workflow operation with no
        /// behaviors attached</param>
        /// <param name="configuration">The current workflow configuration</param>
        /// <returns>The input operation decorated with any behaviors created</returns>
        public static IOperation ApplyBehaviors(IOperation operation, WorkflowConfiguration configuration)
        {
            if (configuration.BehaviorFactories.Count == 0) return operation;

            var behaviors = configuration.BehaviorFactories.SelectMany(f => f.CreateBehaviors(operation, configuration));
            var sortedBehaviors = behaviors.OrderBy(b => b.Precedence).ToList();
            foreach (var behavior in sortedBehaviors)
                operation = behavior.AttachTo(operation);

            return operation;
        }
    }
}
