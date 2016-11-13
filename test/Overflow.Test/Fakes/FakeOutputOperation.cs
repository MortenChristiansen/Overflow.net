using System;

namespace Overflow.Test.Fakes
{
    public class FakeOutputOperation<TOutput> : Operation where TOutput : class
    {
        public Action<TOutput> OnReceiveOutput { get; set; }
        [Output] public TOutput OutputValue { get; set; }

        protected override void OnExecute() =>
            OnReceiveOutput?.Invoke(OutputValue);
    }
}
