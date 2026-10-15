# FlowCenter — API de Gerenciamento de Tarefas

API REST desenvolvida como entrega acadêmica para o gerenciamento centralizado de tarefas.

## 🚀 Tecnologias e Ferramentas
- **Linguagem:** C# (.NET 8)
- **Framework Web:** ASP.NET Core
- **Banco de Dados:** PostgreSQL
- **ORM:** Entity Framework Core
- **Testes:** xUnit & Moq
- **Containers:** Docker & Docker Compose
- **Orquestração de IA:** Guidelines via `AGENTS.md`

## 🛠️ Como Executar a Aplicação

### Pré-requisito
- Docker Desktop instalado.

### Executando com Docker Compose
```bash
docker-compose up --build
```

### Executando localmente (.NET SDK)
Enquanto a configuração via Docker Compose não é finalizada (issue #5), a API pode ser executada localmente:

```bash
dotnet restore
dotnet run --project src/FlowCenter.csproj
```

A aplicação sobe por padrão em `http://localhost:5080` e a documentação interativa (Swagger UI) fica disponível em:

```
http://localhost:5080/swagger
```

É necessária uma instância do PostgreSQL acessível; ajuste a string de conexão em `src/appsettings.json` (`ConnectionStrings:DefaultConnection`) conforme o seu ambiente.

### Executando os testes
```bash
dotnet test
```

## 📚 Endpoints da API

Recurso base: `/api/tarefas`

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/tarefas` | Lista tarefas (filtros opcionais `?status=` e `?prioridade=`) |
| GET | `/api/tarefas/{id}` | Consulta uma tarefa específica |
| POST | `/api/tarefas` | Cadastra uma nova tarefa |
| PUT | `/api/tarefas/{id}` | Atualiza uma tarefa existente |
| PATCH | `/api/tarefas/{id}/concluir` | Marca uma tarefa como concluída |
| DELETE | `/api/tarefas/{id}` | Remove uma tarefa |

Detalhes completos dos contratos de entrada/saída e regras de negócio estão em [`docs/SDD.md`](docs/SDD.md).

# FlowCenter 🚀

O *FlowCenter* é uma aplicação desenvolvida em *.NET 8* utilizando os princípios da *Clean Architecture*, projetada para gerenciamento e orquestração de fluxos de tarefas.

---

## 🏛️ Arquitetura do Sistema

A solução está estruturada em camadas independentes para garantir alta testabilidade, baixo acoplamento e separação de responsabilidades:

```text
FlowCenter/
├── src/
│   ├── FlowCenter.Domain/        # Entidades de Domínio e Regras de Negócio
│   ├── FlowCenter.Application/   # Casos de Uso, DTOs e Interfaces
│   ├── FlowCenter.Infrastructure/# Mapeamento de Banco de Dados e Repositórios
│   └── FlowCenter.Api/           # Endpoints HTTP REST
└── tests/
    └── FlowCenter.Tests/         # Suíte de Testes Unitários   


---

## 📋 Registro de Decisões de Arquitetura (ADRs)

### ADR 01: Adoção do .NET 8 e Clean Architecture
* **Status:** Aprovado
* **Contexto:** Necessidade de uma plataforma robusta e de alto desempenho para a API do FlowCenter.
* **Decisão:** Utilizar .NET 8 estruturado em Clean Architecture.
* **Consequência:** Separação clara entre regras de negócio e infraestrutura, facilitando a escrita de testes unitários.

### ADR 02: Harness de Testes com xUnit
* **Status:** Aprovado
* **Contexto:** Garantir a qualidade do código e execução automatizada de testes.
* **Decisão:** Adoção da biblioteca `xUnit` no projeto `FlowCenter.Tests`.
* **Consequência:** Execução rápida de testes integrados ao CLI do .NET (`dotnet test`).

### ADR 03: Automação de CI/CD com GitHub Actions
* **Status:** Aprovado
* **Contexto:** Validar build e testes a cada Pull Request antes de integrar na branch `develop`.
* **Decisão:** Criar o workflow `.github/workflows/ci.yml`.
* **Consequência:** Garantia de que código com erro não entra na branch principal.

### ADR 04: Padronização de Ambiente com Docker
* **Status:** Aprovado
* **Contexto:** Evitar divergências de ambiente entre os membros da equipa.
* **Decisão:** Inclusão de `Dockerfile` e `docker-compose.yml`.
* **Consequência:** Reprodutibilidade do ambiente de desenvolvimento.