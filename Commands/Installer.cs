using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Helper
{
    public static class Installer
    {
        public static bool IsInstalled()
        {
            if (Directory.Exists(@"C:\Program Files\Helper") && File.Exists(@"C:\Program Files\Helper\Helper.exe"))
            {
                return true;
            }
            return false;
        }
        public static bool ThisIsInstalledVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly.Location == @"C:\Program Files\Helper\Helper.exe")
            {
                return true;
            }
            return false;
        }
        [RegisterCommand("Installs helper.", true, false)]
        public static void Install()
        {
            bool installedSuccessfully = AddToProgramFiles();
            if (installedSuccessfully)
            {
                RegisterToPath();
                AddShortcut();
            }
            else
            {
                throw new Exception("Installation aborted due to an error.");
            }
        }
        [RegisterCommand("Uninstalls helper", true, true)]
        public static void Uninstall()
        {
            RemoveShortcut();
            UnregisterFromPath();
            RemoveFromProgramFiles();
        }
        private static bool AddToProgramFiles()
        {
            if (!Directory.Exists(@"C:\Program Files\Helper"))
            {
                Directory.CreateDirectory(@"C:\Program Files\Helper");
                File.Copy(Assembly.GetCallingAssembly().Location, @"C:\Program Files\Helper\Helper.exe");

                Assembly assembly = Assembly.GetCallingAssembly();
                Stream iconStream = assembly.GetManifestResourceStream("Helper.Icon.ico");
                byte[] iconBytes = new byte[iconStream.Length];
                iconStream.Read(iconBytes, 0, (int)iconStream.Length);
                File.WriteAllBytes(@"C:\Program Files\Helper\Icon.ico", iconBytes);

                return true;
            }
            else
            {
                Console.WriteLine(@"Helper is already installed at ""C:\Program Files\Helper"" if you want to reinstall Helper then run ""Uninstall"" and then run ""Install"".");
                return false;
            }
        }
        private static void RemoveFromProgramFiles()
        {
            if (Directory.Exists(@"C:\Program Files\Helper"))
            {
                if (Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) == @"C:\Program Files\Helper")
                {
                    ProcessStartInfo cmdProcessInfo = new ProcessStartInfo();

                    cmdProcessInfo.Arguments = @"-Command del 'C:\Program Files\Helper' -r -Force";
                    cmdProcessInfo.CreateNoWindow = true;
                    cmdProcessInfo.UseShellExecute = true;
                    cmdProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    cmdProcessInfo.ErrorDialog = false;
                    cmdProcessInfo.FileName = @"powershell.exe";
                    cmdProcessInfo.Verb = "runas";

                    Process.Start(cmdProcessInfo);

                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    Directory.Delete(@"C:\Program Files\Helper", true);
                }
            }
        }
        private static void AddShortcut()
        {
            if (!Directory.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Helper"))
            {
                Directory.CreateDirectory(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Helper");
            }

            if (!File.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Helper\Helper.lnk"))
            {
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Helper\Helper.lnk");
                shortcut.Description = "Helper is an open source command line utility for windows which includes commands for preforming a huge number of basic tasks.";
                shortcut.Hotkey = "";
                shortcut.RelativePath = @"C:\Program Files\Helper\Helper.exe";
                shortcut.Arguments = "";
                shortcut.WindowStyle = 0;
                shortcut.WorkingDirectory = @"C:\Program Files\Helper";
                shortcut.TargetPath = @"C:\Program Files\Helper\Helper.exe";
                shortcut.IconLocation = @"C:\Program Files\Helper\Icon.ico";

                shortcut.Save();
            }
            else
            {
                Console.WriteLine(@"Start menu shortcut was not be created because the file ""C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Helper\Helper.lnk"" already exists.");
            }
        }
        private static void RemoveShortcut()
        {
            if (Directory.Exists(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Helper"))
            {
                Directory.Delete(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Helper", true);
            }
        }
        private static void RegisterToPath()
        {
            UnregisterFromPath();

            string pathValue = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

            List<string> pathValues = new List<string>(pathValue.Split(';'));

            pathValues.Add(@"C:\Program Files\Helper");

            pathValue = "";

            for (int i = 0; i < pathValues.Count; i++)
            {
                if (i == pathValues.Count - 1)
                {
                    pathValue += pathValues[i];
                }
                else
                {
                    pathValue += pathValues[i] + ";";
                }
            }

            Environment.SetEnvironmentVariable("PATH", pathValue, EnvironmentVariableTarget.Machine);
        }
        private static void UnregisterFromPath()
        {
            string pathValue = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

            List<string> pathValues = new List<string>(pathValue.Split(';'));

            for (int i = 0; i < pathValues.Count; i++)
            {
                if (pathValues[i].Replace("/", @"\") == @"C:\Program Files\Helper")
                {
                    pathValues.RemoveAt(i);
                    i--;
                }
            }

            pathValue = "";

            for (int i = 0; i < pathValues.Count; i++)
            {
                if (i == pathValues.Count - 1)
                {
                    pathValue += pathValues[i];
                }
                else
                {
                    pathValue += pathValues[i] + ";";
                }
            }

            Environment.SetEnvironmentVariable("PATH", pathValue, EnvironmentVariableTarget.Machine);
        }
    }
}
