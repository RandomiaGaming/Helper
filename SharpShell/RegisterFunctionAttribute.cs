//Approved 06/23/2022
namespace SharpShell
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public sealed class RegisterFunctionAttribute : System.Attribute
    {
        #region Public Variables
        public readonly string Description = null;
        public readonly string Name = null;
        #endregion
        #region Public Properties
        public string[] Ailiases
        {
            get
            {
                if (_aliases is null || _aliases.Length == 0)
                {
                    return new string[0];
                }
                return (string[])_aliases.Clone();
            }
        }
        #endregion
        #region Internal Variables
        internal string[] _aliases = null;
        #endregion
        #region Public Constructors
        public RegisterFunctionAttribute(string description)
        {
            if (description is null)
            {
                throw new System.Exception("description cannot be null.");
            }
            if (description == string.Empty)
            {
                throw new System.Exception("description cannot be empty.");
            }
            Description = description;
            Name = null;
            _aliases = null;
        }
        public RegisterFunctionAttribute(string description, string name)
        {
            if (description is null)
            {
                throw new System.Exception("description cannot be null.");
            }
            if (description == string.Empty)
            {
                throw new System.Exception("description cannot be empty.");
            }
            Description = description;
            if (name == string.Empty)
            {
                throw new System.Exception("name cannot be empty.");
            }
            Name = name;
            _aliases = null;
        }
        public RegisterFunctionAttribute(string description, string[] aliases)
        {
            if (description is null)
            {
                throw new System.Exception("description cannot be null.");
            }
            if (description == string.Empty)
            {
                throw new System.Exception("description cannot be empty.");
            }
            Description = description;
            Name = null;
            if (aliases is null)
            {
                _aliases = null;
            }
            else if (aliases.Length == 0)
            {
                _aliases = null;
            }
            else
            {
                foreach (string alias in aliases)
                {
                    if (alias is null)
                    {
                        throw new System.Exception("aliases cannot contain null.");
                    }
                    else if (alias == string.Empty)
                    {
                        throw new System.Exception("aliases cannot contain empty strings.");
                    }
                }

                _aliases = aliases;
            }
        }
        public RegisterFunctionAttribute(string description, string name, string[] aliases)
        {
            if (description is null)
            {
                throw new System.Exception("description cannot be null.");
            }
            if (description == string.Empty)
            {
                throw new System.Exception("description cannot be empty.");
            }
            Description = description;
            if (name == string.Empty)
            {
                throw new System.Exception("name cannot be empty.");
            }
            Name = name;
            if (aliases is null)
            {
                _aliases = null;
            }
            else if (aliases.Length == 0)
            {
                _aliases = null;
            }
            else
            {
                foreach (string alias in aliases)
                {
                    if (alias is null)
                    {
                        throw new System.Exception("aliases cannot contain null.");
                    }
                    else if (alias == string.Empty)
                    {
                        throw new System.Exception("aliases cannot contain empty strings.");
                    }
                }

                _aliases = aliases;
            }
        }
        #endregion
        #region Public Methods
        public RegisterFunctionAttribute Clone()
        {
            return new RegisterFunctionAttribute(Description, Name, _aliases);
        }
        #endregion
        #region Private Methods
        private string AliasesToString()
        {
            if (_aliases is null || _aliases.Length == 0)
            {
                return "System.String[0]";
            }

            string output = $"System.String[{_aliases.Length}] {{ ";

            for (int i = 0; i < _aliases.Length; i++)
            {
                if (i == _aliases.Length - 1)
                {
                    output += $"\"{_aliases[i]}\"";
                }
                else
                {
                    output += $"\"{_aliases[i]}\", ";
                }
            }

            output += " }";

            return output;
        }
        private string NameToString()
        {
            if (Name is null)
            {
                return "null";
            }

            return $"\"{Name}\"";
        }
        #endregion
        #region Public Overrides
        public override string ToString()
        {
            return $"Helper.RegisterFunctionAttribute(\"{Description}\", {NameToString()}, {AliasesToString()})";
        }
        #endregion
    }
}