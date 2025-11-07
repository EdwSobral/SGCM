using System;
using System.Linq;
using SGCM.Models;
using SGCM.Controllers;
using SGCM.Enums;
using SGCM.Utils;

namespace SGCM
{
    class Program
    {
        // Gerenciadores
        private static GerenciadorPacientes gerenciadorPacientes = null!;
        private static GerenciadorMedicos gerenciadorMedicos = null!;
        private static GerenciadorConsultas gerenciadorConsultas = null!;

        static void Main(string[] args)
        {
            InicializarSistema();
            ExibirConsultasDoDia();
            ExibirMenuPrincipal();
        }

        /// <summary>
        /// Inicializa o sistema e os gerenciadores
        /// </summary>
        static void InicializarSistema()
        {
            gerenciadorPacientes = new GerenciadorPacientes();
            gerenciadorMedicos = new GerenciadorMedicos();
            gerenciadorConsultas = new GerenciadorConsultas();

            // Dados de exemplo (opcional - para testes)
            CarregarDadosExemplo();
        }

        /// <summary>
        /// Carrega dados de exemplo para testes
        /// </summary>
        static void CarregarDadosExemplo()
        {
            try
            {
                // Usa a classe CargaDados para carregar dados completos
                SGCM.Data.CargaDados.CarregarDadosExemplo(gerenciadorPacientes, gerenciadorMedicos, gerenciadorConsultas);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar dados de exemplo: {ex.Message}");
            }
        }

        /// <summary>
        /// Exibe consultas do dia ao iniciar o sistema (RN-006)
        /// </summary>
        static void ExibirConsultasDoDia()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                                      ║");
            Console.WriteLine("║          SISTEMA DE GERENCIAMENTO DE CONSULTAS MÉDICAS              ║");
            Console.WriteLine("║                    Consultório Vida Plena                           ║");
            Console.WriteLine("║                                                                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            var consultasHoje = gerenciadorConsultas.ListarConsultasDoDia();
            
            Console.WriteLine($"═══ CONSULTAS DE HOJE - {DateTime.Now:dd/MM/yyyy} ═══");
            Console.WriteLine();

            if (consultasHoje.Count == 0)
            {
                Console.WriteLine("  ℹ  Nenhuma consulta agendada para hoje");
            }
            else
            {
                // Verifica consultas próximas
                var proximasConsultas = gerenciadorConsultas.VerificarConsultasProximas();
                if (proximasConsultas.Count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  ⚠  PRÓXIMAS CONSULTAS (nas próximas 2 horas):");
                    foreach (var c in proximasConsultas)
                    {
                        Console.WriteLine($"  [{c.DataHora:HH:mm}] {c.Medico.Nome} ({c.Medico.ObterEspecialidadeFormatada()}) - {c.Paciente.Nome}");
                    }
                    Console.ResetColor();
                    Console.WriteLine();
                }

                Console.WriteLine("  AGENDA DO DIA:");
                foreach (var consulta in consultasHoje)
                {
                    string statusIcon = consulta.Status switch
                    {
                        StatusConsulta.Agendada => "⏰",
                        StatusConsulta.Concluida => "✓",
                        StatusConsulta.Cancelada => "✗",
                        _ => "•"
                    };

                    Console.WriteLine($"  {statusIcon} [{consulta.DataHora:HH:mm}] {consulta.Status.ToString().ToUpper()}");
                    Console.WriteLine($"      {consulta.Medico.Nome} ({consulta.Medico.ObterEspecialidadeFormatada()})");
                    Console.WriteLine($"      Paciente: {consulta.Paciente.Nome}");
                    Console.WriteLine();
                }

                var stats = gerenciadorConsultas.ObterEstatisticas();
                Console.WriteLine($"  RESUMO: Total: {stats.Total} | Concluídas: {stats.Concluidas} | Canceladas: {stats.Canceladas} | Pendentes: {stats.Agendadas}");
            }

            Console.WriteLine();
            Console.WriteLine("Pressione ENTER para continuar...");
            Console.ReadLine();
        }

        /// <summary>
        /// Exibe o menu principal
        /// </summary>
        static void ExibirMenuPrincipal()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                      MENU PRINCIPAL - SGCM                           ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine();
                Console.WriteLine("  👥 GESTÃO DE PACIENTES");
                Console.WriteLine("     1 - Cadastrar Paciente");
                Console.WriteLine("     2 - Editar Paciente");
                Console.WriteLine("     3 - Listar Pacientes");
                Console.WriteLine("     4 - Inativar/Reativar Paciente");
                Console.WriteLine("     5 - Buscar Histórico de Paciente");
                Console.WriteLine();
                Console.WriteLine("  👨‍⚕️ GESTÃO DE MÉDICOS");
                Console.WriteLine("     6 - Cadastrar Médico");
                Console.WriteLine("     7 - Editar Médico");
                Console.WriteLine("     8 - Listar Médicos");
                Console.WriteLine("     9 - Inativar/Reativar Médico");
                Console.WriteLine();
                Console.WriteLine("  📅 GESTÃO DE CONSULTAS");
                Console.WriteLine("    10 - Agendar Consulta");
                Console.WriteLine("    11 - Listar Consultas");
                Console.WriteLine("    12 - Registrar Atendimento Realizado");
                Console.WriteLine("    13 - Cancelar Consulta");
                Console.WriteLine();
                Console.WriteLine("  📊 RELATÓRIOS");
                Console.WriteLine("    14 - Relatório de Consultas do Dia");
                Console.WriteLine("    15 - Relatório de Cancelamentos");
                Console.WriteLine();
                Console.WriteLine("  🚪 SAIR");
                Console.WriteLine("     0 - Sair do Sistema");
                Console.WriteLine();
                Console.Write("Digite a opção desejada: ");

                string opcao = Console.ReadLine();
                ProcessarOpcaoMenu(opcao);
            }
        }

        /// <summary>
        /// Processa a opção selecionada no menu
        /// </summary>
        static void ProcessarOpcaoMenu(string opcao)
        {
            switch (opcao)
            {
                case "1": CadastrarPaciente(); break;
                case "2": EditarPaciente(); break;
                case "3": ListarPacientes(); break;
                case "4": InativarReativarPaciente(); break;
                case "5": BuscarHistoricoPaciente(); break;
                case "6": CadastrarMedico(); break;
                case "7": EditarMedico(); break;
                case "8": ListarMedicos(); break;
                case "9": InativarReativarMedico(); break;
                case "10": AgendarConsulta(); break;
                case "11": ListarConsultas(); break;
                case "12": RegistrarAtendimento(); break;
                case "13": CancelarConsulta(); break;
                case "14": RelatorioConsultasDia(); break;
                case "15": RelatorioCancelamentos(); break;
                case "0": EncerrarSistema(); break;
                default:
                    Console.WriteLine("\n❌ Opção inválida!");
                    PausarTela();
                    break;
            }
        }

        #region GESTÃO DE PACIENTES

        static void CadastrarPaciente()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    CADASTRO DE PACIENTE                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                Console.Write("Nome Completo: ");
                string nome = Console.ReadLine();

                Console.Write("CPF (somente números): ");
                string cpf = Console.ReadLine();

                Console.Write("Telefone (com DDD): ");
                string telefone = Console.ReadLine();

                Console.Write("E-mail: ");
                string email = Console.ReadLine();

                Console.WriteLine();
                Console.Write("Confirma o cadastro deste paciente? (S/N): ");
                string confirma = Console.ReadLine();

                if (confirma?.ToUpper() == "S")
                {
                    var paciente = gerenciadorPacientes.CadastrarPaciente(nome, cpf, telefone, email);
                    
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ SUCESSO!");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Paciente cadastrado com sucesso!");
                    Console.WriteLine();
                    Console.WriteLine($"  ID: {paciente.Id}");
                    Console.WriteLine($"  Nome: {paciente.Nome}");
                    Console.WriteLine($"  CPF: {paciente.CPF}");
                    Console.WriteLine($"  Telefone: {paciente.Telefone}");
                    Console.WriteLine($"  E-mail: {paciente.Email}");
                    Console.WriteLine($"  Status: {paciente.Status}");
                    Console.WriteLine($"  Cadastrado em: {paciente.DataCadastro:dd/MM/yyyy HH:mm}");
                }
                else
                {
                    Console.WriteLine("\nCadastro cancelado.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        static void EditarPaciente()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     EDITAR PACIENTE                                  ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                Console.Write("Digite o CPF do paciente: ");
                string cpf = Console.ReadLine();

                var paciente = gerenciadorPacientes.BuscarPorCPF(cpf);

                if (paciente == null)
                {
                    Console.WriteLine("\n❌ Paciente não encontrado!");
                }
                else
                {
                    Console.WriteLine("\n✓ Paciente encontrado:");
                    Console.WriteLine($"  {paciente.ToString()}");
                    Console.WriteLine();

                    Console.Write("Novo Telefone (Enter para manter o atual): ");
                    string telefone = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(telefone))
                        telefone = paciente.Telefone;

                    Console.Write("Novo E-mail (Enter para manter o atual): ");
                    string email = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(email))
                        email = paciente.Email;

                    gerenciadorPacientes.EditarPaciente(paciente.Id, telefone, email);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n✓ Paciente editado com sucesso!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        static void ListarPacientes()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      LISTAGEM DE PACIENTES                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("  Filtrar por:");
            Console.WriteLine("     1 - Todos os pacientes");
            Console.WriteLine("     2 - Apenas ativos");
            Console.WriteLine("     3 - Apenas inativos");
            Console.Write("\n  Opção: ");
            string opcao = Console.ReadLine();

            var pacientes = opcao switch
            {
                "2" => gerenciadorPacientes.ListarAtivos(),
                "3" => gerenciadorPacientes.ListarInativos(),
                _ => gerenciadorPacientes.ListarTodos()
            };

            Console.WriteLine();
            Console.WriteLine("─────────────────────────────────────────────────────────────────────");
            Console.WriteLine($"{"ID",-10} | {"NOME",-25} | {"CPF",-15} | {"TELEFONE",-16} | {"STATUS",-8}");
            Console.WriteLine("─────────────────────────────────────────────────────────────────────");

            if (pacientes.Count == 0)
            {
                Console.WriteLine("Nenhum paciente encontrado.");
            }
            else
            {
                foreach (var p in pacientes)
                {
                    Console.WriteLine($"{p.Id,-10} | {p.Nome,-25} | {p.CPF,-15} | {p.Telefone,-16} | {p.Status,-8}");
                }
            }

            Console.WriteLine("─────────────────────────────────────────────────────────────────────");
            Console.WriteLine($"\nTotal: {pacientes.Count} pacientes");

            PausarTela();
        }

        static void InativarReativarPaciente()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 INATIVAR/REATIVAR PACIENTE                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                Console.Write("Digite o CPF do paciente: ");
                string cpf = Console.ReadLine();

                var paciente = gerenciadorPacientes.BuscarPorCPF(cpf);

                if (paciente == null)
                {
                    Console.WriteLine("\n❌ Paciente não encontrado!");
                }
                else
                {
                    Console.WriteLine($"\nPaciente: {paciente.Nome}");
                    Console.WriteLine($"Status atual: {paciente.Status}");
                    Console.WriteLine();

                    if (paciente.Status == StatusPaciente.Ativo)
                    {
                        Console.Write("Deseja INATIVAR este paciente? (S/N): ");
                        if (Console.ReadLine()?.ToUpper() == "S")
                        {
                            gerenciadorPacientes.InativarPaciente(paciente.Id);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n✓ Paciente inativado com sucesso!");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.Write("Deseja REATIVAR este paciente? (S/N): ");
                        if (Console.ReadLine()?.ToUpper() == "S")
                        {
                            gerenciadorPacientes.ReativarPaciente(paciente.Id);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n✓ Paciente reativado com sucesso!");
                            Console.ResetColor();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        static void BuscarHistoricoPaciente()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                   HISTÓRICO DE PACIENTE                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                Console.Write("Digite o CPF do paciente: ");
                string cpf = Console.ReadLine();

                var paciente = gerenciadorPacientes.BuscarPorCPF(cpf);

                if (paciente == null)
                {
                    Console.WriteLine("\n❌ Paciente não encontrado!");
                }
                else
                {
                    Console.WriteLine("\n═══ DADOS DO PACIENTE ═══");
                    Console.WriteLine($"  ID: {paciente.Id}");
                    Console.WriteLine($"  Nome: {paciente.Nome}");
                    Console.WriteLine($"  CPF: {paciente.CPF}");
                    Console.WriteLine($"  Telefone: {paciente.Telefone}");
                    Console.WriteLine($"  E-mail: {paciente.Email}");
                    Console.WriteLine($"  Status: {paciente.Status}");

                    Console.WriteLine();
                    Console.WriteLine("═══ HISTÓRICO DE CONSULTAS ═══");
                    
                    var consultas = gerenciadorConsultas.ListarPorPaciente(paciente);

                    if (consultas.Count == 0)
                    {
                        Console.WriteLine("  ℹ  Paciente não possui histórico de consultas");
                    }
                    else
                    {
                        foreach (var c in consultas)
                        {
                            Console.WriteLine($"\n{c.ObterResumo()}");
                            Console.WriteLine("─────────────────────────────────────────────────");
                        }

                        var total = consultas.Count;
                        var concluidas = consultas.Count(c => c.Status == StatusConsulta.Concluida);
                        var canceladas = consultas.Count(c => c.Status == StatusConsulta.Cancelada);
                        var agendadas = consultas.Count(c => c.Status == StatusConsulta.Agendada);

                        Console.WriteLine($"\nRESUMO: Total: {total} | Concluídas: {concluidas} | Canceladas: {canceladas} | Agendadas: {agendadas}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        #endregion

        #region GESTÃO DE MÉDICOS

        static void CadastrarMedico()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      CADASTRO DE MÉDICO                              ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                Console.Write("Nome Completo: ");
                string nome = Console.ReadLine();

                Console.Write("CRM (formato: 12345-UF): ");
                string crm = Console.ReadLine();

                Console.WriteLine("\nEspecialidade:");
                Console.WriteLine("  1 - Clínica Geral");
                Console.WriteLine("  2 - Cardiologia");
                Console.WriteLine("  3 - Pediatria");
                Console.WriteLine("  4 - Ortopedia");
                Console.WriteLine("  5 - Dermatologia");
                Console.WriteLine("  6 - Ginecologia");
                Console.Write("Escolha (1-6): ");
                
                int opcaoEsp = int.Parse(Console.ReadLine());
                Especialidade especialidade = (Especialidade)(opcaoEsp - 1);

                Console.Write("\nTelefone (com DDD): ");
                string telefone = Console.ReadLine();

                Console.WriteLine();
                Console.Write("Confirma o cadastro deste médico? (S/N): ");
                string confirma = Console.ReadLine();

                if (confirma?.ToUpper() == "S")
                {
                    var medico = gerenciadorMedicos.CadastrarMedico(nome, crm, especialidade, telefone);
                    
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ SUCESSO!");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine("Médico cadastrado com sucesso!");
                    Console.WriteLine();
                    Console.WriteLine($"  ID: {medico.Id}");
                    Console.WriteLine($"  Nome: {medico.Nome}");
                    Console.WriteLine($"  CRM: {medico.CRM}");
                    Console.WriteLine($"  Especialidade: {medico.ObterEspecialidadeFormatada()}");
                    Console.WriteLine($"  Telefone: {medico.Telefone}");
                    Console.WriteLine($"  Status: {medico.Status}");
                }
                else
                {
                    Console.WriteLine("\nCadastro cancelado.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        static void EditarMedico()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      EDITAR MÉDICO                                   ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                Console.Write("Digite o CRM do médico: ");
                string crm = Console.ReadLine();

                var medico = gerenciadorMedicos.BuscarPorCRM(crm);

                if (medico == null)
                {
                    Console.WriteLine("\n❌ Médico não encontrado!");
                }
                else
                {
                    Console.WriteLine("\n✓ Médico encontrado:");
                    Console.WriteLine($"  {medico.ToString()}");
                    Console.WriteLine();

                    Console.WriteLine("Nova Especialidade (Enter para manter):");
                    Console.WriteLine("  1 - Clínica Geral");
                    Console.WriteLine("  2 - Cardiologia");
                    Console.WriteLine("  3 - Pediatria");
                    Console.WriteLine("  4 - Ortopedia");
                    Console.WriteLine("  5 - Dermatologia");
                    Console.WriteLine("  6 - Ginecologia");
                    Console.Write("Escolha (1-6 ou Enter): ");
                    
                    string opcaoEsp = Console.ReadLine();
                    Especialidade especialidade = medico.Especialidade;
                    
                    if (!string.IsNullOrWhiteSpace(opcaoEsp))
                        especialidade = (Especialidade)(int.Parse(opcaoEsp) - 1);

                    Console.Write("\nNovo Telefone (Enter para manter): ");
                    string telefone = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(telefone))
                        telefone = medico.Telefone;

                    gerenciadorMedicos.EditarMedico(medico.Id, especialidade, telefone);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n✓ Médico editado com sucesso!");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        static void ListarMedicos()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                       LISTAGEM DE MÉDICOS                            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("  Filtrar por:");
            Console.WriteLine("     1 - Todos os médicos");
            Console.WriteLine("     2 - Apenas ativos");
            Console.Write("\n  Opção: ");
            string opcao = Console.ReadLine();

            var medicos = opcao == "2" ? gerenciadorMedicos.ListarAtivos() : gerenciadorMedicos.ListarTodos();

            Console.WriteLine();
            Console.WriteLine("─────────────────────────────────────────────────────────────────────");
            Console.WriteLine($"{"ID",-10} | {"NOME",-25} | {"CRM",-12} | {"ESPECIALIDADE",-18} | {"STATUS",-8}");
            Console.WriteLine("─────────────────────────────────────────────────────────────────────");

            if (medicos.Count == 0)
            {
                Console.WriteLine("Nenhum médico encontrado.");
            }
            else
            {
                foreach (var m in medicos)
                {
                    Console.WriteLine($"{m.Id,-10} | {m.Nome,-25} | {m.CRM,-12} | {m.ObterEspecialidadeFormatada(),-18} | {m.Status,-8}");
                }
            }

            Console.WriteLine("─────────────────────────────────────────────────────────────────────");
            Console.WriteLine($"\nTotal: {medicos.Count} médicos");

            PausarTela();
        }

        static void InativarReativarMedico()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  INATIVAR/REATIVAR MÉDICO                            ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                Console.Write("Digite o CRM do médico: ");
                string crm = Console.ReadLine();

                var medico = gerenciadorMedicos.BuscarPorCRM(crm);

                if (medico == null)
                {
                    Console.WriteLine("\n❌ Médico não encontrado!");
                }
                else
                {
                    Console.WriteLine($"\nMédico: {medico.Nome}");
                    Console.WriteLine($"Status atual: {medico.Status}");
                    Console.WriteLine();

                    if (medico.Status == StatusMedico.Ativo)
                    {
                        Console.Write("Deseja INATIVAR este médico? (S/N): ");
                        if (Console.ReadLine()?.ToUpper() == "S")
                        {
                            gerenciadorMedicos.InativarMedico(medico.Id);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n✓ Médico inativado com sucesso!");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.Write("Deseja REATIVAR este médico? (S/N): ");
                        if (Console.ReadLine()?.ToUpper() == "S")
                        {
                            gerenciadorMedicos.ReativarMedico(medico.Id);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\n✓ Médico reativado com sucesso!");
                            Console.ResetColor();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        #endregion

        #region GESTÃO DE CONSULTAS

        static void AgendarConsulta()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    AGENDAMENTO DE CONSULTA                           ║");
            Console.WriteLine("║                      Etapa 1 de 4: Paciente                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                // ETAPA 1: Buscar Paciente
                Console.Write("Digite o CPF do paciente: ");
                string cpf = Console.ReadLine();

                var paciente = gerenciadorPacientes.BuscarPorCPF(cpf);

                if (paciente == null)
                {
                    Console.WriteLine("\n❌ Paciente não encontrado!");
                    PausarTela();
                    return;
                }

                if (!paciente.PodeAgendar())
                {
                    Console.WriteLine("\n❌ Paciente inativo! Não é possível agendar consultas.");
                    Console.Write("\nDeseja reativar o paciente? (S/N): ");
                    if (Console.ReadLine()?.ToUpper() == "S")
                    {
                        gerenciadorPacientes.ReativarPaciente(paciente.Id);
                        Console.WriteLine("✓ Paciente reativado!");
                    }
                    else
                    {
                        PausarTela();
                        return;
                    }
                }

                Console.WriteLine("\n✓ Paciente encontrado:");
                Console.WriteLine($"  Nome: {paciente.Nome}");
                Console.WriteLine($"  CPF: {paciente.CPF}");

                // ETAPA 2: Selecionar Médico
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    AGENDAMENTO DE CONSULTA                           ║");
                Console.WriteLine("║                      Etapa 2 de 4: Médico                            ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine($"\nPaciente: {paciente.Nome}");
                Console.WriteLine();

                var medicosAtivos = gerenciadorMedicos.ListarAtivos();

                if (medicosAtivos.Count == 0)
                {
                    Console.WriteLine("❌ Nenhum médico ativo disponível!");
                    PausarTela();
                    return;
                }

                Console.WriteLine("Médicos disponíveis:");
                for (int i = 0; i < medicosAtivos.Count; i++)
                {
                    var m = medicosAtivos[i];
                    Console.WriteLine($"  {i + 1} - {m.Nome} ({m.ObterEspecialidadeFormatada()}) - CRM: {m.CRM}");
                }

                Console.Write("\nDigite o número do médico: ");
                int indiceMedico = int.Parse(Console.ReadLine()) - 1;

                if (indiceMedico < 0 || indiceMedico >= medicosAtivos.Count)
                {
                    Console.WriteLine("\n❌ Opção inválida!");
                    PausarTela();
                    return;
                }

                var medico = medicosAtivos[indiceMedico];

                // ETAPA 3: Data e Horário
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    AGENDAMENTO DE CONSULTA                           ║");
                Console.WriteLine("║                   Etapa 3 de 4: Data e Horário                       ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine($"\nPaciente: {paciente.Nome}");
                Console.WriteLine($"Médico: {medico.Nome} - {medico.ObterEspecialidadeFormatada()}");
                Console.WriteLine();

                Console.Write("Data da consulta (DD/MM/AAAA): ");
                string dataStr = Console.ReadLine();
                DateTime dataConsulta = DateTime.ParseExact(dataStr, "dd/MM/yyyy", null);

                Console.Write("Horário (HH:MM): ");
                string horaStr = Console.ReadLine();
                TimeSpan horario = TimeSpan.Parse(horaStr);

                DateTime dataHora = dataConsulta.Date + horario;

                // Valida se horário está disponível
                if (!gerenciadorConsultas.ValidarHorarioDisponivel(medico, dataHora))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n⚠ HORÁRIO JÁ OCUPADO!");
                    Console.ResetColor();
                    Console.WriteLine($"\nO Dr(a). {medico.Nome} já possui consulta neste horário.");
                    
                    var consultasDia = medico.ObterAgenda(dataConsulta);
                    if (consultasDia.Count > 0)
                    {
                        Console.WriteLine("\nAgenda do médico para este dia:");
                        foreach (var c in consultasDia)
                        {
                            if (c.Status == StatusConsulta.Agendada)
                                Console.WriteLine($"  • {c.DataHora:HH:mm} - Ocupado");
                        }
                    }
                    
                    Console.Write("\nDeseja escolher outro horário? (S/N): ");
                    if (Console.ReadLine()?.ToUpper() != "S")
                    {
                        PausarTela();
                        return;
                    }
                    
                    Console.Write("\nNovo horário (HH:MM): ");
                    horaStr = Console.ReadLine();
                    horario = TimeSpan.Parse(horaStr);
                    dataHora = dataConsulta.Date + horario;
                    
                    if (!gerenciadorConsultas.ValidarHorarioDisponivel(medico, dataHora))
                    {
                        Console.WriteLine("\n❌ Horário também ocupado!");
                        PausarTela();
                        return;
                    }
                }

                Console.WriteLine("\n✓ Horário disponível!");

                // ETAPA 4: Confirmação
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
                Console.WriteLine("║                    AGENDAMENTO DE CONSULTA                           ║");
                Console.WriteLine("║                     Etapa 4 de 4: Confirmação                        ║");
                Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
                Console.WriteLine();

                Console.Write("Observações (opcional): ");
                string observacoes = Console.ReadLine();

                Console.WriteLine();
                Console.WriteLine("═══ RESUMO DA CONSULTA ═══");
                Console.WriteLine($"  Paciente: {paciente.Nome} (CPF: {paciente.CPF})");
                Console.WriteLine($"  Médico: {medico.Nome} ({medico.ObterEspecialidadeFormatada()})");
                Console.WriteLine($"  Data: {dataHora:dd/MM/yyyy}");
                Console.WriteLine($"  Horário: {dataHora:HH:mm}");
                if (!string.IsNullOrWhiteSpace(observacoes))
                    Console.WriteLine($"  Observações: {observacoes}");

                Console.WriteLine();
                Console.Write("Confirma o agendamento? (S/N): ");
                
                if (Console.ReadLine()?.ToUpper() == "S")
                {
                    var consulta = gerenciadorConsultas.AgendarConsulta(paciente, medico, dataHora, observacoes);
                    
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✓ CONSULTA AGENDADA COM SUCESSO!");
                    Console.ResetColor();
                    Console.WriteLine();
                    Console.WriteLine($"ID da Consulta: {consulta.Id}");
                    Console.WriteLine($"Status: {consulta.Status}");
                    Console.WriteLine($"Agendado em: {consulta.DataAgendamento:dd/MM/yyyy HH:mm}");
                }
                else
                {
                    Console.WriteLine("\nAgendamento cancelado.");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        static void ListarConsultas()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      LISTAGEM DE CONSULTAS                           ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.WriteLine("  Filtrar por:");
            Console.WriteLine("     1 - Todas as consultas");
            Console.WriteLine("     2 - Consultas do dia");
            Console.WriteLine("     3 - Por status (Agendada/Concluída/Cancelada)");
            Console.Write("\n  Opção: ");
            string opcao = Console.ReadLine();

            var consultas = new List<Consulta>();

            switch (opcao)
            {
                case "1":
                    consultas = gerenciadorConsultas.ListarTodas();
                    break;
                case "2":
                    consultas = gerenciadorConsultas.ListarConsultasDoDia();
                    break;
                case "3":
                    Console.WriteLine("\n  1 - Agendada");
                    Console.WriteLine("  2 - Concluída");
                    Console.WriteLine("  3 - Cancelada");
                    Console.Write("  Status: ");
                    int statusOp = int.Parse(Console.ReadLine());
                    StatusConsulta status = (StatusConsulta)(statusOp - 1);
                    consultas = gerenciadorConsultas.ListarPorStatus(status);
                    break;
                default:
                    consultas = gerenciadorConsultas.ListarTodas();
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("─────────────────────────────────────────────────────────────────────");

            if (consultas.Count == 0)
            {
                Console.WriteLine("Nenhuma consulta encontrada.");
            }
            else
            {
                foreach (var c in consultas)
                {
                    string statusIcon = c.Status switch
                    {
                        StatusConsulta.Agendada => "⏰",
                        StatusConsulta.Concluida => "✓",
                        StatusConsulta.Cancelada => "✗",
                        _ => "•"
                    };

                    Console.WriteLine($"{statusIcon} {c.Id} | {c.DataHora:dd/MM/yyyy HH:mm} | {c.Status}");
                    Console.WriteLine($"   Paciente: {c.Paciente.Nome}");
                    Console.WriteLine($"   Médico: {c.Medico.Nome} ({c.Medico.ObterEspecialidadeFormatada()})");
                    Console.WriteLine("─────────────────────────────────────────────────────────────────────");
                }
            }

            Console.WriteLine($"\nTotal: {consultas.Count} consultas");

            PausarTela();
        }

        static void RegistrarAtendimento()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                  REGISTRAR ATENDIMENTO REALIZADO                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            var pendentes = gerenciadorConsultas.ListarPendentesDodia();

            if (pendentes.Count == 0)
            {
                Console.WriteLine("  ℹ  Nenhuma consulta pendente para hoje");
                PausarTela();
                return;
            }

            Console.WriteLine("CONSULTAS PENDENTES DE HOJE:");
            Console.WriteLine();

            for (int i = 0; i < pendentes.Count; i++)
            {
                var c = pendentes[i];
                Console.WriteLine($"  {i + 1} - {c.Id}");
                Console.WriteLine($"      [{c.DataHora:HH:mm}] {c.Medico.Nome} ({c.Medico.ObterEspecialidadeFormatada()})");
                Console.WriteLine($"      Paciente: {c.Paciente.Nome}");
                Console.WriteLine();
            }

            try
            {
                Console.Write("Digite o número da consulta ou ID: ");
                string entrada = Console.ReadLine();

                Consulta consulta = null;

                if (int.TryParse(entrada, out int indice) && indice > 0 && indice <= pendentes.Count)
                {
                    consulta = pendentes[indice - 1];
                }
                else
                {
                    consulta = gerenciadorConsultas.BuscarPorId(entrada);
                }

                if (consulta == null)
                {
                    Console.WriteLine("\n❌ Consulta não encontrada!");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("═══ CONSULTA SELECIONADA ═══");
                    Console.WriteLine($"  ID: {consulta.Id}");
                    Console.WriteLine($"  Data/Hora: {consulta.DataHora:dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"  Paciente: {consulta.Paciente.Nome}");
                    Console.WriteLine($"  Médico: {consulta.Medico.Nome}");
                    Console.WriteLine();

                    Console.Write("Confirma que o atendimento foi realizado? (S/N): ");
                    
                    if (Console.ReadLine()?.ToUpper() == "S")
                    {
                        Console.Write("\nObservações do atendimento (opcional): ");
                        string obs = Console.ReadLine();

                        gerenciadorConsultas.RegistrarAtendimento(consulta.Id, obs);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n✓ ATENDIMENTO REGISTRADO COM SUCESSO!");
                        Console.ResetColor();
                        Console.WriteLine($"\nRegistrado em: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    }
                    else
                    {
                        Console.WriteLine("\nOperação cancelada.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        static void CancelarConsulta()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    CANCELAMENTO DE CONSULTA                          ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            try
            {
                Console.Write("Digite o ID da consulta: ");
                string id = Console.ReadLine();

                var consulta = gerenciadorConsultas.BuscarPorId(id);

                if (consulta == null)
                {
                    Console.WriteLine("\n❌ Consulta não encontrada!");
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("═══ CONSULTA ENCONTRADA ═══");
                    Console.WriteLine($"  ID: {consulta.Id}");
                    Console.WriteLine($"  Status: {consulta.Status}");
                    Console.WriteLine($"  Paciente: {consulta.Paciente.Nome}");
                    Console.WriteLine($"  Médico: {consulta.Medico.Nome}");
                    Console.WriteLine($"  Data/Hora: {consulta.DataHora:dd/MM/yyyy HH:mm}");
                    Console.WriteLine();

                    if (!consulta.PodeCancelar())
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Esta consulta não pode ser cancelada!");
                        Console.ResetColor();
                        
                        if (consulta.Status != StatusConsulta.Agendada)
                            Console.WriteLine($"Motivo: Consulta já está com status '{consulta.Status}'");
                        else
                            Console.WriteLine("Motivo: Consulta é de data passada (RN-004)");
                    }
                    else
                    {
                        Console.Write("Confirma o cancelamento desta consulta? (S/N): ");
                        
                        if (Console.ReadLine()?.ToUpper() == "S")
                        {
                            Console.Write("\nInforme o motivo do cancelamento: ");
                            string motivo = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(motivo))
                            {
                                Console.WriteLine("\n❌ Motivo é obrigatório! (RN-008)");
                            }
                            else
                            {
                                gerenciadorConsultas.CancelarConsulta(consulta.Id, motivo);

                                Console.WriteLine();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("✓ CONSULTA CANCELADA COM SUCESSO!");
                                Console.ResetColor();
                                Console.WriteLine();
                                Console.WriteLine($"Motivo: {motivo}");
                                Console.WriteLine($"Cancelado em: {DateTime.Now:dd/MM/yyyy HH:mm}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nCancelamento não realizado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Erro: {ex.Message}");
                Console.ResetColor();
            }

            PausarTela();
        }

        #endregion

        #region RELATÓRIOS

        static void RelatorioConsultasDia()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                 RELATÓRIO DE CONSULTAS DO DIA                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.Write("Data do relatório (DD/MM/AAAA ou Enter para hoje): ");
            string dataStr = Console.ReadLine();

            DateTime? data = null;
            if (!string.IsNullOrWhiteSpace(dataStr))
            {
                data = DateTime.ParseExact(dataStr, "dd/MM/yyyy", null);
            }

            Console.WriteLine("\nGerando relatório...\n");

            string relatorio = gerenciadorConsultas.GerarRelatorioDia(data);

            Console.WriteLine(relatorio);

            Console.WriteLine();
            Console.WriteLine("Opções:");
            Console.WriteLine("  1 - Exportar para arquivo TXT");
            Console.WriteLine("  0 - Voltar ao menu");
            Console.Write("\nOpção: ");
            
            string opcao = Console.ReadLine();
            
            if (opcao == "1")
            {
                try
                {
                    string nomeArquivo = $"Relatorio_Consultas_{(data ?? DateTime.Now):yyyyMMdd}.txt";
                    System.IO.File.WriteAllText(nomeArquivo, relatorio);
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n✓ Relatório exportado: {nomeArquivo}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n❌ Erro ao exportar: {ex.Message}");
                    Console.ResetColor();
                }
            }

            PausarTela();
        }

        static void RelatorioCancelamentos()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                    RELATÓRIO DE CANCELAMENTOS                        ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            Console.Write("Data inicial (DD/MM/AAAA ou Enter para último mês): ");
            string dataIniStr = Console.ReadLine();

            Console.Write("Data final (DD/MM/AAAA ou Enter para hoje): ");
            string dataFimStr = Console.ReadLine();

            DateTime? dataInicio = null;
            DateTime? dataFim = null;

            if (!string.IsNullOrWhiteSpace(dataIniStr))
                dataInicio = DateTime.ParseExact(dataIniStr, "dd/MM/yyyy", null);

            if (!string.IsNullOrWhiteSpace(dataFimStr))
                dataFim = DateTime.ParseExact(dataFimStr, "dd/MM/yyyy", null);

            Console.WriteLine("\nGerando relatório...\n");

            string relatorio = gerenciadorConsultas.GerarRelatorioCancelamentos(dataInicio, dataFim);

            Console.WriteLine(relatorio);

            Console.WriteLine();
            Console.WriteLine("Opções:");
            Console.WriteLine("  1 - Exportar para arquivo TXT");
            Console.WriteLine("  0 - Voltar ao menu");
            Console.Write("\nOpção: ");
            
            string opcao = Console.ReadLine();
            
            if (opcao == "1")
            {
                try
                {
                    string nomeArquivo = $"Relatorio_Cancelamentos_{DateTime.Now:yyyyMMdd}.txt";
                    System.IO.File.WriteAllText(nomeArquivo, relatorio);
                    
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n✓ Relatório exportado: {nomeArquivo}");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n❌ Erro ao exportar: {ex.Message}");
                    Console.ResetColor();
                }
            }

            PausarTela();
        }

        #endregion

        #region UTILITÁRIOS

        static void PausarTela()
        {
            Console.WriteLine("\nPressione ENTER para continuar...");
            Console.ReadLine();
        }

        static void EncerrarSistema()
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                                                                      ║");
            Console.WriteLine("║                    ENCERRANDO SISTEMA SGCM                           ║");
            Console.WriteLine("║                                                                      ║");
            Console.WriteLine("║              Obrigado por utilizar nosso sistema!                    ║");
            Console.WriteLine("║                                                                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════════════╝");
            Console.WriteLine();
            Environment.Exit(0);
        }

        #endregion
    }
}