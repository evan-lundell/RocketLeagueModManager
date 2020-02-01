using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace RocketLeagueModManager.App
{
    public class AppSettings
    {
        internal const string ModFileExtension = ".udk";
        public event EventHandler AppSettingsChanged;

        public string WorkshopPath { get; set; }
        public string ModPath { get; set; }
        public string ActiveFileName { get; set; }
        public string BakkesModPath { get; set; }
        public string RocketLeaguePath { get; set; }

        public void SaveSettings()
        {
            var jsonString = JsonSerializer.Serialize<AppSettings>(this);
            File.WriteAllText("appsettings.json", jsonString);

            AppSettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
