using System;
using System.Reflection;
using System.Collections.Generic;
namespace SharpShell
{
    public sealed class Function
    {
        #region Public Variables
        public readonly RegisterFunctionAttribute RegisterFunctionAttribute = null;
        public readonly MethodInfo SourceMethod = null;
        #endregion
        #region Public Properties
        public string Description
        {
            get
            {
                return RegisterFunctionAttribute.Description;
            }
        }
        public string Name
        {
            get
            {
                if (RegisterFunctionAttribute.Name is null)
                {
                    return SourceMethod.Name;
                }
                return RegisterFunctionAttribute.Name;
            }
        }
        public string[] Aliases
        {
            get
            {
                if (RegisterFunctionAttribute._aliases is null || RegisterFunctionAttribute._aliases.Length == 0)
                {
                    return new string[0];
                }
                return (string[])RegisterFunctionAttribute._aliases.Clone();
            }
        }
        public bool asdf
        {
            get
            {
                SourceMethod.
            }
        }
        #endregion
        public Function(MethodInfo sourceMethod)
        {
            if (sourceMethod is null)
            {
                throw new NullReferenceException($"Could not create function from method because method was null.");
            }
            RegisterFunctionAttribute = sourceMethod.GetCustomAttribute<RegisterFunctionAttribute>();
            if (RegisterFunctionAttribute is null)
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
            if (sourceMethod.ContainsGenericParameters || sourceMethod.IsGenericMethod || sourceMethod.IsGenericMethodDefinition)
            {
                throw new ArgumentException($"Could not create function from method \"{sourceMethod.Name}\" because method contains generic types.");
            }
            this.SourceMethod = sourceMethod;
            Parameters = sourceMethod.GetParameters();
            foreach (ParameterInfo parameter in Parameters)
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
                if (parameter.IsRetval)
                {
                    throw new Exception($"Could not create function from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was marked as retval.");
                }
            }
            SourceType = sourceMethod.DeclaringType;
            SourceAssembly = sourceMethod.DeclaringType.Assembly;
        }
        public object Invoke(List<object> arguments)
        {
            if (arguments is null)
            {
                arguments = new List<object>();
            }
            if (arguments.Count < Parameters.Length)
            {
                throw new ArgumentException($"No argument was provided for property \"{Parameters[arguments.Count].Name}\".");
            }
            else if (arguments.Count > Parameters.Length)
            {
                throw new ArgumentException("Too many arguments were provided.");
            }
            return SourceMethod.Invoke(null, arguments.ToArray());
        }
        public override string ToString()
        {
            return $"Helper.Function(\"{Name}\", \"{Description}\")";
        }
    }
}
