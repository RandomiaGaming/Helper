public static class Program
{
    [System.STAThread]
    public static void Main()
    {

        SharpShell.CommandProcessor commandProcessor = new SharpShell.CommandProcessor();

        while (true)
        {
            string command = System.Console.ReadLine();

            commandProcessor.RunCommand(command);
        }
    }
}