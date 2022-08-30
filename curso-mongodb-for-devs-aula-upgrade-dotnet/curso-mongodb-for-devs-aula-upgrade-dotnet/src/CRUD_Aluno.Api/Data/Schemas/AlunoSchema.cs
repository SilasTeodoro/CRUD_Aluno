using CRUD_Aluno.Api.Domain.Entities;
using CRUD_Aluno.Api.Domain.Enums;
using CRUD_Aluno.Api.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CRUD_Aluno.Api.Data.Schemas
{ 
    public class AlunoSchema
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Nome { get; set; }
        public ECurso Curso { get; set; }
        public EnderecoSchema Endereco { get; set; }
    }

    public static class AlunoSchemaExtensao
    {
        public static Aluno ConverterParaDomain(this AlunoSchema document)
        {
            var aluno = new Aluno(document.Id, document.Nome, document.Curso);
            var endereco = new Endereco(document.Endereco.Logradouro, document.Endereco.Numero, document.Endereco.Cidade, document.Endereco.UF, document.Endereco.Cep);
            aluno.AtribuirEndereco(endereco);

            return aluno;
        }
    }
}