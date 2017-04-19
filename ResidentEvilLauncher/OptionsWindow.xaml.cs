using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ResidentEvilLauncher
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public bool OptionEnhanceModeEnabled
        {
            get { return (bool)GetValue(OptionEnhanceModeEnabledProperty); }
            set { SetValue(OptionEnhanceModeEnabledProperty, value); }
        }
        public static readonly DependencyProperty OptionEnhanceModeEnabledProperty =
            DependencyProperty.Register("OptionEnhanceModeEnabled", typeof(bool), typeof(OptionsWindow), new PropertyMetadata(Properties.Settings.Default.EnhancedResolutionMode));

        public bool OptionEnhanceModeDisabled
        {
            get { return (bool)GetValue(OptionEnhanceModeDisabledProperty); }
            set { SetValue(OptionEnhanceModeDisabledProperty, value); }
        }
        public static readonly DependencyProperty OptionEnhanceModeDisabledProperty =
            DependencyProperty.Register("OptionEnhanceModeDisabled", typeof(bool), typeof(OptionsWindow), new PropertyMetadata(true));

        public DelegateCommand SavePrefs { get; private set; }
        public DelegateCommand CancelPrefs { get; private set; }
        public DelegateCommand DisableEnhanceMode { get; private set; }
        public DelegateCommand EnableEnhanceMode { get; private set; }

        public OptionsWindow()
        {
            SavePrefs = new DelegateCommand(SavePrefs_Execute);
            CancelPrefs = new DelegateCommand(CancelPrefs_Execute);
            DisableEnhanceMode = new DelegateCommand(DisableEnhanceMode_Execute);
            EnableEnhanceMode = new DelegateCommand(EnableEnhanceMode_Execute);
            SetCurrentValue(OptionEnhanceModeEnabledProperty, Properties.Settings.Default.EnhancedResolutionMode);
            SetCurrentValue(OptionEnhanceModeDisabledProperty, !Properties.Settings.Default.EnhancedResolutionMode);

            InitializeComponent();
        }

        private void SavePrefs_Execute(object parameters)
        {
            Properties.Settings.Default.EnhancedResolutionMode = (bool)GetValue(OptionEnhanceModeEnabledProperty);
            Properties.Settings.Default.Save();
            Close();
        }

        private void CancelPrefs_Execute(object parameters)
        {
            Close();
        }

        private void DisableEnhanceMode_Execute(object parameters)
        {
            SetCurrentValue(OptionEnhanceModeEnabledProperty, false);
            SetCurrentValue(OptionEnhanceModeDisabledProperty, true);
        }

        private void EnableEnhanceMode_Execute(object parameters)
        {
            SetCurrentValue(OptionEnhanceModeEnabledProperty, true);
            SetCurrentValue(OptionEnhanceModeDisabledProperty, false);
        }
    }
}
