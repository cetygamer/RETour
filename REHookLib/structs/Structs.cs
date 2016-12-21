using System;
using System.Runtime.InteropServices;

namespace REHookLib.Structs
{
    public struct MCINativeConstants
    {
        public const string WaveAudio = "waveaudio";

        public const uint MM_MCINOTIFY = 0x3B9;

        public const uint MCI_NOTIFY_SUCCESSFUL = 0x0001;
        public const uint MCI_NOTIFY_SUPERSEDED = 0x0002;
        public const uint MCI_NOTIFY_ABORTED = 0x0004;
        public const uint MCI_NOTIFY_FAILURE = 0x0008;

        public const uint MCI_OPEN = 0x0803;
        public const uint MCI_CLOSE = 0x0804;
        public const uint MCI_PLAY = 0x0806;
        public const uint MCI_SEEK = 0x0807;
        public const uint MCI_STOP = 0x0808;
        public const uint MCI_PAUSE = 0x0809;
        public const uint MCI_RECORD = 0x080F;
        public const uint MCI_RESUME = 0x0855;
        public const uint MCI_SAVE = 0x0813;
        public const uint MCI_LOAD = 0x0850;
        public const uint MCI_STATUS = 0x0814;


        public const uint MCI_SAVE_FILE = 0x00000100;
        public const uint MCI_OPEN_ELEMENT = 0x00000200;
        public const uint MCI_OPEN_TYPE = 0x00002000;
        public const uint MCI_LOAD_FILE = 0x00000100;
        public const uint MCI_STATUS_POSITION = 0x00000002;
        public const uint MCI_STATUS_LENGTH = 0x00000001;
        public const uint MCI_STATUS_ITEM = 0x00000100;

        public const uint MCI_NOTIFY = 0x00000001;
        public const uint MCI_WAIT = 0x00000002;
        public const uint MCI_FROM = 0x00000004;
        public const uint MCI_TO = 0x00000008;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MCI_OPEN_PARMS
    {
        public IntPtr dwCallback;
        public uint wDeviceID;
        public IntPtr lpstrDeviceType;
        public IntPtr lpstrElementName;
        public IntPtr lpstrAlias;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MCI_RECORD_PARMS
    {
        public IntPtr dwCallback;
        public uint dwFrom;
        public uint dwTo;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MCI_PLAY_PARMS
    {
        public IntPtr dwCallback;
        public uint dwFrom;
        public uint dwTo;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MCI_GENERIC_PARMS
    {
        public IntPtr dwCallback;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MCI_SEEK_PARMS
    {
        public IntPtr dwCallback;
        public uint dwTo;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MCI_SAVE_PARMS
    {
        public IntPtr dwCallback;
        public IntPtr lpfilename;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MCI_STATUS_PARMS
    {
        public IntPtr dwCallback;
        public uint dwReturn;
        public uint dwItem;
        public uint dwTrack;
    };

    /*[DllImport("winmm.dll", CharSet = CharSet.Ansi, BestFitMapping = true, ThrowOnUnmappableChar = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool mciGetErrorString(
        uint mcierr,
        [MarshalAs(UnmanagedType.LPStr)]
        System.Text.StringBuilder pszText,
        uint cchText);*/

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MMIOINFO
    {
        int dwFlags;
        int fccIOProc;
        int pIOProc;
        int wErrorRet;
        int htask;
        int cchBuffer;
        [MarshalAs(UnmanagedType.LPStr)]
        string pchBuffer;
        [MarshalAs(UnmanagedType.LPStr)]
        string pchNext;
        [MarshalAs(UnmanagedType.LPStr)]
        string pchEndRead;
        [MarshalAs(UnmanagedType.LPStr)]
        string pchEndWrite;
        long lBufOffset;
        long lDiskOffset;
        int[] adwInfo;
        int dwReserved1;
        int dwReserved2;
        int hmmio;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct STARTUPINFOEX
    {
        public STARTUPINFO StartupInfo;
        public IntPtr lpAttributeList;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct STARTUPINFO
    {
        public Int32 cb;
        public string lpReserved;
        public string lpDesktop;
        public string lpTitle;
        public Int32 dwX;
        public Int32 dwY;
        public Int32 dwXSize;
        public Int32 dwYSize;
        public Int32 dwXCountChars;
        public Int32 dwYCountChars;
        public Int32 dwFillAttribute;
        public Int32 dwFlags;
        public Int16 wShowWindow;
        public Int16 cbReserved2;
        public IntPtr lpReserved2;
        public IntPtr hStdInput;
        public IntPtr hStdOutput;
        public IntPtr hStdError;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct PROCESS_INFORMATION
    {
        public IntPtr hProcess;
        public IntPtr hThread;
        public int dwProcessId;
        public int dwThreadId;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SECURITY_ATTRIBUTES
    {
        public int nLength;
        public IntPtr lpSecurityDescriptor;
        public int bInheritHandle;
    }
}