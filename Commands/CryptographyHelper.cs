using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
public static class CryptographyHelper
{
    /*public static void DecryptDirectory(string inputFilePath, string outputDirectoryPath, string password)
    {
        if (!File.Exists(inputFilePath))
        {
            throw new ArgumentException($"Input file \"{inputFilePath}\" does not exist.");
        }
        if (Directory.Exists(outputDirectoryPath))
        {
            throw new ArgumentException($"Output directory at \"{outputDirectoryPath}\" already exists.");
        }
        string tempFilePath = "";
        for (int i = 0; i <= 9999; i++)
        {
            if (!File.Exists($"{GetTempRoot()}{i}.zip"))
            {
                tempFilePath = $"{GetTempRoot()}{i}.zip";
                break;
            }
        }
        if (tempFilePath == "")
        {
            throw new Exception("Temp file could not be created because all availible temp paths are in use.");
        }
        SHA256 sha256 = SHA256.Create();
        byte[] key = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
        byte[] data = File.ReadAllBytes(inputFilePath);
        try
        {
            byte[] decryptedData = AESDecryptBytes(data, key);
            File.WriteAllBytes(tempFilePath, decryptedData);
            Directory.CreateDirectory(outputDirectoryPath);
            ZipFile.ExtractToDirectory(tempFilePath, outputDirectoryPath);
            File.Delete(tempFilePath);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    public static void EncryptDirectory(string inputDirectoryPath, string outputFilePath, string password)
    {
        if (!Directory.Exists(inputDirectoryPath))
        {
            throw new ArgumentException($"Input directory at \"{inputDirectoryPath}\" does not exist.");
        }
        if (File.Exists(outputFilePath))
        {
            throw new ArgumentException($"Output file \"{outputFilePath}\" already exists.");
        }
        string tempFilePath = "";
        for (int i = 0; i <= 9999; i++)
        {
            if (!File.Exists($"{GetTempRoot()}\\{i}.zip"))
            {
                tempFilePath = $"{GetTempRoot()}\\{i}.zip";
                break;
            }
        }
        if (tempFilePath == "")
        {
            throw new Exception("Temp file could not be created because all availible temp paths are in use.");
        }
        ZipFile.CreateFromDirectory(inputDirectoryPath, tempFilePath);
        byte[] data = File.ReadAllBytes(tempFilePath);
        SHA256 sha256 = SHA256.Create();
        byte[] key = sha256.ComputeHash(Encoding.ASCII.GetBytes(password));
        byte[] encryptedData = AESEncryptBytes(data, key);
        File.WriteAllBytes(outputFilePath, encryptedData);
        File.Delete(tempFilePath);
    }*/
    public static byte[] AESEncrypt(byte[] data, byte[] key)
    {
        Aes aes = Aes.Create();

        aes.KeySize = key.Length * 8;
        aes.IV = new byte[aes.BlockSize / 8];
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.ISO10126;

        ICryptoTransform decryptor = aes.CreateEncryptor();
        MemoryStream ms = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();
        return ms.ToArray();
    }
    public static byte[] AESEncrypt(byte[] data, string key)
    {
        return AESEncrypt(data, SHA256Hash(key));
    }
    public static byte[] AESDecrypt(byte[] data, byte[] key)
    {
        Aes aes = Aes.Create();

        aes.KeySize = key.Length * 8;
        aes.IV = new byte[aes.BlockSize / 8];
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.ISO10126;

        ICryptoTransform decryptor = aes.CreateDecryptor();
        MemoryStream ms = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Write);
        cryptoStream.Write(data, 0, data.Length);
        cryptoStream.FlushFinalBlock();
        return ms.ToArray();
    }
    public static byte[] AESDecrypt(byte[] data, string key)
    {
        return AESDecrypt(data, SHA256Hash(key));
    }
    public static byte[] SHA256Hash(byte[] data)
    {
        SHA256 hash = SHA256.Create();
        return hash.ComputeHash(data);
    }
    public static byte[] SHA256Hash(string data)
    {
        return SHA256Hash(Encoding.Unicode.GetBytes(data));
    }
}