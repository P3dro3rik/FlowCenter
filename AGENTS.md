# Diretrizes do Agente de IA — FlowCenter

Este documento estabelece as diretrizes, regras de engajamento e histórico de uso de inteligência artificial (IA) no projeto **FlowCenter**.

---

## 1. Agente e Ferramentas Utilizadas
* **Agente Principal:** Gemini / GitHub Copilot
* **Papel:** Colaborador e par de programação (Pair Programming), atuando na geração de artefatos de governança, arquitetura de software, código .NET 8 e testes unitários.

---

## 2. Instruções e Regras para o Agente de IA
Qualquer IA que atue no desenvolvimento do FlowCenter deve seguir rigorosamente as diretrizes abaixo:

1. **Respeito à Arquitetura Clean Architecture:**
   - O código deve manter a separação estrita de camadas: `Domain`, `Application`, `Infrastructure`, `API` e `Tests`.
   - Regras de negócio devem residir exclusivamente na camada `Domain`.

2. **Padrões de Código e Convenções:**
   - Linguagem: C# (.NET 8).
   - Seguir princípios de Clean Code, SOLID e nomes autoexplicativos em português/inglês padronizados no SDD.
   - Tratar exceções de forma explícita e retornar retornos HTTP adequados conforme o SDD.

3. **Governança e Fluxo Git:**
   - Não sugerir commits diretos na branch `main` ou `develop`.
   - Todas as modificações devem ser propostas via *Pull Requests* vinculadas às *Issues*.
   - Mensagens de commit devem seguir o padrão Conventional Commits (ex: `feat:`, `fix:`, `docs:`).

---

## 3. Como a IA foi Utilizada no Projeto
A IA foi integrada ao longo de todo o ciclo de vida do desenvolvimento:

- **Especifição Técnica (SDD):** Auxílio no refinamento de requisitos funcionais, não funcionais e regras de negócio.
- **Registro de Decisões (ADRs):** Apoio na fundamentação da escolha da Clean Architecture, PostgreSQL e xUnit.
- **Automação e Scripts:** Geração de scripts PowerShell e bash para criação da solução .NET e estrutura de pastas em ambientes restritos.
- **Revisão e Qualidade:** Validação de formatação de documentação em Markdown e checklist de Pull Requests.

---

## 4. Registro de Decisões e Refinamentos com Auxílio da IA
* **Refinamento do SDD (v0.1 ao v0.7):** A IA auxiliou a delimitar o escopo, separando explicitamente funcionalidades incluídas daquelas fora de escopo (como autenticação e e-mail).
* **Estratégia para Ambientes Restritos:** Definição do fluxo de desenvolvimento desacoplado, onde tarefas de governança/docs são realizadas via Web UI do GitHub e a compilação do SDK .NET é executada por membros da equipe com ambiente local configurado.
