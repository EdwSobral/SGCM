using System;
using System.Collections.Generic;
using System.Linq;
using SGCM.Enums;
using SGCM.Utils;

namespace SGCM.Models
{
    /// <summary>
    /// Representa um médico do consultório
    /// </summary>
    public class Medico
    {
        // Propriedades
        public string Id { get; private set; }
        public string Nome { get; set; }
        public string CRM { get; private set; }
        public Especialidade Especialidade { get; set; }
        public string Telefone { get; set; }
        public StatusMedico Status { get; set; }
        public DateTime DataCadastro { get; private set; }
        public List<Consulta> Consultas { get; private set; }

        // Construtor
        public Medico(string id, string nome, string crm, Especialidade especialidade, string telefone)
        {
            // Validações
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório");

            if (!Validador.ValidarCRM(crm))
                throw new ArgumentException("CRM inválido. Formato esperado: 12345-UF");

            if (!Validador.ValidarTelefone(telefone))
                throw new ArgumentException("Telefone inválido");

            Id = id;
            Nome = nome;
            CRM = crm.ToUpper();
            Especialidade = especialidade;
            Telefone = Validador.FormatarTelefone(telefone);
            Status = StatusMedico.Ativo;
            DataCadastro = DateTime.Now;
            Consultas = new List<Consulta>();
        }

        // Métodos

        /// <summary>
        /// Ativa o médico no sistema
        /// </summary>
        public void Ativar()
        {
            Status = StatusMedico.Ativo;
        }

        /// <summary>
        /// Inativa o médico no sistema
        /// </summary>
        public void Inativar()
        {
            Status = StatusMedico.Inativo;
        }

        /// <summary>
        /// Atualiza dados do médico
        /// </summary>
        public void AtualizarDados(Especialidade especialidade, string telefone)
        {
            if (!Validador.ValidarTelefone(telefone))
                throw new ArgumentException("Telefone inválido");

            Especialidade = especialidade;
            Telefone = Validador.FormatarTelefone(telefone);
        }

        /// <summary>
        /// Verifica se o médico pode receber consultas
        /// </summary>
        public bool VerificarDisponibilidade()
        {
            return Status == StatusMedico.Ativo;
        }

        /// <summary>
        /// Obtém agenda do médico para uma data específica
        /// </summary>
        public List<Consulta> ObterAgenda(DateTime data)
        {
            return Consultas
                .Where(c => c.DataHora.Date == data.Date)
                .OrderBy(c => c.DataHora)
                .ToList();
        }

        /// <summary>
        /// Verifica se médico tem horário livre em uma data/hora específica
        /// </summary>
        public bool TemHorarioLivre(DateTime dataHora)
        {
            return !Consultas.Any(c => 
                c.DataHora == dataHora && 
                c.Status == StatusConsulta.Agendada);
        }

        /// <summary>
        /// Adiciona consulta à agenda
        /// </summary>
        public void AdicionarConsulta(Consulta consulta)
        {
            if (consulta == null)
                throw new ArgumentNullException(nameof(consulta));

            Consultas.Add(consulta);
        }

        /// <summary>
        /// Retorna nome da especialidade formatado
        /// </summary>
        public string ObterEspecialidadeFormatada()
        {
            return Especialidade switch
            {
                Especialidade.ClinicaGeral => "Clínica Geral",
                Especialidade.Cardiologia => "Cardiologia",
                Especialidade.Pediatria => "Pediatria",
                Especialidade.Ortopedia => "Ortopedia",
                Especialidade.Dermatologia => "Dermatologia",
                Especialidade.Ginecologia => "Ginecologia",
                _ => Especialidade.ToString()
            };
        }

        /// <summary>
        /// Retorna representação em string do médico
        /// </summary>
        public override string ToString()
        {
            return $"{Id} - {Nome} (CRM: {CRM}) - {ObterEspecialidadeFormatada()} - Status: {Status}";
        }
    }
}