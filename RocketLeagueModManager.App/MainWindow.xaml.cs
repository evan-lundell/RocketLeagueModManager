﻿using Microsoft.Extensions.Options;
using RocketLeagueModManager.App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RocketLeagueModManager.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppSettings _appSettings;

        public MainWindow(AppSettings appSettings)
        {
            InitializeComponent();
            _appSettings = appSettings;
            DataContext = new MainWindowViewModel(_appSettings);
        }

        private void btnWorkshopPathChange_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (Directory.Exists(_appSettings.WorkshopPath))
            {
                dialog.SelectedPath = _appSettings.WorkshopPath;
            }

            dialog.ShowDialog();
            _appSettings.WorkshopPath = dialog.SelectedPath;
            _appSettings.SaveSettings();
        }

        private void btnModPathChange_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (Directory.Exists(_appSettings.ModPath))
            {
                dialog.SelectedPath = _appSettings.ModPath;
            }

            dialog.ShowDialog();
            _appSettings.ModPath = dialog.SelectedPath;
            _appSettings.SaveSettings();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsEditor(_appSettings);
            settings.ShowDialog();
        }
    }
}
