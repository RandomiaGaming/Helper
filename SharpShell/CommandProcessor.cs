using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace SharpShell
{
    public class CommandProcessor
    {
        private static List<Function> loadedFunctions = new List<Function>();
        public  void Main()
        {
            try
            {
                LoadFunctionsFromAssembly(Assembly.GetCallingAssembly());

                string argumentFunction = GetArgumentFunction();
                if (!(argumentFunction is null) && argumentFunction != string.Empty)
                {
                    try
                    {
                        RunCommand(argumentFunction);
                    }
                    catch (Exception ex)
                    {
                        LogFatalException(ex);
                    }
                    Process.GetCurrentProcess().Kill();
                }

                while (true)
                {
                    try
                    {
                        RunCommand(Console.ReadLine());
                    }
                    catch (Exception ex)
                    {
                        LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogFatalException(ex);
            }
        }
        public static string GetArgumentFunction()
        {
            try
            {
                List<string> args = new List<string>(Environment.GetCommandLineArgs());
                if (args.Count >= 1)
                {
                    args.RemoveAt(0);
                }
                else
                {
                    return string.Empty;
                }

                string output = string.Empty;

                foreach (string arg in args)
                {
                    if (arg.Contains(" "))
                    {
                        output += $" \"{arg}\"";
                    }
                    else
                    {
                        output += $" {arg}";
                    }
                }

                if (output is null | output == string.Empty)
                {
                    return null;
                }
                else
                {
                    return output;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not get function from command line args due to exception: {GetExceptionMessage(ex)}");
            }
        }

        public static void LoadFunctionsFromAssembly(Assembly assembly)
        {
            if (assembly is null)
            {
                throw new Exception($"Could not load functions from assembly because assembly was null.");
            }
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    foreach (MethodInfo method in type.GetMethods())
                    {
                        if (method.GetCustomAttribute<RegisterFunctionAttribute>() != null)
                        {
                            try
                            {
                                LoadFunctionFromMethod(method);
                            }
                            catch (Exception ex)
                            {
                                LogException(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not load functions from assembly \"{assembly.FullName}\" due to exception \"{GetExceptionMessage(ex)}\".");
            }
        }
        public static void LoadFunctionFromMethod(MethodInfo method)
        {
            if (method is null)
            {
                throw new Exception($"Could not load function from method because method was null.");
            }
            try
            {
                LoadFunction(new Function(method));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load function from method \"{method.Name}\" due to exception \"{GetExceptionMessage(ex)}\".");
            }
        }
        public static void LoadFunction(Function function)
        {
            if (function is null)
            {
                throw new Exception($"Could not load function because function was null.");
            }
            try
            {
                for (int i = 0; i < loadedFunctions.Count; i++)
                {
                    if (loadedFunctions[i].Name.ToUpper() == function.Name.ToUpper())
                    {
                        throw new Exception($"A function with the name \"{function.Name}\" already exists.");
                    }
                }
                loadedFunctions.Add(function);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not load function \"{function.Name}\" due to exception \"{GetExceptionMessage(ex)}\".");
            }
        }

        public static void UnloadFunctionsByAssembly(Assembly assembly)
        {
            if (assembly is null)
            {
                throw new Exception($"Could not unload functions by assembly because assembly was null.");
            }
            try
            {
                for (int i = 0; i < loadedFunctions.Count; i++)
                {
                    if (loadedFunctions[i].SourceAssembly == assembly)
                    {
                        UnloadFunction(loadedFunctions[i]);
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not unload functions by assembly \"{assembly.FullName}\" due to exception \"{GetExceptionMessage(ex)}\".");
            }
        }
        public static void UnloadFunctionByMethod(MethodInfo method)
        {
            if (method is null)
            {
                throw new Exception($"Could not unload function by method because method was null.");
            }
            try
            {
                for (int i = 0; i < loadedFunctions.Count; i++)
                {
                    if (loadedFunctions[i].Name == method.Name)
                    {
                        UnloadFunction(loadedFunctions[i]);
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not unload function \"{method.Name}\" due to exception \"{GetExceptionMessage(ex)}\".");
            }
        }
        public static void UnloadFunction(Function function)
        {
            if (function is null)
            {
                throw new Exception($"Could not unload function because function was null.");
            }
            try
            {
                for (int i = 0; i < loadedFunctions.Count; i++)
                {
                    if (loadedFunctions[i].Name == function.Name)
                    {
                        loadedFunctions.RemoveAt(i);
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not unload function \"{function.Name}\" due to exception \"{GetExceptionMessage(ex)}\".");
            }
        }

        public static List<Function> GetLoadedFunctions()
        {
            return new List<Function>(loadedFunctions);
        }

        public void RunCommand(string command)
        {
            if (command is null || command == string.Empty)
            {
                throw new Exception("Could not run command because command was null.");
            }
            try
            {
                List<string> splitCommand = SplitCommand(command);

                if (splitCommand is null || splitCommand.Count == 0)
                {
                    throw new Exception("Could not run command because split command was null.");
                }

                string commandName = splitCommand[0].ToUpper();

                List<string> arguments;

                if (splitCommand.Count >= 2)
                {
                    arguments = splitCommand.GetRange(1, splitCommand.Count - 1);
                }
                else
                {
                    arguments = new List<string>();
                }

                bool foundMatch = false;
                for (int i = 0; i < loadedFunctions.Count; i++)
                {
                    if (loadedFunctions[i].Name.ToUpper() == commandName)
                    {
                        loadedFunctions[i].Invoke(arguments);
                        foundMatch = true;
                    }
                }
                if (!foundMatch)
                {
                    throw new Exception($"No function exists with the name \"{commandName}\".");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not run command \"{command}\" due to exception \"{GetExceptionMessage(ex)}\".");
            }
        }
        public static List<string> SplitCommand(string source)
        {
            if (source is null || source == string.Empty)
            {
                return new List<string>();
            }

            try
            {
                List<string> output = new List<string>();

                string currentStatement = string.Empty;
                bool inQuotes = false;
                for (int i = 0; i < source.Length; i++)
                {
                    if (source[i] == '"')
                    {
                        if (currentStatement != string.Empty)
                        {
                            output.Add(currentStatement);
                        }
                        currentStatement = string.Empty;
                        inQuotes = !inQuotes;
                    }
                    else if (source[i] == ' ' && !inQuotes)
                    {
                        if (currentStatement != string.Empty)
                        {
                            output.Add(currentStatement);
                        }
                        currentStatement = string.Empty;
                    }
                    else
                    {
                        currentStatement += source[i];
                    }
                }
                if (currentStatement != string.Empty)
                {
                    output.Add(currentStatement);
                }

                return output;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not split string \"{source}\" due to exception \"{GetExceptionMessage(ex)}\".");
            }
        }

        public static void LogException(Exception ex)
        {
            try
            {
                string exceptionLogEntry = GetExceptionLogEntry(ex);
                try
                {
                    Console.WriteLine(exceptionLogEntry);
                }
                catch
                {

                }
                try
                {
                    string exceptionLogLocation = (string)Registry.CurrentUser.OpenSubKey("SOFTWARE").OpenSubKey("Helper").GetValue("exceptionLog");
                    string exceptionLogContents = File.ReadAllText(exceptionLogLocation);
                    File.WriteAllText(exceptionLogLocation, $"{exceptionLogEntry}\n{exceptionLogContents}");
                }
                catch
                {

                }
            }
            catch
            {

            }
        }
        public static void LogFatalException(Exception ex)
        {
            try
            {
                string exceptionLogEntry = GetExceptionLogEntry(ex);
                string exceptionLogLocation = (string)Registry.CurrentUser.OpenSubKey("SOFTWARE").OpenSubKey("Helper").GetValue("exceptionLog");
                string exceptionLogContents = File.ReadAllText(exceptionLogLocation);
                File.WriteAllText(exceptionLogLocation, $"{exceptionLogEntry}\n{exceptionLogContents}");
                Process.Start(exceptionLogLocation);
            }
            catch
            {

            }
            Process.GetCurrentProcess().Kill();
        }
        public static string GetExceptionLogEntry(Exception ex)
        {
            string exceptionType;
            try
            {
                exceptionType = ex.GetType().Name;
            }
            catch
            {
                exceptionType = "Exception";
            }

            string exceptionMessage = GetExceptionMessage(ex);

            string exceptionDateTime;
            try
            {
                exceptionDateTime = $"{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}";
            }
            catch
            {
                exceptionDateTime = "00/00/00 00:00:00";
            }

            return $"{exceptionType} thrown at {exceptionDateTime}: {exceptionMessage}";
        }
        public static string GetExceptionMessage(Exception ex)
        {
            try
            {
                string output = ex.Message;
                output = output.Replace("\n", string.Empty);
                output = output.Replace("\r", string.Empty);
                output = output.Replace("\t", string.Empty);
                return output;
            }
            catch
            {
                return "An unknown exception was thrown.";
            }
        }

        public static bool IsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to determine if process is administrator because of exception: {GetExceptionMessage(ex)}");
            }
        }
        public static bool IsInstalled()
        {
            return Registry.CurrentUser.OpenSubKey("SOFTWARE").OpenSubKey("Helper").GetValue("installLocation") != null;
        }
    }
}