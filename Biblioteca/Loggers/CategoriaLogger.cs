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
    public class CategoriaLogger : IModelLogger<Categoria>
    {
        private readonly MongoClient _dbClient;
        private readonly IConfiguration _configuration;

        public CategoriaLogger(IConfiguration configuration)
        {
            _configuration = configuration;
            _dbClient = new MongoClient(_configuration.GetConnectionString("MongoDbConnectionString"));
        }

        public void LogCreation(Categoria modeloAtual)
        {
            var document = new BsonDocument
            {
                { "NomeAlterado", modeloAtual.Nome },
                { "AtivoAlterado", modeloAtual.Ativo },
                { "DataAlteracao", DateTime.Now },
                { "Acao", Acao.INSERT.ToString() }
            };

            InsertLogCategoria(document);
        }

        public void LogDelete(string nomeOriginal)
        {
            var document = new BsonDocument
            {
                { "NomeOriginal", nomeOriginal },
                { "DataAlteracao", DateTime.Now },
                { "Acao", Acao.DELETE.ToString() }
            };

            InsertLogCategoria(document);
        }

        public void LogUpdate(Categoria modeloOriginal, Categoria modeloAtual)
        {
            var document = new BsonDocument
            {
                { "NomeOriginal", modeloOriginal.Nome },
                { "NomeAlterado", modeloAtual.Nome },
                { "AtivoOriginal", modeloOriginal.Ativo },
                { "AtivoAlterado", modeloAtual.Ativo },
                { "DataAlteracao", DateTime.Now },
                { "Acao", Acao.UPDATE.ToString() }
            };

            InsertLogCategoria(document);
        }

        private void InsertLogCategoria(BsonDocument document)
        {
            var database = _dbClient.GetDatabase("biblioteca");
            var collection = database.GetCollection<BsonDocument>("logCategorias");

            collection.InsertOne(document);
        }
    }
}
