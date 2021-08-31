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
        public static List<HelperCommand> methods = new List<HelperCommand>();
        [STAThread]
        public static void Main()
        {
            try
            {
                LoadMethods();
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
        public static void LoadMethods()
        {
            methods = new List<HelperCommand>();
            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods())
                {
                    Attribute attribute = method.GetCustomAttribute(typeof(RegisterCommandAttribute));
                    if (!(attribute is null))
                    {
                        LoadMethod(method);
                    }
                }
            }
        }
        public static void LoadMethod(MethodInfo baseMethod)
        {
            try
            {
                if (baseMethod is null)
                {
                    throw new Exception("Cannot load method because target C# method was null.");
                }
                if (!baseMethod.IsStatic)
                {
                    throw new Exception("Cannot load method because target C# method requires an object reference.");
                }

                Attribute uncastAttribute = baseMethod.GetCustomAttribute(typeof(RegisterCommandAttribute));
                if (uncastAttribute is null || uncastAttribute.GetType() != typeof(RegisterCommandAttribute))
                {
                    throw new Exception("Cannot load method because the C# method does not have the propper atribute.");
                }
                RegisterCommandAttribute attribute = (RegisterCommandAttribute)uncastAttribute;
                methods.Add(new HelperCommand(baseMethod, baseMethod.Name, attribute.description, attribute.requiresAdministrator));
            }
            catch (Exception e)
            {
                LogException(GetExceptionMessage(e));
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
                HelperCommand target = null;

                foreach (HelperCommand helperMethod in methods)
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