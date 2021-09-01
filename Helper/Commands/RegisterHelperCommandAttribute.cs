using System;

namespace Helper
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RegisterHelperCommandAttribute : Attribute
    {
        public readonly bool requiresAdministrator = false;
        public readonly string description = "No description provided.";
        public RegisterHelperCommandAttribute(string description)
        {
            this.description = description;
            requiresAdministrator = false;
        }
        public RegisterHelperCommandAttribute(bool requiresAdministrator)
        {
            description = "No description provided.";
            this.requiresAdministrator = requiresAdministrator;
        }
        public RegisterHelperCommandAttribute(string description, bool requiresAdministrator)
        {
            this.description = description;
            this.requiresAdministrator = requiresAdministrator;
        }
    }
}
