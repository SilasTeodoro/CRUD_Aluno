namespace CRUD_Aluno.Api.Controllers.Inputs
{
    public class AlunoAlteracaoCompleta
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Curso { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Cidade { get; set; }
        public string UF { get; set; }
        public string Cep { get; set; }
    }
}