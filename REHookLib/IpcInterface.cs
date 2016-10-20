using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace REHookLib
{
    public class IpcInterface : MarshalByRefObject
    {
        public void IsInstalled(Int32 InClientPID)
        {
            Console.WriteLine("REHook has been installed in target {0}.\r\n", InClientPID);
        }

        public void OnOpenFileMapping(Int32 InClientPID, String[] InFileNames)
        {
            Debug.WriteLine("OnOpenFileMapping code");
        }

        public void ReportException(Exception InInfo)
        {
            Console.WriteLine("The target process has reported an error:\r\n" + InInfo.ToString());
        }

        public void Ping()
        {
        }
    }
}
