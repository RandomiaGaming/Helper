//Approved 06/23/2022
namespace SharpShell
{
    public sealed class ConsoleOutputHandler : OutputHandler
    {
        #region Public Overrides
        public override void Handle(object output)
        {
            if (output is null)
            {
                System.Console.WriteLine("null");
            }
            else
            {
                System.Console.WriteLine(output.ToString());
            }
        }
        #endregion
    }
}