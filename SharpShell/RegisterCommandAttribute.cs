using System;
namespace Helper.Core.Simple
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RegisterSimpleFunctionAttribute : Attribute
    {
        private string _description = null;
        public string description
        {
            get
            {
                return _description;
            }
        }
        public RegisterFunctionAttribute(string description)
        {
            _description = description;
        }
        public RegisterFunctionAttribute Clone()
        {
            return new RegisterFunctionAttribute(_description);
        }
        public override string ToString()
        {
            return $"Helper.RegisterFunctionAttribute(\"{description}\")";
        }
    }
}