using System;
namespace Helper
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RegisterHelperCommandAttribute : Attribute
    {
        public readonly bool requiresAdministrator = false;
        public readonly bool requiresInstall = false;
        public readonly string description = null;
        public RegisterHelperCommandAttribute(string description, bool requiresAdministrator = false, bool requiresInstall = false)
        {
            this.description = description;
            this.requiresAdministrator = requiresAdministrator;
            this.requiresInstall = requiresInstall;
        }
    }
}