using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using Microsoft.Win32;

namespace KQuery
{
    public class Functions
    {
        public static bool CheckArgument(string param, string[] args, int i)
        {
            if (args.Length > i + 1)
                if (args[i + 1].Length > 0)
                    if (args[i + 1][0] != '/')
                        return true;
            Console.WriteLine(Environment.NewLine + "Parameter for switch {0} is missing.", param);

            return false;
        }

        public static bool CheckParameter(string param, string[] args, int i, int index, string param2)
        {
            string[] switches = param2.Split(' ');
            if (args.Length > i + index && i + index >= 0)
                if (args[i + index].Length > 0)
                {
                    foreach (string s in switches)
                    {
                        if (args[i + index].ToUpper().Equals(s.ToUpper()))
                            return true;
                    }
                    Console.WriteLine(Environment.NewLine + "The {0} switch must follow after one of these preceding switches: {1}", param, param2);
                    return false;
                }
            Console.WriteLine(Environment.NewLine + "Parameter for switch {0} is invalid.", param);

            return false;
        }

        public static string GetSwitch(string[] args, int i, int index)
        {
            return args[i + index].ToUpper();
        }

        public static bool CheckFileExist(string path)
        {
            bool result = File.Exists(path);

            if(result)
                return true;

            return false;
        }

        public static bool CheckProcessExist(string p)
        {
            Process[] process = Process.GetProcessesByName(p);

            if (process.Length > 0)
                return true;

            return false;
        }

        public static bool CheckVersionFormat(string version)
        {

            long n;
            if (long.TryParse(version.Replace(".", "").Replace(",", ""), out n))
                return true;
            return false;
        }

        public static int CheckFileVersion(string path, string version)
        {
            if(File.Exists(path))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(path);
                Version fileVersion = new Version(versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
                return CheckVersion(fileVersion.ToString(), version);
            }

            return 2;
        }

        public static int CheckVersion(string currentVersion, string latestVersion)
        {
            try
            {
                currentVersion = currentVersion.Replace(",", ".");
                latestVersion = latestVersion.Replace(",", ".");

                if (!currentVersion.Contains("."))
                    currentVersion += ".0";

                if (!latestVersion.Contains("."))
                    latestVersion += ".0";

                Version v1 = new Version(currentVersion);
                Version v2 = new Version(latestVersion);

                return v1.CompareTo(v2);
            }
            catch
            {
                return 3;
            }
        }

        public static bool CheckRegKeyExist(RegistryKey root, string key)
        {
            RegistryKey baseKey = root.OpenSubKey(key);

            if(baseKey != null)
                return true;

            return false;
        }

        public static bool CheckRegKeyValueExist(RegistryKey root, string key, string value)
        {
            RegistryKey baseKey;
            baseKey = root.OpenSubKey(key);

            if (baseKey != null)
                if (baseKey.GetValue(value, null) != null)
                    return true;

            return false;
        }

        public static int CheckRegKeyValueVersion(RegistryKey root, string key, string value, string version)
        {
            RegistryKey baseKey;
            baseKey = root.OpenSubKey(key);

            if (baseKey != null)
                if (baseKey.GetValue(value, null) != null)
                {
                    string currentVersion = baseKey.GetValue(value).ToString();
                    return CheckVersion(currentVersion, version);
                }
                else
                    return 2;

            return 2;
        }

        public static bool CheckComputerName(string computerList)
        {
            List<string> computerNames = computerList.ToUpper().Split(',').ToList();

            foreach (string computerName in computerNames)
                Console.WriteLine("\t{0}", computerName);

            string query = computerNames.Find(computerName => computerName == Environment.MachineName.ToUpper());
            if (query != null)
                return true;


            return false;
        }

        public static bool CheckComputerSecurityGroup(string computerName, string groupNames, string domain)
        {
            List<string> groupName = groupNames.ToUpper().Split(',').ToList();
            List<string> groups = new List<string>();

            PrincipalContext domainContext = new PrincipalContext(ContextType.Domain);
            ComputerPrincipal computer = ComputerPrincipal.FindByIdentity(domainContext, computerName);

            using (var directoryEntry = new DirectoryEntry("LDAP://" + computer.DistinguishedName))
            {
                PropertyValueCollection ValueCollection = directoryEntry.Properties["memberOf"];
                IEnumerator en = ValueCollection.GetEnumerator();
                while (en.MoveNext())
                    if (en.Current != null)
                    {
                        string group = en.Current.ToString().Substring(3, (en.Current.ToString()).IndexOf(',') - 3);
                        if (!groups.Contains(group))
                        {
                            groups.Add(group);
                            //if (recursive)
                            //AttributeValuesMultiString(attributeName, en.Current.ToString(), valuesCollection, true);
                        }
                    }
            }

            foreach (string name in groupName)
                Console.WriteLine("\t{0}", name);

            foreach (string g in groups)
            {
                string query = groupName.Find(group => group == g.ToUpper());
                if (query != null)
                    return true;
            }

            return false;

        }

        public static void DisplayHelp()
        {
            Console.WriteLine("KQuery    [/I] [/|+|-F filepath] [/|+|-P filepath] [/V version]");
            Console.WriteLine("          [[/|+|-K keypath] /|+|-VALUE keyvalue]] [/|+|-T task]" + Environment.NewLine);
            Console.WriteLine("  /I      Prepends filepath or keypath with %PROGRAMFILES% or HKLM:\\SOFTWARE");
            Console.WriteLine("          when used with /|+|-P or /|+|-K switch, respectively." + Environment.NewLine);
            Console.WriteLine("  /F      Checks if filepath exists. Assumes YES if it does not.");
            Console.WriteLine("  +F      Checks if filepath exists.");
            Console.WriteLine("  -F      Checks if filepath does not exist." + Environment.NewLine);
            Console.WriteLine("  /K      Checks if registry keypath exists. Assumes YES if it does not.");
            Console.WriteLine("  +K      Checks if registry keypath exists.");
            Console.WriteLine("  -K      Checks if registry keypath does not exist.");
            Console.WriteLine("          The K switch prepends HKLM:\\SOFTWARE or HKLM:\\SOFTWARE\\WOW6432Node to keypath." + Environment.NewLine);
            Console.WriteLine("  /P      Checks if filepath exists. Assumes YES if it does not.");
            Console.WriteLine("  +P      Checks if filepath exists.");
            Console.WriteLine("  -P      Checks if filepath does not exist.");
            Console.WriteLine("          The P switch prepends %PROGRAMFILES(X86)% to filepath." + Environment.NewLine);
            Console.WriteLine("  /T      Checks if task exists. Assumes YES if it does not.");
            Console.WriteLine("  +T      Checks if task exists.");
            Console.WriteLine("  -T      Checks if task does not exist." + Environment.NewLine);
            Console.WriteLine("  /VALUE  Checks if registry keyvalue exist. Assumes YES if it does not.");
            Console.WriteLine("  +VALUE  Checks if registry keyvalue exist.");
            Console.WriteLine("  -VALUE  Checks if registry keyvalue does not exist.");
            Console.WriteLine("          The VALUE switch must follow after /|+K." + Environment.NewLine);
            Console.WriteLine("  /V      Specifies the version number.");
            Console.WriteLine("          The V switch must follow after /|+F, /|+P, or /|+VALUE." + Environment.NewLine);
            Console.WriteLine("Usage:" + Environment.NewLine);
            Console.WriteLine("  KQuery /F \"C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe\" /V 5.1.2");
            Console.WriteLine("  KQuery /P \"Mozilla Firefox\\firefox.exe\" /V 5.1.2");
            Console.WriteLine("  KQuery /I /P \"Mozilla Firefox\\firefox.exe\" /V 5.1.2");
            Console.WriteLine("  KQuery /K \"JavaSoft\\Java Runtime Environment\" /VALUE CurrentVersion /V 1.8");
            Console.WriteLine("  KQuery /T chrome" + Environment.NewLine);
            Console.WriteLine("Return Code:" + Environment.NewLine);
            Console.WriteLine("  0 - Successful, all conditions were met.");
            Console.WriteLine("  1 - Failed, one of the conditions did not pass.");
        }
    }

    public class Program
    {
        static void PrintResult(bool passed = true, int errorCode = 0)
        {
            if (!passed)
                switch (errorCode)
                {
                    case 0:
                        Console.WriteLine("NO");
                        break;
                    case 1:
                        Console.WriteLine("NO, BUT ASSUMING YES");
                        break;
                    case 2:
                        Console.WriteLine("UNABLE TO DETERMINE VERSION NUMBER");
                        break;
                }
            else
                Console.WriteLine("YES");
        }

        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Functions.DisplayHelp();
                return 0;
            }

            bool ignoreBit = false;
            string prefix = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\";
            RegistryKey root = null;

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg.ToUpper())
                {
                    case "/I":
                        if (args.Length == 1)
                        {
                            Functions.DisplayHelp();
                            return 0;
                        }
                        ignoreBit = true;
                        prefix = @"C:\Program Files\";
                        break;
                    case "/V":
                        if (Functions.CheckArgument("/V", args, i))
                        {
                            if (!Functions.CheckVersionFormat(args[i + 1]))
                            {
                                Console.WriteLine(Environment.NewLine + "Version number format is not valid: {0}", args[i + 1]);
                                return -1;
                            }
                        }
                        else
                            return -1;
                        break;
                    case "/?":
                        Functions.DisplayHelp();
                        return 0;
                }
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                switch (arg.ToUpper())
                {
                    case "/K":
                    case "+K":
                    case "-K":
                        if ((Environment.Is64BitOperatingSystem && !ignoreBit) || !Environment.Is64BitOperatingSystem)
                            root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SOFTWARE");
                        else
                            root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SOFTWARE");
                        break;
                }
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                bool bResult = true;
                int flag = 0;

                switch (arg.ToUpper())
                {
                    case "/I":
                        break;
                    case "/F":
                        if (Functions.CheckArgument("/F", args, i))
                        {
                            Console.Write("Checking if file \"{0}\" exists: ", args[i + 1]);
                            bResult = Functions.CheckFileExist(args[++i]);
                            PrintResult(bResult, 1);
                        }
                        else
                            flag = 1;
                        break;
                    case "+F":
                        if (Functions.CheckArgument("+F", args, i))
                        {
                            Console.Write("Checking if file \"{0}\" exists: ", args[i + 1]);
                            bResult = Functions.CheckFileExist(args[++i]);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "-F":
                        if (Functions.CheckArgument("-F", args, i))
                        {
                            Console.Write("Checking if file \"{0}\" does not exist: ", args[i + 1]);
                            bResult = !Functions.CheckFileExist(args[++i]);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "/K":
                        if (Functions.CheckArgument("/K", args, i))
                        {
                            Console.Write("Checking if registry key \"{0}\\{1}\" exists: ", root.ToString(), args[i + 1]);
                            bResult = Functions.CheckRegKeyExist(root, args[++i]);
                            PrintResult(bResult, 1);
                        }
                        else
                            flag = 1;
                        break;
                    case "+K":
                        if (Functions.CheckArgument("+K", args, i))
                        {
                            Console.Write("Checking if registry key \"{0}\\{1}\" exists: ", root.ToString(), args[i + 1]);
                            bResult = Functions.CheckRegKeyExist(root, args[++i]);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "-K":
                        if (Functions.CheckArgument("-K", args, i))
                        {
                            Console.Write("Checking if registry key \"{0}\\{1}\" does not exist: ", root.ToString(), args[i + 1]);
                            bResult = !Functions.CheckRegKeyExist(root, args[++i]);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "/P":
                        if (Functions.CheckArgument("/P", args, i))
                        {
                            Console.Write("Checking if file \"{0}{1}\" exists: ", prefix, args[i + 1]);
                            bResult = Functions.CheckFileExist(prefix + args[++i]);
                            PrintResult(bResult, 1);
                        }
                        else
                            flag = 1;
                        break;
                    case "+P":
                        if (Functions.CheckArgument("+P", args, i))
                        {
                            Console.Write("Checking if file \"{0}{1}\" exists: ", prefix, args[i + 1]);
                            bResult = Functions.CheckFileExist(prefix + args[++i]);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "-P":
                        if (Functions.CheckArgument("-P", args, i))
                        {
                            Console.Write("Checking if file \"{0}{1}\" does not exist: ", prefix, args[i + 1]);
                            bResult = !Functions.CheckFileExist(prefix + args[++i]);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "/T":
                        if (Functions.CheckArgument("/T", args, i))
                        {
                            Console.Write("Checking if task \"{0}\" exists: ", args[i + 1]);
                            bResult = Functions.CheckProcessExist(args[++i]);
                            PrintResult(bResult, 1);
                        }
                        else
                            flag = 1;
                        break;
                    case "+T":
                        if (Functions.CheckArgument("+T", args, i))
                        {
                            Console.Write("Checking if task \"{0}\" is running: ", args[i + 1]);
                            bResult = Functions.CheckProcessExist(args[++i]);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "-T":
                        if (Functions.CheckArgument("-T", args, i))
                        {
                            Console.Write("Checking if task \"{0}\" is not running: ", args[i + 1]);
                            bResult = !Functions.CheckProcessExist(args[++i]);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "/V":
                        int result = -1;
                        if (Functions.CheckParameter("/V", args, i, -2, "/F +F /P +P /VALUE +VALUE"))
                        {
                            if (Functions.CheckArgument("/V", args, i))
                            {
                                if (Functions.GetSwitch(args, i, -2).Equals("/F") || Functions.GetSwitch(args, i, -2).Equals("+F"))
                                {
                                    Console.Write("Checking if file version \"{0}\" is outdated: ", args[i - 1]);
                                    result = Functions.CheckFileVersion(args[i - 1], args[++i]);
                                }
                                else if (Functions.GetSwitch(args, i, -2).Equals("/P") || Functions.GetSwitch(args, i, -2).Equals("+P"))
                                {
                                    Console.Write("Checking if file version \"{0}{1}\" is outdated: ", prefix, args[i - 1]);
                                    result = Functions.CheckFileVersion(prefix + args[i - 1], args[++i]);
                                }
                                else if ((Functions.GetSwitch(args, i, -4).Equals("/K") || Functions.GetSwitch(args, i, -4).Equals("+K")) && Functions.GetSwitch(args, i, -2).Equals("/VALUE"))
                                {
                                    Console.Write("Checking if registry key value version \"{0}\\{1}:{2}\" is outdated: ", root.ToString(), args[i - 3], args[i - 1]);
                                    result = Functions.CheckRegKeyValueVersion(root, args[i - 3], args[i - 1], args[++i]);
                                }

                                if (result < 0)
                                    PrintResult();
                                else if (result == 0 || result == 1)
                                {
                                    PrintResult(false);
                                    flag = 1;
                                }
                                else if (result == 2)
                                    PrintResult(false, 1);
                                else
                                {
                                    PrintResult(false, 2);
                                    flag = 1;
                                }
                            }
                            else
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "/VALUE":
                        if (Functions.CheckParameter("/VALUE", args, i, -2, "/K +K"))
                        {
                            if (Functions.CheckArgument("/VALUE", args, i))
                            {
                                Console.Write("Checking if registry key value \"{0}\\{1}:{2}\" exists: ", root.ToString(), args[i - 1], args[i + 1]);
                                bResult = Functions.CheckRegKeyValueExist(root, args[i - 1], args[++i]);
                                PrintResult(bResult, 1);
                            }
                            else
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "+VALUE":
                        if (Functions.CheckParameter("+VALUE", args, i, -2, "/K +K"))
                        {
                            if (Functions.CheckArgument("+VALUE", args, i))
                            {
                                Console.Write("Checking if registry key value \"{0}\\{1}:{2}\" exists: ", root.ToString(), args[i - 1], args[i + 1]);
                                bResult = Functions.CheckRegKeyValueExist(root, args[i - 1], args[++i]);
                                PrintResult(bResult);
                            }
                            else
                                flag = 1;
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "-VALUE":
                        if (Functions.CheckParameter("-VALUE", args, i, -2, "/K +K"))
                        {
                            if (Functions.CheckArgument("-VALUE", args, i))
                            {
                                Console.Write("Checking if registry key value \"{0}\\{1}:{2}\" does not exist: ", root.ToString(), args[i - 1], args[i + 1]);
                                bResult = !Functions.CheckRegKeyValueExist(root, args[i - 1], args[++i]);
                                PrintResult(bResult);
                            }
                            else
                                flag = 1;
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "/X86":
                        Console.Write("Checking if {0} is running 32-bit: ", Environment.MachineName);
                        bResult = !Environment.Is64BitOperatingSystem;
                        PrintResult(bResult);
                        if(!bResult)
                            flag = 1;
                        break;
                    case "/X64":
                        Console.Write("Checking if {0} is running 64-bit: ", Environment.MachineName);
                        bResult = Environment.Is64BitOperatingSystem;
                        PrintResult(bResult);
                        if (!bResult)
                            flag = 1;
                        break;
                    case "+CN":
                        if (Functions.CheckArgument("+CN", args, i))
                        {
                            Console.WriteLine("Checking if {0} is contained in the following list:" + Environment.NewLine, Environment.MachineName);
                            bResult = Functions.CheckComputerName(args[++i]);
                            Console.Write(Environment.NewLine + "Computer {0} is contained in the list above: ", Environment.MachineName);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "-CN":
                        if (Functions.CheckArgument("-CN", args, i))
                        {
                            Console.WriteLine("Checking if {0} is not contained in the following list:" + Environment.NewLine, Environment.MachineName);
                            bResult = !Functions.CheckComputerName(args[++i]);
                            Console.Write(Environment.NewLine + "Computer {0} is not contained in the list above: ", Environment.MachineName);
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "+SG":
                        if (Functions.CheckArgument("+SG", args, i))
                        {
                            Console.WriteLine("Checking if {0} is a member of any of the following security groups:" + Environment.NewLine, Environment.MachineName);
                            bResult = Functions.CheckComputerSecurityGroup(Environment.MachineName, args[++i], "campus");
                            Console.Write(Environment.NewLine + "Member of any security groups above: ");
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    case "-SG":
                        if (Functions.CheckArgument("-SG", args, i))
                        {
                            Console.WriteLine("Checking if {0} is not a member of any of the following security group:" + Environment.NewLine, Environment.MachineName);
                            bResult = !Functions.CheckComputerSecurityGroup(Environment.MachineName, args[++i], "campus");
                            Console.Write(Environment.NewLine + "Not a member of all security groups above: ");
                            PrintResult(bResult);
                            if (!bResult)
                                flag = 1;
                        }
                        else
                            flag = 1;
                        break;
                    default:
                        Console.WriteLine(Environment.NewLine + "Unknown switch: " + arg.ToUpper());
                        flag = 1;
                        break;
                }

                if (flag != 0)
                {
                    Console.WriteLine(Environment.NewLine + "Result: FAILED");
                    return 1;
                }
            }

            Console.WriteLine(Environment.NewLine + "Result: SUCCESS");
            return 0;
        }
    }
}
