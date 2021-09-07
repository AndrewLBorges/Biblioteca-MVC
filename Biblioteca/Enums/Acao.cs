using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Enums
{
    public enum Acao
    {
        INSERT,
        UPDATE,
        DELETE
    }

    public static class AcaoExtensions
    {
        public static string FastToString(this Acao acoes)
        {
            return acoes switch
            {
                Acao.INSERT => nameof(Acao.INSERT),
                Acao.UPDATE => nameof(Acao.UPDATE),
                Acao.DELETE => nameof(Acao.DELETE),
                _ => throw new ArgumentOutOfRangeException(nameof(acoes), acoes, null),
            };
        }
    }

}
