namespace Overflow
{
    /// <summary>
    /// The operation expects input of a given type.
    /// </summary>
    /// <typeparam name="TInput">The type of expected input.</typeparam>
    public interface IInputOperation<in TInput>
        where TInput : class
    {
        /// <summary>
        /// Provide the expected input to the operation.
        /// </summary>
        /// <param name="input">The input</param>
        void Input(TInput input);
    }
}
