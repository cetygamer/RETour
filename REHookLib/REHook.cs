using EasyHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace REHookLib
{
    public class REHook : IEntryPoint
    {
        IpcInterface IpcInterface;
        LocalHook OpenFileMappingLocalHook;
        Stack<String> Queue = new Stack<String>();

        /*public REHook(EasyHook.InjectionLoader injectionloader, System.String entryInfo)
        {

        }*/


        // The matching delegate for OpenFileMapping
        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        delegate IntPtr OpenFileMappingDelegate(
            uint dwDesiredAccess,
            bool bInheritHandle,
            string lpName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr OpenFileMapping(
            [In] uint dwDesiredAccess,
            [In] bool bInheritHandle,
            [In] string lpName);

        public static IntPtr OpenFileMappingHooked(uint dwDesiredAccess,
            bool bInheritHandle,
            string lpName)
        {
            Console.Write("...intercepted...");

            try
            {
                REHook This = (REHook)HookRuntimeInfo.Callback;

                lock (This.Queue)
                {
                    //change parameters
                }
            }
            catch
            {
            }

            //Call original API
            return OpenFileMapping(dwDesiredAccess, bInheritHandle, lpName);
        }

        public REHook(
            RemoteHooking.IContext InContext,
            String InChannelName)
        {
            // connect to host...
            IpcInterface = RemoteHooking.IpcConnectClient<IpcInterface>(InChannelName);

            IpcInterface.Ping();
        }

        public void Run(
            RemoteHooking.IContext InContext,
            String InChannelName)
        {
            // install hook...
            try
            {

                OpenFileMappingLocalHook = LocalHook.Create(
                    LocalHook.GetProcAddress("kernel32.dll", "OpenFileMappingA"),
                    new OpenFileMappingDelegate(OpenFileMappingHooked),
                    this);

                OpenFileMappingLocalHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            }
            catch (Exception ExtInfo)
            {
                IpcInterface.ReportException(ExtInfo);

                return;
            }

            IpcInterface.IsInstalled(RemoteHooking.GetCurrentProcessId());

            RemoteHooking.WakeUpProcess();

            // wait for host process termination...
            try
            {
                while (true)
                {
                    Thread.Sleep(100);

                    // transmit newly monitored file accesses...
                    if (Queue.Count > 0)
                    {
                        String[] Package = null;

                        lock (Queue)
                        {
                            Package = Queue.ToArray();

                            Queue.Clear();
                        }

                        IpcInterface.OnOpenFileMapping(RemoteHooking.GetCurrentProcessId(), Package);
                    }
                    else
                        IpcInterface.Ping();
                }
            }
            catch
            {
                // Ping() will raise an exception if host is unreachable
            }

            /*create exit delegates for when the process has exited
            _reProcess.EnableRaisingEvents = true;

            //Start Process in background (managed)
            _reProcess = PrepareREProcess();
            //Install hook
            Console.WriteLine("\nInstalling local hook for kernel32!OpenFileMapping");
            // Create the local hook using our MessageBeepDelegate and MessageBeepHook function
            using (var hook = EasyHook.LocalHook.Create(
                    EasyHook.LocalHook.GetProcAddress("kernel32.dll", "OpenFileMappingA"),
                    new OpenFileMappingDelegate(OpenFileMappingHook),
                    null))
            {
                hook.ThreadACL.SetExclusiveACL(null);

                int processId = 0;

                RemoteHooking.CreateAndInject(_reProcess.StartInfo.FileName, "", 0, System.Reflection.Assembly.GetExecutingAssembly().Location, System.Reflection.Assembly.GetExecutingAssembly().Location, out processId, new object[0]);
                RemoteHooking.WakeUpProcess();

                ProcessThread reMainThread = GetUIThread(_reProcess);

                hook.ThreadACL.SetInclusiveACL(new int[] { reMainThread.Id });
                Console.Write("\nPress <enter> to disable hook for current thread:");
                Console.ReadLine();
                Console.WriteLine("\nDisabling hook for current thread.");
                hook.ThreadACL.SetExclusiveACL(new int[] { reMainThread.Id });

                Console.Write("\nPress <enter> to uninstall hook and exit.");
                Console.ReadLine();
            } // hook.Dispose() will uninstall the hook for us

            //Change parameters if need be

            //use original with changed parameters
            */
        }
    }
}
