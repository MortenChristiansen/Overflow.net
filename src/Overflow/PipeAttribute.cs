using System;

namespace Overflow
{
    /// <summary>
    /// When used on a property decorated with the Input attribute, it tells the
    /// operation that the input value should automatically be piped on to be available 
    /// to the child operations.
    /// 
    /// When used on a property decorated with the Output attribute, it tells the
    /// operation that the property should be assigned the child operation output
    /// value of the same type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PipeAttribute : Attribute
    {
    }
}
