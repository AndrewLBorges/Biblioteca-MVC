using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Biblioteca.Data;
using Biblioteca.Models;
using System.Diagnostics;
using Biblioteca.Loggers;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Biblioteca.Helpers;

namespace Biblioteca.Controllers
{
    public class LivrosController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly IModelLogger<Livro> _livroLogger;
        private readonly ILogger<LivrosController> _logger;

        public LivrosController(BibliotecaContext context, IConfiguration configuration, ILogger<LivrosController> logger)
        {
            _context = context;
            _livroLogger = new LivroLogger(configuration);
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Livros
        public async Task<IActionResult> Index()
        {
            _logger.LogTrace("Entrou no metodo Index(get) de Livros");
            _logger.LogInformation("Entrando no metodo Index(Get) de Livros");

            var bibliotecaContext = _context.Livros.Include(l => l.Categoria);

            _logger.LogTrace("Saiu do metodo Index(get) de Livros");

            return View(await bibliotecaContext.ToListAsync());

        }

        [HttpPost]
        public async Task<IActionResult> Index(string mostrarAtivos)
        {
            _logger.LogTrace("Entrou no metodo Index(post) de Livros");
            _logger.LogInformation("Entrando no metodo Index(post) de Livros");

            ViewData["mostrarAtivos"] = mostrarAtivos;
            var bibliotecaContext = _context.Livros.Include(l => l.Categoria);

            _logger.LogTrace("Saiu do metodo Index(post) de Livros");

            return View(await bibliotecaContext.ToListAsync());
        }

        // GET: Livros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            _logger.LogTrace("Entrou no metodo Details(get) de Livros");
            _logger.LogInformation("Entrando no metodo Details(Get) de Livros");

            if (id == null)
            {
                _logger.LogTrace("Saiu do metodo Details(get) pois o Id de entrada eh nulo.");

                _logger.LogWarning("Id de entrada esperado eh nulo.");

                return NotFound();
            }

            var livro = await _context.Livros
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livro == null)
            {
                _logger.LogWarning("Modelo de livro relacionado ao Id {Id} de entrada nao existe", id);

                _logger.LogTrace("Saiu do metodo Details(get) pois o modelo de livro relacionado ao id {Id} de entrada eh nulo.", id);

                return NotFound();
            }

            _logger.LogTrace("Saiu do metodo Details(get) com sucesso retornando o modelo {@Modelo}.", livro);
            return View(livro);
        }

        // GET: Livros/Create
        public IActionResult Create()
        {
            _logger.LogTrace("Entrou no metodo Create(get) de Livros");

            _logger.LogInformation("Entrando no metodo Create(Get) de Livros");

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome");

            _logger.LogTrace("Saiu do metodo Create(get) de Livros");

            return View();
        }

        // POST: Livros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,CategoriaId,Autor,Ativo")] Livro livro)
        {
            _logger.LogTrace("Entrou no metodo Create(post) de Livros");
            _logger.LogInformation("Entrando no metodo Create(Post) de Livros");

            if (ModelState.IsValid)
            {
                _context.Add(livro);
                var categoria = await _context.Categorias.FindAsync(livro.CategoriaId);

                if (!categoria.Ativo)
                {
                    categoria.Ativo = true;
                    _context.Update(categoria);
                }
                await _context.SaveChangesAsync();
                _livroLogger.LogCreation(livro);

                _logger.LogTrace("Saiu no metodo Create(post) criando o novo livro {@Livro} e adicionando ele na base corretamente.", livro);

                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Modelo novo nao criado pois o estado dele nao eh valido.");

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", livro.CategoriaId);

            _logger.LogTrace("Saiu do metodo Create(post) de Livros voltando a pagina de criacao sem criar um novo livro.");

            return View(livro);
        }

        // GET: Livros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            _logger.LogTrace("Entrou no metodo Edit(get) de Livros");
            _logger.LogInformation("Entrando no metodo Edit(Get) de Livros");

            if (id == null)
            {
                _logger.LogWarning("Id de entrada esperado eh nulo.");

                _logger.LogTrace("Saiu do metodo Edit(get) pois o Id {id} de entrada é nulo", id);

                return NotFound();
            }

            var livro = await _context.Livros.FindAsync(id);
            if (livro == null)
            {
                _logger.LogWarning("Modelo de livro relacionado ao Id {Id} de entrada nao existe", id);

                _logger.LogTrace("Saiu do metodo Edit(get) pois o modelo de livro relacionado ao {Id} de entrada é nulo.", id);

                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", livro.CategoriaId);

            _logger.LogTrace("Saiu do metodo Edit(get) com sucesso retornando o modelo {@Modelo}.", livro);

            return View(livro);
        }

        // POST: Livros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,CategoriaId,Autor,Ativo")] Livro livro)
        {
            _logger.LogTrace("Entrou no metodo Edit(post) de Livros");
            _logger.LogInformation("Entrando no metodo Edit(Post) de Livros");

            if (id != livro.Id)
            {
                _logger.LogWarning("Id {Id} de entrada não corresponde ao Id {idModelo} do modelo a ser editado.", id, livro.Id);

                _logger.LogTrace("Saiu do metodo Edit(post) pois o Id {id} de entrada não se iguala ao Id {idModelo} do modelo", id, livro.Id);

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var categoriaAtual = await _context.Categorias.FindAsync(livro.CategoriaId);
                try
                {
                    UpdateCategoriaAntigaPorIdLivro(id);

                    if (!categoriaAtual.Ativo)
                        categoriaAtual.Ativo = true;

                    livro.Categoria = categoriaAtual;

                    _livroLogger.LogUpdate(GetLivroOriginalPorId(id), livro);

                    _context.Update(categoriaAtual);
                    _context.Update(livro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!LivroExists(livro.Id))
                    {
                        _logger.LogWarning("Modelo de livro com o Id {Id} nao existe", livro.Id);

                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError("Excecao alcancada. mensagem = {mensagem}\n stack trace = {stackTrace}", ex.Message, ex.StackTrace);
                    }
                }
                _logger.LogTrace("Saiu do metodo Edit(post) concluindo a edicao do modelo {@Modelo} e atualizando a base.", livro);

                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Modelo novo nao criado pois o estado dele nao eh valido.");

            ViewData["CategoriaId"] = new SelectList(_context.Categorias, "Id", "Nome", livro.CategoriaId);

            _logger.LogTrace("Saiu do metodo Edit(post) voltando a pagina de edicao sem editar o livro.");
            return View(livro);
        }

        private void UpdateCategoriaAntigaPorIdLivro(int id)
        {
            _logger.LogTrace("Entrou no metodo interno {metodo} de Livros", MethodBase.GetCurrentMethod().Name);
            _logger.LogInformation("Entrando no metodo Interno {metodo} de Livros", MethodBase.GetCurrentMethod().Name);

            var categoriaAntiga = _context.Livros.Where(l => l.Id == id).Select(l => l.Categoria).FirstOrDefault();
            var referenciasCategoriaAntiga = _context.Livros.Count(l => l.CategoriaId == categoriaAntiga.Id);

            if (referenciasCategoriaAntiga <= 1)
                categoriaAntiga.Ativo = false;

            _context.Update(categoriaAntiga);

            _logger.LogTrace("Saiu do metodo UpdateCategoriaAntigaPorIdLivro de Livros");
        }

        private Livro GetLivroOriginalPorId(int id)
        {
            _logger.LogTrace("Entrou no metodo interno {metodo} de Livros", MethodBase.GetCurrentMethod().Name);
            _logger.LogInformation("Entrando no metodo interno {metodo} de Livros", MethodBase.GetCurrentMethod().Name);

            var livroOriginal = _context.Livros.FirstOrDefault(l => l.Id == id);
            _context.Entry(livroOriginal).State = EntityState.Detached;

            _logger.LogTrace("Saiu do metodo {metodo} de Livros", MethodBase.GetCurrentMethod().Name);

            return livroOriginal;

        }

        // GET: Livros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogTrace("Entrou no metodo Delete(get) de Livros");
            _logger.LogInformation("Entrando no metodo Delete(Get) de Livros");

            if (id == null)
            {
                _logger.LogWarning("Id de entrada esperado eh nulo.");

                _logger.LogTrace("Saiu do metodo Delete(get) pois o Id {id} de entrada é nulo", id);

                return NotFound();
            }

            var livro = await _context.Livros
                .Include(l => l.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (livro == null)
            {
                _logger.LogWarning("Modelo de livro relacionado ao Id {Id} de entrada nao existe", id);

                _logger.LogTrace("Saiu do metodo Delete(get) pois o modelo de livro relacionado ao {Id} de entrada é nulo.", id);

                return NotFound();
            }

            if (livro.Ativo)
            {
                _logger.LogWarning("Livro de Id {Id} nao foi deletado por ja estar em uso.", livro.Id);

                _logger.LogTrace("Saiu do metodo Delete(get) sem deletar o livro de modelo {@Modelo} pois ele se encontra em uso.", livro);

                return RedirectToAction(nameof(Error), new { message = "Livro em uso, não é possível deletá-lo." });
            }

            _logger.LogTrace("Saiu do metodo Delete(get) retornando a pagina com o modelo de livro correto. Modelo = {@Modelo}", livro);

            return View(livro);
        }

        // POST: Livros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogTrace("Entrou no metodo Delete(post) de Livros");
            _logger.LogInformation("Entrando no metodo Delete(post) de Livros");

            var livro = await _context.Livros.FindAsync(id);
            await UpdateCategoriaSeForUltimaReferencia(livro.CategoriaId);

            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();

            _livroLogger.LogDelete(livro.Nome);

            _logger.LogTrace("Saiu do metodo Delete(post) deletando o livro do banco corretamente.");

            return RedirectToAction(nameof(Index));
        }

        private async Task UpdateCategoriaSeForUltimaReferencia(int id)
        {
            _logger.LogTrace("Entrou no metodo interno {Metodo} de Livros", MethodBase.GetCurrentMethod().Name);
            _logger.LogInformation("Entrando no metodo interno {Metodo} de Livros", MethodBase.GetCurrentMethod().Name);

            var categoria = await _context.Categorias.FindAsync(id);
            var referenciasCategoria = _context.Livros.Count(l => l.CategoriaId == categoria.Id);

            if (referenciasCategoria <= 1)
            {
                categoria.Ativo = false;
                _context.Update(categoria);
            }

            _logger.LogTrace("Saiu do metodo interno {Metodo} de Livros", MethodBase.GetCurrentMethod().Name);
        }

        private bool LivroExists(int id)
        {
            _logger.LogTrace("Entrou no metodo interno {Metodo} de Livros", MethodBase.GetCurrentMethod().Name);
            _logger.LogInformation("Entrando no metodo interno {Metodo} de Livros", MethodBase.GetCurrentMethod().Name);

            _logger.LogTrace("Saiu do metodo interno {Metodo} de Livros", MethodBase.GetCurrentMethod().Name);
            return _context.Livros.Any(e => e.Id == id);
        }

        public IActionResult Error(string message)
        {
            _logger.LogTrace("Entrou no metodo Error(Get) de Livros");
            _logger.LogInformation("Entrando no metodo Error(Ge) de Livros");

            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            _logger.LogTrace("Saiu do metodo Error(Get) retornando a ViewModel {@Model}", viewModel);

            return View(viewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ImportarCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return RedirectToAction(nameof(Error), new { message = "Arquivo não encontrado ou vazio." });
            }

            var fileString = await FileImportHelper.ReadAsStringAsync(file);
            var fileStringList = FileImportHelper.StringLinesToEnumerable(fileString);
            var livroList = GetFileLinesAsLivros(fileStringList);
            await PopulateDataAsync(livroList);

            return RedirectToAction("Index");
        }

        private IEnumerable<Livro> GetFileLinesAsLivros(IEnumerable<string> fileStringList)
        {
            var livros = new List<Livro>();
            foreach (var line in fileStringList)
            {
                var livroProperties = line.Split(',');
                var categoria = _context.Categorias.Where(c => c.Nome == livroProperties[1]).FirstOrDefault();
                if (categoria == null)
                {
                    categoria = new Categoria { Nome = livroProperties[1], Ativo = true };
                    _context.Categorias.Add(categoria);
                    _context.SaveChanges();
                }

                livros.Add(new Livro
                {
                    Nome = livroProperties[0],
                    Categoria = categoria,
                    CategoriaId = categoria.Id,
                    Autor = livroProperties[2],
                    Ativo = false
                });
            }

            return livros;
        }

        private async Task PopulateDataAsync(IEnumerable<Livro> data)
        {
            await _context.Livros.AddRangeAsync(data);
            await _context.SaveChangesAsync();
        }
    }
}
