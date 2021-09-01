using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Reflection;

namespace Helper.Commands
{
    public static class PhotoHelper
    {
        [RegisterMethod("Duplicates", false)]
        public static void Duplicates(string path, string args)
        {
            string targetDir = args;

            if (path != Path.GetDirectoryName(Assembly.GetCallingAssembly().Location))
            {
                targetDir = path;
            }

            if (!Directory.Exists(targetDir))
            {
                throw new ArgumentException();
            }

            List<byte[]> usedHashes = new List<byte[]>();

            int index = 0;
            string[] files = Directory.GetFiles(targetDir);
            foreach (string p in files)
            {
                byte[] hash = CryptographyHelper.SHA256HashBytes(File.ReadAllBytes(p));

                bool fileWasDuplicate = false;

                foreach (byte[] usedHash in usedHashes)
                {
                    if (ByteArraysEqual(hash, usedHash))
                    {
                        if (!Directory.Exists(targetDir + "\\Duplicates"))
                        {
                            Directory.CreateDirectory(targetDir + "\\Duplicates");
                        }
                        File.Move(p, targetDir + "\\Duplicates\\" + Path.GetFileName(p));
                        fileWasDuplicate = true;
                        break;
                    }
                }

                if (!fileWasDuplicate)
                {
                    usedHashes.Add(hash);
                }

                Console.WriteLine($"Indexed file \"{p}\". {index} out of {files.Length} completed.");
                index++;
            }
        }
        public static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }
        [RegisterMethod("Index", false)]
        public static void Index(string path, string args)
        {
            string targetDir = args;

            if (path != Path.GetDirectoryName(Assembly.GetCallingAssembly().Location))
            {
                targetDir = path;
            }

            if (!Directory.Exists(targetDir))
            {
                throw new ArgumentException();
            }

            if (Directory.Exists(targetDir + "\\Temp"))
            {
                throw new Exception($"Temp folder already exists. Please delete \"{targetDir + "\\Temp"}\" and then try again.");
            }

            Directory.CreateDirectory(targetDir + "\\Temp");
            string[] files = Directory.GetFiles(targetDir);

            for (int i = 0; i < files.Length; i++)
            {
                File.Move(files[i], targetDir + "\\Temp\\" + i.ToString() + Path.GetExtension(files[i]));
            }

            foreach (string file in Directory.GetFiles(targetDir + "\\Temp"))
            {
                File.Move(file, targetDir + "\\" + Path.GetFileName(file));
            }

            Directory.Delete(targetDir + "\\Temp");
        }
        [RegisterMethod("AES", false)]
        public static void AES(string args)
        {
            if (!Directory.Exists(args))
            {
                throw new ArgumentException();
            }
            string tempDirPath = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + @"\Temp";

            if (!Directory.Exists(tempDirPath))
            {
                Directory.CreateDirectory(tempDirPath);
            }

            string targetDirName = "";

            if (args.EndsWith(@"\") || args.EndsWith("/"))
            {
                args = args.Substring(0, args.Length - 1);
            }

            for (int i = 0; i < args.Length; i++)
            {
                targetDirName += args[i];

                if (args[i] == '\\' || args[i] == '/')
                {
                    targetDirName = "";
                }
            }

            ZipFile.CreateFromDirectory(args, tempDirPath + $"\\{targetDirName}.aes");
            Directory.Delete(args, true);
            string aesFilePath = args.Substring(0, args.Length - targetDirName.Length - 1) + $"\\{targetDirName}.aes";
            File.Move(tempDirPath + $"\\{targetDirName}.aes", aesFilePath);
            Directory.Delete(tempDirPath, true);

            byte[] rawBytes = File.ReadAllBytes(aesFilePath);
        }
    }
}
