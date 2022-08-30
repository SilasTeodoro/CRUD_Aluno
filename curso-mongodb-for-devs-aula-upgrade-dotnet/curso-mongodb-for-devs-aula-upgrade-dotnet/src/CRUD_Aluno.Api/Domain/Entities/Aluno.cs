using System.Collections.Generic;
using CRUD_Aluno.Api.Domain.Enums;
using CRUD_Aluno.Api.Domain.ValueObjects;
using FluentValidation;
using FluentValidation.Results;

namespace CRUD_Aluno.Api.Domain.Entities
{
    public class Aluno : AbstractValidator<Aluno>
    {
        public Aluno(string nome, ECurso curso)
        {
            Nome = nome;
            Curso = curso;
        }
        
        public Aluno(string id, string nome, ECurso curso)
        {
            Id = id;
            Nome = nome;
            Curso = curso;
        }

        public string Id { get; private set; }
        public string Nome { get; private set; }
        public ECurso Curso { get; private set; }
        public Endereco Endereco { get; private set; }
        public ValidationResult ValidationResult { get; set; }

        public void AtribuirEndereco(Endereco endereco)
        {
            Endereco = endereco;
        }

        public virtual bool Validar()
        {
            ValidarNome();
            ValidationResult = Validate(this);

            ValidarEndereco();

            return ValidationResult.IsValid;
        }

        private void ValidarNome()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("Nome não pode ser nulo.")
                .MaximumLength(40).WithMessage("Maximo 40 caracteres.");
        }

        private void ValidarEndereco()
        {
            if (Endereco.Validar())
                return;

            foreach (var erro in Endereco.ValidationResult.Errors)
                ValidationResult.Errors.Add(erro);
        }
    }
}