using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Enums
{
    public enum FileType
    {
        CSV,
        XML
    }

    public static class FileTypeExtensions
    {
        public static string FastToString(this FileType types)
        {
            return types switch
            {
                FileType.CSV => nameof(FileType.CSV),
                FileType.XML => nameof(FileType.XML),
                _ => throw new ArgumentOutOfRangeException(nameof(types), types, null)
            };
        }
    }
}
