using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RocketLeagueModManager.App.Utilities;
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
        private readonly ILogger<MainWindow> _logger;

        public MainWindow(AppSettings appSettings, ILogger<MainWindow> logger)
        {
            InitializeComponent();
            _appSettings = appSettings;
            _logger = logger;
            _logger.LogInformation("Application Started");
            ((MainWindowViewModel)DataContext).UserMessaged += MainWindowViewModel_UserMessage;
        }

        private void btnWorkshopPathChange_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
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
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
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
            var settings = App.ServiceProvider.GetRequiredService<SettingsEditor>();
            settings.ShowDialog();
        }

        private void MainWindowViewModel_UserMessage(object sender, UserMessageEventArgs e)
        {
            MessageBox.Show(e.Message);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).Initialize();
        }
    }
}
