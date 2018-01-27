using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

namespace ResidentEvilLauncher
{
    public partial class MainWindow : Window
    {
        public DelegateCommand PlayCommand { get; private set; }
        public DelegateCommand ControlsCommand { get; private set; }
        public DelegateCommand OptionsCommand { get; private set; }
        public DelegateCommand QuitCommand { get; private set; }

        public MainWindow()
        {
            PlayCommand = new DelegateCommand(PlayCommand_Execute);
            ControlsCommand = new DelegateCommand(ControlsCommand_Execute);
            OptionsCommand = new DelegateCommand(OptionsCommand_Execute);
            QuitCommand = new DelegateCommand(QuitCommand_Execute);
            InitializeComponent();
        }

        private void PlayCommand_Execute(object parameters)
        {
            try
            {
                string workDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if (Properties.Settings.Default.FirstRun && HasAMDGPU())
                {
                    Properties.Settings.Default.FirstRun = false;
                    Properties.Settings.Default.DdrawCompatForce = true;
                    Properties.Settings.Default.Save();
                }

                if (Properties.Settings.Default.DdrawCompatForce == false)
                {
                    if (File.Exists(Path.Combine(workDir, "ddraw.dll")))
                    {
                        System.IO.File.Delete(Path.Combine(workDir, "ddraw.dll"));
                    }
                }
                else
                {
                    try
                    {
                        System.IO.File.Copy(Path.Combine(workDir, "ddrawCompat", "ddraw.dll"), Path.Combine(workDir, "ddraw.dll"), true);
                    }
                    catch
                    {

                    }
                }

                string exeName = "RESIDENTEVIL.EXE";

                if (IsEnhancedResolutionModeEnabled())
                {
                    exeName = "RESIDENTEVIL-1024.EXE";
                }

                string residentEvilExecutablePath = Path.Combine(workDir, exeName);
                if (File.Exists(residentEvilExecutablePath))
                {
                    ProcessStartInfo psInfo = new ProcessStartInfo(residentEvilExecutablePath);

                    string RETourExecutablePath = Path.Combine(workDir, "RESIDENT_EVIL.exe");
                    if (File.Exists(RETourExecutablePath))
                    {
                        psInfo = new ProcessStartInfo(RETourExecutablePath, Path.GetFileName(residentEvilExecutablePath));
                    }
                    psInfo.UseShellExecute = true;
                    MessageBox.Show("Do not Alt-Tab during the game, otherwise you will have to restart the game");
                    Process.Start(psInfo);
                }
                else
                {
                    MessageBox.Show(String.Format("{0} not found.", residentEvilExecutablePath));
                }
            }
            catch
            {

            }
        }

        private bool HasAMDGPU()
        {
            string gpuName = GetWMIInformation("Win32_VideoController"); //Windows 7 and newer
            string secondGpuName = GetWMIInformation("Win32_DisplayConfiguration"); //pre-Windows 7

            bool amdGPUFound = (gpuName.Contains("ATI ") || gpuName.Contains("AMD ") || gpuName.Contains(" Radeon"));
            if(amdGPUFound == false)
            {
                amdGPUFound = (secondGpuName.Contains("ATI ") || secondGpuName.Contains("AMD ") || secondGpuName.Contains(" Radeon"));
            }

            return amdGPUFound;
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/aa394512(v=vs.85).aspx
        /// https://msdn.microsoft.com/en-us/library/aa394137(v=vs.85).aspx
        /// </summary>
        private string GetWMIInformation(string wmiClass)
        {
            string value = "";

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(String.Format("SELECT * FROM {0}", wmiClass));

            foreach (ManagementObject mo in searcher.Get())
            {
                foreach (PropertyData property in mo.Properties)
                {
                    if (property.Name == "Description" || property.Name == "DeviceName" || property.Name == "Name" || property.Name == "VideoProcessor")
                    {
                        value = String.Format("{0} {1}", value, property.Value);
                    }
                }
            }

            return value;
        }

        private bool IsEnhancedResolutionModeEnabled()
        {
            return Properties.Settings.Default.EnhancedResolutionMode == true;
        }

        private void ControlsCommand_Execute(object parameters)
        {
            var controlsWindow = new ControlsWindow();
            controlsWindow.Owner = this;
            controlsWindow.ShowDialog();
        }

        private void OptionsCommand_Execute(object parameters)
        {
            var optionsWindow = new OptionsWindow();
            optionsWindow.Owner = this;
            optionsWindow.ShowDialog();
        }

        public void QuitCommand_Execute(object parameters)
        {
            Application.Current.Shutdown();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
