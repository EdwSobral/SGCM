using SGCM.Models;
using SGCM.Controllers;
using SGCM.Enums;

namespace SGCM.Data
{
    public static class CargaDados
    {
        // Método para uso no Program.cs
        public static void CarregarDadosExemplo(
            GerenciadorPacientes gerenciadorPacientes,
            GerenciadorMedicos gerenciadorMedicos,
            GerenciadorConsultas gerenciadorConsultas)
        {
            Inicializar(gerenciadorPacientes, gerenciadorMedicos, gerenciadorConsultas);
        }

        public static void Inicializar(
            GerenciadorPacientes gerenciadorPacientes,
            GerenciadorMedicos gerenciadorMedicos,
            GerenciadorConsultas gerenciadorConsultas)
        {
            // Verifica se já existem dados
            if (gerenciadorPacientes.ListarTodos().Any())
            {
                return; // Dados já foram carregados
            }

            Console.WriteLine("Carregando dados iniciais...");

            // Cadastrar Pacientes
            var paciente1 = gerenciadorPacientes.CadastrarPaciente(
                "Maria Silva Santos",
                "123.456.789-00",
                "(47) 99999-1111",
                "maria.silva@email.com"
            );

            var paciente2 = gerenciadorPacientes.CadastrarPaciente(
                "João Pedro Oliveira",
                "987.654.321-00",
                "(47) 99999-2222",
                "joao.oliveira@email.com"
            );

            var paciente3 = gerenciadorPacientes.CadastrarPaciente(
                "Ana Costa Ferreira",
                "456.789.123-00",
                "(47) 99999-3333",
                "ana.costa@email.com"
            );

            // Cadastrar Médicos
            var medico1 = gerenciadorMedicos.CadastrarMedico(
                "Dr. Carlos Eduardo Mendes",
                "CRM-SC 12345",
                Especialidade.Cardiologia,
                "(47) 3333-1111"
            );

            var medico2 = gerenciadorMedicos.CadastrarMedico(
                "Dra. Juliana Almeida",
                "CRM-SC 23456",
                Especialidade.Pediatria,
                "(47) 3333-2222"
            );

            var medico3 = gerenciadorMedicos.CadastrarMedico(
                "Dr. Roberto Machado",
                "CRM-SC 34567",
                Especialidade.Ortopedia,
                "(47) 3333-3333"
            );

            var medico4 = gerenciadorMedicos.CadastrarMedico(
                "Dra. Patricia Lima",
                "CRM-SC 45678",
                Especialidade.Dermatologia,
                "(47) 3333-4444"
            );

            // Agendar Consultas
            var consulta1 = gerenciadorConsultas.AgendarConsulta(
                paciente1,
                medico1,
                DateTime.Now.AddDays(2).Date.AddHours(9),
                "Consulta de rotina - checkup anual"
            );

            var consulta2 = gerenciadorConsultas.AgendarConsulta(
                paciente2,
                medico2,
                DateTime.Now.AddDays(3).Date.AddHours(10).AddMinutes(30),
                "Primeira consulta pediátrica"
            );

            var consulta3 = gerenciadorConsultas.AgendarConsulta(
                paciente3,
                medico3,
                DateTime.Now.AddDays(5).Date.AddHours(14),
                "Dor no joelho após atividade física"
            );

            var consulta4 = gerenciadorConsultas.AgendarConsulta(
                paciente1,
                medico4,
                DateTime.Now.AddDays(7).Date.AddHours(11),
                "Avaliação de manchas na pele"
            );

            // Criar uma consulta já realizada
            var consultaRealizada = gerenciadorConsultas.AgendarConsulta(
                paciente2,
                medico1,
                DateTime.Now.AddDays(-5).Date.AddHours(15),
                "Consulta realizada - seguimento cardiológico"
            );

            // Registrar atendimento da consulta já realizada
            gerenciadorConsultas.RegistrarAtendimento(
                consultaRealizada.Id,
                "Paciente apresentou melhora significativa. Pressão arterial normalizada. Manter medicação atual."
            );

            Console.WriteLine("Dados iniciais carregados com sucesso!");
            Console.WriteLine($"- {gerenciadorPacientes.ListarTodos().Count} pacientes cadastrados");
            Console.WriteLine($"- {gerenciadorMedicos.ListarTodos().Count} médicos cadastrados");
            Console.WriteLine($"- {gerenciadorConsultas.ListarTodas().Count} consultas agendadas");
            Console.WriteLine();
        }
    }
}