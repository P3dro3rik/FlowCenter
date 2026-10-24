using FlowCenter.Api.Controllers;
using FlowCenter.Application.DTOs;
using FlowCenter.Domain.Entities;
using FlowCenter.Domain.Enums;
using FlowCenter.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FlowCenter.Tests.Api.Controllers;

public class TarefasControllerTests
{
    private static TarefasController CriarController(out TarefaRepository repository)
    {
        repository = new TarefaRepository();
        return new TarefasController(repository);
    }

    [Fact]
    public async Task Listar_DeveRetornarTodasAsTarefas_QuandoNaoHouverFiltros()
    {
        // Arrange
        var controller = CriarController(out var repository);
        await repository.AdicionarAsync(new Tarefa("Tarefa 1", PrioridadeTarefa.Alta, "Descrição 1"));
        await repository.AdicionarAsync(new Tarefa("Tarefa 2", PrioridadeTarefa.Media, "Descrição 2"));

        // Act
        var result = await controller.Listar(null, null, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var tarefas = okResult.Value.Should().BeAssignableTo<IEnumerable<TarefaResponse>>().Subject;
        tarefas.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarTarefa_QuandoIdExistir()
    {
        // Arrange
        var controller = CriarController(out var repository);
        var tarefa = new Tarefa("Tarefa Exemplo", PrioridadeTarefa.Alta, "Descrição Exemplo");
        await repository.AdicionarAsync(tarefa);

        // Act
        var result = await controller.ObterPorId(tarefa.Id, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<TarefaResponse>().Subject;
        response.Id.Should().Be(tarefa.Id);
        response.Titulo.Should().Be("Tarefa Exemplo");
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarNotFound_QuandoIdNaoExistir()
    {
        // Arrange
        var controller = CriarController(out _);

        // Act
        var result = await controller.ObterPorId(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Criar_DeveAdicionarTarefaERetornarCreated()
    {
        // Arrange
        var controller = CriarController(out var repository);
        var request = new CriarTarefaRequest 
        { 
            Titulo = "Nova Tarefa", 
            Prioridade = PrioridadeTarefa.Alta, 
            Descricao = "Nova Descrição" 
        };

        // Act
        var result = await controller.Criar(request, CancellationToken.None);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<TarefaResponse>().Subject;
        response.Titulo.Should().Be("Nova Tarefa");

        var tarefaNoRepo = await repository.ObterPorIdAsync(response.Id);
        tarefaNoRepo.Should().NotBeNull();
    }

    [Fact]
    public async Task Atualizar_DeveAtualizarTarefa_QuandoExistir()
    {
        // Arrange
        var controller = CriarController(out var repository);
        var tarefa = new Tarefa("Título Antigo", PrioridadeTarefa.Baixa, "Descrição Antiga");
        await repository.AdicionarAsync(tarefa);

        var request = new AtualizarTarefaRequest 
        { 
            Titulo = "Título Novo", 
            Prioridade = PrioridadeTarefa.Alta, 
            Descricao = "Descrição Nova" 
        };

        // Act
        var result = await controller.Atualizar(tarefa.Id, request, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<TarefaResponse>().Subject;
        response.Titulo.Should().Be("Título Novo");
    }

    [Fact]
    public async Task Concluir_DeveMarcarComoConcluida_QuandoExistir()
    {
        // Arrange
        var controller = CriarController(out var repository);
        var tarefa = new Tarefa("Tarefa a Concluir", PrioridadeTarefa.Media, null);
        await repository.AdicionarAsync(tarefa);

        // Act
        var result = await controller.Concluir(tarefa.Id, CancellationToken.None);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<TarefaResponse>().Subject;
        response.Status.Should().Be(StatusTarefa.Concluida);
    }

    [Fact]
    public async Task Remover_DeveDeletarTarefa_QuandoExistir()
    {
        // Arrange
        var controller = CriarController(out var repository);
        var tarefa = new Tarefa("Tarefa a Deletar", PrioridadeTarefa.Baixa, null);
        await repository.AdicionarAsync(tarefa);

        // Act
        var result = await controller.Remover(tarefa.Id, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var tarefaExcluida = await repository.ObterPorIdAsync(tarefa.Id);
        tarefaExcluida.Should().BeNull();
    }
}