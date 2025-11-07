# 📋 SGCM - Sistema de Gerenciamento de Consultas Médicas

## 👥 Integrantes do Grupo
- [Eduardo Sobral]

## 📊 Informações do Trabalho
- **Disciplinas:** Requisitos, Modelagem e Análise de Dados + Programação Orientada a Objetos
- **Instituição:** Católica SC - Pós-Graduação
- **Turma:** 2º Semestre A
- **Data de Entrega:** [12/11/2025]

---

## 📖 Sobre o Sistema

O **SGCM (Sistema de Gerenciamento de Consultas Médicas)** é uma solução desenvolvida para automatizar e otimizar a gestão de consultas médicas do Consultório Vida Plena. O sistema elimina controles manuais em papel, reduz erros operacionais e garante rastreabilidade completa das operações.

### 🎯 Objetivos Principais
- Automatizar o controle de agendamento de consultas
- Manter histórico completo e auditável
- Fornecer visibilidade em tempo real
- Identificar responsáveis por cada operação
- Gerar relatórios gerenciais

---

## 🏗️ Arquitetura do Sistema

### Tecnologias Utilizadas
- **Linguagem:** C# (.NET 6.0+)
- **Paradigma:** Programação Orientada a Objetos
- **Interface Console:** Windows Terminal
- **Interface Web:** HTML5, CSS3, JavaScript, Bootstrap 5
- **Armazenamento:** Memória (console) / LocalStorage (web)

### Estrutura de Pastas

```
SGCM/
├── Controllers/              # Gerenciadores (CRUD)
│   ├── GerenciadorPacientes.cs
│   ├── GerenciadorMedicos.cs
│   └── GerenciadorConsultas.cs
│
├── Models/                   # Entidades de negócio
│   ├── Paciente.cs
│   ├── Medico.cs
│   └── Consulta.cs
│
├── Enums/                    # Enumerações
│   ├── StatusPaciente.cs
│   ├── StatusMedico.cs
│   ├── StatusConsulta.cs
│   └── Especialidade.cs
│
├── Utils/                    # Utilitários
│   └── Validador.cs
│
├── Data/                     # Carga de dados
│   └── CargaDados.cs
│
├── Program.cs                # Arquivo principal
└── SGCM.csproj              # Configuração do projeto
```

---

## 🚀 Como Executar o Projeto

### Pré-requisitos
- .NET SDK 6.0 ou superior
- Visual Studio Code (recomendado)
- Extensão C# para VS Code

### Executar Projeto Console

```bash
# 1. Navegue até a pasta do projeto
cd SGCM

# 2. Compile o projeto
dotnet build

# 3. Execute o sistema
dotnet run
```

### Executar Interface Web

1. Abra o arquivo `index.html` no navegador
2. OU use um servidor local:
   ```bash
   # Com Python
   python -m http.server 8000
   
   # Com Node.js
   npx http-server
   ```

---

## 📊 Funcionalidades Implementadas

### ✅ Gestão de Pacientes (RF-001 a RF-004)
- Cadastro de pacientes com validação de CPF
- Edição de dados cadastrais
- Inativação/Reativação de pacientes
- Listagem com filtros
- Busca de histórico completo

### ✅ Gestão de Médicos (RF-005 a RF-009)
- Cadastro de médicos com validação de CRM
- Edição de dados e especialidade
- Inativação/Reativação de médicos
- Listagem por especialidade
- Consulta de agenda

### ✅ Gestão de Consultas (RF-009 a RF-014)
- Agendamento com validação de horários
- Verificação automática de conflitos (RN-003)
- Registro de atendimentos realizados
- Cancelamento com registro de motivo (RN-008)
- Impedimento de alteração em consultas passadas (RN-004)

### ✅ Relatórios (RF-015 a RF-018)
- Relatório de consultas do dia
- Relatório de cancelamentos
- Estatísticas gerenciais
- Exportação para arquivo

### ✅ Validações e Regras de Negócio
- **RN-001:** Paciente deve estar cadastrado
- **RN-002:** Médico deve estar cadastrado
- **RN-003:** Horário único por médico
- **RN-004:** Consultas passadas não podem ser canceladas
- **RN-005:** Data, horário e especialidade obrigatórios
- **RN-006:** Exibição automática de consultas do dia
- **RN-007:** Pacientes inativos não podem agendar
- **RN-008:** Cancelamento requer motivo e data

---

## 💾 Carga de Dados de Exemplo

O sistema inclui carga automática de dados para demonstração:

### 📊 Dados Pré-carregados
- **10 Pacientes** cadastrados com dados completos
- **6 Médicos** (um por especialidade):
  - Clínica Geral
  - Cardiologia
  - Pediatria
  - Ortopedia
  - Dermatologia
  - Ginecologia

### 📅 Consultas Simuladas (por médico)
- **3 Realizadas** (datas passadas - status Concluída)
- **5 Agendadas** (datas futuras - status Agendada)
- **2 Canceladas** (status Cancelada com motivo)
- **Total:** 60+ consultas para demonstração de relatórios

---

## 📐 Diagramas UML

### Diagrama de Casos de Uso
O sistema possui 29 casos de uso organizados em 5 módulos:
1. Gestão de Pacientes (5 casos de uso)
2. Gestão de Médicos (5 casos de uso)
3. Gestão de Consultas (7 casos de uso)
4. Relatórios (4 casos de uso)
5. Validações Automáticas (8 casos de uso)

### Diagrama de Classes
12 classes principais implementadas:
- **Entidades:** Paciente, Medico, Consulta
- **Gerenciadores:** GerenciadorPacientes, GerenciadorMedicos, GerenciadorConsultas
- **Utilitários:** Validador, CargaDados
- **Enumerações:** 4 enums de controle

---

## 🎨 Interface Web

### Características
- ✅ Design moderno e responsivo (Bootstrap 5)
- ✅ Dashboard com estatísticas em tempo real
- ✅ Cadastros funcionais com validação
- ✅ Agendamento de consultas interativo
- ✅ Relatórios com filtros de data
- ✅ Persistência de dados (LocalStorage)

### Funcionalidades Web
1. **Dashboard:** Visão geral com cards de estatísticas
2. **Pacientes:** CRUD completo via modais
3. **Médicos:** CRUD completo via modais
4. **Consultas:** Agendamento e listagem com filtros
5. **Relatórios:** Geração dinâmica com seleção de datas

---

## 🧪 Testes Realizados

### Cenários Testados
✅ Cadastro de pacientes com CPF válido e inválido
✅ Cadastro de médicos com CRM duplicado
✅ Agendamento com horário ocupado
✅ Tentativa de cancelar consulta passada
✅ Agendamento para paciente inativo
✅ Geração de relatórios com diferentes datas
✅ Validação de campos obrigatórios
✅ Persistência de dados entre sessões (web)

---

## 📚 Documentação Adicional

### Arquivos Incluídos na Entrega
1. ✅ **Código-fonte completo** (pasta SGCM/)
2. ✅ **Interface web** (index.html)
3. ✅ **Requisitos Funcionais** (18 RFs documentados)
4. ✅ **Diagrama de Casos de Uso** (XML gerado)
5. ✅ **Diagrama de Classes** (XML gerado)
6. ✅ **Descrição de Cenários** (8 casos principais)
7. ✅ **Protótipos de Tela** (15 telas console)
8. ✅ **Relatório Final** (documento Word)

---

## 🎓 Conceitos de POO Aplicados

### Encapsulamento
- Atributos privados com getters/setters
- Controle de acesso aos dados

### Abstração
- Classes representam entidades reais
- Métodos escondem complexidade

### Validações
- Classe Validador centraliza regras
- Validação de CPF, CRM, Email, Datas

### Relacionamentos
- Associação entre Paciente/Médico/Consulta
- Composição entre Gerenciadores e Entidades

### Enumerações
- Tipos seguros para Status e Especialidade
- Evita valores inválidos

---

## 📝 Licença

Este projeto foi desenvolvido para fins acadêmicos como parte do trabalho conjunto das disciplinas de Requisitos, Modelagem e Análise de Dados e Programação Orientada a Objetos.

**© 2025 - Católica SC - Todos os direitos reservados**