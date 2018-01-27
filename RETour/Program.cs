using EasyHook;
using REHookLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Runtime.InteropServices;

namespace RETour
{
    public class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private static string _targetExe = "";
        private static String ChannelName;

        static void Main(string[] args)
        {
            _targetExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), args[0]);
            Int32 targetPID = 0;
            try
            {
                if(!File.Exists(_targetExe))
                {
                    Console.WriteLine(String.Format("{0} not found !", _targetExe));
                    Console.WriteLine("Press any key to exit... ");
                    Console.ReadKey();
                    return;
                }

                RemoteHooking.IpcCreateServer<IpcInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);
                string injectionLibrary = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "REHookLib.dll");
                RemoteHooking.CreateAndInject(_targetExe, "", 0, InjectionOptions.DoNotRequireStrongName, injectionLibrary, injectionLibrary, out targetPID, ChannelName);
                Console.WriteLine("Resident Evil successfully launched.");

                ShowWindow(GetConsoleWindow(), SW_HIDE);

                string processName = Path.GetFileNameWithoutExtension(_targetExe);

                while (Process.GetProcessesByName(processName).Length >= 1)
                {
                    Thread.Sleep(2000);
                }

                Environment.Exit(0);
            }
            catch (Exception exception)
            {
                ShowWindow(GetConsoleWindow(), SW_SHOW);

                Console.WriteLine("Unexpected error occurred while launching Resident Evil \r\n{0}", exception.ToString());
                Console.WriteLine("Press any key to exit... ");
                Console.ReadKey();
            }
        }
    }
}
