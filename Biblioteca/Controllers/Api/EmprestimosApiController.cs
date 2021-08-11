using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Controllers.Api
{
    [Route("api/emprestimos")]
    [ApiController]
    public class EmprestimosApiController : ControllerBase
    {
        private readonly BibliotecaContext _context;

        public EmprestimosApiController(BibliotecaContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("get")]
        public IEnumerable<Emprestimo> Get()
        {
            var emprestimos = _context.Emprestimos.Include(e => e.Cliente).Include(e => e.Livro).AsEnumerable();

            return emprestimos;
        }

        [HttpGet]
        [Route("listaClientes")]
        public IEnumerable<string> GetListaDeClientesComEmprestimo()
        {
            var emprestimos = _context.Emprestimos.Select(e => e.Cliente.Nome).AsEnumerable();

            return emprestimos;
        }

        [HttpGet]
        [Route("listaLivros")]
        public IEnumerable<Livro> GetListaDeLivrosEmEmprestimo()
        {
            var emprestimos = _context.Emprestimos.Select(e => e.Livro).AsEnumerable();

            return emprestimos;
        }


    }
}
