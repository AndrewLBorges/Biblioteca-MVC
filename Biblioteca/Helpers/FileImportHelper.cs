using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.Helpers
{
    public static class FileImportHelper
    {
        public static async Task<string> ReadAsStringAsync(IFormFile file)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(await reader.ReadLineAsync());
            }
            return result.ToString();
        }

        public static IEnumerable<string> StringLinesToEnumerable(string input)
        {
            var stringList = new List<string>();

            foreach (var line in input.Split("\r\n")[..^1])
            {
                stringList.Add(line);
            }
            return stringList;
        }
    }
}
