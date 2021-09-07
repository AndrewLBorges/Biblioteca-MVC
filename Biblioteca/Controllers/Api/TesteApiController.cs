using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Biblioteca.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TesteApiController : ControllerBase
    {
        private BibliotecaContext _context;
        private IConfiguration _configuration;
        private readonly MongoClient _dbClient;


        public TesteApiController(BibliotecaContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
            _dbClient = new MongoClient(_configuration.GetConnectionString("MongoDbConnectionString"));
        }

        [HttpGet]
        [Route("teste")]
        public IActionResult Get()
        {
            var livros = new List<Livro>();
            var livroSqlDb = _context.Livros.Include(l => l.Categoria).FirstOrDefault();
            livros.Add(livroSqlDb);

            var database = _dbClient.GetDatabase("biblioteca");
            var collection = database.GetCollection<BsonDocument>("logLivros");
            var livroMongo = collection.Find(new BsonDocument()).FirstOrDefaultAsync();
            livros.Add(new Livro
            {
                Nome = livroMongo.Result.GetValue("NomeAlterado").AsString,
                Categoria = new Categoria() { Nome = livroMongo.Result.GetValue("CategoriaAlterada").AsString, Ativo = true },
                Autor = livroMongo.Result.GetValue("AutorAlterado").AsString,
                Ativo = true
            });

            return Ok(livros);
        }
    }
}
