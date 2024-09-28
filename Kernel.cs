using Cosmos.System.ExtendedASCII;
using Cosmos.System.FileSystem;
using Cosmos.System.FileSystem.VFS;
using Cosmos.System.Graphics;
using Cosmos.System.Network.Config;
using Cosmos.System.Network.IPv4.UDP.DHCP;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Text;
using Sys = Cosmos.System;

namespace GLaDOSOS
{
    public class Kernel : Sys.Kernel
    {
        string cwd = @"0:\";
        char mode = '\0';
        string usn = "user";
        string addr = "";
        CosmosVFS cVFS = new CosmosVFS();
        protected override void BeforeRun() {
            VFSManager.RegisterVFS(cVFS); 
            
            using (var xClient = new DHCPClient()) {
                /** Send a DHCP Discover packet **/
                //This will automatically set the IP config after DHCP response
                xClient.SendDiscoverPacket();
            }
            addr = NetworkConfiguration.CurrentAddress.ToString();
            addr = (addr == "0.0.0.0") ? "localhost" : addr;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Clear();
            Console.WriteLine("Aperture's own:");
            Console.WriteLine("GLaDOSOS b3.1415926535");
            while (mode == '\0') {
                Console.Write("Type C for single-threaded console, T for single-threaded TUI, and G for multi-threaded graphics.\n>");
                mode = Console.ReadKey().KeyChar ;
                if (mode != 'c' && mode != 't' && mode != 'g') {
                    mode = '\0';
                }
            }
            if (mode == 'c') {
                ConsoleRun();
            }
        }

        protected void ConsoleRun() {
            Console.Clear();
            if (!File.Exists(@"0:\__$passwd$__.obe")) {
                File.Create(@"0:\__$passwd$__.obe");
                Console.WriteLine("This system has no accounts. NOW MAKE ONE.");
                Console.Write("Username:");
                usn = Console.ReadLine();
                Console.Write("Password:");
                var str = $"\n{usn}|{Console.ReadLine()}\n";
                File.AppendAllText(@"0:\__$passwd$__.obe", str);
            } else {
                Console.Write("Login\nUsername:");
                usn = $"{Console.ReadLine()}";
                Console.Write("Password:");
                var loginstr = $"\n{usn}|{Console.ReadLine()}\n";
                if (File.ReadAllText(@"0:\__$passwd$__.obe").IndexOf("loginstr") == -1){
                    Console.WriteLine("Your username or password is wrong. Shutting system down once you press a key...");
                    Console.ReadKey(true);
                    if (addr != "localhost") {
                        using (DHCPClient xClient = new DHCPClient()) {
                            xClient.SendReleasePacket();
                        }
                    }
                    Sys.Power.Shutdown();
                }
            }
            Console.Clear();
            Console.WriteLine("\nAperture's own:");
            Console.WriteLine("GLaDOSOS b3.1415926535 (sorry it'sn't orange!)");
            while (true) {
                Console.Write($"{cwd}|{usn}@{addr}>");
                var cmd = Console.ReadLine().Split(" ");
            }
        }
        protected void cmd_r(string[] argv){

        }

        protected override void Run() {
        }
    }
}
