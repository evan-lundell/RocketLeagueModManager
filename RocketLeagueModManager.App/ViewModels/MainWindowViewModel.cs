using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RocketLeagueModManager.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly AppSettings _appSettings;
        private FileInfo _activeFile;

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

        public ObservableCollection<ModFile> WorkshopFiles { get; private set; }
        public ObservableCollection<ModFile> ModFiles { get; private set; }

        private ModFile _selectedModFile;
        public ModFile SelectedModFile 
        { 
            get
            {
                return _selectedModFile;
            }
            set
            {
                _selectedModFile = value;
                OnPropertyChanged();
            }
        }

        private ModFile _selectedWorkshopFile;
        public ModFile SelectedWorkshopFile
        {
            get
            {
                return _selectedWorkshopFile;
            }
            set
            {
                _selectedWorkshopFile = value;
                OnPropertyChanged();
            }
        }

        public ICommand ActivateCommand { get; private set; }
        public ICommand CopyToModsCommand { get; private set; }
        public ICommand RemoveFromModsCommand { get; private set; }

        public MainWindowViewModel(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _appSettings.AppSettingsChanged += AppSettings_Changed;
            WorkshopFiles = new ObservableCollection<ModFile>();
            ModFiles = new ObservableCollection<ModFile>();
            WorkshopPath = _appSettings.WorkshopPath;
            ModPath = _appSettings.ModPath;
            LoadWorkshopFiles();
            LoadModFiles();
            ActivateCommand = new RelayCommand<ModFile>(ActivateFile, CanActivateFile);
            CopyToModsCommand = new RelayCommand<ModFile>(CopyToModFiles);
            RemoveFromModsCommand = new RelayCommand<ModFile>(RemoveFromModFiles);
        }

        private void LoadWorkshopFiles()
        {
            try
            {
                WorkshopFiles.Clear();
                var workshopDirectory = new DirectoryInfo(WorkshopPath);
                var modDirectory = new DirectoryInfo(ModPath);
                var workshopFiles = workshopDirectory
                    .GetFiles($"*{AppSettings.ModFileExtension}", SearchOption.AllDirectories);
                var modFiles = modDirectory.GetFiles($"*{AppSettings.ModFileExtension}", SearchOption.TopDirectoryOnly);
                foreach (var fileInfo in workshopFiles
                    .Except(modFiles, new FileInfoNameComparer())
                    .OrderByDescending(f => f.LastWriteTime))
                {
                    WorkshopFiles.Add(new ModFile(fileInfo));
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
                var files = directory.GetFiles($"*{AppSettings.ModFileExtension}", SearchOption.TopDirectoryOnly);
                if (File.Exists(Path.Combine(_appSettings.ModPath, _appSettings.ActiveFileName)))
                {
                    _activeFile = new FileInfo(Path.Combine(_appSettings.ModPath, _appSettings.ActiveFileName));
                }
                else
                {
                    _activeFile = null;
                }

                foreach (var fileInfo in files
                    .Where(f => !f.Name.Equals(_appSettings.ActiveFileName, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(f => f.Name))
                {
                    ModFiles.Add(new ModFile(fileInfo, 
                        _activeFile != null && AreFilesEqual(fileInfo, _activeFile)));
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to load Mod Files.");
            }
        }

        private bool AreFilesEqual(FileInfo file1, FileInfo file2)
        {
            int bytesToRead = 64;
            if (file1.Length != file2.Length)
            {
                return false;
            }

            if (file1 == null || file2 == null)
            {
                return false;
            }

            int iterations = (int)Math.Ceiling((double)file1.Length / bytesToRead);

            using (FileStream stream1 = file1.OpenRead())
            using (FileStream stream2 = file2.OpenRead())
            {
                byte[] one = new byte[bytesToRead];
                byte[] two = new byte[bytesToRead];

                for (int i = 0; i < iterations; i++)
                {
                    //System.Diagnostics.Debug.WriteLine($"Iteration: {i}");
                    stream1.Read(one, 0, bytesToRead);
                    stream2.Read(two, 0, bytesToRead);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ActivateFile(ModFile selectedModFile)
        {
            try
            {
                _activeFile.Delete();
                File.Copy(selectedModFile.FileInfo.FullName, Path.Combine(_appSettings.ModPath, _appSettings.ActiveFileName));
                LoadModFiles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to activate file");
            }
        }

        private bool CanActivateFile(ModFile selectedModFile)
        {
            return selectedModFile != null && !selectedModFile.IsActive;
        }

        private void CopyToModFiles(ModFile selectedWorkshopFile)
        {
            try
            {
                File.Copy(selectedWorkshopFile.FileInfo.FullName, Path.Combine(_appSettings.ModPath, selectedWorkshopFile.FileName));
                LoadWorkshopFiles();
                LoadModFiles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to move file to mods folder");
            }
        }

        private void RemoveFromModFiles(ModFile selectedModFile)
        {
            try
            {
                selectedModFile.FileInfo.Delete();
                LoadWorkshopFiles();
                LoadModFiles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to delete mod file");
            }
        }

        private void AppSettings_Changed(object sender, EventArgs e)
        {
            var settings = (AppSettings)sender;
            WorkshopPath = settings.WorkshopPath;
            ModPath = settings.ModPath;
            _activeFile = new FileInfo(Path.Combine(ModPath, settings.ActiveFileName));
            LoadWorkshopFiles();
            LoadModFiles();
        }
    }
}
