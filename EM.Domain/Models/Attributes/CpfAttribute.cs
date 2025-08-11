using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EM.Domain.Models.Attributes
{
    public class CpfAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            // CPF é opcional - se estiver vazio, é válido
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return true;

            var cpf = value.ToString()!.Replace(".", "").Replace("-", "");
            
            // Se informado, deve ter 11 dígitos
            if (cpf.Length != 11 || !Regex.IsMatch(cpf, @"^\d{11}$")) 
                return false;

            // Validação matemática do CPF
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}