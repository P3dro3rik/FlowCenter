SDD — FlowCenter
Sistema: FlowCenter
 Tipo: API REST para gerenciamento de tarefas
 Objetivo da Entrega: Especificação técnica, colaboração com IA e validação automatizada.

1. Problema
Atualmente, o gerenciamento de tarefas pode ser realizado de maneira descentralizada, utilizando anotações, planilhas ou diferentes ferramentas. Essa abordagem pode dificultar o acompanhamento das atividades, prioridades e respectivos status, além de aumentar a possibilidade de perda de informações e atrasos.
O FlowCenter surge como uma solução para centralizar o gerenciamento de tarefas por meio de uma API REST, permitindo que as atividades sejam cadastradas, consultadas, atualizadas, concluídas e removidas de maneira padronizada.
A aplicação também permitirá a organização das tarefas por status e prioridade, facilitando o acompanhamento do trabalho e possibilitando a integração com diferentes aplicações clientes.

2. Objetivo do sistema
O objetivo do FlowCenter é fornecer uma API REST para gerenciamento centralizado de tarefas, permitindo controlar suas informações, prioridades e estados durante seu ciclo de vida.
O sistema deverá:
Cadastrar novas tarefas;
Consultar tarefas existentes;
Consultar uma tarefa específica;
Atualizar informações de uma tarefa;
Alterar o status de uma tarefa;
Concluir tarefas;
Excluir tarefas;
Filtrar tarefas por status;
Filtrar tarefas por prioridade;
Disponibilizar documentação da API por meio do Swagger/OpenAPI.

3. Escopo
3.1 Funcionalidades incluídas
O FlowCenter contemplará:
Cadastro de tarefas;
Consulta de tarefas;
Consulta individual por identificador;
Atualização de tarefas;
Conclusão de tarefas;
Exclusão de tarefas;
Filtragem por status;
Filtragem por prioridade;
Persistência dos dados em banco PostgreSQL;
API REST;
Documentação dos endpoints;
Validação das regras de negócio;
Testes automatizados;
Execução padronizada utilizando Docker;
Pipeline de testes utilizando GitHub Actions.
3.2 Fora do escopo
Para evitar que o projeto fique maior do que o necessário, não serão implementados nesta entrega:
Sistema de login e autenticação;
Cadastro de usuários;
Controle de permissões;
Aplicativo mobile;
Interface web completa;
Notificações por e-mail;
Chat entre usuários;
Anexos de arquivos;
Integração com serviços externos.
Esses itens podem ser considerados como possíveis evoluções futuras.

4. Requisitos funcionais
ID
Requisito
RF01
O sistema deverá permitir o cadastro de uma nova tarefa.
RF02
O sistema deverá permitir listar todas as tarefas cadastradas.
RF03
O sistema deverá permitir consultar uma tarefa pelo seu identificador.
RF04
O sistema deverá permitir atualizar os dados de uma tarefa.
RF05
O sistema deverá permitir concluir uma tarefa.
RF06
O sistema deverá permitir excluir uma tarefa.
RF07
O sistema deverá permitir filtrar tarefas pelo status.
RF08
O sistema deverá permitir filtrar tarefas pela prioridade.
RF09
O sistema deverá registrar a data de criação da tarefa.
RF10
O sistema deverá registrar a data de conclusão quando uma tarefa for concluída.
RF11
O sistema deverá disponibilizar documentação dos endpoints por meio do Swagger/OpenAPI.
RF12
O sistema deverá validar os dados recebidos antes de realizar operações sobre as tarefas.


5. Requisitos não funcionais
ID
Requisito
RNF01
A aplicação deverá ser desenvolvida utilizando C# e ASP.NET Core.
RNF02
A aplicação deverá utilizar arquitetura organizada em camadas, separando responsabilidades.
RNF03
Os dados deverão ser persistidos utilizando PostgreSQL.
RNF04
O acesso ao banco deverá utilizar Entity Framework Core.
RNF05
O sistema deverá possuir testes automatizados utilizando xUnit.
RNF06
O ambiente da aplicação deverá poder ser executado de forma padronizada utilizando Docker.
RNF07
O projeto deverá possuir documentação dos endpoints utilizando Swagger/OpenAPI.
RNF08
O código deverá seguir princípios de Clean Code e separação de responsabilidades.
RNF09
O projeto deverá possuir pipeline automatizado para compilação e execução dos testes.
RNF10
O repositório deverá utilizar controle de versões com Git e fluxo baseado em branches e Pull Requests.


6. Regras de negócio
Aqui definimos o que o sistema permite ou não permite fazer.
ID
Regra
RB01
O título da tarefa é obrigatório.
RB02
O título deverá possuir entre 3 e 100 caracteres.
RB03
A descrição da tarefa será opcional.
RB04
A prioridade deverá ser Baixa, Média ou Alta.
RB05
Toda nova tarefa deverá ser criada com status Pendente.
RB06
Uma tarefa poderá possuir os status Pendente, Em Andamento ou Concluída.
RB07
Uma tarefa concluída deverá possuir uma data de conclusão.
RB08
Uma tarefa que ainda não foi concluída não deverá possuir data de conclusão.
RB09
Uma tarefa inexistente deverá resultar em resposta HTTP 404.
RB10
Uma tarefa já concluída não poderá retornar para o status Pendente.
RB11
O identificador da tarefa deverá ser único.
RB12
Uma requisição com dados inválidos deverá ser rejeitada com resposta adequada.


7. Endpoints da API
A API utilizará o recurso /api/tarefas.
Método
Endpoint
Função
GET
/api/tarefas
Listar tarefas
GET
/api/tarefas/{id}
Consultar tarefa específica
POST
/api/tarefas
Criar tarefa
PUT
/api/tarefas/{id}
Atualizar tarefa
PATCH
/api/tarefas/{id}/concluir
Concluir tarefa
DELETE
/api/tarefas/{id}
Excluir tarefa

Filtros
O endpoint de listagem poderá receber:
GET /api/tarefas?status=Pendente
ou:
GET /api/tarefas?prioridade=Alta
Também poderá permitir combinação:
GET /api/tarefas?status=EmAndamento&prioridade=Alta

8. Contratos de entrada e saída
Os contratos definem o que a API recebe e o que devolve.
8.1 Criar tarefa
Requisição
{
  "titulo": "Implementar API",
  "descricao": "Criar os endpoints de tarefas",
  "prioridade": "Alta"
}
Resposta — 201 Created
{
  "id": 1,
  "titulo": "Implementar API",
  "descricao": "Criar os endpoints de tarefas",
  "prioridade": "Alta",
  "status": "Pendente",
  "dataCriacao": "2026-09-10T10:00:00",
  "dataConclusao": null
}

8.2 Consultar tarefa
Requisição
GET /api/tarefas/1
Resposta — 200 OK
{
  "id": 1,
  "titulo": "Implementar API",
  "descricao": "Criar os endpoints de tarefas",
  "prioridade": "Alta",
  "status": "Pendente",
  "dataCriacao": "2026-09-10T10:00:00",
  "dataConclusao": null
}

8.3 Tarefa inexistente
Resposta — 404 Not Found
{
  "mensagem": "Tarefa não encontrada."
}

8.4 Concluir tarefa
Requisição
PATCH /api/tarefas/1/concluir
Resposta
{
  "id": 1,
  "titulo": "Implementar API",
  "descricao": "Criar os endpoints de tarefas",
  "prioridade": "Alta",
  "status": "Concluida",
  "dataCriacao": "2026-09-10T10:00:00",
  "dataConclusao": "2026-09-10T15:30:00"
}

9. Componentes da aplicação
A aplicação será organizada seguindo uma arquitetura em camadas.
FlowCenter
│
├── API
│   ├── Controllers
│   └── Configurações da API
│
├── Application
│   ├── Services
│   ├── DTOs
│   └── Interfaces
│
├── Domain
│   ├── Entities
│   ├── Enums
│   └── Regras de negócio
│
├── Infrastructure
│   ├── DbContext
│   ├── Repositories
│   └── Configurações do banco
│
└── Tests
    ├── Unitários
    └── Integração
Responsabilidade de cada camada
Domain
Responsável pelas entidades e regras fundamentais do sistema.
Application
Responsável pela lógica de aplicação, serviços, DTOs e contratos internos.
Infrastructure
Responsável pelo acesso ao PostgreSQL, Entity Framework Core e persistência dos dados.
API
Responsável por receber as requisições HTTP e devolver as respostas.
Tests
Responsável pelos testes automatizados e pela validação do comportamento do sistema.

10. Histórico de refinamento
O histórico de refinamento registra como a especificação foi evoluindo durante o desenvolvimento.
Podemos iniciar com:
Versão
Alteração
Motivo
v0.1
Definição inicial do FlowCenter como API de gerenciamento de tarefas.
Definir o problema e a proposta do sistema.
v0.2
Definição do escopo e das funcionalidades principais.
Delimitar o que será desenvolvido na entrega.
v0.3
Definição dos requisitos funcionais e não funcionais.
Transformar o objetivo em requisitos verificáveis.
v0.4
Definição das regras de negócio.
Estabelecer comportamentos obrigatórios do sistema.
v0.5
Definição dos endpoints e contratos da API.
Estabelecer a comunicação entre cliente e servidor.
v0.6
Definição da arquitetura em camadas.
Separar responsabilidades e facilitar manutenção e testes.
v0.7
Definição da estratégia de testes automatizados e ambiente Docker/CI.
Garantir validação e reprodução do ambiente.


