//Approved 06/23/2022
namespace SharpShell
{
    public abstract class ExceptionHandler
    {
        #region Public Abstracts
        public abstract void Handle(System.Exception exception);
        #endregion
    }
}