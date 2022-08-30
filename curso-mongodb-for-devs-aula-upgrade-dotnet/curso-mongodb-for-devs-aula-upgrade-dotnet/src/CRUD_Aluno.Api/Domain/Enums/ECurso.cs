using System;

namespace CRUD_Aluno.Api.Domain.Enums
{
    public enum ECurso
    {
        Ingles = 1,
        Espanhol = 2,
        Italiano = 3,
        Frances = 4,
        Japones = 5
    }

    public static class ECursoHelper
    {
        public static ECurso ConverterDeInteiro(int valor)
        {
            if (Enum.TryParse(valor.ToString(), out ECurso curso))
                return curso;

            throw new ArgumentOutOfRangeException("curso");
        }
    }
}