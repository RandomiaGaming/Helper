using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System;

namespace SharpShell
{
    public static class SettingsManager
    {
        public static string installLocation
        {
            get
            {
                return typeof(SettingsManager).Assembly.Location;
            }
        }
        public static string installRoot
        {
            get
            {
                return Path.GetDirectoryName(installLocation);
            }
        }
        public static string exceptionLogLocation
        {
            get
            {
                RegistryKey currentUser = Registry.CurrentUser;
                RegistryKey software = currentUser.OpenSubKey("SOFTWARE", true);
                if (software is null)
                {
                    software = currentUser.CreateSubKey("SOFTWARE", true);
                }
                RegistryKey helper = software.OpenSubKey("Helper", true);
                if (helper is null)
                {
                    helper = software.CreateSubKey("Helper", true);
                }
                string exceptionLogLocation = (string)helper.GetValue("exceptionLogLocation");
                if (exceptionLogLocation is null)
                {
                    exceptionLogLocation = installRoot + "\\exceptionLog.txt";
                    helper.SetValue("exceptionLogLocation", exceptionLogLocation);
                }
                if (!File.Exists(exceptionLogLocation))
                {
                    File.WriteAllText(exceptionLogLocation, string.Empty);
                }
                return exceptionLogLocation;
            }
            set
            {
                if (value is null || value == string.Empty)
                {
                    throw new NullReferenceException("Exception log location cannot be null or empty.");
                }
                if(value == exceptionLogLocation)
                {
                    return;
                }
                else if (File.Exists(value))
                {
                    throw new ArgumentException("Exception log location was already in use .");
                }
                else
                {

                }
            }
        }
    }
}
