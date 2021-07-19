using Biblioteca.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Models
{
    public class Emprestimo
    {
        public int Id { get; set; }

        [ModelItem(2)]
        public Livro Livro { get; set; }


        [Required(ErrorMessage = "{0} Requerido")]
        public int LivroId { get; set; }

        public Cliente Cliente { get; set; }

        [Required(ErrorMessage = "{0} Requerido")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "{0} Requerido")]
        [Display(Name = "Emprestado em")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Emprestado { get; set; }

        [Required(ErrorMessage = "{0} Requerido")]
        [Display(Name = "Previsao de devolucao")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime PrevisaoDevolucao { get; set; }

        [Display(Name = "Data de Devolucao")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Devolucao { get; set; }
    }
}
