using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.IO;
using System.Diagnostics;
namespace Helper
{
    public static class Program
    {
        private static List<Command> loadedCommands = new List<Command>();
        [STAThread]
        public static void Main()
        {
            try
             {
                 LoadCommands();
                 while (true)
                 {
                     InvokeCommand(Console.ReadLine());
                 }
             }
             catch (Exception e)
             {
                 LogException(GetExceptionMessage(e));
                 Crash();
             }
        }
        public static void LoadCommandsFromAssembly(Assembly sourceAssembly)
        {
            foreach (Type type in sourceAssembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    if (method.GetCustomAttribute<RegisterCommandAttribute>() != null)
                    {
                        try
                        {
                            LoadCommand(method);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to load command \"{method.Name}\" from assembly \"{sourceAssembly.FullName}\" due to exception \"{ex.Message}\".");
                        }
                    }
                }
            }
        }
        public static void LoadCommand(MethodInfo sourceMethod)
        {
            try
            {
                loadedCommands()
                    }
            catch (Exception ex)
            {
                Console.WriteLine
            }
        }
        public static void InvokeCommand(string Command)
        {
            try
            {
                if (Command is null || Command == "")
                {
                    throw new Exception("Cannot invoke command because it was null or empty.");
                }

                if (!IsValidCommand(Command))
                {
                    throw new Exception("Cannot invoke command because it contains invalid characters.");
                }

                List<string> methodTrees = new List<string>();
                string currentMethodTree = "";

                for (int i = 0; i < Command.Length; i++)
                {
                    if (Command[i] == ';')
                    {
                        if (currentMethodTree == "")
                        {
                            throw new Exception("Command syntax error: Duplicate semicolon.");
                        }
                        else
                        {
                            methodTrees.Add(currentMethodTree);
                            currentMethodTree = "";
                        }
                    }
                    else if (Command[i] != ' ' && Command[i] != '\n' && Command[i] != '\t' && Command[i] != '\r')
                    {
                        currentMethodTree += Command[i];
                    }
                }

                if (currentMethodTree != "")
                {
                    methodTrees.Add(currentMethodTree);
                }

                foreach (string methodTree in methodTrees)
                {
                    InvokeMethodTree(methodTree);
                }
            }
            catch (Exception e)
            {
                LogException(GetExceptionMessage(e));
            }
        }
        public static void ParseCommand(string command)
        {
            try
            {
                MethodTreeNode baseNode = GetNodeFromString(methodTree);
                object output = InvokeMethodTreeNode(baseNode);
                if (!(output is null))
                {
                    Console.WriteLine(output.ToString());
                }
            }
            catch (Exception e)
            {
                LogException(GetExceptionMessage(e));
            }
        }
        private static object InvokeMethodTreeNode(MethodTreeNode node)
        {
            if (node.isConstant)
            {
                return node.value;
            }
            else
            {
                Command target = null;

                foreach (Command helperMethod in methods)
                {
                    if (helperMethod.name.ToUpper() == node.value.ToUpper())
                    {
                        target = helperMethod;
                        break;
                    }
                }

                if (target is null)
                {
                    throw new Exception($"Could not invoke method tree because no method exists with the name {node.value}.");
                }

                List<object> parameters = new List<object>();
                foreach (MethodTreeNode parameterNode in node.parameterNodes)
                {
                    parameters.Add(InvokeMethodTreeNode(parameterNode));
                }

                List<Type> parameterTypes = target.GetParameterTypes();

                if (parameters.Count > parameterTypes.Count)
                {
                    throw new Exception($"Too many parameters were given for the method {target.name}.");
                }
                else if (parameters.Count < parameterTypes.Count)
                {
                    throw new Exception($"Not enough parameters were given for the method {target.name}.");
                }

                for (int i = 0; i < parameterTypes.Count; i++)
                {
                    if (parameters[i] is null)
                    {
                        if (parameterTypes[i].IsValueType)
                        {
                            throw new Exception($"Null is not valid for the parameter {target.GetParameterName(i)} because it is not nullable.");
                        }
                    }
                    else if (!parameterTypes[i].IsAssignableFrom(parameters[i].GetType()))
                    {
                        throw new Exception($"Value for the parameter {target.GetParameterName(i)} was not valid because type {parameters.GetType().FullName} cannot be cast to {parameterTypes[i]}.");
                    }
                }

                return target.Invoke(parameters);
            }
        }
        public static void LogException(string errorMessage)
        {
            if (errorMessage is null)
            {
                errorMessage = "Unknown exception.";
            }

            errorMessage = $"Exception: {errorMessage}";

            if (!File.Exists(GetExceptionLog()))
            {
                File.WriteAllText(GetExceptionLog(), "");
            }
            string exceptionLogContents = File.ReadAllText(GetExceptionLog());
            if (exceptionLogContents is null || exceptionLogContents == "")
            {
                exceptionLogContents = errorMessage;
            }
            else
            {
                exceptionLogContents = errorMessage + "\n" + exceptionLogContents;
            }
            File.WriteAllText(GetExceptionLog(), exceptionLogContents);
            Console.WriteLine(errorMessage);
        }
        public static void Crash()
        {
            if (!File.Exists(GetExceptionLog()))
            {
                File.WriteAllText(GetExceptionLog(), "");
            }
            Process.Start(GetExceptionLog());
            Process.GetCurrentProcess().Kill();
        }
        public static string GetExceptionMessage(Exception e)
        {
            if (e is null)
            {
                return "Unknown exception.";
            }
            while (!(e.InnerException is null))
            {
                e = e.InnerException;
            }
            return e.Message;
        }
        public static bool AskYesOrNo()
        {
            Console.WriteLine("Please select [Y]es or [N]o...");
            string answer = Console.ReadLine();
            if (answer.ToUpper() == "Y" || answer.ToUpper() == "YES")
            {
                return true;
            }
            return false;
        }
        public static bool IsAdministrator()
        {
            try
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to determine if process is administrator because of exception: {GetExceptionMessage(e)}");
            }
        }
        public static string GetRoot()
        {
            return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
        }
        public static string GetExceptionLog()
        {
            return GetRoot() + "\\ExceptionLog.txt";
        }
        public static string GetAssemblyLocation()
        {
            return Assembly.GetCallingAssembly().Location;
        }
    }
}