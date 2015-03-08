namespace Overflow
{
    public enum BehaviorIntegrityMode
    {
        /// <summary>
        /// The behavior provides no promises regarding the integrity of the workflow after it has run. 
        /// Behaviors with this mode are always executed last.
        /// </summary>
        NoIntegrityContract = 0,
        /// <summary>
        /// The behavior can interfere with the workflow, changing the sequence and execution of operations. The 
        /// FullIntegrity, MaintainsDataIntegrity and MaintainsOperationIntegrity modes are allowed to 
        /// run before this behavior mode.
        /// </summary>
        MaintainsWorkflowIntegrity = 100,
        /// <summary>
        /// The behavior can interfere with the execution of the operation in an effort to correct errors
        /// or otherwise avoid problems from escalating out of the operation. The FullIntegrity and 
        /// MaintainsDataIntegrity modes are allowed to run before this behavior mode.
        /// </summary>
        MaintainsOperationIntegrity = 200,
        /// <summary>
        /// The behavior can make changes to to external/persisted data as a result of an error, but does
        /// not otherwise interfere with the execution of the operation. Only FullIntegrity mode behaviors
        /// are allowed to run before this behavior mode.
        /// </summary>
        MaintainsDataIntegrity = 300,
        /// <summary>
        /// The behavior does not interfere with the execution of the operation, with the exception of
        /// catching and rethrowing exceptions. The workflow applies all behaviors with this integrity 
        /// mode before behaviors with any other mode. If the integrity mode is respected in the behaviors,
        /// they will all have a chance to perform their logic, no matter what happens.
        /// </summary>
        FullIntegrity = 400,
    }
}
