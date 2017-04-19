using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Navigation;

namespace ResidentEvilLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            string exeName = "RESIDENTEVIL.EXE";
            string workDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if(IsEnhancedResolutionModeEnabled())
            {
                exeName = "RESIDENTEVIL-1024.EXE";
            }
            string path = Path.Combine(workDir, exeName);
            if(File.Exists(path))
            {
                ProcessStartInfo psInfo = new ProcessStartInfo(path);
                psInfo.UseShellExecute = true;
                MessageBox.Show("Ne pas faire Alt-Tab en cours de jeu, sous peine de devoir redémarrer le jeu");
                Process.Start(psInfo);
            }
            else
            {
                MessageBox.Show(String.Format("{0} introuvable.", path));
            }
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
