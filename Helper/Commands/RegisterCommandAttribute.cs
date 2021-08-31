using System;

namespace Helper
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RegisterCommandAttribute : Attribute
    {
        public readonly bool requiresAdministrator = false;
        public readonly string description = "No description provided.";
        public RegisterCommandAttribute(string description)
        {
            this.description = description;
            requiresAdministrator = false;
        }
        public RegisterCommandAttribute(bool requiresAdministrator)
        {
            description = "No description provided.";
            this.requiresAdministrator = requiresAdministrator;
        }
        public RegisterCommandAttribute(string description, bool requiresAdministrator)
        {
            this.description = description;
            this.requiresAdministrator = requiresAdministrator;
        }
    }
}
