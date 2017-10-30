using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Management;
using System.DirectoryServices;
using System.Collections;

namespace BadRabbitVaccine
{
    class Program
    {
        static string[] VaccineFiles = new string[]
        {
            @"C:\WINDOWS\infpub.dat",
            @"C:\WINDOWS\cscc.dat",
            @"C:\WINDOWS\dispci.exe"
        };

        static void Main(string[] args)
        {
            Console.Title = "BadRabbit Vaccine";

            Console.WriteLine("BADRABBIT VACCINE");
            Console.WriteLine("=================");
            Console.WriteLine("Written and provided by rGunti, 2017");
            Console.WriteLine("Icon by icons8.com");
            Console.WriteLine("");

            Console.WriteLine("This application will \"vaccinate\" your computer");
            Console.WriteLine("from the BadRabbit Crypto Trojan by creating the ");
            Console.WriteLine("following files on your harddrive and removing");
            Console.WriteLine("all permissions from them so they cannot be ");
            Console.WriteLine("accessed anymore:");
            Console.WriteLine("");
            foreach (string file in VaccineFiles)
                Console.WriteLine($" - {file}");
            Console.WriteLine("");
            Console.WriteLine("This will force the malware to shutdown and not ");
            Console.WriteLine("touch your system. Note that you WILL also NOT ");
            Console.WriteLine("able to access or remove these files anymore!");
            Console.WriteLine("");

            Console.Write("[?] Press any key to continue");
            Console.ReadKey();
            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("BADRABBIT VACCINE: THE LEGAL STUFF!");
            Console.WriteLine("===================================");
            Console.WriteLine("");
            Console.WriteLine("I want to be very clear with you: I am not a security expert. " +
                "This is a known method to prevent said crypto-troyan from working as of October 2017." +
                "If this method is not working for you I cannot help you with that!");
            Console.WriteLine("Therefore this application is provided under the following license:");
            Console.WriteLine("");
            Console.WriteLine("MIT License");
            Console.WriteLine("© 2017 rGunti");
            Console.WriteLine("");
            Console.WriteLine("Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the \"Software\"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:");
            Console.WriteLine("");
            Console.WriteLine("The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.");
            Console.WriteLine("");
            Console.WriteLine("THE SOFTWARE IS PROVIDED \"AS IS\", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.");
            Console.WriteLine("");

            bool noValidInputDetected = true;
            do
            {
                Console.Write("[?] Do you agree to these Terms and Conditions? [Y/N] ");
                switch (Console.ReadKey().KeyChar)
                {
                    case 'y':
                    case 'Y':
                        Console.WriteLine("");
                        noValidInputDetected = false;
                        break;
                    case 'n':
                    case 'N':
                        Console.WriteLine("\n[i] No actions will be taken and the application will terminate.");
                        Console.WriteLine("    Press any key to quit!");
                        Console.ReadKey();
                        return;
                    default:
                        Console.WriteLine("");
                        break;
                }
            } while (noValidInputDetected);

            // Create Files
            Console.WriteLine("[ ] Creating Vaccine Files ...");
            foreach (string file in VaccineFiles)
            {
                if (!File.Exists(file)) {
                    noValidInputDetected = true;
                    do
                    {
                        Console.Write($"[?] File \"{file}\" already exists. Overwrite it? [Y/N] ");
                        switch (Console.ReadKey().KeyChar)
                        {
                            case 'y':
                            case 'Y':
                                Console.WriteLine("");
                                noValidInputDetected = false;
                                break;
                            case 'n':
                            case 'N':
                                Console.WriteLine($"\n[i] Skipped \"{file}\"");
                                continue;
                            default:
                                Console.WriteLine("");
                                break;
                        }
                    } while (noValidInputDetected);
                }

                try
                {
                    Console.WriteLine($"[ ] Creating Vaccine File \"{file}\" ...");
                    File.WriteAllText(file, "VACCINATED! NO 1337 CRYPTO-TROJANS FOR YOU!");
                } catch (Exception ex) {
                    Console.WriteLine($"[!] Could not create file \"{file}\" because of an {ex.GetType().Name}.");
                    Console.WriteLine($"    Perhaps you have already vaccinated this machine?");
                }
            }

            noValidInputDetected = true;
            do
            {
                Console.Write("[?] Remove all permissions from Vaccine files? [Y/N] ");
                switch (Console.ReadKey().KeyChar)
                {
                    case 'y':
                    case 'Y':
                        Console.WriteLine("\n[ ] Removing all permissions from vaccine files.");
                        foreach (string file in VaccineFiles)
                        {
                            try
                            {
                                RemoveAllRules(file);
                            } catch (Exception ex)
                            {
                                Console.WriteLine($"[!] Could not modify permissions on file \"{file}\" because of an {ex.GetType().Name}.");
                                Console.WriteLine($"    Perhaps you have already vaccinated this machine?");
                            }
                        }   
                        noValidInputDetected = false;
                        break;
                    case 'n':
                    case 'N':
                        Console.WriteLine("\n[ ] File Permissions not created, vaccine will not be effective.");
                        noValidInputDetected = false;
                        break;
                    case 'l':
                    case 'L':
                        Console.WriteLine("\n[ ] Listing all current file permissions ...");
                        foreach (string file in VaccineFiles)
                            ListRules(file);
                        break;
                    default:
                        Console.WriteLine("");
                        break;
                }
            } while (noValidInputDetected);

            // Wait for closure
            Console.Write(" === APPLICATION HAS ENDED, PRESS ANY KEY TO QUIT === ");
            Console.ReadKey();
        }

        static void IdentitiyInfo()
        {
            var myIdentity = WindowsIdentity.GetCurrent();
            Console.WriteLine("Current Windows Identity:");
            PrintIdentity(myIdentity);

            Console.WriteLine("\nRegistered Users:");

            DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName);
            DirectoryEntry admGroup = localMachine.Children.Find("users", "group");
            object members = admGroup.Invoke("members", null);
            foreach (object groupMember in (IEnumerable)members)
            {
                DirectoryEntry member = new DirectoryEntry(groupMember);
                PrintDirectoryEntry(member);
            }
        }

        static void PrintIdentity(WindowsIdentity identity)
        {
            string print = $"{identity.Name,-30} " +
                (identity.IsAnonymous ? 'A' : ' ') +
                (identity.IsAuthenticated ? 'U' : ' ') +
                (identity.IsGuest ? 'G' : ' ') +
                (identity.IsSystem ? 'S' : ' ');
            Console.WriteLine(print);
        }

        static void PrintDirectoryEntry(DirectoryEntry entry)
        {
            string print = $"{entry.Name,-30} ";
            try { print += $"{entry.Guid}"; }
            catch { print += "<no GUID>"; }
            Console.WriteLine(print);
        }

        static void ListRules(String path)
        {
            FileInfo file = new FileInfo(path);
            FileSecurity fileSecurity = file.GetAccessControl();

            Console.WriteLine($"    Permissions for File \"{path}\"");
            AuthorizationRuleCollection rules = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));
            foreach (FileSystemAccessRule rule in rules)
            {
                Console.WriteLine($"     - {rule.IdentityReference.Value} : {rule.FileSystemRights}");
            }
        }

        static void RemoveAllRules(String path)
        {
            FileInfo file = new FileInfo(path);
            FileSecurity fileSecurity = file.GetAccessControl();
            fileSecurity.SetAccessRuleProtection(true, false);

            Console.WriteLine($"[ ] Removing Permissions from file \"{path}\" ...");
            AuthorizationRuleCollection rules = fileSecurity.GetAccessRules(true, true, typeof(NTAccount));
            foreach (FileSystemAccessRule rule in rules)
            {
                Console.WriteLine($"     - {path} : {rule.IdentityReference.Value} ({rule.FileSystemRights})");
                fileSecurity.RemoveAccessRule(rule);
            }

            Console.WriteLine($"[ ] Commiting changes on \"{path}\" ...");
            file.SetAccessControl(fileSecurity);
        }
    }
}
