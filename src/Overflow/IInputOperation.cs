using System;

namespace Overflow
{
    /// <summary>
    /// The operation expects input of a given type.
    /// </summary>
    /// <typeparam name="TInput">The type of expected input.</typeparam>
    [Obsolete("Use the InputAttribute class to annotate properties that receive input values. This interface will be removed in a future release.")]
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
