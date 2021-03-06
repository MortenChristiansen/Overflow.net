using System;

namespace Overflow
{
    /// <summary>
    /// The operation produces output of a given type.
    /// </summary>
    /// <typeparam name="TOutput">The type of output produced</typeparam>
    [Obsolete("Use the OutputAttribute class to annotate properties that receive output values. This interface will be removed in a future release.")]
    public interface IOutputOperation<out TOutput>
        where TOutput : class
    {
        /// <summary>
        /// Provide the operation with a callback for providing 
        /// the produced value.
        /// </summary>
        /// <param name="onReceiveOutput">A callback function for the produced value</param>
        void Output(Action<TOutput> onReceiveOutput);
    }
}
