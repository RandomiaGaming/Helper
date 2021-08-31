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
        [RegisterCommand("Prints a message to the console.", false)]
        public static void Echo(string message)
        {
            if (message != null && message != "")
            {
                Console.WriteLine(message);
            }
        }
        [RegisterCommand("Provides information about a method.", false)]
        public static void Help(string methodName)
        {
            foreach (HelperCommand method in Program.methods)
            {
                if (method.name.ToUpper() == methodName.ToUpper())
                {
                    Console.WriteLine($"Method: {method.name}(). Description: {method.description}");
                    return;
                }
            }
        }
        [RegisterCommand("Asks for administrator if the current process is not already an administrator.", false)]
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
        [RegisterCommand("Installs helper to program files if it is not already installed", true)]
        public static void Install()
        {
            Installer.Install();
        }
        [RegisterCommand("Prints how long it will be until charlie gets home to the console.", false)]
        public static void Charlie()
        {
            TimeSpan ttccb = new TimeSpan(new DateTime(2020, 12, 12, 10, 0, 0).Ticks - DateTime.Now.Ticks);
            string message = $"I will finally get to see Charlie in {ttccb.Days} days, {ttccb.Hours} hours, {ttccb.Minutes} minutes, {ttccb.Seconds} seconds, and {ttccb.Milliseconds} milliseconds, which is {LongToStringWithCommas(ttccb.Ticks)} ticks.";
            Console.WriteLine($"I will finally get to see Charlie in {ttccb.Days} days, {ttccb.Hours} hours, {ttccb.Minutes} minutes, {ttccb.Seconds} seconds, and {ttccb.Milliseconds} milliseconds, which is {LongToStringWithCommas(ttccb.Ticks)} ticks.");
        }
        private static string LongToStringWithCommas(long value)
        {
            string rawValueString = ReverseString(value.ToString());
            string valueString = "";
            int index = 0;
            for (int i = 0; i < rawValueString.Length; i++)
            {
                valueString += rawValueString[i];
                index++;
                if (index == 3)
                {
                    index = 0;
                    valueString += ",";
                }
            }
            return ReverseString(valueString);
        }
        public static string ReverseString(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
        [RegisterCommand("Prints the environment directory to the console.", false)]
        public static void LogDirectory()
        {
            Console.WriteLine(Environment.CurrentDirectory);
        }
        [RegisterCommand("Returns the environment directory to the console.", false)]
        public static string GetDirectory()
        {
            return Environment.CurrentDirectory;
        }
        [RegisterCommand("Formats all the image files in a specifyied folder to the PNG format.", false)]
        public static void FormatPNG(string folderPath)
        {
            if (folderPath is null)
            {
                throw new NullReferenceException();
            }
            if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException();
            }

            string[] images = Directory.GetFiles(folderPath);

            foreach (string image in images)
            {
                if (Path.GetExtension(image).ToLower() == ".jpg")
                {
                    Image img = Image.FromStream(new MemoryStream(File.ReadAllBytes(image)));
                    img.Save(Path.GetDirectoryName(image) + "\\" + Path.GetFileNameWithoutExtension(image) + ".png", ImageFormat.Png);
                    File.Delete(image);
                }
            }
        }
        [RegisterCommand("Closes the current instance of helper.", false)]
        public static void Exit()
        {
            Process.GetCurrentProcess().Kill();
        }
        [RegisterCommand("Prints the current helper version to the console.", false)]
        public static void Helper()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            Console.WriteLine($"Helper - {assembly.GetName().Version}");
        }
        [RegisterCommand("Reads out a message using Microsofts text to speech.", false)]
        public static void TTS(string message)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.Rate = 6;
            synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Adult);
            synth.SpeakAsync(message);
        }
        [RegisterCommand("Reads out phil from small wenier club's voice mail.", false)]
        public static void TTSPhil()
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.Rate = 2;
            synth.SelectVoiceByHints(VoiceGender.Neutral, VoiceAge.Adult);
            synth.SpeakAsync("Hey this is Phil from the Small Wenier Club, sorry to get back to you so late, I just finished reviewing your application and information you sent in. But I am sorry to say that I don't think I can allow you to join our group. From what I'm looking at, your weiner is massive. I mean the sheer girth and juciness alone is ridiculous. It looks as if somebody glued a forearm to the bottom of your torso. You could probably stand on it like a tripod, and thats not even mentioning how fat your nuts are. But it does appear that you are going to have to take that ginormous schmeat somewhere else. But thank you for trying, and best of luck to you.");
        }
        [RegisterCommand("Opens the standard exeption log file in the default text editor.", false)]
        public static void ExceptionLog()
        {
            Process.Start(Program.GetRoot() + "\\ErrorLog.txt");
        }
        [RegisterCommand("Lists all the currently registered methods", false)]
        public static void List()
        {
            foreach (HelperCommand method in Program.methods)
            {
                Console.WriteLine($"Method: {method.name}(). Description: {method.description}");
            }
        }
        [RegisterCommand("Uses AES to encrypt a file.", false)]
        public static void AESEncrypt(string filePath, string key)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException();
            }
            File.WriteAllBytes(filePath, CryptographyHelper.AESEncrypt(File.ReadAllBytes(filePath), key));
        }
        [RegisterCommand("Uses AES to decrypt a file.", false)]
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
