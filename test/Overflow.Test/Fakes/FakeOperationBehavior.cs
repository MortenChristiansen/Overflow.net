using Overflow.Extensibility;

namespace Overflow.Test.Fakes
{
    class FakeOperationBehavior : OperationBehavior
    {
        public BehaviorPrecedence SetPrecedence { get; set; }
        public WorkflowConfiguration Configuration { get; private set; }

        public override BehaviorPrecedence Precedence
        {
            get { return SetPrecedence; }
        }

        public FakeOperationBehavior()
        {
            
        }

        public FakeOperationBehavior(WorkflowConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}
