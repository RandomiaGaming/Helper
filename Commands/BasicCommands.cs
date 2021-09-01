﻿using System;
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
        [RegisterHelperCommand("Prints a message to the console.", false, false)]
        public static void Echo(string message)
        {
            Console.WriteLine(message);
        }
        [RegisterHelperCommand("Provides information about a method.", false, false)]
        public static void Help([OptionalArgument] string methodName)
        {
            if (methodName is null || methodName == "")
            {
                Console.WriteLine("To run a command in helper simply type its name and press enter. Here is a list of all currently installed commands...");
                foreach (HelperCommand method in Program.methods)
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
        [RegisterHelperCommand("Asks prompts the user for administrator if helper is not already an administrator.", false, false)]
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
        [RegisterHelperCommand("Installs helper to program files if it is not already installed", true, false)]
        public static void Install()
        {
            Installer.Install();
        }
        [RegisterHelperCommand("Prints the current working directory to the console.", false, false)]
        public static void GetWorkingDirectory()
        {
            Console.WriteLine(Environment.CurrentDirectory);
        }
        [RegisterHelperCommand("Formats an image to the PNG format.", false, false)]
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
        [RegisterHelperCommand("Closes the current instance of helper.", false, false)]
        public static void Exit()
        {
            Process.GetCurrentProcess().Kill();
        }
        [RegisterHelperCommand("Prints the current helper version to the console.", false, false)]
        public static void Helper()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            Console.WriteLine($"Helper - {assembly.GetName().Version}");
        }
        [RegisterHelperCommand("Reads out a message using Microsofts text to speech.", false, false)]
        public static void TTS(string message)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.Rate = 6;
            synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Adult);
            synth.SpeakAsync(message);
        }
        [RegisterHelperCommand("Opens the exeption log file in the default text editor.", false, false)]
        public static void ExceptionLog()
        {
            Process.Start(Program.GetRoot() + "\\ErrorLog.txt");
        }
        [RegisterHelperCommand("Uses AES to encrypt a file.", false, false)]
        public static void AESEncrypt(string filePath, string key)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException();
            }
            File.WriteAllBytes(filePath, CryptographyHelper.AESEncrypt(File.ReadAllBytes(filePath), key));
        }
        [RegisterHelperCommand("Uses AES to decrypt a file.", false, false)]
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
