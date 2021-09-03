using System;
namespace Helper
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class RegisterCommandAttribute : Attribute
    {
        private bool _requiresAdministrator = false;
        public bool requiresAdministrator
        {
            get
            {
                return _requiresAdministrator;
            }
        }
        private bool _requiresInstall = false;
        public bool requiresInstall
        {
            get
            {
                return _requiresInstall;
            }
        }
        private string _description = null;
        public string description
        {
            get
            {
                return CloneHelper.CloneString(_description);
            }
        }
        public RegisterCommandAttribute(string description, bool requiresAdministrator = false, bool requiresInstall = false)
        {
            _description = CloneHelper.CloneString(description);
            _requiresAdministrator = requiresAdministrator;
            _requiresInstall = requiresInstall;
        }
        public RegisterCommandAttribute Clone()
        {
            return new RegisterCommandAttribute(CloneHelper.CloneString(_description), _requiresAdministrator, _requiresInstall);
        }
        public override string ToString()
        {
            return $"Helper.RegisterCommandAttribute(\"{description}\", {requiresAdministrator}, {requiresInstall})";
        }
    }
}