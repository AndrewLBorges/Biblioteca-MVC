using Biblioteca.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Exporting
{
    public class CsvGenerator<TModel> : IFileGenerator
    {
        private readonly IEnumerable<TModel> _data;
        private readonly Type _type;

        public CsvGenerator(IEnumerable<TModel> data)
        {
            _data = data;
            _type = typeof(TModel);
        }

        public string Generate()
        {
            var rows = new StringBuilder();

            rows.Append(CreateHeader()).Append('\n');

            foreach (var item in _data)
                rows.Append(CreateRow(item)).Append('\n');

            return rows.ToString()[..^2];
        }

        private string CreateHeader()
        {
            var properties = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var orderedProps = properties.OrderBy(p => p.GetCustomAttribute<ModelItemAttribute>().ColumnOrder);

            var bob = new StringBuilder();

            foreach (var prop in orderedProps)
            {
                var attr = prop.GetCustomAttribute<ModelItemAttribute>();

                bob.Append(attr.Heading ?? prop.Name).Append(",");
            }

            return bob.ToString()[..^2];
        }

        private string CreateRow(TModel item)
        {
            var properties = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var orderedProps = properties.OrderBy(p => p.GetCustomAttribute<ModelItemAttribute>().ColumnOrder);

            var bob = new StringBuilder();

            foreach (var prop in orderedProps)
            {
                bob.Append(CreateItem(prop, item)).Append(",");
            }

            return bob.ToString()[..^2];
        }

        private string CreateItem(PropertyInfo prop, TModel item)
        {
            var attr = prop.GetCustomAttribute<ModelItemAttribute>();

            return string.Format($"{{0:{attr.Format}}}", prop.GetValue(item));
        }
    }
}
