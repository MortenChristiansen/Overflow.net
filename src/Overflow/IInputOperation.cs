namespace Overflow
{
    public interface IInputOperation<in TInput>
    {
        void Input(TInput list);
    }
}
