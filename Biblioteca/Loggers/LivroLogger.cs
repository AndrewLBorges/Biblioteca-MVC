using Biblioteca.Enums;
using Biblioteca.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biblioteca.Loggers
{
    public class LivroLogger : IModelLogger<Livro>
    {
        private readonly MongoClient _dbClient;
        private readonly IConfiguration _configuration;

        public LivroLogger(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbClient = new MongoClient(_configuration.GetConnectionString("MongoDbConnectionString"));
        }

        public void LogCreation(Livro modeloAtual)
        {
            var document = new BsonDocument
            {
                { "NomeAlterado", modeloAtual.Nome },
                { "AutorAlterado", modeloAtual.Autor },
                { "CategoriaAlterada", modeloAtual.Categoria.Nome },
                { "AtivoAlterado", modeloAtual.Ativo },
                { "DataAlteracao", DateTime.Now },
                { "Acao", Acao.INSERT.ToString() }
            };

            InsertLogLivro(document);
        }

        public void LogDelete(string nomeOriginal)
        {
            var document = new BsonDocument
            {
                { "NomeOriginal", nomeOriginal },
                { "DataAlteracao", DateTime.Now },
                { "Acao", Acao.DELETE.ToString() }
            };

            InsertLogLivro(document);
        }

        public void LogUpdate(Livro modeloOriginal, Livro modeloAtual)
        {
            var document = new BsonDocument
            {
                { "NomeOriginal", modeloOriginal.Nome },
                { "NomeAlterado", modeloAtual.Nome },
                { "AutorOriginal", modeloOriginal.Autor },
                { "AutorAlterado", modeloAtual.Autor },
                { "CategoriaOriginal", modeloOriginal.Categoria.Nome },
                { "CategoriaAlterada", modeloAtual.Categoria.Nome },
                { "AtivoOriginal", modeloOriginal.Ativo },
                { "AtivoAlterado", modeloAtual.Ativo },
                { "DataAlteracao", DateTime.Now },
                { "Acao", Acao.UPDATE.ToString() }
            };

            InsertLogLivro(document);
        }


        private void InsertLogLivro(BsonDocument document)
        {
            var database = _dbClient.GetDatabase("biblioteca");
            var collection = database.GetCollection<BsonDocument>("logLivros");

            collection.InsertOne(document);
        }


    }
}
