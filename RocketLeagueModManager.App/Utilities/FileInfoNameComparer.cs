using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace RocketLeagueModManager.App.Utilities
{
    public class FileInfoNameComparer : IEqualityComparer<FileInfo>
    {
        public bool Equals([AllowNull] FileInfo x, [AllowNull] FileInfo y)
        {
            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] FileInfo obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
