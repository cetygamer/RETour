using EasyHook;
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

        #region mmioOpenW
        LocalHook _mmioOpenLocalHook;

        private static IntPtr MmioOpenHookMethod(
          [MarshalAs(UnmanagedType.LPWStr)]string szFilename,
          IntPtr lpmmioinfo,
          int dwOpenFlags)
        {
            if(CorrectFilePath(szFilename) != szFilename)
                MessageBox.Show("mmioOpenW (WINMM.DLL) modif appel, paramètre szFilename :" + szFilename + "-> " + CorrectFilePath(szFilename));
            return mmioOpenW(CorrectFilePath(szFilename), lpmmioinfo, dwOpenFlags);
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true, CharSet = CharSet.Unicode)]
        public delegate IntPtr MmioOpenDelegate(
          [MarshalAs(UnmanagedType.LPWStr)]string szFilename,
          IntPtr lpmmioinfo,
          int dwOpenFlags);

        [DllImport("WINMM.DLL", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern IntPtr mmioOpenW(
           [MarshalAs(UnmanagedType.LPWStr)]string szFilename,
           IntPtr lpmmioinfo,
           int dwOpenFlags);

        #endregion mmioOpenW

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
            if (CorrectFilePath(filename) != filename)
                MessageBox.Show("mmioOpenW (WINMM.DLL) modif appel, paramètre szFilename :" + filename + "-> " + CorrectFilePath(filename));
            return CreateFile(
                CorrectFilePath(filename),
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
                IntPtr createFileProcAddress = LocalHook.GetProcAddress("kernel32.dll", "CreateFileA");

                _createFileLocalHook = LocalHook.Create(
                    createFileProcAddress,
                    new CreateFileDelegate(CreateFileHookMethod),
                    this);

                _createFileLocalHook.ThreadACL.SetExclusiveACL(new int[] { 0 });

                IntPtr mmioOpenProcAddress = LocalHook.GetProcAddress("WINMM.dll", "mmioOpenW");

                _mmioOpenLocalHook = LocalHook.Create(
                    mmioOpenProcAddress,
                    new MmioOpenDelegate(MmioOpenHookMethod),
                    this);

                _mmioOpenLocalHook.ThreadACL.SetExclusiveACL(new int[] { 0 });
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
