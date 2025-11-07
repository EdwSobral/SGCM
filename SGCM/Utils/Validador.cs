using System;
using System.Text.RegularExpressions;

namespace SGCM.Utils
{
    /// <summary>
    /// Classe utilitária para validações de dados
    /// </summary>
    public static class Validador
    {
        /// <summary>
        /// Valida se o CPF está em formato válido
        /// </summary>
        public static bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove caracteres não numéricos
            cpf = Regex.Replace(cpf, @"[^\d]", "");

            // CPF deve ter 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais (CPF inválido)
            bool todosDigitosIguais = true;
            for (int i = 1; i < 11 && todosDigitosIguais; i++)
            {
                if (cpf[i] != cpf[0])
                    todosDigitosIguais = false;
            }

            if (todosDigitosIguais)
                return false;

            // Validação dos dígitos verificadores
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }

        /// <summary>
        /// Valida se o CRM está em formato válido (12345-UF)
        /// </summary>
        public static bool ValidarCRM(string crm)
        {
            if (string.IsNullOrWhiteSpace(crm))
                return false;

            // Formato: 12345-SC (5 dígitos, hífen, 2 letras)
            var regex = new Regex(@"^\d{5}-[A-Z]{2}$");
            return regex.IsMatch(crm);
        }

        /// <summary>
        /// Valida se o e-mail está em formato válido
        /// </summary>
        public static bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

        /// <summary>
        /// Valida se o telefone está em formato válido
        /// </summary>
        public static bool ValidarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return false;

            // Remove caracteres não numéricos
            string tel = Regex.Replace(telefone, @"[^\d]", "");

            // Deve ter 10 ou 11 dígitos (com DDD)
            return tel.Length == 10 || tel.Length == 11;
        }

        /// <summary>
        /// Valida se a data não é passada
        /// </summary>
        public static bool ValidarDataFutura(DateTime data)
        {
            return data.Date >= DateTime.Now.Date;
        }

        /// <summary>
        /// Valida se o horário está em formato válido (HH:MM)
        /// </summary>
        public static bool ValidarHorario(string horario)
        {
            if (string.IsNullOrWhiteSpace(horario))
                return false;

            var regex = new Regex(@"^([01]?[0-9]|2[0-3]):[0-5][0-9]$");
            return regex.IsMatch(horario);
        }

        /// <summary>
        /// Formata CPF para padrão 123.456.789-00
        /// </summary>
        public static string FormatarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return cpf;

            cpf = Regex.Replace(cpf, @"[^\d]", "");

            if (cpf.Length != 11)
                return cpf;

            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        /// <summary>
        /// Formata telefone para padrão (47) 99999-8888
        /// </summary>
        public static string FormatarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return telefone;

            string tel = Regex.Replace(telefone, @"[^\d]", "");

            if (tel.Length == 11)
                return $"({tel.Substring(0, 2)}) {tel.Substring(2, 5)}-{tel.Substring(7, 4)}";
            else if (tel.Length == 10)
                return $"({tel.Substring(0, 2)}) {tel.Substring(2, 4)}-{tel.Substring(6, 4)}";

            return telefone;
        }
    }
}