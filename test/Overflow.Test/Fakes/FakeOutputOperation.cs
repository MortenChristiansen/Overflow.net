using System;

namespace Overflow.Test.Fakes
{
    public class FakeOutputOperation<TOutput> : Operation, IOutputOperation<TOutput> where TOutput : class
    {
        public Action<TOutput> OnReceiveOutput { get; private set; }
        [Output] public TOutput OutputValue { get; set; }

        protected override void OnExecute() =>
            OnReceiveOutput(OutputValue);

        public void Output(Action<TOutput> onReceiveOutput) =>
            OnReceiveOutput = onReceiveOutput;
    }
}
