using System;
using System.Collections.Generic;
using System.Linq;
using SGCM.Models;
using SGCM.Enums;

namespace SGCM.Controllers
{
    /// <summary>
    /// Gerencia operações CRUD de pacientes
    /// </summary>
    public class GerenciadorPacientes
    {
        private List<Paciente> pacientes;
        private int proximoId;

        public GerenciadorPacientes()
        {
            pacientes = new List<Paciente>();
            proximoId = 1;
        }

        /// <summary>
        /// Cadastra um novo paciente
        /// </summary>
        public Paciente CadastrarPaciente(string nome, string cpf, string telefone, string email)
        {
            // Verifica se CPF já existe (RN-001)
            if (BuscarPorCPF(cpf) != null)
                throw new InvalidOperationException("CPF já cadastrado no sistema");

            string id = $"PAC-{proximoId:D3}";
            Paciente paciente = new Paciente(id, nome, cpf, telefone, email);
            
            pacientes.Add(paciente);
            proximoId++;

            return paciente;
        }

        /// <summary>
        /// Edita dados de um paciente existente
        /// </summary>
        public void EditarPaciente(string id, string telefone, string email)
        {
            Paciente paciente = BuscarPorId(id);
            
            if (paciente == null)
                throw new ArgumentException("Paciente não encontrado");

            paciente.AtualizarDados(telefone, email);
        }

        /// <summary>
        /// Inativa um paciente
        /// </summary>
        public void InativarPaciente(string id)
        {
            Paciente paciente = BuscarPorId(id);
            
            if (paciente == null)
                throw new ArgumentException("Paciente não encontrado");

            paciente.Inativar();
        }

        /// <summary>
        /// Reativa um paciente
        /// </summary>
        public void ReativarPaciente(string id)
        {
            Paciente paciente = BuscarPorId(id);
            
            if (paciente == null)
                throw new ArgumentException("Paciente não encontrado");

            paciente.Ativar();
        }

        /// <summary>
        /// Busca paciente por ID
        /// </summary>
        public Paciente BuscarPorId(string id)
        {
            return pacientes.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Busca paciente por CPF
        /// </summary>
        public Paciente BuscarPorCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return null;

            // Remove formatação do CPF para comparação
            string cpfLimpo = cpf.Replace(".", "").Replace("-", "");
            
            return pacientes.FirstOrDefault(p => 
                p.CPF.Replace(".", "").Replace("-", "").Equals(cpfLimpo, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Busca pacientes por nome (busca parcial)
        /// </summary>
        public List<Paciente> BuscarPorNome(string nome)
        {
            return pacientes
                .Where(p => p.Nome.Contains(nome, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p.Nome)
                .ToList();
        }

        /// <summary>
        /// Lista todos os pacientes
        /// </summary>
        public List<Paciente> ListarTodos()
        {
            return pacientes.OrderBy(p => p.Nome).ToList();
        }

        /// <summary>
        /// Lista apenas pacientes ativos
        /// </summary>
        public List<Paciente> ListarAtivos()
        {
            return pacientes
                .Where(p => p.Status == StatusPaciente.Ativo)
                .OrderBy(p => p.Nome)
                .ToList();
        }

        /// <summary>
        /// Lista apenas pacientes inativos
        /// </summary>
        public List<Paciente> ListarInativos()
        {
            return pacientes
                .Where(p => p.Status == StatusPaciente.Inativo)
                .OrderBy(p => p.Nome)
                .ToList();
        }

        /// <summary>
        /// Retorna quantidade total de pacientes
        /// </summary>
        public int ObterTotal()
        {
            return pacientes.Count;
        }

        /// <summary>
        /// Retorna quantidade de pacientes ativos
        /// </summary>
        public int ObterTotalAtivos()
        {
            return pacientes.Count(p => p.Status == StatusPaciente.Ativo);
        }
    }
}