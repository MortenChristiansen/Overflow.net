namespace Overflow
{
    /// <summary>
    /// Applies the Conditional Execution behavior. The operation can determine 
    /// whether it should be executed or not.
    /// </summary>
    public interface IConditionalOperation
    {
        /// <summary>
        /// Get whether the operation should skip execution.
        /// </summary>
        bool SkipExecution { get; }
    }
}
