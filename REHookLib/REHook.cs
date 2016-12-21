using EasyHook;
using REHookLib.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Linq;
using System.Windows.Forms;

namespace REHookLib
{
    public class REHook : IEntryPoint
    {
        IpcInterface _ipcInterface;
        static String _lastCorrectedMovieFilename;

        #region MCISendCommand
        LocalHook _mciSendCommandLocalHook;

        [DllImport("winmm.dll", CharSet = CharSet.Ansi, BestFitMapping = true, ThrowOnUnmappableChar = true, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern uint MciSendCommand(
                uint mciId,
                uint uMsg,
                uint dwParam1,
                IntPtr dwParam2);

        public static uint MciSendCommandHookMethod(
                uint mciId,
                uint uMsg,
                uint dwParam1,
                IntPtr dwParam2)
        {
            return MciSendCommand(
                mciId,
                uMsg,
                dwParam1,
                dwParam2);
        }

        public delegate uint MciSendCommandDelegate(
                uint mciId,
                uint uMsg,
                uint dwParam1,
                IntPtr dwParam2);

        #endregion MCISendCommand

        #region MmioOpen
        LocalHook _mmioOpenLocalHook;


        private static IntPtr MmioOpenHookMethod(
          [MarshalAs(UnmanagedType.LPStr)]string szFilename,
          MMIOINFO lpmmioinfo,
          int dwOpenFlags)
        {
            return MmioOpen(_lastCorrectedMovieFilename, new MMIOINFO(), dwOpenFlags);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        public delegate IntPtr MmioOpenDelegate(
          [MarshalAs(UnmanagedType.LPStr)]string szFilename,
          MMIOINFO lpmmioinfo,
          int dwOpenFlags);

        [DllImport("WINMM.DLL", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr MmioOpen(
           [MarshalAs(UnmanagedType.LPStr)]string szFilename,
           MMIOINFO lpmmioinfo,
           int dwOpenFlags);

        #endregion MmioOpen

        #region CreateFile
        LocalHook _createFileLocalHook;

        public static IntPtr CreateFileHookMethod(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile)
        {
            _lastCorrectedMovieFilename = "";
            _lastCorrectedMovieFilename = CorrectFilePath(filename);

            return CreateFile(
                _lastCorrectedMovieFilename,
                access,
                share,
                securityAttributes,
                creationDisposition,
                flagsAndAttributes,
                templateFile);
        }

        /// <summary>
        /// E:\horr\FRA\movie\CAPCOM.AVI devient C:\Jeux\RESIDENT EVIL\FRA\MOVIE\CAPCOM.AVI
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string CorrectFilePath(string filename)
        {
            string correctedString = filename;
            if (correctedString.Contains("MOVIE") == false)
            {
                return correctedString;
            }
            else
            {
                correctedString = Path.Combine(@"C:\Jeux\RESIDENT EVIL\FRA\MOVIE", Path.GetFileName(correctedString));
            }

            return correctedString;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        public delegate IntPtr CreateFileDelegate(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern IntPtr CreateFile(
            [MarshalAs(UnmanagedType.LPStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes,
            IntPtr templateFile);

        #endregion CreateFile

        #region CreateProcess
        LocalHook _createProcessLocalHook;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Ansi)]
        public delegate bool CreateProcessDelegate(
          string lpApplicationName,
          string lpCommandLine,
          ref SECURITY_ATTRIBUTES lpProcessAttributes,
          ref SECURITY_ATTRIBUTES lpThreadAttributes,
          bool bInheritHandles,
          uint dwCreationFlags,
          IntPtr lpEnvironment,
          string lpCurrentDirectory,
          [In] ref STARTUPINFO lpStartupInfo,
          out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CreateProcess(
          string lpApplicationName,
          string lpCommandLine,
          ref SECURITY_ATTRIBUTES lpProcessAttributes,
          ref SECURITY_ATTRIBUTES lpThreadAttributes,
          bool bInheritHandles,
          uint dwCreationFlags,
          IntPtr lpEnvironment,
          string lpCurrentDirectory,
          [In] ref STARTUPINFO lpStartupInfo,
          out PROCESS_INFORMATION lpProcessInformation);

        public static bool CreateProcessHookMethod(string lpApplicationName,
          string lpCommandLine,
          ref SECURITY_ATTRIBUTES lpProcessAttributes,
          ref SECURITY_ATTRIBUTES lpThreadAttributes,
          bool bInheritHandles,
          uint dwCreationFlags,
          IntPtr lpEnvironment,
          string lpCurrentDirectory,
          [In] ref STARTUPINFO lpStartupInfo,
          out PROCESS_INFORMATION lpProcessInformation)
        {
            lpProcessInformation = new PROCESS_INFORMATION();

            return CreateProcess(lpApplicationName, lpCommandLine,
              ref lpProcessAttributes,
              ref lpThreadAttributes,
              bInheritHandles,
              dwCreationFlags,
              lpEnvironment,
              lpCurrentDirectory,
              ref lpStartupInfo,
              out lpProcessInformation);
        }
        #endregion CreateProcess
        
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
                IntPtr createProcessProcAddress = LocalHook.GetProcAddress("kernel32.dll", "CreateProcessA");

                _createProcessLocalHook = LocalHook.Create(
                    createProcessProcAddress,
                    new CreateProcessDelegate(CreateProcessHookMethod),
                    this);

                _createProcessLocalHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                

                IntPtr createFileProcAddress = LocalHook.GetProcAddress("kernel32.dll", "CreateFileA");

                _createFileLocalHook = LocalHook.Create(
                    createFileProcAddress,
                    new CreateFileDelegate(CreateFileHookMethod),
                    this);

                _createFileLocalHook.ThreadACL.SetExclusiveACL(new int[] { 0 });

                /*IntPtr mmioOpenProcAddress = LocalHook.GetProcAddress("WINMM.dll", "mmioOpenA");

                _mmioOpenLocalHook = LocalHook.Create(
                    mmioOpenProcAddress,
                    new MmioOpenDelegate(MmioOpenHookMethod),
                    this);

                _mmioOpenLocalHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                */

                /*IntPtr mciSendCommandProcAddress = LocalHook.GetProcAddress("WINMM.dll", "mciSendCommandA");

                _mciSendCommandLocalHook = LocalHook.Create(
                    mciSendCommandProcAddress,
                    new MciSendCommandDelegate(MciSendCommandHookMethod),
                    this);

                _mciSendCommandLocalHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
                */
            }
            catch (Exception exception)
            {
                _ipcInterface.ReportException(exception);

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
