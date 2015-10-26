#Overflow.net

![Project logo](https://raw.githubusercontent.com/MortenChristiansen/Overflow.net/master/files/logo-128.png)

####[![Build status](https://ci.appveyor.com/api/projects/status/m8qkwl9p91jwncjo/branch/master?svg=true)](https://ci.appveyor.com/project/MortenChristiansen/overflow-net/branch/master) // windows / .net

####[![Build Status](https://travis-ci.org/MortenChristiansen/Overflow.net.svg?branch=master)](https://travis-ci.org/MortenChristiansen/Overflow.net) // linux / mono

####[![Coverage Status](https://coveralls.io/repos/MortenChristiansen/Overflow.net/badge.svg?branch=master&service=github)](https://coveralls.io/github/MortenChristiansen/Overflow.net?branch=master)

Overflow is an open source C# library for modeling workflows using simple objects. It allows you to define workflows using a simple and clean syntax, while you apply operational behaviors such as error handling strategies in a declarative fashion using attributes.

The following code sample illustrates how a workflow for sending out party invites could be created. The workflow is a hierarchical set of operations, starting with the root operation `SendPartyInvitesOperation`. As it executes, it instantiates child operations and executes these in order. Each child operation can have further child operations if needed.

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
	
The attributes are generally used for handling unforeseen events during execution or for gaining insight into runtime issues for later analysis.
	
For further samples, see the `examples` folder in the source code.

You can view as well as comment and vote on upcoming features [here](https://trello.com/b/8eWYE4mV/features).

## Behaviors

These are the behaviors that come out of the box with Overflow, which you can use to annotate your operations, though you can easily build your own. See the section on extensibility for more details on how to do this.

### `AtomicAttribute`

The operation is wrapped in a transaction which is committed if the operation does not fail.

### `CompensatingOperationAttribute`

When errors occur during the execution of the operation, an instance of the specified compensating operation type is created and executed. This operation gets behaviors applied as normal and can implement the `IInputOperation` interface. The `IOutputOperation` interface will be ignored.

Note that the exception thrown in the original exception is rethrown once the compensating operation is done executing. Consider using the ContinueOnFailure attribute if this is not the desired behavior.

If the compensating operation throws an exception, the original exception is not rethrown, and the new exception bubbles up instead. To avoid this, make the compensating operation use the ContinueOnFailure attribute.

### `ContinueOnFailureAttribute`

Failures executing the operation are ignored.

### `RetryAttribute`

Failures are retried a number of times. It can be limited to only retry specific exceptions. Note that only child operations with the `Idempotent` attribute can be retried.

### `IConditionalOperation` (interface)

The operation can determine whether it should be executed or not.

## Installation

To install Overflow.net, run the following command in the Package Manager Console:

`Install-Package Overflow.net`

## Data flow

A central part of most workflows will be the production and consumption of values. One operation might produce a collection of values by reading from a database and turning the results into objects. Each of these values might in turn be processed by its own operation. To facilitate this, properties can be adorned with the `Input` and `Output` attributes. The input property must have a public setter and the output property must have a public getter. Before each operation is executed, the runtime looks for and assigns previously outputted values to each matching input property. After each operation has executed, the value of each output property is read and stored for future input requests.

Each operation maintains an operation context where operation values are stored. Once a value has been added to the context it becomes available to each child operation. If an operation takes a value as input and wants to  make it available to its child operations, it nees to call the `PipeInputToChildOperations<TInput>(TInput input)` method. This adds the value to the context of the operation.

If an operation needs to access values produced by any of its already executed child operations, it can call the `GetChildOutputValue<TOutput>()` method. As a convenience, you can retrieve a collection of output values using the `GetChildOutputValues<TOutput>()` method. This method has a special behavior in that the collection can be outputted as any concrete implementation of `IEnumerable<TOutput>`. Normally, you can only get the exact types specified in the output properties, not base types.

For operations that need to send input directly to or return outout from child operations without interacting with it directly, you can streamline the process by adding the `Pipe` attribute to the input or output property. This will make sure that the `PipeInputToChildOperations` (input piping) or `GetChildOutputValue` (output piping) method is used automatically to pipe the values to the proper destination.

If you want to input one or more values directly to a child operation instead of adding it to the context, the child operation can be created with an overload of the `Create` method taking up to three input values. This can be useful when creating a child operation per value in an outputted collection, since the individual items are not available in the context.

To get an idea of how all this fits together, take a loot at the operations in the [Compact example project](https://github.com/MortenChristiansen/Overflow.net/blob/master/examples/Compact/Operations/ImportUsersWorkflow.cs).

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
    public void Test()
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
