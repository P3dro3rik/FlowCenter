using FlowCenter.Application.DTOs;
using FlowCenter.Application.Interfaces;
using FlowCenter.Domain.Entities;
using FlowCenter.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FlowCenter.Api.Controllers;

/// <summary>
/// Endpoints REST para o gerenciamento de tarefas. (RF01–RF08)
/// </summary>
[ApiController]
[Route("api/tarefas")]
[Produces("application/json")]
public class TarefasController : ControllerBase
{
    private readonly ITarefaRepository _repository;

    public TarefasController(ITarefaRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Lista as tarefas cadastradas, com filtros opcionais por status e prioridade. (RF02, RF07, RF08)</summary>
    /// <param name="status">Filtra pelo status da tarefa (Pendente, EmAndamento, Concluida).</param>
    /// <param name="prioridade">Filtra pela prioridade da tarefa (Baixa, Media, Alta).</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TarefaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TarefaResponse>>> Listar(
        [FromQuery] StatusTarefa? status,
        [FromQuery] PrioridadeTarefa? prioridade,
        CancellationToken cancellationToken)
    {
        var tarefas = await _repository.ListarAsync(status, prioridade, cancellationToken);
        return Ok(tarefas.Select(TarefaResponse.FromDomain));
    }

    /// <summary>Consulta uma tarefa específica pelo identificador. (RF03, RB09)</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponse>> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var tarefa = await _repository.ObterPorIdAsync(id, cancellationToken);

        if (tarefa is null)
            return NotFound(); // RB09

        return Ok(TarefaResponse.FromDomain(tarefa));
    }

    /// <summary>Cadastra uma nova tarefa. (RF01, RB05)</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TarefaResponse>> Criar(
        [FromBody] CriarTarefaRequest request, CancellationToken cancellationToken)
    {
        var tarefa = new Tarefa(request.Titulo, request.Prioridade, request.Descricao);

        await _repository.AdicionarAsync(tarefa, cancellationToken);
        await _repository.SalvarAlteracoesAsync(cancellationToken);

        var response = TarefaResponse.FromDomain(tarefa);
        return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, response);
    }

    /// <summary>Atualiza os dados de uma tarefa existente. (RF04)</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponse>> Atualizar(
        Guid id, [FromBody] AtualizarTarefaRequest request, CancellationToken cancellationToken)
    {
        var tarefa = await _repository.ObterPorIdAsync(id, cancellationToken);

        if (tarefa is null)
            return NotFound(); // RB09

        tarefa.Atualizar(request.Titulo, request.Prioridade, request.Descricao);
        await _repository.SalvarAlteracoesAsync(cancellationToken);

        return Ok(TarefaResponse.FromDomain(tarefa));
    }

    /// <summary>Marca uma tarefa como concluída, registrando a data de conclusão. (RF05, RB07, RB10)</summary>
    [HttpPatch("{id:guid}/concluir")]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponse>> Concluir(Guid id, CancellationToken cancellationToken)
    {
        var tarefa = await _repository.ObterPorIdAsync(id, cancellationToken);

        if (tarefa is null)
            return NotFound(); // RB09

        tarefa.Concluir();
        await _repository.SalvarAlteracoesAsync(cancellationToken);

        return Ok(TarefaResponse.FromDomain(tarefa));
    }

    /// <summary>Exclui uma tarefa. (RF06)</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken cancellationToken)
    {
        var tarefa = await _repository.ObterPorIdAsync(id, cancellationToken);

        if (tarefa is null)
            return NotFound(); // RB09

        _repository.Remover(tarefa);
        await _repository.SalvarAlteracoesAsync(cancellationToken);

        return NoContent();
    }
}
