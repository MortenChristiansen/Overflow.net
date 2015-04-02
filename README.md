#Overflow.net

![Project logo](https://raw.githubusercontent.com/MortenChristiansen/Overflow.net/master/files/logo-128.png)

####[![Build status](https://ci.appveyor.com/api/projects/status/m8qkwl9p91jwncjo/branch/master?svg=true)](https://ci.appveyor.com/project/MortenChristiansen/overflow-net/branch/master) // windows / .net

####[![Build Status](https://travis-ci.org/MortenChristiansen/Overflow.net.svg?branch=creating-operations-and-resolving-dependencies)](https://travis-ci.org/MortenChristiansen/Overflow.net) // linux / mono

####[![Coverage Status](https://coveralls.io/repos/MortenChristiansen/Overflow.net/badge.svg)](https://coveralls.io/r/MortenChristiansen/Overflow.net)

Overflow is an open source C# library for modeling workflows using simple objects. It allows you to define workflows using a simple and clean syntax, while you apply operational behaviors such as error handling strategies in a declarative fashion using attributes.

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

## Installation

To install Overflow.net, run the following command in the Package Manager Console:

`Install-Package Overflow.net -Pre`

##Extensibility - Custom Behaviors

Internally, the library uses its own extensibility model for attaching behaviors to operations. This is done by registering custom `IOperationBehaviorFactory` instances when configuring the workflow or by leveraging the built in factory `OperationBehaviorAttributeFactory`, which uses attributes to apply behaviors in a standard way.

Behaviors are implemented as decorators for the operations but the infrastructure takes care of applying them and in the right order.

### Adding behaviors using `OperationBehaviorAttributeFactory`

This is the easiest way to extend the workflow model to add custom behaviors. You simply have to create an attribute class inheriting from the abstract class `OperationBehaviorAttribute`. The only thing required to implement the class is to instantiate your custom behavior. The behavior will be attached automatically to operations with the attribute. To illustrate this, take the way that the `ContinueOnFailureBehavior` class and its associated behavior attribute are implemented:

    public class ContinueOnFailureAttribute : OperationBehaviorAttribute
    {
        public override OperationBehavior CreateBehavior()
        {
            return new ContinueOnFailureOperationBehavior();
        }
    }

    class ContinueOnFailureBehavior : OperationBehavior
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

### Behavior precedence levels

There are many different types of behaviors and to make sure that they are applied in an order that makes sense, they are grouped into a set of precedence levels with specific semantic meaning. Each level describes the type of functionality that the behavior provides for the operation and workflow. This level indicates how the behavior interacts with other behaviors which is important for keeping your workflows functioning the way you would expect. For example, consider a behavior that retries an operation a number of times in case of an error. If this behavior runs before the logging behavior, the log would not catch all the attempts but would instead see it as a single operation execution which might not even fail. In this case, a potential problem is hidden from the logging behavior because of the ordering of the operations.

The different levels are defined by the `BehaviorPredence` enum and are described below with their integer values. The base operation is decorated with the behaviors sorted by their precedence levels, with the lowest valued levels being executed closest to the base operation. If you do not find the existing levels detailed enough, you can define your own levels by casting a different integer to the enum, when assigning the precedence in your behaviors.

**Pre recovery (0)**

If something goes wrong with the actual execution, this type of behavior can be used for logging the error.

**State recovery (100)**

If something goes wrong with the actual execution, this type of behavior can be used for bringing persistent state back into a proper condition, for example by rolling back transactions or deleting created files.

**Work recovery (200)**

If something goes wrong with the actual execution, this type of behavior can be used for recovering from the bad state and completing the task.

**Work compensation (300)**

If something goes wrong with the actual execution that cannot be cleaned up, this type of behavior can be used to perform compensating work.

**Containment (400)**

The last behavior before types related to actual execution of work, it allows you to create a bulkhead, guarding the rest of the workflow against errors and problems caused by the operation.

**Staging (500)**

Before behaviors related to the execution of the operation are run, staging behaviors can be used for things such as preparing for execution, determining whether or how to execute, and other reflective tasks.

**Logging (600)**

The first behavior type to run, logging behaviors can be used to document the work that takes place.

##Testing

For verifying that your workflows executed as expected, the library provides an assertion method which can be used like this:

    [Fact]
    public void GG()
    {
        var workflow = new SendPartyInvitesOperation();

        workflow.ExecutesChildOperationsWithoutErrors(
            typeof(FindGuestListOperation),
            typeof(PrepareInviteTemplateOperation),
            typeof(CreateInviteNotificationsOperation),
            typeof(SendNotificationsOperation)
        );
    }

When the expectations are not met, the AssertionException provides a descriptive error message:

    Operations
    ==========
    FindGuestListOperation [match]
    PrepareInviteTemplateOperation [match, failed]
    none [error: expected CreateInviteNotificationsOperation]
    none [error: expected SendNotificationsOperation]

If you want to test that the proper workflow is executed in case of errors, use the similar but less strict method `ExecutesChildOperations` instead.
