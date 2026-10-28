# Changelog

Todas as mudanças relevantes deste projeto são documentadas neste arquivo.

## [v1.0.0] — Entrega 3 (Solução Final)

### Corrigido
- Credenciais do PostgreSQL que estavam expostas em texto puro no
  repositório (`docker-compose.yml`, `appsettings.json`, factory de
  design-time do EF Core); agora lidas de variáveis de ambiente (#27).
- Arquitetura duplicada/incompleta: a migração para Clean Architecture
  havia deixado os projetos `FlowCenter.API/Application/Domain/Infrastructure`
  como scaffolds vazios, enquanto a implementação real ficava órfã em
  arquivos soltos fora da solução. Código real consolidado nos projetos
  corretos (#28).
- Suíte de testes que não compilava e GUID corrompido em `FlowCenter.sln`
  que fazia o CI reportar sucesso sem executar nenhum teste (falso verde).
  Corrigido; suíte com 36/36 testes aprovados, confirmado em CI (#28).

### Adicionado
- Implementação de `TarefaRepository` sobre PostgreSQL/EF Core, substituindo
  a implementação em memória que havia ficado órfã (#28).
- `docs/DEMO.md`: roteiro de demonstração funcional da aplicação (#30).
- Seções de Arquitetura (As-Built) e Relatório de Testes/Evidências no SDD
  (#29).

### Documentação
- `pontosConflitantes.MD`: checklist dos pontos pendentes da Entrega 3 e
  seu status de correção (#26).

---

Histórico detalhado de cada correção: ver `pontosConflitantes.MD` e os PRs
#26, #27, #28, #29, #30.
