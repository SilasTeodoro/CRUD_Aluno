using System;
using CRUD_Aluno.Api.Data.Schemas;
using CRUD_Aluno.Api.Domain.Enums;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace CRUD_Aluno.Api.Data
{
    public class MongoDB
    {
        public IMongoDatabase DB { get; }

        public MongoDB(IConfiguration configuration)
        {
            try
            {
                var client = new MongoClient(configuration["ConnectionString"]);
                DB = client.GetDatabase(configuration["NomeBanco"]);
                MapClasses();
            }
            catch (Exception ex)
            {
                throw new MongoException("Não foi possivel se conectar ao MongoDB", ex);
            }
        }

        private void MapClasses()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(AlunoSchema)))
            {
                BsonClassMap.RegisterClassMap<AlunoSchema>(i =>
                {
                    i.AutoMap();
                    i.MapIdMember(c => c.Id);
                    i.MapMember(c => c.Curso).SetSerializer(new EnumSerializer<ECurso>(BsonType.Int32));
                    i.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}