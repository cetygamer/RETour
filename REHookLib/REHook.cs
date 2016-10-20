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
        IpcInterface _ipcInterface;


        #region OpenFileMapping
        LocalHook _openFileMappingLocalHook;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        delegate IntPtr OpenFileMappingDelegate(uint dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr OpenFileMapping([In] uint dwDesiredAccess, [In] bool bInheritHandle, [In] string lpName);

        public static IntPtr OpenFileMappingHookMethod(uint dwDesiredAccess, bool bInheritHandle, string lpName)
        {
            Console.Write("...intercepted OpenFileMappingA...");
            return OpenFileMapping(dwDesiredAccess, bInheritHandle, lpName);
        }
        #endregion OpenFileMapping

        public REHook(RemoteHooking.IContext inContext, String inChannelName)
        {
            // connect to host...
            _ipcInterface = RemoteHooking.IpcConnectClient<IpcInterface>(inChannelName);
            _ipcInterface.Ping();
        }

        public void Run(RemoteHooking.IContext inContext, String inChannelName)
        {
            // install hook...
            try
            {
                _openFileMappingLocalHook = LocalHook.Create(
                    LocalHook.GetProcAddress("kernel32.dll", "OpenFileMappingA"),
                    new OpenFileMappingDelegate(OpenFileMappingHookMethod),
                    this);

                _openFileMappingLocalHook.ThreadACL.SetExclusiveACL(new Int32[] { 0 });
            }
            catch (Exception ExtInfo)
            {
                _ipcInterface.ReportException(ExtInfo);

                return;
            }

            _ipcInterface.NotifySucessfulInstallation(RemoteHooking.GetCurrentProcessId());

            RemoteHooking.WakeUpProcess();

            // wait for host process termination...
            try
            {
                while (true)
                {
                    _ipcInterface.OnHooking();
                }
            }
            catch
            {
                // Ping() will raise an exception if host is unreachable
            }
        }
    }
}
