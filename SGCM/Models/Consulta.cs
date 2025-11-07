using System;
using SGCM.Enums;

namespace SGCM.Models
{
    /// <summary>
    /// Representa uma consulta médica
    /// </summary>
    public class Consulta
    {
        // Propriedades
        public string Id { get; private set; }
        public Paciente Paciente { get; private set; }
        public Medico Medico { get; private set; }
        public DateTime DataHora { get; private set; }
        public StatusConsulta Status { get; private set; }
        public string Observacoes { get; set; }
        public DateTime DataAgendamento { get; private set; }
        public DateTime? DataCancelamento { get; private set; }
        public string MotivoCancelamento { get; private set; }
        public DateTime? DataConclusao { get; private set; }
        public string ObservacoesAtendimento { get; private set; }

        // Construtor
        public Consulta(string id, Paciente paciente, Medico medico, DateTime dataHora, string observacoes = "")
        {
            // Validações
            if (paciente == null)
                throw new ArgumentNullException(nameof(paciente), "Paciente é obrigatório");

            if (medico == null)
                throw new ArgumentNullException(nameof(medico), "Médico é obrigatório");

            if (!ValidarData(dataHora))
                throw new ArgumentException("Data/hora da consulta não pode ser no passado");

            if (!paciente.PodeAgendar())
                throw new InvalidOperationException("Paciente inativo não pode agendar consultas");

            if (!medico.VerificarDisponibilidade())
                throw new InvalidOperationException("Médico inativo não pode receber consultas");

            Id = id;
            Paciente = paciente;
            Medico = medico;
            DataHora = dataHora;
            Status = StatusConsulta.Agendada;
            Observacoes = observacoes;
            DataAgendamento = DateTime.Now;
        }

        // Métodos

        /// <summary>
        /// Valida se a data não é passada
        /// </summary>
        private bool ValidarData(DateTime data)
        {
            return data.Date >= DateTime.Now.Date;
        }

        /// <summary>
        /// Valida se o horário está livre
        /// </summary>
        public bool ValidarHorario()
        {
            return Medico.TemHorarioLivre(DataHora);
        }

        /// <summary>
        /// Verifica se existe conflito de horário
        /// </summary>
        public bool VerificarConflito()
        {
            return !ValidarHorario();
        }

        /// <summary>
        /// Cancela a consulta
        /// </summary>
        public void Cancelar(string motivo)
        {
            if (!PodeCancelar())
                throw new InvalidOperationException("Consulta não pode ser cancelada (já foi realizada ou é passada)");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Motivo do cancelamento é obrigatório");

            Status = StatusConsulta.Cancelada;
            MotivoCancelamento = motivo;
            DataCancelamento = DateTime.Now;
        }

        /// <summary>
        /// Registra o atendimento como realizado
        /// </summary>
        public void RegistrarAtendimento(string observacoesAtendimento = "")
        {
            if (Status != StatusConsulta.Agendada)
                throw new InvalidOperationException("Apenas consultas agendadas podem ser registradas como concluídas");

            Status = StatusConsulta.Concluida;
            ObservacoesAtendimento = observacoesAtendimento;
            DataConclusao = DateTime.Now;
        }

        /// <summary>
        /// Verifica se a consulta pode ser cancelada
        /// RN-004: Consultas passadas não podem ser canceladas
        /// </summary>
        public bool PodeCancelar()
        {
            // Não pode cancelar se já foi concluída ou cancelada
            if (Status != StatusConsulta.Agendada)
                return false;

            // Não pode cancelar consultas passadas
            return DataHora.Date >= DateTime.Now.Date;
        }

        /// <summary>
        /// Verifica se a consulta está próxima (nas próximas 2 horas)
        /// </summary>
        public bool EstaProxima()
        {
            if (Status != StatusConsulta.Agendada)
                return false;

            TimeSpan diferenca = DataHora - DateTime.Now;
            return diferenca.TotalHours > 0 && diferenca.TotalHours <= 2;
        }

        /// <summary>
        /// Verifica se a consulta está atrasada
        /// </summary>
        public bool EstaAtrasada()
        {
            if (Status != StatusConsulta.Agendada)
                return false;

            return DateTime.Now > DataHora;
        }

        /// <summary>
        /// Obtém resumo da consulta
        /// </summary>
        public string ObterResumo()
        {
            string statusStr = Status switch
            {
                StatusConsulta.Agendada => "⏰ AGENDADA",
                StatusConsulta.Concluida => "✓ CONCLUÍDA",
                StatusConsulta.Cancelada => "✗ CANCELADA",
                _ => Status.ToString()
            };

            string resultado = $"[{Id}] {DataHora:dd/MM/yyyy HH:mm} - {statusStr}\n";
            resultado += $"Paciente: {Paciente.Nome}\n";
            resultado += $"Médico: {Medico.Nome} ({Medico.ObterEspecialidadeFormatada()})\n";

            if (!string.IsNullOrWhiteSpace(Observacoes))
                resultado += $"Observações: {Observacoes}\n";

            if (Status == StatusConsulta.Cancelada)
            {
                resultado += $"Cancelada em: {DataCancelamento:dd/MM/yyyy HH:mm}\n";
                resultado += $"Motivo: {MotivoCancelamento}\n";
            }

            if (Status == StatusConsulta.Concluida)
            {
                resultado += $"Concluída em: {DataConclusao:dd/MM/yyyy HH:mm}\n";
                if (!string.IsNullOrWhiteSpace(ObservacoesAtendimento))
                    resultado += $"Obs. Atendimento: {ObservacoesAtendimento}\n";
            }

            return resultado;
        }

        /// <summary>
        /// Retorna representação em string da consulta
        /// </summary>
        public override string ToString()
        {
            return $"{Id} - {DataHora:dd/MM/yyyy HH:mm} - {Paciente.Nome} com {Medico.Nome} - {Status}";
        }
    }
}