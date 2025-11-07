using System;
using System.Collections.Generic;
using System.Linq;
using SGCM.Models;
using SGCM.Enums;

namespace SGCM.Controllers
{
    /// <summary>
    /// Gerencia operações de consultas (agendar, cancelar, listar, etc)
    /// </summary>
    public class GerenciadorConsultas
    {
        private List<Consulta> consultas;
        private int proximoId;

        public GerenciadorConsultas()
        {
            consultas = new List<Consulta>();
            proximoId = 1;
        }

        /// <summary>
        /// Agenda uma nova consulta
        /// RN-003: Verifica se horário está disponível
        /// </summary>
        public Consulta AgendarConsulta(Paciente paciente, Medico medico, DateTime dataHora, string observacoes = "")
        {
            // Validações de regras de negócio
            if (paciente == null)
                throw new ArgumentNullException(nameof(paciente));

            if (medico == null)
                throw new ArgumentNullException(nameof(medico));

            if (!paciente.PodeAgendar())
                throw new InvalidOperationException("Paciente inativo não pode agendar consultas (RN-007)");

            if (!medico.VerificarDisponibilidade())
                throw new InvalidOperationException("Médico inativo não pode receber consultas");

            // RN-003: Verifica conflito de horário
            if (!ValidarHorarioDisponivel(medico, dataHora))
                throw new InvalidOperationException("Horário já ocupado para este médico (RN-003)");

            // Cria a consulta
            string id = $"CON-{proximoId:D3}";
            Consulta consulta = new Consulta(id, paciente, medico, dataHora, observacoes);
            
            consultas.Add(consulta);
            
            // Adiciona consulta ao histórico do paciente e médico
            paciente.AdicionarConsulta(consulta);
            medico.AdicionarConsulta(consulta);
            
            proximoId++;

            return consulta;
        }

        /// <summary>
        /// Cancela uma consulta existente
        /// RN-004: Consultas passadas não podem ser canceladas
        /// RN-008: Registra motivo e data do cancelamento
        /// </summary>
        public void CancelarConsulta(string id, string motivo)
        {
            Consulta consulta = BuscarPorId(id);
            
            if (consulta == null)
                throw new ArgumentException("Consulta não encontrada");

            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Motivo do cancelamento é obrigatório (RN-008)");

            // RN-004: Verifica se pode cancelar
            if (!consulta.PodeCancelar())
                throw new InvalidOperationException("Consulta não pode ser cancelada (RN-004)");

            consulta.Cancelar(motivo);
        }

        /// <summary>
        /// Registra atendimento como realizado
        /// </summary>
        public void RegistrarAtendimento(string id, string observacoes = "")
        {
            Consulta consulta = BuscarPorId(id);
            
            if (consulta == null)
                throw new ArgumentException("Consulta não encontrada");

            consulta.RegistrarAtendimento(observacoes);
        }

        /// <summary>
        /// Valida se horário está disponível para o médico
        /// RN-003: Um horário não pode ser agendado duas vezes para o mesmo médico
        /// </summary>
        public bool ValidarHorarioDisponivel(Medico medico, DateTime dataHora)
        {
            if (medico == null)
                return false;

            return !consultas.Any(c => 
                c.Medico.Id == medico.Id && 
                c.DataHora == dataHora && 
                c.Status == StatusConsulta.Agendada);
        }

        /// <summary>
        /// Busca consulta por ID
        /// </summary>
        public Consulta BuscarPorId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return consultas.FirstOrDefault(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Lista todas as consultas do dia
        /// RN-006: Sistema deve exibir todas as consultas do dia ao iniciar
        /// </summary>
        public List<Consulta> ListarConsultasDoDia(DateTime? data = null)
        {
            DateTime dataFiltro = data ?? DateTime.Now;
            
            return consultas
                .Where(c => c.DataHora.Date == dataFiltro.Date)
                .OrderBy(c => c.DataHora)
                .ToList();
        }

        /// <summary>
        /// Lista consultas por paciente
        /// </summary>
        public List<Consulta> ListarPorPaciente(Paciente paciente)
        {
            if (paciente == null)
                return new List<Consulta>();

            return consultas
                .Where(c => c.Paciente.Id == paciente.Id)
                .OrderByDescending(c => c.DataHora)
                .ToList();
        }

        /// <summary>
        /// Lista consultas por médico em uma data específica
        /// </summary>
        public List<Consulta> ListarPorMedico(Medico medico, DateTime? data = null)
        {
            if (medico == null)
                return new List<Consulta>();

            var query = consultas.Where(c => c.Medico.Id == medico.Id);

            if (data.HasValue)
                query = query.Where(c => c.DataHora.Date == data.Value.Date);

            return query.OrderBy(c => c.DataHora).ToList();
        }

        /// <summary>
        /// Lista consultas por status
        /// </summary>
        public List<Consulta> ListarPorStatus(StatusConsulta status)
        {
            return consultas
                .Where(c => c.Status == status)
                .OrderBy(c => c.DataHora)
                .ToList();
        }

        /// <summary>
        /// Lista todas as consultas
        /// </summary>
        public List<Consulta> ListarTodas()
        {
            return consultas.OrderBy(c => c.DataHora).ToList();
        }

        /// <summary>
        /// Lista consultas pendentes (agendadas) do dia atual
        /// </summary>
        public List<Consulta> ListarPendentesDodia()
        {
            DateTime hoje = DateTime.Now.Date;
            
            return consultas
                .Where(c => c.DataHora.Date == hoje && c.Status == StatusConsulta.Agendada)
                .OrderBy(c => c.DataHora)
                .ToList();
        }

        /// <summary>
        /// Verifica consultas próximas (nas próximas 2 horas)
        /// </summary>
        public List<Consulta> VerificarConsultasProximas()
        {
            return consultas
                .Where(c => c.EstaProxima())
                .OrderBy(c => c.DataHora)
                .ToList();
        }

        /// <summary>
        /// Lista consultas atrasadas
        /// </summary>
        public List<Consulta> ListarAtrasadas()
        {
            return consultas
                .Where(c => c.EstaAtrasada())
                .OrderBy(c => c.DataHora)
                .ToList();
        }

        /// <summary>
        /// Obtém estatísticas de consultas para uma data
        /// </summary>
        public (int Total, int Agendadas, int Concluidas, int Canceladas) ObterEstatisticas(DateTime? data = null)
        {
            DateTime dataFiltro = data ?? DateTime.Now;
            var consultasDia = ListarConsultasDoDia(dataFiltro);

            int total = consultasDia.Count;
            int agendadas = consultasDia.Count(c => c.Status == StatusConsulta.Agendada);
            int concluidas = consultasDia.Count(c => c.Status == StatusConsulta.Concluida);
            int canceladas = consultasDia.Count(c => c.Status == StatusConsulta.Cancelada);

            return (total, agendadas, concluidas, canceladas);
        }

        /// <summary>
        /// Gera relatório do dia
        /// </summary>
        public string GerarRelatorioDia(DateTime? data = null)
        {
            DateTime dataFiltro = data ?? DateTime.Now;
            var consultasDia = ListarConsultasDoDia(dataFiltro);
            var stats = ObterEstatisticas(dataFiltro);

            string relatorio = "╔══════════════════════════════════════════════════════════════════╗\n";
            relatorio += $"║  RELATÓRIO DE CONSULTAS - {dataFiltro:dd/MM/yyyy}                     ║\n";
            relatorio += "╚══════════════════════════════════════════════════════════════════╝\n\n";

            relatorio += "RESUMO GERAL:\n";
            relatorio += $"  • Total de consultas: {stats.Total}\n";
            
            if (stats.Total > 0)
            {
                double percConcluidas = (stats.Concluidas * 100.0) / stats.Total;
                double percCanceladas = (stats.Canceladas * 100.0) / stats.Total;
                double percPendentes = (stats.Agendadas * 100.0) / stats.Total;

                relatorio += $"  • Concluídas: {stats.Concluidas} ({percConcluidas:F0}%)\n";
                relatorio += $"  • Canceladas: {stats.Canceladas} ({percCanceladas:F0}%)\n";
                relatorio += $"  • Pendentes: {stats.Agendadas} ({percPendentes:F0}%)\n";
                relatorio += $"  • Taxa de ocupação: {percConcluidas + percCanceladas:F0}%\n\n";
            }

            // Consultas Concluídas
            var concluidas = consultasDia.Where(c => c.Status == StatusConsulta.Concluida).ToList();
            if (concluidas.Any())
            {
                relatorio += $"CONSULTAS CONCLUÍDAS ({concluidas.Count}):\n";
                foreach (var c in concluidas)
                {
                    relatorio += $"  [{c.DataHora:HH:mm}] {c.Medico.Nome} - {c.Paciente.Nome}\n";
                }
                relatorio += "\n";
            }

            // Consultas Canceladas
            var canceladas = consultasDia.Where(c => c.Status == StatusConsulta.Cancelada).ToList();
            if (canceladas.Any())
            {
                relatorio += $"CONSULTAS CANCELADAS ({canceladas.Count}):\n";
                foreach (var c in canceladas)
                {
                    relatorio += $"  [{c.DataHora:HH:mm}] {c.Medico.Nome} - {c.Paciente.Nome}\n";
                    relatorio += $"    Motivo: {c.MotivoCancelamento}\n";
                }
                relatorio += "\n";
            }

            // Consultas Pendentes
            var pendentes = consultasDia.Where(c => c.Status == StatusConsulta.Agendada).ToList();
            if (pendentes.Any())
            {
                relatorio += $"CONSULTAS PENDENTES ({pendentes.Count}):\n";
                foreach (var c in pendentes)
                {
                    relatorio += $"  [{c.DataHora:HH:mm}] {c.Medico.Nome} - {c.Paciente.Nome}\n";
                }
                relatorio += "\n";
            }

            // Estatísticas por médico
            var porMedico = consultasDia.GroupBy(c => c.Medico.Nome);
            if (porMedico.Any())
            {
                relatorio += "ESTATÍSTICAS POR MÉDICO:\n";
                foreach (var grupo in porMedico)
                {
                    int totalMed = grupo.Count();
                    int concluidasMed = grupo.Count(c => c.Status == StatusConsulta.Concluida);
                    int canceladasMed = grupo.Count(c => c.Status == StatusConsulta.Cancelada);
                    int pendentesMed = grupo.Count(c => c.Status == StatusConsulta.Agendada);

                    relatorio += $"  {grupo.Key}:\n";
                    relatorio += $"    Total: {totalMed} | Concluídas: {concluidasMed} | ";
                    relatorio += $"Canceladas: {canceladasMed} | Pendentes: {pendentesMed}\n";
                }
            }

            relatorio += $"\nRelatório gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}\n";

            return relatorio;
        }

        /// <summary>
        /// Gera relatório de cancelamentos
        /// </summary>
        public string GerarRelatorioCancelamentos(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            DateTime inicio = dataInicio ?? DateTime.Now.AddMonths(-1);
            DateTime fim = dataFim ?? DateTime.Now;

            var canceladas = consultas
                .Where(c => c.Status == StatusConsulta.Cancelada && 
                            c.DataCancelamento.HasValue &&
                            c.DataCancelamento.Value.Date >= inicio.Date && 
                            c.DataCancelamento.Value.Date <= fim.Date)
                .OrderByDescending(c => c.DataCancelamento)
                .ToList();

            string relatorio = "╔══════════════════════════════════════════════════════════════════╗\n";
            relatorio +=       "║           RELATÓRIO DE CONSULTAS CANCELADAS                      ║\n";
            relatorio +=       "╚══════════════════════════════════════════════════════════════════╝\n\n";

            relatorio += $"Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}\n";
            relatorio += $"Total de cancelamentos: {canceladas.Count}\n\n";

            if (canceladas.Any())
            {
                relatorio += "LISTA DE CANCELAMENTOS:\n";
                relatorio += "─────────────────────────────────────────────────────────────────\n";

                foreach (var c in canceladas)
                {
                    relatorio += $"ID: {c.Id}\n";
                    relatorio += $"Data original: {c.DataHora:dd/MM/yyyy HH:mm}\n";
                    relatorio += $"Paciente: {c.Paciente.Nome}\n";
                    relatorio += $"Médico: {c.Medico.Nome} ({c.Medico.ObterEspecialidadeFormatada()})\n";
                    relatorio += $"Motivo: {c.MotivoCancelamento}\n";
                    relatorio += $"Cancelado em: {c.DataCancelamento:dd/MM/yyyy HH:mm}\n";
                    relatorio += "─────────────────────────────────────────────────────────────────\n";
                }
            }
            else
            {
                relatorio += "Nenhuma consulta cancelada no período.\n";
            }

            relatorio += $"\nRelatório gerado em: {DateTime.Now:dd/MM/yyyy HH:mm}\n";

            return relatorio;
        }

        /// <summary>
        /// Retorna quantidade total de consultas
        /// </summary>
        public int ObterTotal()
        {
            return consultas.Count;
        }
    }
}