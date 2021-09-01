using System;
namespace Helper
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class OptionalArgumentAttribute : Attribute
    {
        public readonly bool optional = false;
        public OptionalArgumentAttribute(bool optional = false)
        {
            this.optional = optional;
        }
    }
}