using Biblioteca.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Exporting
{
    public class FileGeneratorFactory<T>
    {
        private IFileGenerator _fileGenerator;
        private readonly IEnumerable<T> _data;
        private readonly FileType _fileType;
        
        public FileGeneratorFactory(FileType fileType, IEnumerable<T> data)
        {
            _fileType = fileType;
            _data = data;
        }

        public IFileGenerator CreateInstance()
        {
            if (_fileType == FileType.CSV)
                _fileGenerator = new CsvGenerator<T>(_data);

            if (_fileType == FileType.XML)
                _fileGenerator = new XmlGenerator<T>(_data);

            return _fileGenerator;
        }
    }
}
