using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RocketLeagueModManager.App
{
    public class ModFile
    {
        private readonly FileInfo _fileInfo;
        public FileInfo FileInfo
        {
            get
            {
                return _fileInfo;
            }
        }

        public string FileName
        {
            get
            {
                return _fileInfo?.Name ?? string.Empty;
            }
        }

        public DateTime LastModified
        {
            get
            {
                return _fileInfo?.LastWriteTime ?? new DateTime();
            }
        }

        public bool IsActive { get; private set; }

        public ModFile(FileInfo fileInfo, bool isActive = false)
        {
            _fileInfo = fileInfo;
            IsActive = isActive;
        }
    }
}
