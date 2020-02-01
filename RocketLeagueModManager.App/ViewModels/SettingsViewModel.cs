using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using RocketLeagueModManager.App.Utilities;

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

        private string _bakkesModPath;
        public string BakkesModPath
        {
            get
            {
                return _bakkesModPath;
            }
            set
            {
                _bakkesModPath = value;
                OnPropertyChanged();
            }
        }

        private string _rocketLeaguePath;
        public string RocketLeaguePath
        {
            get
            {
                return _rocketLeaguePath;
            }
            set
            {
                _rocketLeaguePath = value;
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
            BakkesModPath = _appSettings.BakkesModPath;
            RocketLeaguePath = _appSettings.RocketLeaguePath;
            SaveCommand = new RelayCommand(Save);
        }

        private void Save()
        {
            _appSettings.WorkshopPath = WorkshopPath;
            _appSettings.ModPath = ModPath;
            _appSettings.ActiveFileName = ActiveFileName;
            _appSettings.BakkesModPath = BakkesModPath;
            _appSettings.RocketLeaguePath = RocketLeaguePath;
            _appSettings.SaveSettings();
            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }
    }
}
