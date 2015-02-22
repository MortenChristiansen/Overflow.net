using System;

namespace Overflow
{
    public interface IOutputOperation<out TOutput>
    {
        void Output(Action<TOutput> onReceiveOutput);
    }
}
