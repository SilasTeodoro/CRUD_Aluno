namespace CRUD_Aluno.Api.Controllers.Outputs
{
    public class AlunoExibicao
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public int Curso { get; set; }
        public EnderecoExibicao Endereco { get; set; }
    }
}