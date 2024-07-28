using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
public static class EncryptionHelper
{
    #region Decrypting Methods
    public static byte[] DecryptBytes(byte[] data, byte[] key)
    {
        if (data is null)
        {
            throw new NullReferenceException("Decrypting operation was aborted because the target data was null.");
        }
        if (data.Length <= 0)
        {
            throw new ArgumentException("Decrypting operation was aborted because the target data had a length of zero.");
        }
        if (key is null)
        {
            throw new NullReferenceException("Decrypting operation was aborted because the given key was null.");
        }
        if (key.Length <= 0)
        {
            throw new ArgumentException("Decrypting operation was aborted because the given key had a length of zero.");
        }

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
    public static string DecryptString(byte[] data, byte[] key)
    {
        return CreateStringFromBytes(DecryptBytes(data, key));
    }
    public static void DecryptFile(string filePath, byte[] key)
    {
        byte[] fileBytes = GetFileBytes(filePath);
        byte[] decryptedFileBytes = DecryptBytes(fileBytes, key);
        CreateFileFromBytesAndOverWrite(decryptedFileBytes, filePath);
    }
    public static void DecryptDirectory(string filePath, byte[] key, string directoryPath)
    {
        byte[] fileBytes = GetFileBytes(filePath);
        File.Delete(filePath);
        byte[] decryptedFileBytes = DecryptBytes(fileBytes, key);
        CreateDirectoryFromBytesAndOverWrite(decryptedFileBytes, directoryPath);
    }
    #endregion
    #region Encrypting Methods
    public static byte[] EncryptBytes(byte[] data, byte[] key)
    {
        if (data is null)
        {
            throw new NullReferenceException("Encrypting operation was aborted because the target data was null.");
        }
        if (data.Length <= 0)
        {
            throw new ArgumentException("Encrypting operation was aborted because the target data had a length of zero.");
        }
        if (key is null)
        {
            throw new NullReferenceException("Encrypting operation was aborted because the given key was null.");
        }
        if (key.Length <= 0)
        {
            throw new ArgumentException("Encrypting operation was aborted because the given key had a length of zero.");
        }
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
    public static byte[] EncryptString(string data, byte[] key)
    {
        return EncryptBytes(GetStringBytes(data), key);
    }
    public static void EncryptFile(string filePath, byte[] key)
    {
        byte[] fileBytes = GetFileBytes(filePath);
        byte[] encryptedFileBytes = EncryptBytes(fileBytes, key);
        CreateFileFromBytesAndOverWrite(encryptedFileBytes, filePath);
    }
    public static void EncryptDirectory(string directoryPath, byte[] key, string filePath)
    {
        byte[] directoryBytes = GetDirectoryBytes(directoryPath);
        Directory.Delete(directoryPath, true);
        byte[] encryptedDirectoryBytes = EncryptBytes(directoryBytes, key);
        CreateFileFromBytesAndOverWrite(encryptedDirectoryBytes, filePath);
    }
    #endregion
    #region Hashing Methods
    public static byte[] HashBytes(byte[] data)
    {
        if (data is null)
        {
            throw new NullReferenceException("Hashing operation was aborted because the target data was null.");
        }
        if (data.Length <= 0)
        {
            throw new ArgumentException("Hashing operation was aborted because the target data had a length of zero.");
        }
        SHA256 hash = SHA256.Create();
        return hash.ComputeHash(data);
    }
    public static byte[] HashString(string data)
    {
        return HashBytes(GetStringBytes(data));
    }
    public static byte[] HashFile(string filePath)
    {
        return HashBytes(GetFileBytes(filePath));
    }
    public static byte[] HashDirectory(string directoryPath)
    {
        return HashBytes(GetDirectoryBytes(directoryPath));
    }
    #endregion
    #region Get Bytes Methods
    public static byte[] GetStringBytes(string data)
    {
        if (data is null)
        {
            throw new NullReferenceException("Decoding operation was aborted because the target data was null.");
        }
        if (data.Length <= 0)
        {
            throw new ArgumentException("Decoding operation was aborted because the target data had a length of zero.");
        }
        return Encoding.Unicode.GetBytes(data);
    }
    public static byte[] GetFileBytes(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new ArgumentException("Decoding operation was aborted because the target file does not exist or is unreadable.");
        }
        return File.ReadAllBytes(filePath);
    }
    public static byte[] GetDirectoryBytes(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new ArgumentException("Decoding operation was aborted because the target directory does not exist or is unreadable.");
        }
        Assembly assembly = Assembly.GetCallingAssembly();
        string assemblyLocation = assembly.Location;
        string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        string tempFilePath = assemblyDirectory + @"\Temp.zip";
        if (File.Exists(tempFilePath))
        {
            throw new Exception("Decoding operation was aborted because the temp file path is in use.");
        }
        ZipFile.CreateFromDirectory(directoryPath, tempFilePath);
        byte[] output = File.ReadAllBytes(tempFilePath);
        File.Delete(tempFilePath);
        return output;
    }
    #endregion
    #region Create Methods
    public static string CreateStringFromBytes(byte[] data)
    {
        if (data is null)
        {
            throw new NullReferenceException("Encoding operation was aborted because the target data was null.");
        }
        if (data.Length <= 0)
        {
            throw new ArgumentException("Encoding operation was aborted because the target data had a length of zero.");
        }
        return Encoding.Unicode.GetString(data);
    }
    public static void CreateFileFromBytes(byte[] data, string filePath)
    {
        if (data is null)
        {
            throw new NullReferenceException("Encoding operation was aborted because the target data was null.");
        }
        if (data.Length <= 0)
        {
            throw new ArgumentException("Encoding operation was aborted because the target data had a length of zero.");
        }
        if (File.Exists(filePath))
        {
            throw new ArgumentException("Encoding operation was aborted because the output file would overwrite an existing file.");
        }
        File.WriteAllBytes(filePath, data);
    }
    public static void CreateFileFromBytesAndOverWrite(byte[] data, string filePath)
    {
        if (data is null)
        {
            throw new NullReferenceException("Encoding operation was aborted because the target data was null.");
        }
        if (data.Length <= 0)
        {
            throw new ArgumentException("Encoding operation was aborted because the target data had a length of zero.");
        }
        File.WriteAllBytes(filePath, data);
    }
    public static void CreateDirectoryFromBytes(byte[] data, string directoryPath)
    {
        if (data is null)
        {
            throw new NullReferenceException("Encoding operation was aborted because the target data was null.");
        }
        if (data.Length <= 0)
        {
            throw new ArgumentException("Encoding operation was aborted because the target data had a length of zero.");
        }
        if (Directory.Exists(directoryPath))
        {
            throw new ArgumentException("Encoding operation was aborted because the output directory would overwrite an existing directory.");
        }
        Assembly assembly = Assembly.GetCallingAssembly();
        string assemblyLocation = assembly.Location;
        string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        string tempFilePath = assemblyDirectory + @"\Temp.zip";
        if (File.Exists(tempFilePath))
        {
            throw new Exception("Decoding operation was aborted because the temp file path is in use.");
        }
        File.WriteAllBytes(tempFilePath, data);
        ZipFile.ExtractToDirectory(tempFilePath, directoryPath);
        File.Delete(tempFilePath);
    }
    public static void CreateDirectoryFromBytesAndOverWrite(byte[] data, string directoryPath)
    {
        if (data is null)
        {
            throw new NullReferenceException("Encoding operation was aborted because the target data was null.");
        }
        if (data.Length <= 0)
        {
            throw new ArgumentException("Encoding operation was aborted because the target data had a length of zero.");
        }
        Assembly assembly = Assembly.GetCallingAssembly();
        string assemblyLocation = assembly.Location;
        string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        string tempFilePath = assemblyDirectory + @"\Temp.zip";
        if (File.Exists(tempFilePath))
        {
            throw new Exception("Decoding operation was aborted because the temp file path is in use.");
        }
        File.WriteAllBytes(tempFilePath, data);
        ZipFile.ExtractToDirectory(tempFilePath, directoryPath);
        File.Delete(tempFilePath);
    }
    #endregion
}