using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Loggers
{
    interface IModelLogger<in T>
    {
        void LogCreation(T modeloAtual);
        void LogUpdate(T modeloOriginal, T modeloAtual);
        void LogDelete(string nomeOriginal);
    }
}
