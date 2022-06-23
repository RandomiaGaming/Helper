using System;
using System.Reflection;
using System.Collections.Generic;
namespace Helper
{
    public sealed class Function
    {
        private MethodInfo _sourceMethod = null;
        public MethodInfo sourceMethod
        {
            get
            {
                return _sourceMethod;
            }
        }
        private Assembly _sourceAssembly = null;
        public Assembly sourceAssembly
        {
            get
            {
                return _sourceAssembly;
            }
        }
        private ParameterInfo[] _parameters = null;
        public ParameterInfo[] parameters
        {
            get
            {
                return (ParameterInfo[])_parameters.Clone();
            }
        }
        private RegisterFunctionAttribute _registerFunctionAttribute = null;
        public RegisterFunctionAttribute registerFunctionAttribute
        {
            get
            {
                return _registerFunctionAttribute.Clone();
            }
        }
        public string name
        {
            get
            {
                return _sourceMethod.Name;
            }
        }
        public string description
        {
            get
            {
                return _registerFunctionAttribute.description;
            }
        }
        public Function(MethodInfo sourceMethod)
        {
            if (sourceMethod is null)
            {
                throw new NullReferenceException($"Could not create function from method because method was null.");
            }
            _registerFunctionAttribute = sourceMethod.GetCustomAttribute<RegisterFunctionAttribute>();
            if (_registerFunctionAttribute is null)
            {
                throw new NullReferenceException($"Could not create function from method \"{sourceMethod.Name}\" beacuse it does not have the RegisterFunction attribute.");
            }
            if (!sourceMethod.IsStatic)
            {
                throw new ArgumentException($"Could not create function from method \"{sourceMethod.Name}\" because method was not static.");
            }
            if (!sourceMethod.IsPublic)
            {
                throw new ArgumentException($"Could not create function from method \"{sourceMethod.Name}\" because method was not public.");
            }
            if (sourceMethod.ReturnType != typeof(void))
            {
                throw new ArgumentException($"Could not create function from method \"{sourceMethod.Name}\" because method was not void.");
            }
            _sourceMethod = sourceMethod;
            _parameters = sourceMethod.GetParameters();
            foreach (ParameterInfo parameter in _parameters)
            {
                if (parameter.IsOut)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was an out parameter.");
                }
                if (parameter.HasDefaultValue)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" has a default value.");
                }
                if (parameter.IsOptional)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was marked as optional.");
                }
                if (parameter.IsLcid)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was marked as a Lcid.");
                }
                if (parameter.ParameterType != typeof(string))
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was not of type\"{typeof(string)}\".");
                }
            }
            _sourceAssembly = sourceMethod.DeclaringType.Assembly;
            if (_sourceAssembly is null)
            {
                throw new NullReferenceException($"Could not create function from method \"{sourceMethod.Name}\" beacuse its assembly was null.");
            }
        }
        public void Invoke(List<string> arguments)
        {
            if (arguments is null)
            {
                arguments = new List<string>();
            }
            if (arguments.Count < _parameters.Length)
            {
                throw new ArgumentException($"No argument was provided for property \"{_parameters[arguments.Count].Name}\".");
            }
            else if (arguments.Count > parameters.Length)
            {
                throw new ArgumentException("Too many arguments were provided.");
            }
            _sourceMethod.Invoke(null, arguments.ToArray());
        }
        public override string ToString()
        {
            return $"Helper.Function(\"{name}\", \"{description}\")";
        }
    }
}
