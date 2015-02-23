using System;

namespace Overflow
{
    public interface IOutputOperation<out TOutput>
        where TOutput : class
    {
        void Output(Action<TOutput> onReceiveOutput);
    }
}
