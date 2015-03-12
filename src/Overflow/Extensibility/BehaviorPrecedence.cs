namespace Overflow.Extensibility
{

    public enum BehaviorPrecedence
    {
        /// <summary>
        /// If something goes wrong with the actual execution, this type of behavior
        /// can be used for bringing persistent state back into a proper condition,
        /// for example by rolling back transactions or deleting created files.
        /// </summary>
        StateRecovery = 100,
        /// <summary>
        /// If something goes wrong with the actual execution, this type of behavior
        /// can be used for recovering from the bad state and completing the task.
        /// </summary>
        WorkRecovery = 200,
        /// <summary>
        /// If something goes wrong with the actual execution that cannot be cleaned up,
        /// this type of behavior can be used to perform compensating work.
        /// </summary>
        WorkCompensation = 300,
        /// <summary>
        /// The last behavior before types related to actual execution of work, it
        /// allows you to create a bulkhead, guarding the rest of the workflow against
        /// errors and problems caused by the operation.
        /// </summary>
        Containment = 400,
        /// <summary>
        /// Before behaviors related to the execution of the operation are run, staging 
        /// behaviors can be used for things such as preparing for execution,
        /// determining whether or how to execute, and other reflective tasks.
        /// </summary>
        Staging = 500,
        /// <summary>
        /// The first behavior type to run, logging behaviors can be used to document 
        /// work, errors and other events.
        /// about what happens.
        /// </summary>
        Logging = 600
    }
}
