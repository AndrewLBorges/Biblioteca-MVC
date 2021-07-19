using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} Requerido")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "{0} Requerido")]
        public bool Ativo { get; set; }
    }
}
