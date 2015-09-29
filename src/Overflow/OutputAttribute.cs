using System;

namespace Overflow
{
    /// <summary>
    /// Only valid for properties on IOperation types with public getters. Marks the property 
    /// as an output property. After executing the operation, an output value is retrieved from the 
    /// property if set.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OutputAttribute : Attribute
    {
    }
}
