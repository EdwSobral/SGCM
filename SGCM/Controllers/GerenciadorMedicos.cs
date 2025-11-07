using System;
using System.Collections.Generic;
using System.Linq;
using SGCM.Models;
using SGCM.Enums;

namespace SGCM.Controllers
{
    /// <summary>
    /// Gerencia operações CRUD de médicos
    /// </summary>
    public class GerenciadorMedicos
    {
        private List<Medico> medicos;
        private int proximoId;

        public GerenciadorMedicos()
        {
            medicos = new List<Medico>();
            proximoId = 1;
        }

        /// <summary>
        /// Cadastra um novo médico
        /// </summary>
        public Medico CadastrarMedico(string nome, string crm, Especialidade especialidade, string telefone)
        {
            // Verifica se CRM já existe (RN-002)
            if (BuscarPorCRM(crm) != null)
                throw new InvalidOperationException("CRM já cadastrado no sistema");

            string id = $"MED-{proximoId:D3}";
            Medico medico = new Medico(id, nome, crm, especialidade, telefone);
            
            medicos.Add(medico);
            proximoId++;

            return medico;
        }

        /// <summary>
        /// Edita dados de um médico existente
        /// </summary>
        public void EditarMedico(string id, Especialidade especialidade, string telefone)
        {
            Medico medico = BuscarPorId(id);
            
            if (medico == null)
                throw new ArgumentException("Médico não encontrado");

            medico.AtualizarDados(especialidade, telefone);
        }

        /// <summary>
        /// Inativa um médico
        /// </summary>
        public void InativarMedico(string id)
        {
            Medico medico = BuscarPorId(id);
            
            if (medico == null)
                throw new ArgumentException("Médico não encontrado");

            medico.Inativar();
        }

        /// <summary>
        /// Reativa um médico
        /// </summary>
        public void ReativarMedico(string id)
        {
            Medico medico = BuscarPorId(id);
            
            if (medico == null)
                throw new ArgumentException("Médico não encontrado");

            medico.Ativar();
        }

        /// <summary>
        /// Busca médico por ID
        /// </summary>
        public Medico BuscarPorId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return medicos.FirstOrDefault(m => m.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Busca médico por CRM
        /// </summary>
        public Medico BuscarPorCRM(string crm)
        {
            if (string.IsNullOrWhiteSpace(crm))
                return null;

            // Remove formatação do CRM para comparação
            string crmLimpo = crm.Replace("-", "").ToUpper();
            
            return medicos.FirstOrDefault(m => 
                m.CRM.Replace("-", "").Equals(crmLimpo, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Busca médicos por nome (busca parcial)
        /// </summary>
        public List<Medico> BuscarPorNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return new List<Medico>();

            return medicos
                .Where(m => m.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase))
                .OrderBy(m => m.Nome)
                .ToList();
        }

        /// <summary>
        /// Lista todos os médicos
        /// </summary>
        public List<Medico> ListarTodos()
        {
            return medicos.OrderBy(m => m.Nome).ToList();
        }

        /// <summary>
        /// Lista apenas médicos ativos
        /// </summary>
        public List<Medico> ListarAtivos()
        {
            return medicos
                .Where(m => m.Status == StatusMedico.Ativo)
                .OrderBy(m => m.Nome)
                .ToList();
        }

        /// <summary>
        /// Lista apenas médicos inativos
        /// </summary>
        public List<Medico> ListarInativos()
        {
            return medicos
                .Where(m => m.Status == StatusMedico.Inativo)
                .OrderBy(m => m.Nome)
                .ToList();
        }

        /// <summary>
        /// Lista médicos por especialidade
        /// </summary>
        public List<Medico> ListarPorEspecialidade(Especialidade especialidade)
        {
            return medicos
                .Where(m => m.Especialidade == especialidade && m.Status == StatusMedico.Ativo)
                .OrderBy(m => m.Nome)
                .ToList();
        }

        /// <summary>
        /// Lista médicos com disponibilidade em uma data/hora
        /// </summary>
        public List<Medico> ListarDisponiveis(DateTime dataHora)
        {
            return medicos
                .Where(m => m.Status == StatusMedico.Ativo && m.TemHorarioLivre(dataHora))
                .OrderBy(m => m.Nome)
                .ToList();
        }

        /// <summary>
        /// Retorna quantidade total de médicos
        /// </summary>
        public int ObterTotal()
        {
            return medicos.Count;
        }

        /// <summary>
        /// Retorna quantidade de médicos ativos
        /// </summary>
        public int ObterTotalAtivos()
        {
            return medicos.Count(m => m.Status == StatusMedico.Ativo);
        }

        /// <summary>
        /// Retorna estatísticas de médicos por especialidade
        /// </summary>
        public Dictionary<Especialidade, int> ObterEstatisticasPorEspecialidade()
        {
            return medicos
                .Where(m => m.Status == StatusMedico.Ativo)
                .GroupBy(m => m.Especialidade)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}