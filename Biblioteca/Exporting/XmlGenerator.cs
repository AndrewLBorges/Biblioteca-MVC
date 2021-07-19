using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Biblioteca.Exporting
{
    public class XmlGenerator<TModel> : IFileGenerator
    {
        private readonly IEnumerable<TModel> _data;
        
        public XmlGenerator(IEnumerable<TModel> data)
        {
            _data = data;
        }
        public string Generate()
        {
            XmlSerializer serializer = new(typeof(List<TModel>));
            using var stringWriter = new StringWriter();

            using XmlWriter writer = XmlWriter.Create(stringWriter);
            serializer.Serialize(writer, _data);
            return stringWriter.ToString();
        }
    }
}
