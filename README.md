#Overflow.net

[![Build Status](https://travis-ci.org/MortenChristiansen/Overflow.net.svg?branch=creating-operations-and-resolving-dependencies)](https://travis-ci.org/MortenChristiansen/Overflow.net)

Overflow is an open source C# portable class library for modeling workflows using simple objects. It allows you to define workflows using a simple and clean syntax, while you apply operational behaviors such as error handling strategies in a declarative fashion using attributes.

    public class SendPartyInvitesOperation : Operation
    {
        public override IEnumerable<IOperation> GetChildOperations()
        {
            yield return Create<FindGuestListOperation>();
            yield return Create<PrepareInviteTemplateOperation>();
            yield return Create<CreateInviteNotificationsOperation>();
            yield return Create<SendNotificationsOperation>();
        }
    }

    [ContinueOnFailure]
    public class FindGuestListOperation : Operation
    {
        protected override void OnExecute()
        {
            ...
        }
    }

    public static class PartyWorkflows
    {
        public static void SendPartyInvites()
        {
            var workflow =
                Workflow.Configure<SendPartyInvitesOperation>().
                WithLogger(new TextWriterWorkflowLogger(System.Console.Out)).
                CreateOperation();
            workflow.Execute();
        }
    }

The library is in the very early stages of development and is still very much in a state of flux while the central APIs get defined.

##Extensibility - Custom Behaviors

Internally, the library uses its own extensibility model for attaching behaviors to operations. This is done by registering custom `IOperationBehaviorFactory` instances when configuring the workflow or by leveraging the built in factory `OperationBehaviorAttributeFactory`, which uses attributes to apply behaviors in a standard way.

Behaviors are implemented as decorators for the operations but the infrastructure takes care of applying them and in the right order.

### Adding beahviors using `OperationBehaviorAttributeFactory`

This is the easiest way to extend the workflow model to add custom behaviors. You simply have to create an attribute class inheriting from the abstract class `OperationBehaviorAttribute`. The only thing required to implement the class is to instantiate your custom behavior. The behavior will be attached automatically to operations with the attribute. To illustrate this, take the way that the `ContinueOnFailureOperationBehavior` class and its associated behavior attribute are implemented:

    public class ContinueOnFailureAttribute : OperationBehaviorAttribute
    {
        public override OperationBehavior CreateBehavior()
        {
            return new ContinueOnFailureOperationBehavior();
        }
    }

    class ContinueOnFailureOperationBehavior : OperationBehavior
    {
        public override BehaviorIntegrityMode IntegrityMode
        {
            get { return BehaviorIntegrityMode.MaintainsOperationIntegrity; }
        }

        public override void Execute()
        {
            try { base.Execute(); }
            catch { }
        }
    }

The result of this is that you can make an operation ignore errors by adding the attribute `[ContinueOnFailure]`. Of course you would probably not want to do that without some sort of logging.

### Custom behavior factories

If you want more control of how and when behaviors are created, you can create your own implementation of the `IOperationBehaviorFactory` interface. The factory `OperationBehaviorAttributeFactory` mentioned in the previous section is an example of a factory. Another useful factory could be for applying logging behavior to all operations if a logger is defined on the workflow configuration. Such a class happens to exist in the library and it is implemented like this:

    class OperationLoggingBehaviorFactory : IOperationBehaviorFactory
    {
        public IList<OperationBehavior> CreateBehaviors(IOperation operation, WorkflowConfiguration configuration)
        {
            if (operation == null)
                throw new ArgumentNullException("operation");

            if (configuration == null)
                throw new ArgumentNullException("configuration");

            if (configuration.Logger == null)
                return new OperationBehavior[0];

            return new [] { new OperationLoggingBehavior(configuration.Logger) };
        }
    }

Both the behavior factories mentioned so far are registered by default but if you want to add your own you have to register them when configuring the workflow.

    var workflow =
        Workflow.Configure<SendPartyInvitesOperation>().
        WithBehaviorFactory(new MyCustomFactory()).
        CreateOperation();

### A note about behavior integrity modes

There are many different types of behaviors and to make sure that they are applied in an order that makes sense, they are grouped into a set of modes with specific semantic meaning. Each mode describes the impact that the behavior can have on the operation and workflow. This impact determines how the behavior interacts with other behaviors which is important for keeping your workflows functioning the way you would expect. For example, consider a behavior that retries an operation a number of times in case of an error. If this behavior runs before the logging behavior, the log would not catch all the attempts but would instead see it as a single operation execution which might not even fail. In this case, a potential problem is hidden from the logging behavior because of the ordering of the operations.

The different integrity modes are defined by the `BehaviorIntegrityMode` enum and are described below with their integer values. The base operation is decorated with the behaviors sorted by their integrity modes, with the highest valued modes being executed closest to the base operation. If you do not find the existing modes detailed enough, you can define your own modes by casting a different integer to the enum, when assigning the mode in your behaviors.

**No integrity mode (0)**

The behavior provides no promises regarding the integrity of the workflow after it has run. Behaviors with this mode are always executed last.

**Maintains workflow integrity (100)**

The behavior can interfere with the workflow, changing the sequence of operations. The FullIntegrity, MaintainsDataIntegrity and MaintainsOperationIntegrity modes are allowed to run before this behavior mode.

**Maintains operation integrity (200)**

The behavior can interfere with the execution of the operation in an effort to correct errors or otherwise avoid problems from escalating out of the operation. The FullIntegrity and MaintainsDataIntegrity modes are allowed to run before this behavior mode.

**Maintains data integrity (300)**

The behavior can make changes to to external/persisted data as a result of an error, but does not otherwise interfere with the execution of the operation. Only FullIntegrity mode behaviors are allowed to run before this behavior mode.

**Full integrity 400)**

The behavior does not interfere with the execution of the operation, with the exception of catching and rethrowing exceptions. The workflow applies all behaviors with this integrity mode before behaviors with any other mode. If the integrity mode is respected in the behaviors, they will all have a chance to perform their logic, no matter what happens.
