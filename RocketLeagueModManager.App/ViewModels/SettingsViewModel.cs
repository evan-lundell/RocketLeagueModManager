using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace RocketLeagueModManager.App.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly AppSettings _appSettings;
        public event EventHandler SettingsSaved;

        private string _workshopPath;
        public string WorkshopPath 
        { 
            get
            {
                return _workshopPath;
            }
            set
            {
                _workshopPath = value;
                OnPropertyChanged();
            }
        }

        private string _modPath;
        public string ModPath
        {
            get
            {
                return _modPath;
            }
            set
            {
                _modPath = value;
                OnPropertyChanged();
            }
        }

        private string _activeFileName;
        public string ActiveFileName
        {
            get
            {
                return _activeFileName;
            }
            set
            {
                _activeFileName = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; set; }

        public SettingsViewModel(AppSettings appSettings)
        {
            _appSettings = appSettings;
            WorkshopPath = _appSettings.WorkshopPath;
            ModPath = _appSettings.ModPath;
            ActiveFileName = _appSettings.ActiveFileName;
            SaveCommand = new RelayCommand(Save);
        }

        private void Save(object param)
        {
            _appSettings.WorkshopPath = WorkshopPath;
            _appSettings.ModPath = ModPath;
            _appSettings.ActiveFileName = ActiveFileName;
            _appSettings.SaveSettings();
            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}
