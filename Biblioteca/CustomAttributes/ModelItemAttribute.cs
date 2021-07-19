using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelItemAttribute : Attribute
    {
        public string Heading { get; set; }
        public string Format { get; set; }
        public bool ComplexType { get; set; } = false;
        public int ColumnOrder { get; }

        public ModelItemAttribute(int columnOrder)
        {
            ColumnOrder = columnOrder;
        }
    }
}
