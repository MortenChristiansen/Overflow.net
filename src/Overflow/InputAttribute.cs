using System;

namespace Overflow
{
    /// <summary>
    /// Only valid for properties on IOperation types with public setters. Marks the property 
    /// as an input property. Before executing the operation, an input value is set on the 
    /// property if available in the operation context.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InputAttribute : Attribute
    {
    }
}
