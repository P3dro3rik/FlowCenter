using FlowCenter.Api.Controllers;
using FlowCenter.Application.DTOs;
using FlowCenter.Domain.Entities;
using FlowCenter.Domain.Enums;
using FlowCenter.Domain.Exceptions;
using FlowCenter.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlowCenter.Tests.Api.Controllers;

/// <summary>
/// Testes dos endpoints de <see cref="TarefasController"/>, utilizando o
/// provedor InMemory do Entity Framework Core para exercitar o repositório real.
/// Cobre a issue #4: GET/POST/PUT/PATCH/DELETE de /api/tarefas.
/// </summary>
public class TarefasControllerTests
{
    private static TarefasController CriarController(out FlowCenterDbContext context)
    {
        var options = new DbContextOptionsBuilder<FlowCenterDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        context = new FlowCenterDbContext(options);
        var repository = new TarefaRepository(context);
        return new TarefasController(repository);
    }

    // ── GET /api/tarefas ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Listar_SemFiltros_DeveRetornarTodasAsTarefas() // RF02
    {
        var controller = CriarController(out var context);
        context.Tarefas.AddRange(
            new Tarefa("Tarefa 1", PrioridadeTarefa.Baixa),
            new Tarefa("Tarefa 2", PrioridadeTarefa.Alta));
        await context.SaveChangesAsync();

        var resultado = await controller.Listar(null, null, CancellationToken.None);

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeAssignableTo<IEnumerable<TarefaResponse>>()
            .Which.Should().HaveCount(2);
    }

    [Fact]
    public async Task Listar_ComFiltroDeStatus_DeveRetornarApenasCorrespondentes() // RF07
    {
        var controller = CriarController(out var context);
        var pendente = new Tarefa("Pendente", PrioridadeTarefa.Media);
        var concluida = new Tarefa("Concluída", PrioridadeTarefa.Media);
        concluida.Concluir();
        context.Tarefas.AddRange(pendente, concluida);
        await context.SaveChangesAsync();

        var resultado = await controller.Listar(StatusTarefa.Concluida, null, CancellationToken.None);

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var tarefas = ok.Value.Should().BeAssignableTo<IEnumerable<TarefaResponse>>().Subject;
        tarefas.Should().ContainSingle().Which.Id.Should().Be(concluida.Id);
    }

    [Fact]
    public async Task Listar_ComFiltroDePrioridade_DeveRetornarApenasCorrespondentes() // RF08
    {
        var controller = CriarController(out var context);
        var alta = new Tarefa("Alta prioridade", PrioridadeTarefa.Alta);
        var baixa = new Tarefa("Baixa prioridade", PrioridadeTarefa.Baixa);
        context.Tarefas.AddRange(alta, baixa);
        await context.SaveChangesAsync();

        var resultado = await controller.Listar(null, PrioridadeTarefa.Alta, CancellationToken.None);

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var tarefas = ok.Value.Should().BeAssignableTo<IEnumerable<TarefaResponse>>().Subject;
        tarefas.Should().ContainSingle().Which.Id.Should().Be(alta.Id);
    }

    // ── GET /api/tarefas/{id} ────────────────────────────────────────────────────

    [Fact]
    public async Task ObterPorId_TarefaExistente_DeveRetornarOk() // RF03
    {
        var controller = CriarController(out var context);
        var tarefa = new Tarefa("Tarefa existente", PrioridadeTarefa.Media);
        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync();

        var resultado = await controller.ObterPorId(tarefa.Id, CancellationToken.None);

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<TarefaResponse>().Which.Id.Should().Be(tarefa.Id);
    }

    [Fact]
    public async Task ObterPorId_TarefaInexistente_DeveRetornarNotFound() // RB09
    {
        var controller = CriarController(out _);

        var resultado = await controller.ObterPorId(Guid.NewGuid(), CancellationToken.None);

        resultado.Result.Should().BeOfType<NotFoundResult>();
    }

    // ── POST /api/tarefas ────────────────────────────────────────────────────────

    [Fact]
    public async Task Criar_ComDadosValidos_DeveRetornarCreatedComTarefaPendente() // RF01, RB05
    {
        var controller = CriarController(out var context);
        var request = new CriarTarefaRequest
        {
            Titulo = "Implementar API",
            Descricao = "Criar os endpoints de tarefas",
            Prioridade = PrioridadeTarefa.Alta
        };

        var resultado = await controller.Criar(request, CancellationToken.None);

        var created = resultado.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = created.Value.Should().BeOfType<TarefaResponse>().Subject;
        response.Titulo.Should().Be("Implementar API");
        response.Status.Should().Be(StatusTarefa.Pendente);
        created.ActionName.Should().Be(nameof(TarefasController.ObterPorId));

        (await context.Tarefas.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Criar_ComTituloInvalido_DeveLancarDomainException() // RB01, RB12
    {
        var controller = CriarController(out _);
        var request = new CriarTarefaRequest { Titulo = "ab", Prioridade = PrioridadeTarefa.Baixa };

        Func<Task> acao = () => controller.Criar(request, CancellationToken.None);

        await acao.Should().ThrowAsync<DomainException>();
    }

    // ── PUT /api/tarefas/{id} ────────────────────────────────────────────────────

    [Fact]
    public async Task Atualizar_TarefaExistente_DeveAtualizarDados() // RF04
    {
        var controller = CriarController(out var context);
        var tarefa = new Tarefa("Título original", PrioridadeTarefa.Baixa);
        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync();
        var request = new AtualizarTarefaRequest
        {
            Titulo = "Título atualizado",
            Descricao = "Nova descrição",
            Prioridade = PrioridadeTarefa.Alta
        };

        var resultado = await controller.Atualizar(tarefa.Id, request, CancellationToken.None);

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<TarefaResponse>().Subject;
        response.Titulo.Should().Be("Título atualizado");
        response.Prioridade.Should().Be(PrioridadeTarefa.Alta);
    }

    [Fact]
    public async Task Atualizar_TarefaInexistente_DeveRetornarNotFound() // RB09
    {
        var controller = CriarController(out _);
        var request = new AtualizarTarefaRequest { Titulo = "Qualquer", Prioridade = PrioridadeTarefa.Baixa };

        var resultado = await controller.Atualizar(Guid.NewGuid(), request, CancellationToken.None);

        resultado.Result.Should().BeOfType<NotFoundResult>();
    }

    // ── PATCH /api/tarefas/{id}/concluir ─────────────────────────────────────────

    [Fact]
    public async Task Concluir_TarefaExistente_DeveMarcarComoConcluidaComData() // RF05, RB07
    {
        var controller = CriarController(out var context);
        var tarefa = new Tarefa("A concluir", PrioridadeTarefa.Media);
        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync();

        var resultado = await controller.Concluir(tarefa.Id, CancellationToken.None);

        var ok = resultado.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<TarefaResponse>().Subject;
        response.Status.Should().Be(StatusTarefa.Concluida);
        response.DataConclusao.Should().NotBeNull();
    }

    [Fact]
    public async Task Concluir_TarefaInexistente_DeveRetornarNotFound() // RB09
    {
        var controller = CriarController(out _);

        var resultado = await controller.Concluir(Guid.NewGuid(), CancellationToken.None);

        resultado.Result.Should().BeOfType<NotFoundResult>();
    }

    // ── DELETE /api/tarefas/{id} ─────────────────────────────────────────────────

    [Fact]
    public async Task Remover_TarefaExistente_DeveRetornarNoContentERemoverDoBanco() // RF06
    {
        var controller = CriarController(out var context);
        var tarefa = new Tarefa("A remover", PrioridadeTarefa.Baixa);
        context.Tarefas.Add(tarefa);
        await context.SaveChangesAsync();

        var resultado = await controller.Remover(tarefa.Id, CancellationToken.None);

        resultado.Should().BeOfType<NoContentResult>();
        (await context.Tarefas.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task Remover_TarefaInexistente_DeveRetornarNotFound() // RB09
    {
        var controller = CriarController(out _);

        var resultado = await controller.Remover(Guid.NewGuid(), CancellationToken.None);

        resultado.Should().BeOfType<NotFoundResult>();
    }
}
