using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data;
using Biblioteca.Models;
using System.Diagnostics;
using Biblioteca.Enums;
using Biblioteca.Exporting;
using System.Text;
using Biblioteca.Models.DTO_s;

namespace Biblioteca.Controllers
{
    public class EmprestimosController : Controller
    {
        private readonly BibliotecaContext _context;
        private const int UMA_SEMANA = 7;

        public EmprestimosController(BibliotecaContext context)
        {
            _context = context;
        }

        // GET: Emprestimos
        public async Task<IActionResult> Index()
        {
            var bibliotecaContext = _context.Emprestimos.Include(e => e.Cliente).Include(e => e.Livro);

            //var tipoArquivoLista = Enum.GetValues(typeof(FileType))
            //                .Cast<FileType>()
            //                .ToList();

            ViewBag.FileType = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "CSV", Value = ((int)FileType.CSV).ToString()},
                new SelectListItem { Text = "XML", Value = ((int)FileType.XML).ToString()}
            }, "Value", "Text");

            return View(await bibliotecaContext.ToListAsync());
        }

        // GET: Emprestimos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimos
                .Include(e => e.Cliente)
                .Include(e => e.Livro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }

        // GET: Emprestimos/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome");
            ViewData["LivroId"] = new SelectList(_context.Livros, "Id", "Nome");
            return View();
        }

        // POST: Emprestimos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LivroId,ClienteId,Emprestado,PrevisaoDevolucao,Devolucao")] Emprestimo emprestimo)
        {
            if (ModelState.IsValid)
            {
                emprestimo.Devolucao = null;
                emprestimo.Emprestado = DateTime.Now;
                var cliente = await _context.Clientes.FindAsync(emprestimo.ClienteId);
                var livro = await _context.Livros.FindAsync(emprestimo.LivroId);

                if(cliente.Ativo)
                    return RedirectToAction(nameof(Error), new { message = "Empréstimo negado. Cliente já possui um empréstimo em aberto!" });

                if(!livro.Ativo)
                {
                    livro.Ativo = true;
                    _context.Update(livro);
                }
                cliente.Ativo = true;
                _context.Update(cliente);

                _context.Add(emprestimo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", emprestimo.ClienteId);
            ViewData["LivroId"] = new SelectList(_context.Livros, "Id", "Nome", emprestimo.LivroId);
            return View(emprestimo);
        }

        // GET: Emprestimos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimos.FindAsync(id);
            if (emprestimo == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", emprestimo.ClienteId);
            ViewData["LivroId"] = new SelectList(_context.Livros, "Id", "Nome", emprestimo.LivroId);
            return View(emprestimo);
        }

        // POST: Emprestimos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LivroId,ClienteId,Emprestado,PrevisaoDevolucao,Devolucao")] Emprestimo emprestimo)
        {
            if (id != emprestimo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var clienteAtual = await _context.Clientes.FindAsync(emprestimo.ClienteId);
                var livroAtual = await _context.Livros.FindAsync(emprestimo.LivroId);
                try
                {
                    UpdateClienteAntigoPorIdEmprestimo(id);
                    UpdateLivroAntigoPorIdEmprestimo(id);

                    if (!clienteAtual.Ativo)
                        clienteAtual.Ativo = true;

                    if (!livroAtual.Ativo)
                        livroAtual.Ativo = true;

                    _context.Update(clienteAtual);
                    _context.Update(emprestimo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmprestimoExists(emprestimo.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Nome", emprestimo.ClienteId);
            ViewData["LivroId"] = new SelectList(_context.Livros, "Id", "Nome", emprestimo.LivroId);
            return View(emprestimo);
        }

        private void UpdateClienteAntigoPorIdEmprestimo(int id)
        {
            var clienteAntigo = _context.Emprestimos.Where(e => e.Id == id).Select(c => c.Cliente).FirstOrDefault();
            var referenciasClienteAntigo = _context.Emprestimos.Count(e => e.ClienteId == clienteAntigo.Id);

            if (referenciasClienteAntigo <= 1)
                clienteAntigo.Ativo = false;

            _context.Update(clienteAntigo);
        }

        private void UpdateLivroAntigoPorIdEmprestimo(int id)
        {
            var livroAntigo = _context.Emprestimos.Where(e => e.Id == id).Select(l => l.Livro).FirstOrDefault();
            var referenciasLivroAntigo = _context.Emprestimos.Count(e => e.LivroId == livroAntigo.Id);

            if (referenciasLivroAntigo <= 1)
                livroAntigo.Ativo = false;

            _context.Update(livroAntigo);
        }

        // GET: Emprestimos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimos
                .Include(e => e.Cliente)
                .Include(e => e.Livro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }

        // POST: Emprestimos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);
            await UpdateLivroSeForUltimaReferencia(emprestimo.LivroId);
            await UpdateClienteSeForUltimaReferencia(emprestimo.ClienteId);

            _context.Emprestimos.Remove(emprestimo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Emprestimos/Devolver/5
        public async Task<IActionResult> Devolver(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimos
                .Include(e => e.Cliente)
                .Include(e => e.Livro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            if(emprestimo.Devolucao != null)
                return RedirectToAction(nameof(Error), new { message = "Livro já devolvido" });

            return View(emprestimo);
        }

        // POST: Emprestimos/Devolver/5
        [HttpPost, ActionName("Devolver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DevolverConfirmed(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);

            emprestimo.Devolucao = DateTime.Now;

            _context.Update(emprestimo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Emprestimos/Renovar/5
        public async Task<IActionResult> Renovar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprestimo = await _context.Emprestimos
                .Include(e => e.Cliente)
                .Include(e => e.Livro)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emprestimo == null)
            {
                return NotFound();
            }

            if (emprestimo.Devolucao != null)
                return RedirectToAction(nameof(Error), new { message = "Livro já devolvido" });

            return View(emprestimo);
        }

        // POST: Emprestimos/Renovar/5
        [HttpPost, ActionName("Renovar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenovarConfirmed(int id)
        {
            var emprestimo = await _context.Emprestimos.FindAsync(id);

            emprestimo.PrevisaoDevolucao = emprestimo.PrevisaoDevolucao.AddDays(UMA_SEMANA);

            _context.Update(emprestimo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task UpdateLivroSeForUltimaReferencia(int id)
        {
            var livro = await _context.Livros.FindAsync(id);
            var referenciasLivro = _context.Emprestimos.Count(e => e.LivroId == livro.Id);

            if (referenciasLivro <= 1)
            {
                livro.Ativo = false;
                _context.Update(livro);
            }
        }

        private async Task UpdateClienteSeForUltimaReferencia(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            var referenciasCliente = _context.Emprestimos.Count(e => e.ClienteId == cliente.Id);

            if (referenciasCliente <= 1)
            {
                cliente.Ativo = false;
                _context.Update(cliente);
            }
        }

        private bool EmprestimoExists(int id)
        {
            return _context.Emprestimos.Any(e => e.Id == id);
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Exportar")]
        [ValidateAntiForgeryToken]
        public IActionResult Exportar(FileType fileType)
        {
            var dadosEmprestimos = _context.Emprestimos.Include(e => e.Livro).Include(e => e.Cliente).ToList();
            var dadosEmDto = new List<EmprestimoDto>();
            dadosEmprestimos.ForEach(de => dadosEmDto.Add(new EmprestimoDto
            {
                Id = de.Id,
                LivroNome = de.Livro.Nome,
                ClienteNome = de.Cliente.Nome,
                Emprestado = de.Emprestado,
                PrevisaoDevolucao = de.PrevisaoDevolucao,
                Devolucao = de.Devolucao
            }));

            var fileGenerator = new FileGeneratorFactory<EmprestimoDto>(fileType, dadosEmDto).CreateInstance();
            var fileContent = fileGenerator.Generate();

            byte[] contentByteArray = Encoding.UTF8.GetBytes(fileContent);
            string contentTypeString = GetFileTypeStringForExporting(fileType);
            string fileDonwloadName = "Emprestimos." + fileType.ToString().ToLower();

            return File(contentByteArray, contentTypeString, fileDonwloadName);
        }

        private string GetFileTypeStringForExporting(FileType fileType)
        {
            return fileType == FileType.CSV ? "text/csv" : "text/xml";
        }
    }
}
