using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Speech.Synthesis;
namespace Helper.Commands
{
    public static class BasicCommands
    {
        [RegisterCommand("Prints a message to the console.", false, false)]
        public static void Echo(string message)
        {
            Console.WriteLine(message);
        }
        [RegisterCommand("Provides information about a method.", false, false)]
        public static void Help([OptionalArgument]string methodName)
        {
            if (methodName is null || methodName == "")
            {
                Console.WriteLine("To run a command in helper simply type its name and press enter. Here is a list of all currently installed commands...");
                foreach (Command method in global::Helper.Program.methods)
                {
                    if (method.name.ToUpper() == methodName.ToUpper())
                    {
                        Console.WriteLine($"\"{method.name}\" - \"{method.description}\"");
                        return;
                    }
                }
            }
            else
            {
                Console.WriteLine("Finlay is lazy");
            }
        }
        [RegisterCommand("Asks prompts the user for administrator if helper is not already an administrator.", false, false)]
        public static void Elevate()
        {
            if (!global::Helper.Program.IsAdministrator())
            {
                Process process = new Process();
                process.StartInfo.FileName = Assembly.GetCallingAssembly().Location;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                Process.GetCurrentProcess().Kill();
            }
        }
        [RegisterCommand("Prints the current working directory to the console.", false, false)]
        public static void WorkingDir()
        {
            Console.WriteLine(Environment.CurrentDirectory);
        }
        [RegisterCommand("Formats an image to the PNG format.", false, false)]
        public static void ConvertToPNG(string filePath)
        {
            if (filePath is null)
            {
                throw new NullReferenceException();
            }
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"The given file path was not valid.");
            }
            Image img = Image.FromStream(new MemoryStream(File.ReadAllBytes(filePath)));
            img.Save(Path.GetFileNameWithoutExtension(filePath) + ".png", ImageFormat.Png);
            File.Delete(filePath);
        }
        [RegisterCommand("Closes the current instance of helper.", false, false)]
        public static void Exit()
        {
            Process.GetCurrentProcess().Kill();
        }
        [RegisterCommand("Prints the current helper version to the console.", false, false)]
        public static void Helper()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            Console.WriteLine($"Helper - {assembly.GetName().Version}");
        }
        [RegisterCommand("Reads out a message using Microsofts text to speech.", false, false)]
        public static void TTS(string message)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.Rate = 6;
            synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Adult);
            synth.SpeakAsync(message);
        }
        [RegisterCommand("Opens the exeption log file in the default text editor.", false, false)]
        public static void ExceptionLog()
        {
            Process.Start(global::Helper.Program.GetRoot() + "\\ErrorLog.txt");
        }
        [RegisterCommand("Uses AES to encrypt a file.", false, false)]
        public static void AESEncrypt(string filePath, string key)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException();
            }
            File.WriteAllBytes(filePath, CryptographyHelper.AESEncrypt(File.ReadAllBytes(filePath), key));
        }
        [RegisterCommand("Uses AES to decrypt a file.", false, false)]
        public static void AESDecrypt(string filePath, string key)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException();
            }
            File.WriteAllBytes(filePath, CryptographyHelper.AESDecrypt(File.ReadAllBytes(filePath), key));
        }
    }
}
