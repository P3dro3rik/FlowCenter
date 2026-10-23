# Pontos Conflitantes / Pendentes — Entrega 3

> Checklist de verificação derivada da análise de `specsBootcamp3` comparada ao
> estado real do repositório em 2026-10-23. Cada item será atualizado conforme
> for corrigido, com o link da branch/PR correspondente.
>
> Observação importante: o arquivo `specsBootcamp3` está incompleto — ele numera
> "1" no início da lista de "Itens Obrigatórios para Entrega", mas o texto é
> cortado antes de descrever os itens 2, 3, etc. Esta checklist cobre o item 1
> por completo (os três sub-requisitos) e os demais objetivos citados no
> cabeçalho do documento (testes aprovados, SDD As-Built, ambiente empacotado,
> demonstração funcional).

## Status geral

| # | Item | Status | Branch / PR |
|---|------|--------|--------------|
| 1 | Credenciais expostas em texto puro | ✅ Corrigido (aguardando aprovação) | `fix/remove-exposed-credentials` — PR #27 |
| 2 | Suíte de testes não compila / CI roda 0 testes (falso verde) | ✅ Corrigido (aguardando aprovação) — 36/36 testes aprovados localmente | `fix/consolidate-clean-architecture` — PR #28 |
| 3 | SDD atualizado para "As-Built" | ✅ Corrigido (aguardando aprovação) | `docs/as-built-sdd` — PR #29 |
| 4 | Material de demonstração funcional | ✅ Corrigido (aguardando aprovação) | `docs/demo-material` — PR #30 |
| 5 | Tag de release `v1.0.0` | ⏳ PR aberto, **deve ser o ÚLTIMO aprovado** (depende de #27–#30) | `release/v1.0.0` — PR #31 |
| 6 | Branch `main` consolidada, sem PRs/conflitos abertos | ✅ Já estava ok (nenhuma ação necessária) | — |

---

## 1. Credenciais expostas (texto puro no repositório)

**Onde:** `docker-compose.yml`, `src/FlowCenter.API/appsettings.json`,
`src/FlowCenter.Infrastructure/Data/AppDbContextFactory.cs`.

**Problema:** a senha do PostgreSQL (`flowcenter_password123`) estava
hardcoded em três arquivos versionados no Git. Viola diretamente o requisito
"sem... credenciais expostas" da Entrega 3.

**Correção aplicada (PR #27):** senha/usuário movidos para variáveis de
ambiente lidas de um `.env` (gitignorado), com `.env.example` documentando os
placeholders; a connection string do `appsettings.json` passou a vir de
`ConnectionStrings__DefaultConnection` via ambiente; a factory de design-time
do EF Core agora falha de forma explícita se a variável não estiver definida,
em vez de usar um valor hardcoded.

---

## 2. Suíte de testes não compila / CI reporta sucesso sem rodar testes

**Causa raiz (duas, independentes):**

1. **Arquitetura duplicada e incompleta.** O projeto migrou para Clean
   Architecture (`src/FlowCenter.API`, `FlowCenter.Application`,
   `FlowCenter.Domain`, `FlowCenter.Infrastructure`), mas a migração não foi
   concluída: os projetos novos continham apenas *scaffolds* vazios
   (`Class1.cs`), enquanto a implementação real (Controller, DTOs,
   Interfaces, regras de negócio, middleware) continuava órfã em arquivos
   soltos dentro de `src/` (`src/Api`, `src/Application`, `src/Domain`,
   `src/Infrastructure`, `src/Program.cs`), compilados por um projeto
   `src/FlowCenter.csproj` que **não fazia parte da solução**
   (`FlowCenter.sln`). O projeto de testes (`tests/FlowCenter.Tests.csproj`)
   referenciava os projetos novos (vazios), mas o código do teste
   (`TarefasControllerTests.cs`) foi escrito contra os tipos do código antigo
   — resultado: erro de compilação (`FlowCenter.API.Controllers` não existe,
   `FlowCenterDbContext` não existe, etc.).
2. **GUID corrompido em `FlowCenter.sln`.** O GUID do projeto de testes na
   linha `Project(...)` (`F90F91A0-65D4-FB0AC90-0DA969CCB503`, faltando um
   dígito) não correspondia ao GUID usado nas seções `GlobalSection`
   (`F90F91A0-65D4-4FB0-AC90-0DA969CCB503`). Por isso `dotnet test
   FlowCenter.sln` no CI só compilava a solução e saía com "0 Warnings, 0
   Errors" **sem nunca executar um teste** — o pipeline estava verde de forma
   falsa, mascarando o problema nº 1 há várias execuções.

**Correção aplicada (PR #28):**
- Consolidado o código real (regras de negócio RB01–RB12, entidade `Tarefa`
  com `Guid` e setters privados, DTOs, `ITarefaRepository`, controller,
  middleware de exceções) dentro dos projetos corretos da Clean Architecture
  (`FlowCenter.Domain`, `FlowCenter.Application`, `FlowCenter.API`).
- Implementado `TarefaRepository` em `FlowCenter.Infrastructure` usando EF
  Core/PostgreSQL via `AppDbContext` (antes só existia uma implementação em
  memória, incompatível com RNF03/RNF04 do SDD, que exigem PostgreSQL + EF
  Core).
- Atualizado `FlowCenter.API/Program.cs` para registrar `AppDbContext`,
  `ITarefaRepository`, middleware de exceções, Swagger com comentários XML e
  conversão de enums para string em JSON.
- Regenerada a migration inicial do EF Core para o novo formato de Id
  (`Guid`, client-generated) da entidade `Tarefa`.
- Removidos os arquivos e o projeto órfãos (`src/FlowCenter.csproj`,
  `src/Program.cs`, `src/Api`, `src/Application`, `src/Domain`,
  `src/Infrastructure`, `src/Properties`, `src/appsettings*.json`) e os
  scaffolds vazios (`Class1.cs` em Application/Domain/Infrastructure,
  `src/FlowCenter.Tests` — projeto de teste placeholder que nunca esteve na
  solução, e `tests/UnitTest1.cs`).
- Corrigido o GUID do projeto de testes em `FlowCenter.sln`.
- Ajustado `tests/Api/Controllers/TarefasControllerTests.cs` e
  `tests/Domain/Entities/TarefaTests.cs` para usar os namespaces/tipos reais
  (`FlowCenter.Api.Controllers`, `FlowCenter.Domain.Enums`,
  `FlowCenter.Domain.Exceptions`, `AppDbContext`, `TarefaRepository` do
  Infrastructure).
- Suíte validada localmente: `dotnet test FlowCenter.sln` → **36/36 testes
  aprovados**, confirmando que o comando agora efetivamente descobre e
  executa a suíte (antes executava 0 testes).
- Confirmado também no CI real (GitHub Actions) do PR #28: o step
  "Executar testes unitários" agora reporta `Total tests: 36 / Passed: 36`
  (antes só exibia "Build succeeded, 0 Warnings, 0 Errors", sem nenhuma
  contagem de testes).

---

## 3. SDD como especificação "As-Built"

**Problema:** o SDD descrevia decisões (ex.: persistência) que não refletiam
mais o código final após a consolidação da arquitetura.

**Correção aplicada (PR #29):** seção de contratos (8) completada (estava
cortada no meio de um bloco JSON); adicionada a seção 9 (Arquitetura
As-Built), documentando os 5 projetos da Clean Architecture; adicionada a
seção 10 (Relatório de Testes e Evidências), registrando o estado real da
suíte (36 testes) e uma nota explicando que as capturas de tela da fase
intermediária mostravam apenas um teste placeholder e um CI que não
executava a suíte de fato.

---

## 4. Material de demonstração funcional

**Problema:** não havia roteiro/material para demonstrar a aplicação final
funcionando.

**Correção aplicada (PR #30):** adicionado `docs/DEMO.md` com roteiro de
demonstração (passo a passo: subir via Docker Compose, abrir Swagger,
exercitar os 6 endpoints RF01–RF08 e as validações RB01, RB02, RB09, RB10,
RB12), com link a partir do README.

---

## 5. Tag de release `v1.0.0`

**Pendente.** PR de release aberto: `release/v1.0.0` — **PR #31**. Este PR
adiciona apenas o `CHANGELOG.md` e **deve ser o ÚLTIMO a ser aprovado e
mesclado**, depois de #27, #28, #29 e #30. Veja o disclaimer na descrição
do PR #31.

Depois do merge do PR #31, criar a tag `v1.0.0` no commit resultante na
`main` e publicar a release oficial no GitHub.

---

## 6. Branch `main` consolidada

Já estava em conformidade: sem PRs abertos, sem conflitos, sincronizada com
o remoto. Nenhuma ação necessária além de manter esse estado após os merges
dos PRs listados acima.
