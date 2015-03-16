namespace Overflow
{
    /// <summary>
    /// Creates operation instances, applying behaviors and resolving dependencies.
    /// </summary>
    public interface IOperationResolver
    {
        /// <summary>
        /// Create a new instance of an operation. All beahviors are correctly attached
        /// and initialized. Constructor arguments are supplied.
        /// </summary>
        /// <typeparam name="TOperation">The IOperation implementation to create</typeparam>
        /// <param name="configuration">The global workflow configuration</param>
        /// <returns>The newly created operation instance</returns>
        IOperation Resolve<TOperation>(WorkflowConfiguration configuration) where TOperation : IOperation;
    }
}
