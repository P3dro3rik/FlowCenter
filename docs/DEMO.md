# Roteiro de Demonstração — FlowCenter

Roteiro passo a passo para demonstrar a aplicação final funcionando,
cobrindo os requisitos funcionais RF01–RF08. Pode ser seguido ao vivo ou
usado como script para gravação de vídeo.

## 1. Pré-requisitos

- Docker Desktop instalado.
- Porta `8080` (API) e `5432` (PostgreSQL) livres na máquina.

## 2. Subindo o ambiente

```
cp .env.example .env
# edite .env e defina um valor para POSTGRES_PASSWORD
docker-compose up --build
```

A API aplica as migrations do EF Core automaticamente na primeira
inicialização (banco começa vazio). Aguarde a mensagem de que a aplicação
está ouvindo em `http://+:8080`.

## 3. Abrindo a documentação interativa

Acesse `http://localhost:8080/swagger` no navegador. A tela deve exibir os
6 endpoints de `/api/tarefas` descritos no SDD (seção 7).

## 4. Roteiro de exercício dos endpoints

Execute os passos abaixo pelo Swagger UI (botão "Try it out" em cada
endpoint) ou via `curl`.

### 4.1 Criar tarefas (RF01, RB01–RB05)

```
curl -X POST http://localhost:8080/api/tarefas \
  -H "Content-Type: application/json" \
  -d '{"titulo":"Preparar apresentação","descricao":"Slides da entrega final","prioridade":"Alta"}'
```

Resultado esperado: `201 Created`, corpo com `status: "Pendente"` e um `id`
(GUID) gerado. Guarde o `id` retornado para os próximos passos.

Repita criando uma segunda tarefa com prioridade `"Baixa"`, para exercitar os
filtros no próximo passo.

Demonstre também a validação de negócio (RB01, RB02, RB12): tente criar uma
tarefa com título vazio ou com 2 caracteres — a API deve responder
`400 Bad Request`.

### 4.2 Listar e filtrar tarefas (RF02, RF07, RF08)

```
curl http://localhost:8080/api/tarefas
curl "http://localhost:8080/api/tarefas?prioridade=Alta"
curl "http://localhost:8080/api/tarefas?status=Pendente"
```

Resultado esperado: a primeira chamada retorna todas as tarefas criadas; as
seguintes retornam apenas as que casam com o filtro.

### 4.3 Consultar uma tarefa específica (RF03, RB09)

```
curl http://localhost:8080/api/tarefas/{id}
```

Use um `id` válido (deve retornar `200 OK`) e depois um GUID aleatório
(deve retornar `404 Not Found`), demonstrando RB09.

### 4.4 Atualizar uma tarefa (RF04)

```
curl -X PUT http://localhost:8080/api/tarefas/{id} \
  -H "Content-Type: application/json" \
  -d '{"titulo":"Preparar apresentação final","descricao":"Slides revisados","prioridade":"Media"}'
```

Resultado esperado: `200 OK` com os dados atualizados.

### 4.5 Concluir uma tarefa (RF05, RB07, RB10)

```
curl -X PATCH http://localhost:8080/api/tarefas/{id}/concluir
```

Resultado esperado: `200 OK`, `status: "Concluida"` e `dataConclusao`
preenchida. Demonstre a regra RB10 tentando, em seguida, alterar o status
dessa mesma tarefa de volta para `Pendente` via `PUT` — a API deve recusar.

### 4.6 Excluir uma tarefa (RF06)

```
curl -X DELETE http://localhost:8080/api/tarefas/{id}
```

Resultado esperado: `204 No Content`. Uma nova chamada de consulta (4.3)
para o mesmo `id` deve retornar `404 Not Found`.

## 5. Encerrando o ambiente

```
docker-compose down
```

Use `docker-compose down -v` caso também queira descartar o volume do
PostgreSQL e recomeçar com o banco vazio na próxima demonstração.

## 6. Evidência da suíte de testes

Para demonstrar a suíte de testes automatizados (36 testes cobrindo
RB01–RB12 e RF01–RF08):

```
dotnet test FlowCenter.sln
```

O pipeline de CI (GitHub Actions, `.github/workflows/ci.yml`) executa o
mesmo comando a cada push/PR — ver aba "Actions" do repositório para o
histórico de execuções.
