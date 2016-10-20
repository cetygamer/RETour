using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using EasyHook;
using System.Runtime.Remoting;
using REHookLib;
using System.Threading;

namespace RETour
{
    public class Program
    {
        private static Process _reProcess;
        private static string _targetExe = @"C:\Jeux\RESIDENTEVIL\RESIDENTEVIL.EXE";

        private static String ChannelName;

        private static void PrepareREProcess()
        {
            _reProcess = new Process();
            var reStartInfo = new ProcessStartInfo();
            reStartInfo.UseShellExecute = true;
            reStartInfo.FileName = _targetExe;

            _reProcess.StartInfo = reStartInfo;
            _reProcess.Start();
        }

        private static ProcessThread GetUIThread(Process proc)
        {
            if (proc.MainWindowHandle == null) return null;
            int id = GetWindowThreadProcessId(proc.MainWindowHandle, IntPtr.Zero);
            foreach (ProcessThread pt in proc.Threads)
                if (pt.Id == id) return pt;
            return null;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, IntPtr procid);


        static void Main()
        {

            Int32 targetPID = 0;
            /*PrepareREProcess();
            targetPID = _reProcess.Id;
            */

            bool useExistingProc = targetPID != 0;

            try
            {
                RemoteHooking.IpcCreateServer<IpcInterface>(ref ChannelName, WellKnownObjectMode.SingleCall);

                string injectionLibrary = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "REHookLib.dll");
                if (useExistingProc)
                {
                    RemoteHooking.Inject(
                        targetPID,
                        injectionLibrary,
                        injectionLibrary,
                        ChannelName);

                    Console.WriteLine("Injected to process {0}", targetPID);
                }
                else
                {
                    RemoteHooking.CreateAndInject(_targetExe, "", 0, InjectionOptions.DoNotRequireStrongName, injectionLibrary, injectionLibrary, out targetPID, ChannelName);
                    Console.WriteLine("Created and injected process {0}", targetPID);
                }
                Console.WriteLine("<Press any key to exit>");
                Console.ReadKey();
            }
            catch (Exception ExtInfo)
            {
                Debug.WriteLine("There was an error while connecting to target:\r\n{0}", ExtInfo.ToString());
                Debug.WriteLine("<Press any key to exit>");
                _reProcess.Kill();
            }
        }
    }
}
