//Approved 06/23/2022
namespace SharpShell
{
    public abstract class OutputHandler
    {
        #region Public Abstracts
        public abstract void Handle(object output);
        #endregion
    }
}