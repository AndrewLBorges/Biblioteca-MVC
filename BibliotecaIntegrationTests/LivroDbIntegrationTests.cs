using Biblioteca;
using Biblioteca.Controllers;
using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BibliotecaIntegrationTests
{
    public class LivroDbIntegrationTests : IDisposable
    {

        private readonly BibliotecaContext _context;
        private readonly IConfigurationBuilder _configurationBuilder;
        private readonly IConfigurationRoot _configuration;


        public LivroDbIntegrationTests()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkSqlServer().BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<BibliotecaContext>();

            builder.UseSqlServer($"Server=(localdb)\\mssqllocaldb;Database=biblioteca_db_{Guid.NewGuid()};Trusted_Connection=True;MultipleActiveResultSets=true")
                    .UseInternalServiceProvider(serviceProvider);

            _context = new BibliotecaContext(builder.Options);
            _context.Database.Migrate();

            _configurationBuilder = new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json");
            _configuration = _configurationBuilder.Build();
        }

        [Fact]
        public void PegarPrimeiroLivroCertoNoBancoDeDadosTeste()
        {

            _context.Livros.Add(new Livro { Nome = "LivroTeste", Categoria = new Categoria { Nome = "CategoriaTeste", Ativo = false }, Autor = "AutorTeste", Ativo = false });
            _context.Livros.Add(new Livro { Nome = "LivroTeste2", Categoria = new Categoria { Nome = "CategoriaTeste2", Ativo = false }, Autor = "AutorTeste2", Ativo = false });
            _context.SaveChanges();

            var livros = _context.Livros;
            var primeiroLivro = livros.First();

            Assert.Equal(2, livros.Count());
            Assert.Equal("LivroTeste", primeiroLivro.Nome);
            Assert.Equal("CategoriaTeste", primeiroLivro.Categoria.Nome);
            Assert.False(primeiroLivro.Categoria.Ativo);
            Assert.Equal("AutorTeste", primeiroLivro.Autor);
            Assert.False(primeiroLivro.Ativo);
        }

        [Fact]
        public async Task PegarPrimeiroLivroDaControllerUsandoAActionDetailsRetornaPrimeiroLivroDoBancoDeDados()
        {
            _context.Livros.Add(new Livro { Nome = "LivroTeste", Categoria = new Categoria { Nome = "CategoriaTeste", Ativo = false }, Autor = "AutorTeste", Ativo = false });
            _context.Livros.Add(new Livro { Nome = "LivroTeste2", Categoria = new Categoria { Nome = "CategoriaTeste2", Ativo = false }, Autor = "AutorTeste2", Ativo = false });
            _context.SaveChanges();
            var loggerMock = new Mock<ILogger<LivrosController>>();


            var primeiroLivroDb = _context.Livros.FirstOrDefault();
            var livroController = new LivrosController(_context, _configuration, loggerMock.Object);
            var detailsAcao = await livroController.Details(primeiroLivroDb.Id) as ViewResult;
            var primeiroLivroAcao = detailsAcao.Model as Livro;

            Assert.Equal(primeiroLivroDb, primeiroLivroAcao);

            
            //if(detailsAcao.Model is Livro primeiroLivroAcao)
            //    Assert.Equal(primeiroLivroDb, primeiroLivroAcao);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
        }
    }
}
