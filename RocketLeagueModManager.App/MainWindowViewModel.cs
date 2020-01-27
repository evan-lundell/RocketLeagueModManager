using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace RocketLeagueModManager.App
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        const string FileExtension = ".udk";

        private string _workshopPath;

        public event PropertyChangedEventHandler PropertyChanged;

        public string WorkshopPath
        {
            get
            {
                return _workshopPath;
            }
            set
            {
                _workshopPath = value;
                OnPropertyChange();
                LoadWorkshopFiles();
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
                _workshopPath = value;
                OnPropertyChange();
                LoadModFiles();
            }
        }

        public ObservableCollection<string> WorkshopFiles { get; set; }
        public ObservableCollection<string> ModFiles { get; set; }

        public MainWindowViewModel()
        {
            WorkshopFiles = new ObservableCollection<string>();
            ModFiles = new ObservableCollection<string>();
        }

        public MainWindowViewModel(string workshopPath, string modPath)
        {
            WorkshopFiles = new ObservableCollection<string>();
            ModFiles = new ObservableCollection<string>();
            WorkshopPath = workshopPath;
            ModPath = modPath;
        }

        private void OnPropertyChange([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadWorkshopFiles()
        {
            try
            {
                WorkshopFiles.Clear();
                var directory = new DirectoryInfo(WorkshopPath);
                foreach (var fileInfo in directory.GetFiles($"*{FileExtension}", SearchOption.AllDirectories))
                {
                    WorkshopFiles.Add(fileInfo.Name);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load Workshop Files.");
            }
        }

        private void LoadModFiles()
        {
            try
            {
                ModFiles.Clear();
                var directory = new DirectoryInfo(ModPath);
                foreach (var fileInfo in directory.GetFiles($"*{FileExtension}", SearchOption.TopDirectoryOnly))
                {
                    ModFiles.Add(fileInfo.Name);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load Mod Files.");
            }
        }
    }
}
