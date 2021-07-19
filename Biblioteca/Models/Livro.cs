using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Models
{
    public class Livro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} Requerido")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "{0} precisa ter entre {2} e {1} caracteres")]
        public string Nome { get; set; }
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "{0} Requerido")]
        [Display(Name = "Categoria")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "{0} Requerido")]
        public string Autor { get; set; }

        [Required(ErrorMessage = "{0} Requerido")]
        public bool Ativo { get; set; }

    }
}
