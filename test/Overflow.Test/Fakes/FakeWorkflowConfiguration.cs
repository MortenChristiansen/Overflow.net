namespace Overflow.Test.Fakes
{
    class FakeWorkflowConfiguration : WorkflowConfiguration
    {
        public IOperation Operation { get; set; }

        public override IOperation CreateOperation() =>
            Operation;
    }
}
