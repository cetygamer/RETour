using EasyHook;
using REHookLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;

namespace RETour
{
    public class Program
    {
        private static string _targetExe = @"C:\Jeux\RESIDENT EVIL\RESIDENTEVIL.EXE";
        private static String ChannelName;

        static void Main()
        {
            Int32 targetPID = 0;
            try
            {
                RemoteHooking.IpcCreateServer<IpcInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);

                string injectionLibrary = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "REHookLib.dll");
                RemoteHooking.CreateAndInject(_targetExe, "", 0, InjectionOptions.DoNotRequireStrongName, injectionLibrary, injectionLibrary, out targetPID, ChannelName);
                Console.WriteLine("Created and injected process {0}", targetPID);
                Console.WriteLine("<Press any key to exit>");
                Console.ReadKey();
            }
            catch (Exception exception)
            {
                Debug.WriteLine("There was an error while connecting to target:\r\n{0}", exception.ToString());
                Debug.WriteLine("<Press any key to exit>");
            }
        }
    }
}
