using Microsoft.Extensions.Logging;
using RocketLeagueModManager.App.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace RocketLeagueModManager.App.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger<MainWindowViewModel> _logger;
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

        public ICommand ActivateCommand { get; }
        public ICommand CopyToModsCommand { get; }
        public ICommand RemoveFromModsCommand { get; }
        public ICommand LaunchBakkesModCommand { get; }
        public ICommand LaunchRocketLeagueCommand { get; }
        public ICommand DeactivateCommand { get; }

        public event EventHandler<UserMessageEventArgs> UserMessaged;

        public MainWindowViewModel(AppSettings appSettings, ILogger<MainWindowViewModel> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
            _appSettings.AppSettingsChanged += AppSettings_Changed;
            WorkshopFiles = new ObservableCollection<ModFile>();
            ModFiles = new ObservableCollection<ModFile>();
            WorkshopPath = _appSettings.WorkshopPath;
            ModPath = _appSettings.ModPath;
            ActivateCommand = new RelayCommand<ModFile>(ActivateFile, CanActivateFile);
            CopyToModsCommand = new RelayCommand<ModFile>(CopyToModFiles);
            RemoveFromModsCommand = new RelayCommand<ModFile>(RemoveFromModFiles);
            LaunchBakkesModCommand = new RelayCommand(LaunchBakkesMod);
            LaunchRocketLeagueCommand = new RelayCommand(LaunchRocketLeague);
            DeactivateCommand = new RelayCommand<ModFile>(DeactivateFile, CanDeactivateFile);
        }

        public void Initialize()
        {
            LoadWorkshopFiles();
            LoadModFiles();
        }

        private void LoadWorkshopFiles()
        {
            try
            {
                if (!Directory.Exists(WorkshopPath))
                {
                    return;
                }

                WorkshopFiles.Clear();
                var workshopDirectory = new DirectoryInfo(WorkshopPath);
                var modDirectory = new DirectoryInfo(ModPath);
                var workshopFiles = workshopDirectory
                    .GetFiles($"*{AppSettings.ModFileExtension}", SearchOption.AllDirectories);

                FileInfo[] modFiles;
                if (Directory.Exists(ModPath))
                {
                    modFiles = modDirectory.GetFiles($"*{AppSettings.ModFileExtension}", SearchOption.TopDirectoryOnly);
                }
                else
                {
                    modFiles = new FileInfo[0];
                }

                foreach (var fileInfo in workshopFiles
                    .Except(modFiles, new FileInfoNameComparer())
                    .OrderByDescending(f => f.LastWriteTime))
                {
                    WorkshopFiles.Add(new ModFile(fileInfo));
                }
            }
            catch (Exception ex)
            {
                HandleError("Failed to load workshop files. See the log file for more details", ex);
            }
        }

        private void LoadModFiles()
        {
            try
            {
                if (!Directory.Exists(ModPath))
                {
                    return;
                }

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
                HandleError("Failed to load mod files. See the log file for more details", ex);
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
                _activeFile?.Delete();
                File.Copy(selectedModFile.FileInfo.FullName, Path.Combine(_appSettings.ModPath, _appSettings.ActiveFileName));
                LoadModFiles();
            }
            catch (Exception ex)
            {
                HandleError($"Failed to activate file {selectedModFile?.FileName ?? "<Null file>"}. See the log file for more details", ex);
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
                if (!Directory.Exists(ModPath))
                {
                    HandleError("The mod path is set to a valid folder. Please select a valid folder and try again.");
                    return;
                }

                File.Copy(selectedWorkshopFile.FileInfo.FullName, Path.Combine(_appSettings.ModPath, selectedWorkshopFile.FileName));
                LoadWorkshopFiles();
                LoadModFiles();
            }
            catch (Exception ex)
            {
                HandleError($"Failed to move file {selectedWorkshopFile?.FileName ?? "<Null file>"} to mods folder. See the log file for more details", ex);
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
                HandleError($"Failed to delete mod file {selectedModFile?.FileName ?? " <Null file>"}. See the log file for more details", ex);
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

        private void HandleError(string message, Exception ex = null, bool notifyUser = true)
        {
            if (ex != null)
            {
                _logger.LogError(message, ex);
            }

            if (notifyUser)
            {
                UserMessaged?.Invoke(this, new UserMessageEventArgs(message));
            }
        }

        private void LaunchBakkesMod()
        {
            try
            {
                if (!File.Exists(_appSettings.BakkesModPath))
                {
                    HandleError("Unable to find Bakkes mod executable. Set the path in the settings and try again.");
                    return;
                }

                var processes = Process.GetProcessesByName("BakkesMod");
                if (processes.Length > 0)
                {
                    HandleError("Bakkes Mod is already running.");
                    return;
                }

                Process.Start(_appSettings.BakkesModPath);
            }
            catch (Exception ex)
            {
                HandleError("Failed to launch Bakke's Mod. See the log for more details.", ex);
            }
        }

        private void LaunchRocketLeague()
        {
            try
            {
                if (!File.Exists(_appSettings.RocketLeaguePath))
                {
                    HandleError("Unable to find Bakkes mod executable. Set the path in the settings and try again.");
                    return;
                }

                var processes = Process.GetProcessesByName("RocketLeague");
                if (processes.Length > 0)
                {
                    HandleError("Rocket League is already running.");
                    return;
                }

                Process.Start(_appSettings.RocketLeaguePath);
            }
            catch (Exception ex)
            {
                HandleError("Failed to launch Rocket League. See the log for more details.", ex);
            }
        }

        private void DeactivateFile(ModFile selectedFile)
        {
            try
            {
                _activeFile.Delete();
                LoadModFiles();
            }
            catch (Exception ex)
            {
                HandleError($"Failed to deactive file {selectedFile.FileName}.", ex);
            }
        }

        private bool CanDeactivateFile(ModFile selectedFile)
        {
            return selectedFile?.IsActive ?? false;
        }
    }
}
