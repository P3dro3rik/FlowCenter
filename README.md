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
