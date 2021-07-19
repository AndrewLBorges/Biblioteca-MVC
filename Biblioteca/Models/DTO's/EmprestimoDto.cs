using Biblioteca.CustomAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Models.DTO_s
{
    public class EmprestimoDto
    {
        [ModelItem(1)]
        public int Id { get; set; }

        [ModelItem(2, Heading ="Livro")]
        public string LivroNome { get; set; }

        [ModelItem(3, Heading ="Cliente")]
        public string ClienteNome { get; set; }

        [ModelItem(4, Heading = "Emprestrado em", Format = "dd/MM/yyyy")]
        public DateTime Emprestado { get; set; }

        [ModelItem(5, Heading ="Previsao de devolucao", Format = "dd/MM/yyyy")]
        public DateTime PrevisaoDevolucao { get; set; }

        [ModelItem(6, Heading ="Devolvido em", Format = "dd/MM/yyyy")]
        public DateTime? Devolucao { get; set; }
    }
}
