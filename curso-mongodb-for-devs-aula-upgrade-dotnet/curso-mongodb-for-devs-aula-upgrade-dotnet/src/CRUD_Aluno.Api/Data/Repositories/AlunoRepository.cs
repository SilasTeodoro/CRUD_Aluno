using System.Collections.Generic;
using System.Threading.Tasks;
using CRUD_Aluno.Api.Data.Schemas;
using CRUD_Aluno.Api.Domain.Entities;
using CRUD_Aluno.Api.Domain.ValueObjects;
using MongoDB.Driver;
using System.Linq;
using CRUD_Aluno.Api.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Driver.Linq;

namespace CRUD_Aluno.Api.Data.Repositories
{
    public class AlunoRepository
    {
        IMongoCollection<AlunoSchema> _aluno;
        

        public AlunoRepository(MongoDB mongoDB)
        {
            _aluno = mongoDB.DB.GetCollection<AlunoSchema>("alunos");
        }

        public void Inserir(Aluno aluno)
        {
            var document = new AlunoSchema
            {
                Nome = aluno.Nome,
                Curso = aluno.Curso,
                Endereco = new EnderecoSchema
                {
                    Logradouro = aluno.Endereco.Logradouro,
                    Numero = aluno.Endereco.Numero,
                    Cidade = aluno.Endereco.Cidade,
                    Cep = aluno.Endereco.Cep,
                    UF = aluno.Endereco.UF
                }
            };

            _aluno.InsertOne(document);
        }

        public async Task<IEnumerable<Aluno>> ObterTodos()
        {
            var aluno = new List<Aluno>();

            await _aluno.AsQueryable().ForEachAsync(d =>
            {
                var r = new Aluno(d.Id.ToString(), d.Nome, d.Curso);
                var e = new Endereco(d.Endereco.Logradouro, d.Endereco.Numero, d.Endereco.Cidade, d.Endereco.UF, d.Endereco.Cep);
                r.AtribuirEndereco(e);
                aluno.Add(r);
            });

            return aluno;
        }

        public Aluno ObterPorId(string id)
        {
            var document = _aluno.AsQueryable().FirstOrDefault(_ => _.Id == id);

            if (document == null)
                return null;

            return document.ConverterParaDomain();
        }

        public bool AlterarCompleto(Aluno aluno)
        {
            var document = new AlunoSchema
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Curso = aluno.Curso,
                Endereco = new EnderecoSchema
                {
                    Logradouro = aluno.Endereco.Logradouro,
                    Numero = aluno.Endereco.Numero,
                    Cidade = aluno.Endereco.Cidade,
                    Cep = aluno.Endereco.Cep,
                    UF = aluno.Endereco.UF
                }
            };

            var resultado = _aluno.ReplaceOne(_ => _.Id == document.Id, document);

            return resultado.ModifiedCount > 0;
        }

        public bool AlterarCurso(string id, ECurso curso)
        {
            var atualizacao = Builders<AlunoSchema>.Update.Set(_ => _.Curso, curso);

            var resultado = _aluno.UpdateOne(_ => _.Id == id, atualizacao);

            return resultado.ModifiedCount > 0;
        }

        public IEnumerable<Aluno> ObterPorNome(string nome)
        {
            var aluno = new List<Aluno>();

            _aluno.AsQueryable()
                .Where(_ => _.Nome.ToLower().Contains(nome.ToLower()))
                .ToList()
                .ForEach(d => aluno.Add(d.ConverterParaDomain()));

            return aluno;
        }

        public long Remover(string alunoId)
        {
            var resultadoAluno = _aluno.DeleteOne(_ => _.Id == alunoId);

            return (resultadoAluno.DeletedCount);
        }

        public async Task<IEnumerable<Aluno>> ObterPorBuscaTextual(string texto)
        {
            var aluno = new List<Aluno>();

            var filter = Builders<AlunoSchema>.Filter.Text(texto);

            await _aluno
                .AsQueryable()
                .Where(_ => filter.Inject())
                .ForEachAsync(d => aluno.Add(d.ConverterParaDomain()));

            return aluno;
        }
    }
}