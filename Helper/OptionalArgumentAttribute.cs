using System;
namespace Helper
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class OptionalArgumentAttribute : Attribute
    {
        public OptionalArgumentAttribute()
        {

        }
        public OptionalArgumentAttribute Clone()
        {
            return new OptionalArgumentAttribute();
        }
        public override string ToString()
        {
            return $"Helper.OptionalArgumentAttribute()";
        }
    }
}