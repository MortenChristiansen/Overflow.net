namespace Overflow
{
    public interface IInputOperation<in TInput>
        where TInput : class
    {
        void Input(TInput list);
    }
}
