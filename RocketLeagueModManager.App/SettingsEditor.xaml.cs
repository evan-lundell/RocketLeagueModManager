using RocketLeagueModManager.App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RocketLeagueModManager.App
{
    /// <summary>
    /// Interaction logic for SettingsEditor.xaml
    /// </summary>
    public partial class SettingsEditor : Window
    {
        private AppSettings _appSettings;

        public SettingsEditor(AppSettings appSettings)
        {
            InitializeComponent();
            _appSettings = appSettings;
            var settingsViewModel = new SettingsViewModel(_appSettings);
            settingsViewModel.SettingsSaved += SettingsViewModel_SettingsSaved;
            DataContext = settingsViewModel;
        }

        private void SettingsViewModel_SettingsSaved(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnWorkshopPathChange_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (Directory.Exists(_appSettings.WorkshopPath))
            {
                dialog.SelectedPath = _appSettings.WorkshopPath;
            }

            dialog.ShowDialog();
            ((SettingsViewModel)DataContext).WorkshopPath = dialog.SelectedPath;

        }

        private void btnModPathChange_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (Directory.Exists(_appSettings.ModPath))
            {
                dialog.SelectedPath = _appSettings.ModPath;
            }

            dialog.ShowDialog();
            ((SettingsViewModel)DataContext).ModPath = dialog.SelectedPath;
        }
    }
}
