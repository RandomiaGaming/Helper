//Approved 06/23/2022
namespace SharpShell
{
    public sealed class ConsoleExceptionHandler : ExceptionHandler
    {
        #region Public Overrides
        public override void Handle(System.Exception exception)
        {
            string exceptionType;
            try
            {
                exceptionType = exception.GetType().Name;
            }
            catch
            {
                exceptionType = "Exception";
            }
            string exceptionDateTime;
            try
            {
                exceptionDateTime = System.DateTime.Now.ToString("MM/dd/yyyy HH:m:s");
            }
            catch
            {
                exceptionDateTime = "00/00/0000 00:00:00";
            }
            string exceptionMessage;
            try
            {
                if (exception.Message is null || exception.Message == string.Empty)
                {
                    exceptionMessage = "An unknown exception was thrown.";
                }
                else
                {
                    exceptionMessage = exception.Message;
                }
            }
            catch
            {
                exceptionMessage = "An unknown exception was thrown.";
            }
            try
            {
                System.Console.WriteLine($"An exception of type \"{exceptionType}\" was thrown at \"{exceptionDateTime}\" with the message \"{exceptionMessage}\".");
            }
            catch
            {
                throw new System.Exception("Exception could not be handled.");
            }
        }
        #endregion
    }
}