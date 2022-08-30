using System.Linq;
using System.Threading.Tasks;
using CRUD_Aluno.Api.Controllers.Inputs;
using CRUD_Aluno.Api.Controllers.Outputs;
using CRUD_Aluno.Api.Data.Repositories;
using CRUD_Aluno.Api.Domain.Entities;
using CRUD_Aluno.Api.Domain.Enums;
using CRUD_Aluno.Api.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace CRUD_Aluno.Api.Controllers
{
    [ApiController]
    public class AlunoController : ControllerBase
    {
        private readonly AlunoRepository _alunoRepository;
        private object totalAlunoRemovido;

        public AlunoController(AlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        [HttpPost("aluno")]
        public ActionResult IncluirAluno([FromBody] AlunoInclusao alunoInclusao)
        {
            var curso = ECursoHelper.ConverterDeInteiro(alunoInclusao.Curso);

            var aluno = new Aluno(alunoInclusao.Nome, curso);
            var endereco = new Endereco(
                alunoInclusao.Logradouro,
                alunoInclusao.Numero,
                alunoInclusao.Cidade,
                alunoInclusao.UF,
                alunoInclusao.Cep);

            aluno.AtribuirEndereco(endereco);

            if (!aluno.Validar())
            {
                return BadRequest(
                    new
                    {
                        errors = aluno.ValidationResult.Errors.Select(_ => _.ErrorMessage)
                    });
            }

            _alunoRepository.Inserir(aluno);

            return Ok(
                new
                {
                    data = "Aluno inserido com sucesso"
                }
            );
        }

        [HttpGet("aluno/todos")]
        public async Task<ActionResult> ObterAluno()
        {
            var aluno = await _alunoRepository.ObterTodos();

            var listagem = aluno.Select(_ => new AlunoListagem
            {
                Id = _.Id,
                Nome = _.Nome,
                Curso = (int)_.Curso,
                Cidade = _.Endereco.Cidade
            });

            return Ok(
                new
                {
                    data = listagem
                }
            );
        }

        [HttpGet("aluno/{id}")]
        public ActionResult ObterAluno(string id)
        {
            var aluno = _alunoRepository.ObterPorId(id);

            if (aluno == null)
                return NotFound();

            var exibicao = new AlunoExibicao
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Curso = (int)aluno.Curso,
                Endereco = new EnderecoExibicao
                {
                    Logradouro = aluno.Endereco.Logradouro,
                    Numero = aluno.Endereco.Numero,
                    Cidade = aluno.Endereco.Cidade,
                    Cep = aluno.Endereco.Cep,
                    UF = aluno.Endereco.UF
                }
            };

            return Ok(
                new
                {
                    data = exibicao
                }
            );
        }

        [HttpPut("aluno")]
        public ActionResult AlterarAluno([FromBody] AlunoAlteracaoCompleta alunoAlteracaoCompleta)
        {
            var aluno = _alunoRepository.ObterPorId(alunoAlteracaoCompleta.Id);

            if (aluno == null)
                return NotFound();

            var curso = ECursoHelper.ConverterDeInteiro(alunoAlteracaoCompleta.Curso);
            aluno = new Aluno(alunoAlteracaoCompleta.Id, alunoAlteracaoCompleta.Nome, curso);
            var endereco = new Endereco(
                alunoAlteracaoCompleta.Logradouro,
                alunoAlteracaoCompleta.Numero,
                alunoAlteracaoCompleta.Cidade,
                alunoAlteracaoCompleta.UF,
                alunoAlteracaoCompleta.Cep);

            aluno.AtribuirEndereco(endereco);

            if (!aluno.Validar())
            {
                return BadRequest(
                    new
                    {
                        errors = aluno.ValidationResult.Errors.Select(_ => _.ErrorMessage)
                    });
            }

            if (!_alunoRepository.AlterarCompleto(aluno))
            {
                return BadRequest(new
                {
                    errors = "Nenhum documento foi alterado"
                });
            }

            return Ok(
                new
                {
                    data = "Aluno alterado com sucesso"
                }
            );
        }

        [HttpPatch("aluno/{id}")]
        public ActionResult AlterarCurso(string id, [FromBody] AlunoAlteracaoParcial alunoAlteracaoParcial)
        {
            var aluno = _alunoRepository.ObterPorId(id);

            if (aluno == null)
                return NotFound();

            var curso = ECursoHelper.ConverterDeInteiro(alunoAlteracaoParcial.Curso);

            if (!_alunoRepository.AlterarCurso(id, curso))
            {
                return BadRequest(new
                {
                    errors = "Nenhum documento foi alterado"
                });
            }

            return Ok(
                new
                {
                    data = "Aluno alterado com sucesso"
                }
            );
        }

        [HttpGet("aluno")]
        public ActionResult ObterAlunoPorNome([FromQuery] string nome)
        {
            var aluno = _alunoRepository.ObterPorNome(nome);

             var listagem = aluno.Select(_ => new AlunoListagem
            {
                Id = _.Id,
                Nome = _.Nome,
                Curso = (int)_.Curso,
                Cidade = _.Endereco.Cidade
            });

            return Ok(
                new
                {
                    data = listagem
                }
            );
        }

        [HttpDelete("aluno/{id}")]
        public ActionResult Remover(string id)
        {
            var aluno = _alunoRepository.ObterPorId(id);

            if (aluno == null)
                return NotFound();

            object var = (totalAlunoRemovido) = _alunoRepository.Remover(id);

            return Ok(
                new
                {
                    data = $"Total de exclusões: {totalAlunoRemovido}"
                }
            );
        }

        [HttpGet("aluno/textual")]
        public async Task<ActionResult> ObterAlunoPorBuscaTextual([FromQuery] string texto)
        {
            var aluno = await _alunoRepository.ObterPorBuscaTextual(texto);

            var listagem = aluno.ToList().Select(_ => new AlunoListagem
            {
                Id = _.Id,
                Nome = _.Nome,
                Curso = (int)_.Curso,
                Cidade = _.Endereco.Cidade
            });

            return Ok(
                new
                {
                    data = listagem
                }
            );
        }
    }
}