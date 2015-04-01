using System;

namespace Overflow
{
    /// <summary>
    /// Indicates that an error occurred during the process of creating an
    /// instance of the TOperation type during a call to the Operation.Create 
    /// method.
    /// </summary>
    /// <typeparam name="TOperation">The type of operation that failed to be created.</typeparam>
    public class OperationCreationException<TOperation> : Exception
        where TOperation : IOperation
    {
        /// <summary>
        /// Create a new instance of the exception.
        /// </summary>
        /// <param name="causeOfFailure">The exception thrown when trying to create
        /// the operation</param>
        public OperationCreationException(Exception causeOfFailure)
            : base(CreateMessage(), causeOfFailure)
        {
            
        }

        private static string CreateMessage()
        {
            var operationType = typeof (TOperation).Name;

            return "An error occurred while attempting to create an instance of the " + operationType + " type. See the inner operation for details.";
        }
    }
}
