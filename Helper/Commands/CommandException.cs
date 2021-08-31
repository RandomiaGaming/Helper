using System;
namespace Helper
{
    public sealed class CommandException : Exception
    {
        public readonly HelperCommand sourceCommand = null;
        public CommandException(HelperCommand sourceCommand = null, Exception innerException = null) : base(MethodToErrorMessage(sourceCommand, innerException), innerException)
        {
            this.sourceCommand = sourceCommand;
            HelpLink = "";
            HResult = 0;
            if (sourceCommand is null)
            {
                Source = "An unknown method.";
            }
            else
            {
                Source = sourceCommand.name;
            }
        }
        private static string MethodToErrorMessage(HelperCommand method, Exception innerException)
        {
            if (method is null && innerException is null)
            {
                return "An unknown method encountered an unknown exception.";
            }
            else if (method is null)
            {
                return $"An unknown method encountered the exception \"{innerException.Message}\".";
            }
            else if (innerException is null)
            {
                return $"The method \"{method.name}\" encountered an unknown exception.";
            }
            return $"The method \"{method.name}\" encountered the exception \"{innerException.Message}\".";
        }
    }
}