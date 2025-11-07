using System;
using System.Collections.Generic;
using SGCM.Enums;
using SGCM.Utils;

namespace SGCM.Models
{
    /// <summary>
    /// Representa um paciente do consultório
    /// </summary>
    public class Paciente
    {
        // Propriedades
        public string Id { get; private set; }
        public string Nome { get; set; }
        public string CPF { get; private set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public StatusPaciente Status { get; set; }
        public DateTime DataCadastro { get; private set; }
        public List<Consulta> Consultas { get; private set; }

        // Construtor
        public Paciente(string id, string nome, string cpf, string telefone, string email)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório");

            if (!Validador.ValidarCPF(cpf))
                throw new ArgumentException("CPF inválido");

            if (!Validador.ValidarTelefone(telefone))
                throw new ArgumentException("Telefone inválido");

            if (!Validador.ValidarEmail(email))
                throw new ArgumentException("E-mail inválido");

            Id = id;
            Nome = nome;
            CPF = Validador.FormatarCPF(cpf);
            Telefone = Validador.FormatarTelefone(telefone);
            Email = email;
            Status = StatusPaciente.Ativo;
            DataCadastro = DateTime.Now;
            Consultas = new List<Consulta>();
        }

        // Métodos
        
        /// <summary>
        /// Ativa o paciente no sistema
        /// </summary>
        public void Ativar()
        {
            Status = StatusPaciente.Ativo;
        }

        /// <summary>
        /// Inativa o paciente no sistema
        /// </summary>
        public void Inativar()
        {
            Status = StatusPaciente.Inativo;
        }

        /// <summary>
        /// Atualiza dados do paciente
        /// </summary>
        public void AtualizarDados(string telefone, string email)
        {
            if (!Validador.ValidarTelefone(telefone))
                throw new ArgumentException("Telefone inválido");

            if (!Validador.ValidarEmail(email))
                throw new ArgumentException("E-mail inválido");

            Telefone = Validador.FormatarTelefone(telefone);
            Email = email;
        }

        /// <summary>
        /// Verifica se o paciente pode agendar consultas
        /// </summary>
        public bool PodeAgendar()
        {
            return Status == StatusPaciente.Ativo;
        }

        /// <summary>
        /// Obtém histórico de consultas do paciente
        /// </summary>
        public List<Consulta> ObterHistorico()
        {
            // Ordena por data decrescente (mais recente primeiro)
            Consultas.Sort((c1, c2) => c2.DataHora.CompareTo(c1.DataHora));
            return Consultas;
        }

        /// <summary>
        /// Adiciona consulta ao histórico
        /// </summary>
        public void AdicionarConsulta(Consulta consulta)
        {
            if (consulta == null)
                throw new ArgumentNullException(nameof(consulta));

            Consultas.Add(consulta);
        }

        /// <summary>
        /// Retorna representação em string do paciente
        /// </summary>
        public override string ToString()
        {
            return $"{Id} - {Nome} (CPF: {CPF}) - Status: {Status}";
        }
    }
}