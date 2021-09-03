using System;
using System.Reflection;
using System.Collections.Generic;
namespace Helper
{
    public sealed class Command
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
        private RegisterCommandAttribute _registerCommandAttribute = null;
        public RegisterCommandAttribute registerCommandAttribute
        {
            get
            {
                return _registerCommandAttribute.Clone();
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
                return _registerCommandAttribute.description;
            }
        }
        public bool requiresAdministrator
        {
            get
            {
                return _registerCommandAttribute.requiresAdministrator;
            }
        }
        public bool requiresInstall
        {
            get
            {
                return _registerCommandAttribute.requiresInstall;
            }
        }
        public Command(MethodInfo sourceMethod)
        {
            if (sourceMethod is null)
            {
                throw new NullReferenceException($"Could not create command from method because method was null.");
            }
            _registerCommandAttribute = sourceMethod.GetCustomAttribute<RegisterCommandAttribute>();
            if(_registerCommandAttribute is null)
            {
                throw new NullReferenceException($"Could not create command from method \"{sourceMethod.Name}\" beacuse it does not have the RegisterCommand attribute.");
            }
            if (!sourceMethod.IsStatic)
            {
                throw new ArgumentException($"Could not create command from method \"{sourceMethod.Name}\" because method was not static.");
            }
            if (!sourceMethod.IsPublic)
            {
                throw new ArgumentException($"Could not create command from method \"{sourceMethod.Name}\" because method was not public.");
            }
            if (sourceMethod.ReturnType != null)
            {
                throw new ArgumentException($"Could not create command from method \"{sourceMethod.Name}\" because method was not void.");
            }
            _parameters = sourceMethod.GetParameters();
            foreach (ParameterInfo parameter in _parameters)
            {
                if (parameter.IsOut)
                {
                    throw new Exception($"Could not create command from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was an out parameter.");
                }
                if (parameter.HasDefaultValue)
                {
                    throw new Exception($"Could not create command from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" has a default value.");
                }
                if (parameter.IsOptional)
                {
                    throw new Exception($"Could not create command from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was marked as optional.");
                }
                if (parameter.IsLcid)
                {
                    throw new Exception($"Could not create command from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was marked as a Lcid.");
                }
                if (parameter.ParameterType == typeof(List<string>) && parameter.Position != _parameters.Length - 1)
                {
                    throw new Exception($"Could not create command from method \"{sourceMethod.Name}\" because it parameter \"{parameter.Name}\" was of type \"{parameter.ParameterType}\" which is only allowed at the end of the parameter list.");
                }
                if (parameter.ParameterType != typeof(string) && parameter.ParameterType != typeof(string[]))
                {
                    throw new Exception($"Could not create command from method \"{sourceMethod.Name}\" because parameter \"{parameter.Name}\" was of type \"{parameter.ParameterType}\".");
                }
            }
            _sourceAssembly = sourceMethod.DeclaringType.Assembly;
            if (_sourceAssembly is null)
            {
                throw new NullReferenceException($"Could not create command from method \"{sourceMethod.Name}\" beacuse its assembly was null.");
            }
        }
        public void Invoke(string[] arguments)
        {
            if (arguments is null)
            {
                arguments = new string[0];
            }
            List<string> argsList = new List<string>(arguments);
            List<object> formattedArgs = new List<object>();
            for (int i = 0; i < _parameters.Length; i++)
            {
                if (i >= argsList.Count)
                {
                    throw new ArgumentException($"No argument was provided for property \"{_parameters[i].Name}\".");
                }
                if (_parameters[i].ParameterType == typeof(string))
                {
                    formattedArgs.Add(argsList[i]);
                }
                else
                {
                    formattedArgs.AddRange(argsList.GetRange(i, argsList.Count - i));
                }
            }
            _sourceMethod.Invoke(null, formattedArgs.ToArray());
        }
        public override string ToString()
        {
            return $"Helper.Command(\"{name}\", \"{description}\", {requiresAdministrator}, {requiresInstall})";
        }
    }
}
