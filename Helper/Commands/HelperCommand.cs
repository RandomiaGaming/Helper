using System;
using System.Collections.Generic;
using System.Reflection;

namespace Helper
{
    public sealed class HelperCommand
    {
        public readonly MethodInfo method = null;

        public readonly string name = null;
        public readonly string description = null;
        public readonly bool requiresAdministrator = false;
        public HelperCommand(MethodInfo method, string name, string description, bool requiresAdministrator)
        {
            if (method is null)
            {
                throw new Exception("Cannot create helper method because the c# method was null.");
            }
            this.method = method;
            if (name is null || name == "")
            {
                throw new Exception("Cannot create helper method because the given name was invalid.");
            }
            this.name = name;
            if (description is null || description == "")
            {
                this.description = "No description availible.";
            }
            else
            {
                this.description = description;
            }
            this.requiresAdministrator = requiresAdministrator;
        }
        public object Invoke()
        {
            try
            {
                return method.Invoke(null, new object[0]);
            }
            catch (Exception ex)
            {
                if (ex is null)
                {
                    throw new NullReferenceException();
                }
                while (!(ex.InnerException is null))
                {
                    ex = ex.InnerException;
                }
                Program.LogException($"Method \"{name}\" encountered the error \"{ex.Message}\" and therefore was aborted.");
                throw new CommandException(sourceCommand: this, innerException: ex);
            }
        }
        public override string ToString()
        {
            return name;
        }
        public override int GetHashCode()
        {
            if (name is null)
            {
                return "".GetHashCode();
            }
            return name.GetHashCode();
        }
    }
}
