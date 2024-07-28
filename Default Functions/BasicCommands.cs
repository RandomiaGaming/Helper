using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Speech.Synthesis;
namespace Helper.Functions
{
    public static class BasicFunctions
    {
        [RegisterFunction("Echos a message to the console.")]
        public static void Echo(string message)
        {
            Console.WriteLine(message);
        }
        [RegisterFunction("Provides a list of all loaded functions and their descriptions.")]
        public static void List()
        {
            foreach (Function method in Program.GetLoadedFunctions())
            {
                Console.WriteLine($"\"{method.name}\" - \"{method.description}\"");
            }
        }
        [RegisterFunction("Crashes helper and opens the error log.")]
        public static void Crash()
        {
            Program.LogFatalException(new Exception());
        }
        [RegisterFunction("Asks prompts the user for administrator if Helper is not already an administrator.")]
        public static void Elevate()
        {
            if (!Program.IsAdministrator())
            {
                Process process = new Process();
                process.StartInfo.FileName = Assembly.GetCallingAssembly().Location;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.Verb = "runas";
                process.Start();
                Process.GetCurrentProcess().Kill();
            }
        }
        [RegisterFunction("Prints the current working directory to the console.")]
        public static void WorkingDir()
        {
            Console.WriteLine(Environment.CurrentDirectory);
        }
        [RegisterFunction("Formats an image to the PNG format.")]
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
        [RegisterFunction("Closes the current instance of helper.")]
        public static void Exit()
        {
            Process.GetCurrentProcess().Kill();
        }
        [RegisterFunction("Prints the current helper version to the console.")]
        public static void Helper()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            Console.WriteLine($"Helper - {assembly.GetName().Version}");
        }
        [RegisterFunction("Reads out a message using Microsofts text to speech.")]
        public static void TTS(string message)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.Rate = 6;
            synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Adult);
            synth.SpeakAsync(message);
        }
         [RegisterFunction("Encrypts a file using AES 256 bit encryption.")]
         public static void EncryptFile(string filePath, string key)
         {
             if (!File.Exists(filePath))
             {
                 throw new ArgumentException();
             }
            EncryptionHelper.EncryptFile(filePath, EncryptionHelper.HashString(key));
         }
         [RegisterFunction("Decrypts a file using AES 256 bit encryption.")]
         public static void DecryptFile(string filePath, string key)
         {
             if (!File.Exists(filePath))
             {
                 throw new ArgumentException();
             }
            EncryptionHelper.DecryptFile(filePath, EncryptionHelper.HashString(key)); 
        }
    }
}
